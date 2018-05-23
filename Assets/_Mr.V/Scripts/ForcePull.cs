using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePull : MonoBehaviour {
	public float grabRadius = .25f;
	public float grabForce = 5;
	public float grabRange = 100;
	public float emitRate = 16;
	// VRTK.VRTK_ControllerEvents cEv;
	VRTK.VRTK_InteractGrab grabber;
	VRTK.VRTK_InteractUse user;
	void Start () {
		// cEv = GetComponent<VRTK.VRTK_ControllerEvents> ();
		grabber = GetComponent<VRTK.VRTK_InteractGrab> ();
		if (grabber == null) { grabber = gameObject.AddComponent<VRTK.VRTK_InteractGrab> (); }
		user = GetComponent<VRTK.VRTK_InteractUse> ();
		if (user == null) { user = gameObject.AddComponent<VRTK.VRTK_InteractUse> (); }
		emitParams.startSize = 1;
		emitParams.startLifetime = 1;
		emitParams.startColor = telekenisisParticle.main.startColor.color;
	}

	GameObject line_ray, line_target, line_hit;
	public ParticleSystem telekenisisParticle;
	private ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

	bool hadGravity;
	Rigidbody rbBeingMovedWithTheForce;

	Rigidbody GetRigidbodyFrom(GameObject go) {
		Rigidbody rb = go.GetComponent<Rigidbody> ();
		if (rb == null) {
			rb = go.GetComponentInParent<Rigidbody> ();
		}
		return rb;
	}

	float timer;
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		Rigidbody rbBeingPointedAt = null;
		Vector3 raydir = -transform.right;
		if (user.IsUseButtonPressed() && !grabber.IsGrabButtonPressed ()) {
			Vector3 camDir = transform.forward;
			if (Camera.main != null) {
				camDir = Camera.main.transform.forward;
			}
			float alignment = Vector3.Dot (raydir, camDir);
	//		NS.Lines.MakeArrow (ref line_ray, transform.position, transform.position + raydir, Color.gray);
			if (alignment > .5f) {
				RaycastHit rh = new RaycastHit ();
				Vector3 rayStart = transform.position - raydir * grabRadius;
				// get all the objects in range
				RaycastHit[] hits = Physics.SphereCastAll (rayStart, grabRadius, raydir, grabRange);
//				if (Physics.SphereCast (rayStart, grabRadius, raydir, out rh)) {
				Vector3 emitFrom = rayStart + raydir * grabRange;;
				if(hits != null) {
	//			NS.Lines.MakeCircle (ref line_hit, rh.point, rh.normal, Color.black, grabRadius);
	//			telekenisisParticle.transform.position = rh.point;
					// select the one that is the most well aligned, and closest
					int bestIndex = -1;
					float bestScore = float.PositiveInfinity;
					for (int i = 0; i < hits.Length; ++i) {
						Rigidbody rb = GetRigidbodyFrom (hits [i].collider.gameObject);
						if (rb != null) {
							Vector3 delta = hits [i].point - rayStart;
							float dist = delta.magnitude;
							float align = (1 - Vector3.Dot (raydir, delta / dist));
							float score = dist * align * align;
							if(bestIndex < 0 || score < bestScore){
								bestScore = score;
								bestIndex = i;
								rbBeingPointedAt = rb;
								emitFrom = hits [i].point;
							}
						}
					}
					// if none are selected, show where the hand is pointing
					if (timer < 0) {
						if (rbBeingPointedAt == null) {
							if (Physics.Raycast (rayStart, raydir, out rh, grabRange)) {
								emitFrom = rh.point;
							}
						}
						if(telekenisisParticle != null) {
							//telekenisisParticle.Emit (emitFrom, -raydir * 1, 1, 1, telekenisisParticle.main.startColor.color);
							emitParams.position = rh.point;
							emitParams.velocity = -raydir;
							telekenisisParticle.Emit(emitParams, 1);
						}
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
				//Debug.Log ("Stopped acting on " + rbBeingMovedWithTheForce);
			}
			rbBeingMovedWithTheForce = rbBeingPointedAt;
			if (rbBeingMovedWithTheForce != null) {
				hadGravity = rbBeingMovedWithTheForce.useGravity;
				rbBeingMovedWithTheForce.useGravity = false;
				//Debug.Log ("Acting on " + rbBeingMovedWithTheForce);
			}
		}
		float speed = 1;
		if (rbBeingMovedWithTheForce != null) {
			//NS.Lines.MakeArrow (ref line_ray, transform.position, acting.transform.position, Color.gray);
			//NS.Lines.MakeCircle(ref line_target, acting.transform.position, raydir, Color.gray, grabRadius);
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
				// telekenisisParticle.Emit (transform.position, raydir * speed, 1, 1, telekenisisParticle.main.startColor.color);
				emitParams.position = transform.position;
				emitParams.velocity = raydir * speed;
				telekenisisParticle.Emit(emitParams, 1);
			}
		}
		if (timer < 0) {
			timer = 1/emitRate;
		}
	}
}
