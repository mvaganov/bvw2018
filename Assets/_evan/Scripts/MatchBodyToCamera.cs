//#define SHOWLINES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchBodyToCamera : MonoBehaviour {
	public Transform cam;
	public Vector3 up = Vector3.up;
	// Use this for initialization
	void Start () {}
	#if SHOWLINES
	GameObject line_1, line_2, line_3;
	#endif
	// Update is called once per frame
	void Update () {
		if (cam == null) {
			cam = Camera.main.transform;
		} else {
			Vector3 right = transform.right;
			if (up != cam.up && up != -cam.up) {
				right = Vector3.Cross (up, cam.up);
			}
			right.Normalize ();
			Vector3 forward = Vector3.Cross (right, up);
			forward.Normalize ();
			transform.rotation = Quaternion.LookRotation (forward, up);
			#if SHOWLINES
			Lines.Make (ref line_3, transform.position, transform.position + up, Color.green);
			Lines.Make (ref line_2, transform.position, transform.position + forward, Color.blue);
			Lines.Make (ref line_1, transform.position, transform.position + right, Color.red);
			#endif
		}
	}
}
