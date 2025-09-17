using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Nền cuộn vô tận theo trục X, hỗ trợ parallax theo chuyển động camera
/// Cách dùng:
/// - Tạo một GameObject cha (ví dụ: BackgroundRoot) và đặt các sprite con xếp liền kề ngang (ít nhất 2-3 đoạn).
/// - Gắn script này lên GameObject cha.
/// - Gán Camera (nếu không, sẽ tự lấy Camera.main khi Start).
/// - Chọn chế độ: dùng vận tốc cố định (scrollSpeed) hoặc dựa theo delta của camera (parallaxMultiplier).
/// Lưu ý: Các đoạn nên có cùng chiều rộng và khớp mép để tránh hở.
/// </summary>
public class InfiniteScrollingBackground : MonoBehaviour
{
	[Header("Camera / Parallax")]
	[SerializeField] private Camera targetCamera;
	[SerializeField] private bool useCameraParallax = true;
	[Range(0f, 1f)]
	[SerializeField] private float parallaxMultiplier = 0.5f; // 0 = đứng yên so với camera, 1 = bám theo camera

	[Header("Cuộn tự động")]
	[SerializeField] private bool useAutoScroll = false;
	[SerializeField] private float scrollSpeed = 1f; // đơn vị: world units/giây, cuộn sang trái

	[Header("Thiết lập đoạn nền")]
	[SerializeField] private List<Transform> segmentTransforms = new List<Transform>();
	[SerializeField] private float padding = 0.1f; // thêm biên an toàn khi kiểm tra ra khỏi màn
	[Tooltip("Khi không gán Camera, dùng nửa bề rộng cửa sổ tham chiếu này để xét loop. 0 = tự suy ra theo bề rộng đoạn.")]
	[SerializeField] private float fallbackViewportHalfWidth = 0f;

	[Header("Tự hoàn thiện đoạn nền")]
	[SerializeField] private bool autoCompleteSegments = true;
	[SerializeField] [Min(2)] private int minimumSegments = 3;

	private float segmentWidth;
	private float lastCameraX;

	private readonly LinkedList<Transform> orderedSegments = new LinkedList<Transform>();

	private void Awake()
	{
		// Nếu chưa gán danh sách, tự lấy tất cả con trực tiếp
		if (segmentTransforms == null || segmentTransforms.Count == 0)
		{
			segmentTransforms = new List<Transform>();
			for (int i = 0; i < transform.childCount; i++)
			{
				segmentTransforms.Add(transform.GetChild(i));
			}
		}

		// Sắp xếp theo vị trí X tăng dần
		segmentTransforms.Sort((a, b) => a.position.x.CompareTo(b.position.x));
		foreach (var t in segmentTransforms)
		{
			orderedSegments.AddLast(t);
		}

		segmentWidth = GetSegmentWorldWidth(segmentTransforms.Count > 0 ? segmentTransforms[0] : null);

		if (autoCompleteSegments)
		{
			EnsureMinimumSegments();
		}
	}

	private void Start()
	{
		if (targetCamera == null)
		{
			targetCamera = Camera.main;
		}
		if (targetCamera != null)
		{
			lastCameraX = targetCamera.transform.position.x;
		}
	}

	private void Update()
	{
		// Nếu dùng parallax theo camera nhưng chưa có camera thì không di chuyển theo camera.

		// 1) Di chuyển gốc theo camera để tạo parallax hoặc cuộn tự động
		if (useCameraParallax && targetCamera != null)
		{
			float camX = targetCamera.transform.position.x;
			float deltaX = camX - lastCameraX;
			if (Mathf.Abs(deltaX) > Mathf.Epsilon)
			{
				transform.position += Vector3.right * (deltaX * parallaxMultiplier);
				lastCameraX = camX;
			}
		}

		if (useAutoScroll)
		{
			transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
		}

		// 2) Tái sắp xếp các đoạn khi ra khỏi màn hình để tạo vòng lặp vô tận
		if (orderedSegments.Count >= 2)
		{
			RepositionSegmentsIfNeeded();
		}
	}

