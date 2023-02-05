using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DonePan : MonoBehaviour {

	public GameObject panWin;
	
	public void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void MainMenu() {
		SceneManager.LoadScene(0);
	}
}