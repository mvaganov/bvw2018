using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limitdistance : MonoBehaviour {

	public GameObject target;
	public float maximum;
	GameObject Line_velocity, Line_line;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Rigidbody rb = target.GetComponent<Rigidbody> ();
		NS.Lines.MakeArrow (ref Line_velocity, target.transform.position,target.transform.position + rb.velocity, Color.green);
		NS.Lines.MakeArrow (ref Line_line, transform.position,target.transform.position, Color.black);

		float d = Vector3.Distance (transform.position, target.transform.position);
		if (d > maximum) {
			Vector3 delta = target.transform.position - transform.position;
			target.transform.position = transform.position + delta.normalized * maximum;
			float pull = Vector3.Dot (delta.normalized, rb.velocity);
			rb.velocity -= delta.normalized * pull;
		}
	}
}
