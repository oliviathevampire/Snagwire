using UnityEngine;

public class RoomTrigger : MonoBehaviour {
	public CameraMovement camMove;
	public Transform room;

	// Start is called before the first frame update
	private void OnTriggerEnter2D(Collider2D collision) {
		camMove.GotoRoom(room);
	}
}