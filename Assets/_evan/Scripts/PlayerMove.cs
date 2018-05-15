using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
	Rigidbody rb;
	public float speed = 10;
	public float mouseXSensitivity = 2;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		Vector3 dir = transform.forward * v + transform.right * h;
		rb.velocity = dir*speed;
		float mx = Input.GetAxis ("Mouse X");
		transform.Rotate(0,mx*mouseXSensitivity,0);
	}

	//This is a testing change

}
