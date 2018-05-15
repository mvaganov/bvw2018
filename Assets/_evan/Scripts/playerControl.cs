using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControl : MonoBehaviour {
	Rigidbody rb;
	public Camera myCamera;
	public float bodySpeed = 10;
	void Start () {
		rb = GetComponent<Rigidbody> ();
		if (myCamera == null) {
			myCamera = Camera.main;
		}
		if (myCamera == null) {
			GameObject camobj = new GameObject ("<Camera>");
			myCamera = camobj.AddComponent<Camera> ();
			camobj.AddComponent<FlareLayer> ();
			camobj.AddComponent<AudioListener> ();
			camobj.AddComponent<cameraMouseRotation> ();
			camobj.transform.SetParent (transform);
			camobj.transform.localPosition = new Vector3 (0, 0, -10);
			CmdLine.Instance.transform.SetParent (camobj.transform);
			CmdLine.Instance.transform.localPosition = new Vector3 (0, 0, 2.5f);
		}
		
	}

	GameObject line_forward;
	GameObject line_velocity;
	void Update () {
		float h = Input.GetAxis ("Horizontal")*bodySpeed;
		float v = Input.GetAxis ("Vertical")*bodySpeed;
		bool jump = Input.GetButtonDown ("Jump");
		Vector3 movement = rb.velocity;
		float gravitySpeed = Vector3.Dot(Vector3.down, rb.velocity);
		movement = Vector3.down*gravitySpeed + transform.forward * v + transform.right * h;
		if (jump) {
			movement.y += 3;
		}
		rb.velocity = movement;

		NS.Lines.MakeArrow (ref line_velocity, transform.position, 
			transform.position + rb.velocity, Color.magenta);
	}

	public void matchCameraLook() {
		Vector3 myRotation = Vector3.Cross(myCamera.transform.right, Vector3.up);
		NS.Lines.Make(ref line_forward, transform.position, transform.position + myRotation, Color.green);
		transform.rotation = Quaternion.LookRotation (myRotation.normalized, Vector3.up);
	}
}
