using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour {
	public float timeRemaining = 10;
	public bool timerIsRunning;
	public TMP_Text timeText;

	public Action onTimerRunOut;
	private float _countdownTimer;

	private void Start() {
		// Starts the timer automatically
		timerIsRunning = false;
	}

	private void Update() {
		if (!timerIsRunning) return;
		if (timeRemaining > 0) {
			timeRemaining -= Time.deltaTime;
			DisplayTime(timeRemaining);
			
			if (timeRemaining is >= 10 or <= 0) return;
			_countdownTimer += Time.deltaTime;
			if(_countdownTimer >= 0.5) {
				timeText.color = Color.red;
			}
			if (!(_countdownTimer >= 1)) return;
			timeText.color = Color.white;
			_countdownTimer = 0;
		}
		else {
			Debug.Log("Time has run out!");
			timeRemaining = 0;
			timerIsRunning = false;
			onTimerRunOut?.Invoke();
		}
	}

	private void DisplayTime(float timeToDisplay) {
		timeToDisplay += 1;
		float minutes = Mathf.FloorToInt(timeToDisplay / 60);
		float seconds = Mathf.FloorToInt(timeToDisplay % 60);
		timeText.text = $"{minutes:00}:{seconds:00}";
	}
}