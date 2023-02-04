using System;
using UnityEngine;

public class RoomDoor : MonoBehaviour {
	private enum DoorState { Locked, Unlocked, Open }
	private DoorState _state;
	
	private Animator _animator;
	private BoxCollider2D _bc;
	
	private static readonly int State = Animator.StringToHash("State");
	
	private bool _isInDoor;

	private void Start() {
		_animator = GetComponent<Animator>();
		_bc = GetComponent<BoxCollider2D>();
		_state = DoorState.Locked;
	}

	public void UnlockDoor() {
		if (_state != DoorState.Locked) return;
		_state = DoorState.Unlocked;
		_animator.SetInteger(State, 1);
	}

	public void OpenDoor() {
		if (_state != DoorState.Unlocked) return;
		_state = DoorState.Open;
		_animator.SetInteger(State, 2);
	}
	
	public void CloseDoor() {
		if (_state != DoorState.Open || _isInDoor) return;
		_state = DoorState.Unlocked;
		_animator.SetInteger(State, 3);
	}
	
	public void CloseLock() {
		if (_isInDoor) return;
		_state = DoorState.Locked;
		_animator.SetInteger(State, 4);
	}

	public void EnableTrigger() {
		_bc.isTrigger = true;
	}

	public void DisableTrigger() {
		_bc.isTrigger = false;
	}

	private void OnTriggerEnter2D(Collider2D col) {
		_isInDoor = true;
	}

	private void OnTriggerExit2D(Collider2D other) {
		_isInDoor = false;
	}
	
}