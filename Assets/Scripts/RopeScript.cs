using UnityEngine;
using System.Collections.Generic;


public class RopeScript : MonoBehaviour {
	public GameObject startPos;

	public float speed = 1;


	public float distance = 2;

	public GameObject nodePrefab;

	public GameObject player;

	public GameObject lastNode;


	private LineRenderer _lr;

	int vertexCount = 2;
	public List<GameObject> Nodes = new();

	bool done;

	// Use this for initialization
	private void Start() {
		_lr = GetComponent<LineRenderer>();
		lastNode = transform.gameObject;
		Nodes.Add(transform.gameObject);
	}

	// Update is called once per frame
	private void Update() {
		transform.position = Vector2.MoveTowards(transform.position, startPos.transform.position, speed);

		if ((Vector2)transform.position != new Vector2(startPos.transform.position.x, startPos.transform.position.y)) {
			if (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance) {
				CreateNode();
			}
		}
		else if (done == false) {
			done = true;
			while (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance) {
				CreateNode();
			}
			lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
		}
		RenderLine();
	}


	private void RenderLine() {
		_lr.positionCount = vertexCount;
		int i;
		for (i = 0; i < Nodes.Count; i++) {
			_lr.SetPosition(i, Nodes[i].transform.position);
		}
		_lr.SetPosition(i, player.transform.position);
	}


	private void CreateNode() {
		Vector2 pos2Create = player.transform.position - lastNode.transform.position;
		pos2Create.Normalize();
		pos2Create *= distance;
		pos2Create += (Vector2)lastNode.transform.position;
		
		GameObject go = Instantiate(nodePrefab, pos2Create, Quaternion.identity);
		go.transform.SetParent(transform);

		lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
		lastNode = go;

		Nodes.Add(lastNode);
		
		vertexCount++;
	}
}