using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathfindr : MonoBehaviour {
	public GameObject target;
	public float speed = 2;
	float radius;
	bool weCanSeeTarget = false, arrived = false;
	public bool showLines = false;
	public bool goToLastSeenLocation = true;
	Rigidbody rb;
	Vector3 lastPlaceTargetWasSeen;
	public Vector3 gravity = Vector3.down;
	// Use this for initialization
	void Start () {
		Debug.Log ("Starting Script");
		lastPlaceTargetWasSeen = transform.position;
		rb = this.GetComponent<Rigidbody> ();
		radius = GetComponent<Collider> ().bounds.extents.x;
	}	
	void FixedUpdate () {
		Vector3 delta = target.transform.position - transform.position;
		float dist = delta.magnitude;
		Vector3 dir = delta / dist; // same as delta.normalized
		float stopDistance = speed;
		Ray r = new Ray(transform.position, dir);
		RaycastHit rh = new RaycastHit ();
		weCanSeeTarget = false;
		if (Physics.Raycast (r, out rh) == true) {
			if (rh.distance >= dist-1) {
				weCanSeeTarget = true;
				lastPlaceTargetWasSeen = target.transform.position;
			}
		}
		arrived = dist <= radius * 2;
		if (!weCanSeeTarget) {
			if (goToLastSeenLocation && !arrived) {
				dir = (lastPlaceTargetWasSeen - transform.position).normalized;
			} else {
				dir = rb.velocity * (1 - Time.deltaTime); // slow down to a stop
			}
		}
		// if we are moving, and there are obstacles in the way, move away from the obstacles
		if(stopDistance != 0) {
			Collider c = GetComponent<Collider> (); c.enabled = false; // ignore this collider when checking if there are colliders in the way
			if (Physics.SphereCast (transform.position, radius, dir, out rh, stopDistance)) {
				float distFromPlayer = Vector3.Distance (rh.point, target.transform.position);
				if (distFromPlayer > radius + 0.125f) {
					float amountIntoDir = Vector3.Dot (dir, rh.normal);
					dir -= rh.normal * amountIntoDir;
					dir.Normalize ();
				}
			}
			c.enabled = true;
		}
		// point at the target, along the ground
		Vector3 right = Vector3.Cross(dir, gravity);
		Vector3 forw = Vector3.Cross (right, -gravity);
		if (forw != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation (forw, -gravity);
		}
		// move toward the target, keeping gravity in mind
		float gravForce = Vector3.Dot(rb.velocity, gravity);
		if (gravForce < 0) { gravForce = 0; }
		rb.velocity = forw*speed + gravity * gravForce;
		if (showLines) {
			NS.Lines.MakeArrow (ref line_right, transform.position, transform.position + right, Color.red);
			NS.Lines.MakeArrow (ref line_grav, transform.position, transform.position + gravity, Color.green);
			NS.Lines.Make (ref line_ray, r.origin, lastPlaceTargetWasSeen, Color.magenta);
			NS.Lines.MakeArrow (ref line_velocity, transform.position, transform.position + rb.velocity, Color.blue);
		}
	}
	GameObject line_ray, line_velocity, line_grav, line_right;
}

