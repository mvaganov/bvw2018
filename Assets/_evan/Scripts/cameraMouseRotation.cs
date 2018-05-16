using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMouseRotation : MonoBehaviour {

	float pitch,yaw;
	float distance;
	public float mouseSensitivity = 10;
	public bool mouseIsDisabled = false;
	playerControl pc;
	void Start () {
		distance = Vector3.Distance (transform.position, transform.parent.position);
		pc = transform.parent.GetComponent<playerControl> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!mouseIsDisabled) {
			float x = Input.GetAxis ("Mouse X") * mouseSensitivity;
			float y = Input.GetAxis ("Mouse Y") * mouseSensitivity;
			float w = Input.GetAxis ("Mouse ScrollWheel");
			distance += w;
			pitch += y;
			yaw += x;
			transform.rotation = Quaternion.identity;
			transform.Rotate (-pitch, yaw, 0);
		}
		transform.position = transform.parent.position - transform.forward*distance;
		transform.SetParent (null);
		pc.matchCameraLook();
		transform.SetParent (pc.transform);
	}
}
