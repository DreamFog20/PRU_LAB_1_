using UnityEngine;

/// <summary>
/// Đại diện cho một đoạn bản đồ (chunk) có thể ghép nối theo trục X.
/// Gắn script này lên prefab chunk. Chunk nên có các collider, platform, obstacle con.
/// </summary>
public class LevelChunk : MonoBehaviour
{
	[SerializeField] private Transform entryPoint;   // Điểm vào (mép trái)
	[SerializeField] private Transform exitPoint;    // Điểm ra (mép phải)
	[SerializeField] private float cachedWidth = -1f;

	public float GetWidth()
	{
		if (cachedWidth > 0f)
		{
			return cachedWidth;
		}

		if (entryPoint != null && exitPoint != null)
		{
			cachedWidth = Mathf.Abs(exitPoint.position.x - entryPoint.position.x);
			return cachedWidth;
		}

		// Nếu không có point, ước lượng theo bounds tổng của renderer/collider
		Bounds bounds = CalculateWorldBounds(gameObject);
		cachedWidth = bounds.size.x;
		return cachedWidth;
	}

	public Vector3 GetExitWorldPosition()
	{
		if (exitPoint != null)
		{
			return exitPoint.position;
		}
		Bounds b = CalculateWorldBounds(gameObject);
		return new Vector3(b.max.x, transform.position.y, transform.position.z);
	}

	public Vector3 GetEntryWorldPosition()
	{
		if (entryPoint != null)
		{
			return entryPoint.position;
		}
		Bounds b = CalculateWorldBounds(gameObject);
		return new Vector3(b.min.x, transform.position.y, transform.position.z);
	}

	private static Bounds CalculateWorldBounds(GameObject root)
	{
		Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
		Collider2D[] colliders = root.GetComponentsInChildren<Collider2D>(true);

		bool hasAny = false;
		Bounds bounds = new Bounds(root.transform.position, Vector3.zero);

		foreach (var r in renderers)
		{
			if (!hasAny)
			{
				bounds = r.bounds; hasAny = true; continue;
			}
			bounds.Encapsulate(r.bounds);
		}

		foreach (var c in colliders)
		{
			if (!hasAny)
			{
				bounds = c.bounds; hasAny = true; continue;
			}
			bounds.Encapsulate(c.bounds);
		}

		if (!hasAny)
		{
			bounds = new Bounds(root.transform.position, new Vector3(10f, 5f, 1f));
		}
		return bounds;
	}
}


