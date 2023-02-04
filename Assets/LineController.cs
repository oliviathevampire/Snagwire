using UnityEngine;

public class LineController : MonoBehaviour {
	public Transform startingPoint;
	private LineRenderer _lr;

	private void Start() {
		_lr = GetComponent<LineRenderer>();
	}

	private void Update() {
		_lr.SetPosition(0, startingPoint.position);
		_lr.SetPosition(1, transform.position);
	}
}