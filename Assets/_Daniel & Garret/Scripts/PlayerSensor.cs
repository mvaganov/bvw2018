using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour {
	public GameObject target;
	bool weCanSeeTarget = false;
	public bool goToLastSeen = true;
	Rigidbody rb;
	Vector3 lastPlaceTargetWasSeen;

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 delta = target.transform.position - transform.position;
		float dist = delta.magnitude;
		Vector3 dir = delta / dist;
		Ray r = new Ray (transform.position, dir);
		RaycastHit rh = new RaycastHit ();
		weCanSeeTarget = false;
		if (Physics.Raycast (r, out rh) == true) {
			if (rh.distance >= dist - 1) {
				weCanSeeTarget = false;
				lastPlaceTargetWasSeen = target.transform.position;
			}
		}
		if (!weCanSeeTarget) {
			if (goToLastSeen) {
				dir = (lastPlaceTargetWasSeen - transform.position).normalized;
			} else {
				dir = rb.velocity * (10 - Time.deltaTime);
			}
		}
		transform.LookAt (lastPlaceTargetWasSeen);
		rb.velocity = dir * 5;
		NS.Lines.MakeArrow (ref line_v, transform.position, transform.position + rb.velocity, Color.red);
	}
	GameObject line_v;
}
