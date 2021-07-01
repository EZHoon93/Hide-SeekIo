using UnityEngine;
using System.Collections;

public class FT_Cam : MonoBehaviour {
	public Transform trackedObject,trackedObjectZoom,targetCamera;
    public Vector3 offset;
	static FT_Cam myslf;
    FT_Timer shakeTimer = new FT_Timer();
	Transform currentTrackedObject;
	void Awake(){
		myslf = this;
	}
	
	void Start () {
		currentTrackedObject = trackedObject;

	}
	
	void Update () {
		targetCamera.position = Vector3.Lerp (targetCamera.position, currentTrackedObject.position, 0.05f)+offset;
		shakeTimer.UpdateTimer ();
		if (shakeTimer.IsActive())
			UpdateShake ();
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			currentTrackedObject=trackedObjectZoom;
		}
		if (Input.GetKeyUp (KeyCode.LeftShift)) {
			currentTrackedObject=trackedObject;
		}
	}
	float shakeDelay=0.03f,lastShakeTime=float.MinValue;
	void UpdateShake(){
		if (lastShakeTime + shakeDelay < Time.time) {
			Vector3 shakePosition = Vector3.zero;
			shakePosition.x += Random.Range (-0.5f, 0.5f);
			shakePosition.y += Random.Range (-0.5f, 0.5f);
			targetCamera.transform.Translate(shakePosition);
			lastShakeTime=Time.time;
		}
	}

	public static void ToggleShake(float shakeTime){
		myslf.shakeTimer.StartTimer (shakeTime);
	}
}
