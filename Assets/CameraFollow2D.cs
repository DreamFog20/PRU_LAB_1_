using UnityEngine;

/// <summary>
/// Theo dõi nhân vật cho camera trực giao 2D với làm mượt.
/// Gắn lên Main Camera, gán Target là player.
/// </summary>
public class CameraFollow2D : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private Vector2 offset = Vector2.zero;
	[SerializeField] private float lookUpOffset = 0f; // Offset để camera nhìn lên trên
	[SerializeField] private float smoothTime = 0.15f;
	[SerializeField] private bool lockY = false;
	[SerializeField] private float minY = -Mathf.Infinity;
	[SerializeField] private float maxY = Mathf.Infinity;

	private Vector3 currentVelocity;

	private void LateUpdate()
	{
		if (target == null)
		{
			return;
		}

		Vector3 desired = new Vector3(
			target.position.x + offset.x,
			lockY ? Mathf.Clamp(transform.position.y, minY, maxY) : Mathf.Clamp(target.position.y + offset.y + lookUpOffset, minY, maxY),
			transform.position.z
		);

		transform.position = Vector3.SmoothDamp(transform.position, desired, ref currentVelocity, smoothTime);
	}
}


