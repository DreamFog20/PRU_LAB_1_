using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
	// Không cần biến nextScene nữa vì chúng ta dùng Index cứng (1).
	// public string nextScene = "Main Menu";

	void Update()
	{
		// Kiểm tra xem người dùng có nhấn bất kỳ phím nào trên bàn phím 
		// hoặc nhấn nút chuột/touch trên màn hình hay không.
		if (Input.anyKeyDown)
		{
			// Tải Scene ở Build Index 1.
			// (Theo Build Settings của bạn, Index 1 là Scene "Scenes/Main Menu").
			SceneManager.LoadScene(1);
		}
	}
}