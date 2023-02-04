using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {
	public void StartGame() {
		SceneManager.LoadScene(1);
	}

	public void Exit() {
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#endif
		Application.Quit();
	}
}