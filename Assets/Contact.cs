using System;
using UnityEngine;

public class Contact : MonoBehaviour {

	public GameObject winPanel;

	private void OnTriggerEnter2D(Collider2D col) {
		if (!col.gameObject.CompareTag("Plug")) return;
		col.gameObject.GetComponentInChildren<LineController>().plugged = true;
		col.gameObject.GetComponentInChildren<LineController>().timer.timerIsRunning = false;
		winPanel.SetActive(true);
	}

}