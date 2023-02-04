using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DonePan : MonoBehaviour {

	public GameObject panWin;
	public GameObject panLost;
	
	// Start is called before the first frame update
	void Start() {
		
	}

	public void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void MainMenu() {
		SceneManager.LoadScene(0);
	}
}