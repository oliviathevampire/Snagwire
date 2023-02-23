using UnityEngine;

public class Contact : MonoBehaviour {

	public GameObject winPanel;
	private Rigidbody2D _rb;

	private void Start() {
		_rb = GetComponent<Rigidbody2D>();
	}

	private void OnTriggerEnter2D(Collider2D col) {
		if (!col.gameObject.CompareTag("Plug")) return;
		var lineController = col.gameObject.GetComponentInChildren<LineController>();
		col.gameObject.GetComponent<FixedJoint2D>().connectedBody = _rb;
		col.gameObject.GetComponent<FixedJoint2D>().connectedAnchor = new Vector2(0.05f, 0f);
		lineController.plugged = true;
		lineController.timer.timerIsRunning = false;
		Invoke(nameof(ActivateWinPanel), 2f);
	}

	private void ActivateWinPanel() {
		winPanel.SetActive(true);
	}

}