using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public Transform player;
	public Transform startRoom;

	public float smooth;

	private float _currentY;
	private float _currentX;

	private float _lowestY;

	private Vector3 _velocity = Vector3.zero;

	// Start is called before the first frame update
	private void Awake() {
		_lowestY = transform.position.y;
		_currentX = 0;
		if (startRoom != null) GotoRoom(startRoom);
	}

	public void GotoRoom(Transform room) {
		_currentX = room.position.x;
	}

	// Update is called once per frame
	private void Update() {
		_currentY = player.position.y < _lowestY ? _lowestY : player.position.y;
		_currentX = player.position.x;
		var position = transform.position;
		position = Vector3.SmoothDamp(position,
			new Vector3(_currentX, _currentY, position.z), ref _velocity, smooth);
		transform.position = position;
	}
}