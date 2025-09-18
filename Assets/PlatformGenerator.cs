using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    // Có thể gán 1 hoặc nhiều prefab; nếu để trống sẽ không spawn
    public GameObject[] platformPrefabs;

    [Header("Tường/Đồ tĩnh đầu màn")]
    public bool spawnWallsAndProps = true;
    public GameObject startWallPrefab;   // bức tường bên phải đầu màn
    public Vector2 startWallOffset = new Vector2(3f, 0f);
    public GameObject leftBoundaryWallPrefab; // tường mép trái (tuỳ chọn)
    public Vector2 leftBoundaryOffset = new Vector2(-3f, 0f);
    public GameObject[] startPropsPrefabs; // đồ trang trí đầu màn

    [Header("Tự tạo KHUNG (đất dưới + tường trái/phải)")]
    public bool autoGenerateFrame = false;
    public GameObject frameGroundPiecePrefab; // nếu trống sẽ dùng phần tử đầu của platformPrefabs
    public GameObject frameWallPiecePrefab;   // nếu trống sẽ dùng startWallPrefab hoặc leftBoundaryWallPrefab
    public float frameWidth = 18f;            // tổng bề ngang khung
    public float frameHeight = 10f;           // tổng chiều cao (từ đáy lên)
    public float frameBottomOffsetY = -3f;    // đáy khung so với player.y
    public float frameSegmentSpacingX = 1f;   // khoảng cách lặp của nền đáy
    public float frameWallSpacingY = 1f;      // khoảng cách lặp của các đoạn tường

    [Header("Bật/tắt sinh platform")]
    public bool generatePlatforms = true;

    [Header("Spawn options")]
    public Transform parentForPlatforms; // nơi chứa các platform spawn ra
    public bool makeSpawnedStatic = true; // chuyển Rigidbody2D của platform sang Static để không bị rơi
	public int numberOfPlatforms = 15;
	public float levelWidth = 3.5f;
	public float minY = 2f;
	public float maxY = 3.5f;

	// Khoảng cách ngang tối thiểu giữa hai platform liên tiếp
	public float minHorizontalSpacing = 1.5f;
	// Số lần thử chọn X hợp lệ nhằm tránh vòng lặp vô hạn nếu Biên quá hẹp
	public int maxAttempts = 8;

	// Bắt đầu spawn gần vị trí của Player để đảm bảo nhìn thấy ngay
	public Transform player;
	public bool startAtPlayer = true;
	public float startYOffset = 1f; // platform đầu tiên cao hơn player bao nhiêu

    private bool generated;

    void Start()
	{
		float baseY = 0f;
		if (startAtPlayer && player != null)
		{
			baseY = player.position.y + startYOffset;
		}
		Vector3 spawnPos = new Vector3(0f, baseY, 0f);
		Vector3? lastPos = null;

        // Spawn tường/đồ tĩnh đầu màn
        if (spawnWallsAndProps && player != null)
        {
            Vector3 basePos = player.position;
            if (startWallPrefab != null)
            {
                Instantiate(startWallPrefab, basePos + (Vector3)startWallOffset, Quaternion.identity);
            }
            if (leftBoundaryWallPrefab != null)
            {
                Instantiate(leftBoundaryWallPrefab, basePos + (Vector3)leftBoundaryOffset, Quaternion.identity);
            }
            if (startPropsPrefabs != null && startPropsPrefabs.Length > 0)
            {
                foreach (var pfb in startPropsPrefabs)
                {
                    if (pfb == null) continue;
                    // rải ngẫu nhiên vài vật thể nhỏ quanh đầu màn
                    Vector3 rp = basePos + new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), 0f);
                    Instantiate(pfb, rp, Quaternion.identity);
                }
            }
        }

        // Tự tạo KHUNG (đất đáy + tường 2 bên)
        if (autoGenerateFrame && player != null)
        {
            GenerateFrameAroundPlayer();
        }

        // Spawn một nền ngay dưới chân player để không rơi khi bắt đầu
        if (startAtPlayer && player != null)
        {
            Vector3 groundPos = new Vector3(player.position.x, player.position.y - 1f, 0f);
            var pf = ChoosePrefab();
            if (pf != null)
            {
                var go = Instantiate(pf, groundPos, Quaternion.identity, parentForPlatforms);
                ApplySpawnedOptions(go);
            }
        }

        // Nếu cần, hoãn generate cho đến khi player vượt qua tường/triggers
        if (generatePlatforms && waitForPlayerToPass && player != null)
        {
            triggerX = player.position.x + triggerOffsetX;
            cachedSpawnStart = spawnPos;
            cachedLastPos = lastPos;
            return; // sẽ generate trong Update khi đủ điều kiện
        }

        if (generatePlatforms)
        {
            Generate(spawnPos, lastPos);
        }
    }

    void Update()
    {
        if (!generated && generatePlatforms && waitForPlayerToPass && player != null)
        {
            if (player.position.x >= triggerX)
            {
                Generate(cachedSpawnStart, cachedLastPos);
            }
        }
    }

    private Vector3 cachedSpawnStart;
    private Vector3? cachedLastPos;
    private float triggerX;

    private void Generate(Vector3 spawnPos, Vector3? lastPos)
    {
		for (int i = 0; i < numberOfPlatforms; i++)
		{
			// Cao dần theo trục Y
            if (i == 0 && startAtPlayer)
			{
				// đã đặt sẵn y cho platform đầu tiên gần player
			}
			else
			{
                // tăng độ khó dần: khoảng cách Y lớn hơn theo i
                float incMin = minY + i * 0.05f;
                float incMax = maxY + i * 0.10f;
                spawnPos.y += Random.Range(incMin, incMax);
			}

			// Tìm X sao cho không quá sát platform trước đó
			int attempts = 0;
			float x;
			do
			{
				x = Random.Range(-levelWidth, levelWidth);
				attempts++;
			}
			while (lastPos.HasValue && Mathf.Abs(x - lastPos.Value.x) < minHorizontalSpacing && attempts < maxAttempts);

			spawnPos.x = Mathf.Clamp(x, -levelWidth, levelWidth);

            var pf2 = ChoosePrefab();
            if (pf2 != null)
            {
                var go = Instantiate(pf2, spawnPos, Quaternion.identity, parentForPlatforms);
                ApplySpawnedOptions(go);
            }
			lastPos = spawnPos;
		}
        generated = true;
	}

    private GameObject ChoosePrefab()
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            return null;
        }
        int idx = Random.Range(0, platformPrefabs.Length);
        return platformPrefabs[idx];
    }

    [Header("Trigger spawn khi ra khỏi tường")]
    public bool waitForPlayerToPass = false;
    public float triggerOffsetX = 1.0f; // generate khi player.x vượt qua (x_lúc_start + offset)

    private void ApplySpawnedOptions(GameObject go)
    {
        if (go == null) return;
        if (makeSpawnedStatic)
        {
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Static;
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    private void GenerateFrameAroundPlayer()
    {
        if (player == null) return;
        Vector3 basePos = player.position;

        GameObject groundPiece = frameGroundPiecePrefab != null ? frameGroundPiecePrefab : ChoosePrefab();
        GameObject wallPiece = frameWallPiecePrefab != null ? frameWallPiecePrefab : (startWallPrefab != null ? startWallPrefab : leftBoundaryWallPrefab);
        if (groundPiece == null || wallPiece == null) return;

        float halfW = frameWidth * 0.5f;
        float bottomY = basePos.y + frameBottomOffsetY;
        float leftX = basePos.x - halfW;
        float rightX = basePos.x + halfW;

        // Nền đáy: lặp từ trái sang phải
        if (frameSegmentSpacingX <= 0.01f) frameSegmentSpacingX = 1f;
        for (float x = leftX; x <= rightX; x += frameSegmentSpacingX)
        {
            var g = Instantiate(groundPiece, new Vector3(x, bottomY, 0f), Quaternion.identity, parentForPlatforms);
            ApplySpawnedOptions(g);
        }

        // Tường trái/phải: lặp từ đáy lên trên
        if (frameWallSpacingY <= 0.01f) frameWallSpacingY = 1f;
        for (float y = bottomY; y <= bottomY + frameHeight; y += frameWallSpacingY)
        {
            var wl = Instantiate(wallPiece, new Vector3(leftX, y, 0f), Quaternion.identity, parentForPlatforms);
            ApplySpawnedOptions(wl);
            var wr = Instantiate(wallPiece, new Vector3(rightX, y, 0f), Quaternion.identity, parentForPlatforms);
            ApplySpawnedOptions(wr);
        }
    }
}


