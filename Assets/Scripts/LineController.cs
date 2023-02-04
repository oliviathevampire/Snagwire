using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LineController : MonoBehaviour {
	public Transform startingPoint;
	public DistanceJoint2D ropeJoint;
	public LayerMask ropeLayerMask;
	public float maxDistance;
	public GameObject anchorPosition;

	public Timer timer;
	public bool plugged;

	private LineRenderer _lr;
	private readonly List<(Vector2 pos, Vector2 normal)> _ropePositions = new();
	private float _currentDistance;

	private void Start() {
		_lr = GetComponent<LineRenderer>();
		if (!plugged) timer.onTimerRunOut += RollBack;
	}

	private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider) {
		var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
			position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
			position => polyCollider.transform.TransformPoint(position));

		var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
		return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
	}

	private void RollBack() {
		maxDistance = 0f;
	}

	private void OnDrawGizmos() {
		if(_ropePositions.Count == 0) return;
		var (pos, dir) = _ropePositions[^1];
		Gizmos.color = Color.red;
		Gizmos.DrawRay(pos, dir);
		
	}

	private void Update() {
		if (_ropePositions.Count >= 1) {
			for (var i = _ropePositions.Count - 1; i >= 0; i--) {
				var (pos, normal) = _ropePositions[i];
				if (Vector2.Dot(pos-(Vector2)transform.position, normal) >= 0) {
					break;
				}

				var prevPosition = i == 0 ? (Vector2)startingPoint.position : _ropePositions[i - 1].pos;
				_currentDistance -= (prevPosition - pos).magnitude;
				_ropePositions.RemoveAt(i);
			}
		}
		
		var lastRopePoint = _ropePositions.Count == 0 ? (Vector2)startingPoint.position : _ropePositions[^1].pos;
		var dir = lastRopePoint - (Vector2)transform.position;
		var playerToCurrentNextHit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude - 0.1f, ropeLayerMask);
		if (playerToCurrentNextHit) {
			var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
			if (colliderWithVertices != null) {
				var closestPointToHit =
					GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

				var cc = Vector2.SignedAngle(closestPointToHit - lastRopePoint, dir) > 0;
				var normal = closestPointToHit - lastRopePoint;
				if (cc) {
					(normal.x, normal.y) = (-normal.y, normal.x);
				}
				else {
					(normal.x, normal.y) = (normal.y, -normal.x);
				}
					
				_ropePositions.Add((closestPointToHit, normal));
				_currentDistance += (closestPointToHit - lastRopePoint).magnitude;
				lastRopePoint = closestPointToHit;
			}
		}

		if (_ropePositions.Count == 0) {
			_currentDistance = 0;
		}
		
		anchorPosition.transform.position = lastRopePoint;
		ropeJoint.distance = maxDistance - _currentDistance;

		UpdateRopePositions();
	}

	private void UpdateRopePositions() {
		_lr.positionCount = _ropePositions.Count + 2;
		_lr.SetPosition(0, startingPoint.position);
		_lr.SetPosition(_lr.positionCount - 1, transform.position);
		for (var i = 0; i < _ropePositions.Count; i++) {
			_lr.SetPosition(i+1, _ropePositions[i].pos);
		}
	}
}