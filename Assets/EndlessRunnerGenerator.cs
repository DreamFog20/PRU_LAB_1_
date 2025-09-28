using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sinh map kiểu endless runner theo trục X.
/// - Cần một danh sách prefab chunk (mỗi prefab có LevelChunk).
/// - Generator sẽ luôn giữ một chiều dài hiển thị phía trước camera/player và tái sử dụng chunk dư ở phía sau.
/// </summary>
public class EndlessRunnerGenerator : MonoBehaviour
{
	[Header("Nguồn chunk")]
	[SerializeField] private List<LevelChunk> chunkPrefabs = new List<LevelChunk>();
	[SerializeField] private int initialChunks = 3;

	[Header("Theo dõi tiến độ")]
	[SerializeField] private Transform followTarget; // Player hoặc Camera
	[SerializeField] private bool followCamera = true; // nếu true sẽ dùng Camera.main làm follow
	[SerializeField] private float aheadDistance = 30f; // luôn bảo đảm có map phía trước khoảng cách này
	[SerializeField] private float behindCullDistance = 20f; // quá xa phía sau sẽ trả về pool

	[Header("Pool nội bộ")]
	[SerializeField] private int poolPrewarmPerPrefab = 0;

	private readonly List<LevelChunk> activeChunks = new List<LevelChunk>();
	private readonly Dictionary<LevelChunk, Stack<LevelChunk>> pool = new Dictionary<LevelChunk, Stack<LevelChunk>>();

	private float lastEndX;

	private void Start()
	{
		if (followCamera || followTarget == null)
		{
			Camera cam = Camera.main;
			if (cam != null)
			{
				followTarget = cam.transform;
			}
		}

		// Prewarm pool
		foreach (var prefab in chunkPrefabs)
		{
			if (!pool.ContainsKey(prefab)) pool[prefab] = new Stack<LevelChunk>();
			for (int i = 0; i < poolPrewarmPerPrefab; i++)
			{
				var inst = Instantiate(prefab, transform);
				inst.gameObject.SetActive(false);
				pool[prefab].Push(inst);
			}
		}

		// Spawn ban đầu
		LevelChunk first = SpawnRandomChunk(Vector3.zero);
		activeChunks.Add(first);
		lastEndX = first.GetExitWorldPosition().x;
		for (int i = 1; i < Mathf.Max(1, initialChunks); i++)
		{
			AppendOneChunk();
		}
	}

	private void Update()
	{
		if (followTarget == null)
		{
			return;
		}

		// Đảm bảo đủ map phía trước
		while (lastEndX - followTarget.position.x < aheadDistance)
		{
			AppendOneChunk();
		}

		// Thu hồi các chunk quá xa phía sau
		CullBehindChunks();
	}

	private void AppendOneChunk()
	{
		LevelChunk prefab = ChooseNextPrefab();
		LevelChunk newChunk = SpawnFromPool(prefab);
		Vector3 placePos = new Vector3(lastEndX, activeChunks[activeChunks.Count - 1].transform.position.y, 0f);
		newChunk.transform.position = placePos - (newChunk.GetEntryWorldPosition() - new Vector3(newChunk.transform.position.x, newChunk.transform.position.y, newChunk.transform.position.z));
		newChunk.gameObject.SetActive(true);
		activeChunks.Add(newChunk);
		lastEndX = newChunk.GetExitWorldPosition().x;
	}

	private void CullBehindChunks()
	{
		float behindX = followTarget.position.x - behindCullDistance;
		for (int i = activeChunks.Count - 1; i >= 0; i--)
		{
			LevelChunk c = activeChunks[i];
			if (c.GetExitWorldPosition().x < behindX)
			{
				activeChunks.RemoveAt(i);
				ReturnToPool(c);
			}
		}
	}

	private LevelChunk ChooseNextPrefab()
	{
		if (chunkPrefabs == null || chunkPrefabs.Count == 0)
		{
			return null;
		}
		int idx = Random.Range(0, chunkPrefabs.Count);
		return chunkPrefabs[idx];
	}

	private LevelChunk SpawnRandomChunk(Vector3 origin)
	{
		LevelChunk prefab = ChooseNextPrefab();
		LevelChunk inst = SpawnFromPool(prefab);
		inst.transform.position = origin;
		inst.gameObject.SetActive(true);
		return inst;
	}

	private LevelChunk SpawnFromPool(LevelChunk prefab)
	{
		if (prefab == null) return null;
		if (!pool.ContainsKey(prefab)) pool[prefab] = new Stack<LevelChunk>();
		if (pool[prefab].Count > 0)
		{
			return pool[prefab].Pop();
		}
		return Instantiate(prefab, transform);
	}

	private void ReturnToPool(LevelChunk chunk)
	{
		foreach (var kv in pool)
		{
			LevelChunk prefab = kv.Key;
			// So sánh bằng tên gốc của prefab nếu cần, ở đây ta đưa tất cả chunk về stack của prefab đầu tiên tương thích
			if (chunk.name.StartsWith(prefab.name))
			{
				chunk.gameObject.SetActive(false);
				pool[prefab].Push(chunk);
				return;
			}
		}
		// Nếu không bắt khớp được, đẩy vào stack đầu tiên
		var firstKey = new List<LevelChunk>(pool.Keys).Count > 0 ? new List<LevelChunk>(pool.Keys)[0] : null;
		if (firstKey != null)
		{
			chunk.gameObject.SetActive(false);
			pool[firstKey].Push(chunk);
		}
		else
		{
			chunk.gameObject.SetActive(false);
		}
	}
}


