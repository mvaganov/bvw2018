using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Open : MonoBehaviour {
	public GameObject leftDoor;
	public GameObject rightDoor;
	public GameObject trigger;
	public GameObject doors;
	float moved = 0;
	public float speed;
	float leftCurrent;
	float rightCurrent;
	bool triggered = false;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (moved < -5 && triggered == true) {
			leftCurrent = leftDoor.transform.position.x;
			//Debug.Log (leftCurrent + speed);
			leftDoor.transform.position = new Vector3 (leftCurrent + speed, leftDoor.transform.position.y, leftDoor.transform.position.z);

			rightCurrent = rightDoor.transform.position.x;
			rightDoor.transform.position = new Vector3 (rightCurrent - speed, rightDoor.transform.position.y, rightDoor.transform.position.z);

			moved += speed;
			//			Debug.Log (moved);
		}
		if (moved >= -4.78) {
			doors.GetComponent<AudioSource> ().mute = true;
		}
	}

	void OnTriggerEnter(Collider c){
		triggered = true;
	}


}
