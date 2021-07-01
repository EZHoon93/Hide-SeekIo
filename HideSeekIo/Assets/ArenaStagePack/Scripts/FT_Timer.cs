using UnityEngine;
using System.Collections;

public class FT_Timer {

	float _timer,_timerTarget;
	bool _timerActive,_timerFinished;

	public FT_Timer(){
		_timerFinished = true;
	}
    
	public void UpdateTimer(){

		if (_timerActive) {
			_timer+=Time.deltaTime;
			if(_timer>_timerTarget){
				_timerActive=false;
				_timerFinished=true;
			}
		}

	}

	public void StartTimer(float timerTarget){
		if (timerTarget < 0.0f) {
			_timerFinished = true;
			_timerActive = false;
		} else {
				_timer = 0.0f;
				_timerTarget = timerTarget;
				_timerFinished = false;
				_timerActive = true;
		}
	}

	public void StopTimer(){
		_timer = 0.0f;
		_timerFinished=false;
		_timerActive = false;
	}

	public bool IsActive(){
		return _timerActive;
	}

	public bool IsFinished(){
		return _timerFinished;
	}

	public float GetValue(){
		return _timer;
	}
}
