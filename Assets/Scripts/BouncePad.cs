using UnityEngine;

public class BouncePad : MonoBehaviour {
	public Vector3 direction;
	public float jumpForce = 5f;

	private void OnCollisionEnter2D(Collision2D col) {
		var rb = col.collider.gameObject.GetComponent<Rigidbody2D>();
		if (col.gameObject.CompareTag("Plug")) Jump(rb);
	}

	private void Jump(Rigidbody2D rb) {
		rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
	}
}