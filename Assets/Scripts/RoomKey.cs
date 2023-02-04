using UnityEngine;

public class RoomKey : MonoBehaviour {

	public RoomDoor door;

	private void OnTriggerEnter2D(Collider2D col) {
		if (!col.CompareTag("Player")) return;
		door.UnlockDoor();
		Destroy(gameObject);
	}
}