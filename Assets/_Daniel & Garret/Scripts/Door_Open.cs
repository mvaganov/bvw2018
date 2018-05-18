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
		if (triggered == true) {
			Debug.Log ("hi");
		}
		if (moved < 5 && triggered == true) {
			doors.SetActive (true);
			leftCurrent = leftDoor.transform.position.x;
			//Debug.Log (leftCurrent + speed);
			leftDoor.transform.Translate(-speed,0,0);

			rightCurrent = rightDoor.transform.position.x;
			rightDoor.transform.Translate(speed,0,0);

			moved += speed;
		}
		if (moved >=4.78) {
			doors.GetComponent<AudioSource> ().mute = true;
		}
	}

	void OnTriggerEnter(Collider c){
		triggered = true;
	}


}