	private void RepositionSegmentsIfNeeded()
	{
		float halfWidth;
		float refX;
		if (targetCamera != null)
		{
			halfWidth = GetCameraHalfWidth(targetCamera);
			refX = targetCamera.transform.position.x;
		}
		else
		{
			// Không có camera: dùng giá trị fallback hoặc suy ra theo bề rộng đoạn
			halfWidth = fallbackViewportHalfWidth > 0f ? fallbackViewportHalfWidth : segmentWidth * 1.5f;
			refX = transform.position.x;
		}

		float cameraLeft = refX - halfWidth - padding;
		float cameraRight = refX + halfWidth + padding;

		// Kiểm tra đoạn ngoài cùng bên trái đã đi quá bên trái màn hình chưa
		Transform first = orderedSegments.First.Value;
		Transform last = orderedSegments.Last.Value;

		float firstRightEdge = first.position.x + (segmentWidth * 0.5f);
		float lastLeftEdge = last.position.x - (segmentWidth * 0.5f);

		// Nếu cạnh phải của đoạn đầu đã nằm bên trái rìa trái camera => đẩy ra sau cùng bên phải
		while (firstRightEdge < cameraLeft)
		{
			float newX = last.position.x + segmentWidth;
			first.position = new Vector3(newX, first.position.y, first.position.z);
			// cập nhật danh sách
			orderedSegments.RemoveFirst();
			orderedSegments.AddLast(first);
			first = orderedSegments.First.Value;
			last = orderedSegments.Last.Value;
			firstRightEdge = first.position.x + (segmentWidth * 0.5f);
			lastLeftEdge = last.position.x - (segmentWidth * 0.5f);
		}

		// Ngược lại, nếu cạnh trái của đoạn cuối đã nằm bên phải rìa phải camera => đẩy ra trước cùng bên trái
		while (lastLeftEdge > cameraRight)
		{
			float newX = first.position.x - segmentWidth;
			last.position = new Vector3(newX, last.position.y, last.position.z);
			orderedSegments.RemoveLast();
			orderedSegments.AddFirst(last);
			first = orderedSegments.First.Value;
			last = orderedSegments.Last.Value;
			firstRightEdge = first.position.x + (segmentWidth * 0.5f);
			lastLeftEdge = last.position.x - (segmentWidth * 0.5f);
		}
	}

	private float GetSegmentWorldWidth(Transform segment)
	{
		if (segment == null)
		{
			return 0f;
		}

		// Ưu tiên lấy từ SpriteRenderer
		SpriteRenderer sr = segment.GetComponent<SpriteRenderer>();
		if (sr != null)
		{
			return sr.bounds.size.x;
		}

		// Thử BoxCollider2D
		BoxCollider2D box = segment.GetComponent<BoxCollider2D>();
		if (box != null)
		{
			return box.bounds.size.x;
		}

		// Fallback: khoảng cách giữa hai segment đầu
		if (segmentTransforms.Count >= 2)
		{
			float dx = Mathf.Abs(segmentTransforms[1].position.x - segmentTransforms[0].position.x);
			if (dx > 0f)
			{
				return dx;
			}
		}

		// Mặc định an toàn
		return 10f;
	}

	private void EnsureMinimumSegments()
	{
		// Cần ít nhất 2 đoạn để loop, mặc định tạo tới minimumSegments
		if (segmentTransforms.Count == 0)
		{
			return;
		}

		Transform template = segmentTransforms[segmentTransforms.Count - 1];
		float width = segmentWidth > 0f ? segmentWidth : GetSegmentWorldWidth(template);

		while (segmentTransforms.Count < Mathf.Max(2, minimumSegments))
		{
			Transform clone = Instantiate(template, template.parent);
			// Đặt clone nối sau cùng bên phải
			Transform last = segmentTransforms[segmentTransforms.Count - 1];
			clone.position = new Vector3(last.position.x + width, last.position.y, last.position.z);
			clone.name = template.name + "_seg" + segmentTransforms.Count;
			segmentTransforms.Add(clone);
			orderedSegments.AddLast(clone);
		}
	}

	private static float GetCameraHalfWidth(Camera cam)
	{
		if (cam == null || !cam.orthographic)
		{
			return 0f;
		}
		float halfHeight = cam.orthographicSize;
		float aspect = (float)Screen.width / Screen.height;
		return halfHeight * aspect;
	}
}


