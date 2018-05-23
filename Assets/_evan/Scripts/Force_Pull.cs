using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force_Pull : MonoBehaviour {
	public float grabRadius = 1;
	public float grabForce = .25f;
	public Color color = Color.cyan;
	// VRTK.VRTK_ControllerEvents cEv;
	VRTK.VRTK_InteractGrab grabber;
	VRTK.VRTK_InteractUse user;
	// Use this for initialization
	void Start () {
		// cEv = GetComponent<VRTK.VRTK_ControllerEvents> ();
		grabber = GetComponent<VRTK.VRTK_InteractGrab> ();
		user = GetComponent<VRTK.VRTK_InteractUse> ();
		emitParams.startSize = 1;
		emitParams.startLifetime = 1;
		emitParams.startColor = color;
	}

	GameObject line_ray, line_target, line_hit;
	public ParticleSystem telekenisisParticle;
	private ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

	bool hadGravity;
	Rigidbody rbBeingMovedWithTheForce;


	float timer;
	// Update is called once per frame
	void Update () {
//		Debug.Log ("force push is updating");
		timer -= Time.deltaTime;
		Rigidbody rbBeingPointedAt = null;
		Vector3 raydir = -transform.right;
		if (user.IsUseButtonPressed() && !grabber.IsGrabButtonPressed ()) {
			Vector3 camDir = transform.forward;
			if (Camera.main != null) {
				camDir = Camera.main.transform.forward;
			}
			float alignment = Vector3.Dot (raydir, camDir);
			//NS.Lines.MakeArrow (ref line_ray, transform.position, transform.position + raydir*100, color, grabRadius, grabRadius);
			if (alignment > .5f) {
				RaycastHit rh = new RaycastHit ();
				if (Physics.SphereCast (transform.position - raydir * grabRadius, grabRadius, raydir, out rh)) {
	//			NS.Lines.MakeCircle (ref line_hit, rh.point, rh.normal, Color.black, grabRadius);
	//			telekenisisParticle.transform.position = rh.point;
					if (timer < 0) {
						// telekenisisParticle.Emit (rh.point, -raydir * 1, 1, 1, color);
						emitParams.position = rh.point;
						emitParams.velocity = -raydir;
						telekenisisParticle.Emit(emitParams, 1);
					}
					rbBeingPointedAt = rh.collider.GetComponent<Rigidbody> ();
					if (rbBeingPointedAt == null) {
						rbBeingPointedAt = rh.collider.GetComponentInParent<Rigidbody> ();
					}
				}
			}
		}
		if (rbBeingPointedAt != rbBeingMovedWithTheForce) {
			if (rbBeingMovedWithTheForce != null) {
				if (hadGravity) {
					rbBeingMovedWithTheForce.useGravity = true;
				}
				rbBeingMovedWithTheForce = null;
			}
			if (rbBeingPointedAt == null) {
				Debug.Log ("Stopped acting on " + rbBeingMovedWithTheForce);
			}
			rbBeingMovedWithTheForce = rbBeingPointedAt;
			if (rbBeingMovedWithTheForce != null) {
				hadGravity = rbBeingMovedWithTheForce.useGravity;
				rbBeingMovedWithTheForce.useGravity = false;
				Debug.Log ("Acting on " + rbBeingMovedWithTheForce);
			}
		}
		float speed = 1;
		if (rbBeingMovedWithTheForce != null) {
			//NS.Lines.MakeArrow (ref line_ray, transform.position, rbBeingMovedWithTheForce.transform.position, Color.cyan);
//			NS.Lines.MakeCircle(ref line_target, acting.transform.position, raydir, color, grabRadius);
			//telekenisisParticle.transform.position = rbBeingMovedWithTheForce.transform.position;
			Vector3 delta = transform.position - rbBeingMovedWithTheForce.position;
			float dist = delta.magnitude;
			//Vector3 dir = delta / dist;
			if (dist > 1) {
				speed = dist;
			}
			Vector3 idealV = -raydir * speed;
			Vector3 accel = idealV - rbBeingMovedWithTheForce.velocity;
			rbBeingMovedWithTheForce.velocity += accel.normalized * grabForce * Time.deltaTime;
			if (timer < 0) {
				// telekenisisParticle.Emit (transform.position, raydir * speed, 1, 1, color);
				emitParams.position = transform.position;
				emitParams.velocity = raydir * speed;
				telekenisisParticle.Emit(emitParams, 1);
			}
		}

		if (timer < 0) {
			timer = 1/16.0f;
		}
	}
}
