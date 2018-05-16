using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBallScript : MonoBehaviour {

	public GameObject thingToCreate;
	public float initialSpeed;
	public float angleFromHorizontal;
	List<GameObject> items = new List<GameObject>();

	void Start()
	{
		Vector3 start = new Vector3 (0, 0, 0), end = new Vector3 (0, 1, 0);
		GameObject upLine = null;
		NS.Lines.MakeArrow (ref upLine, start, end, Color.red);
		GameObject arcLine = null;
		Vector3 dir = (end - start).normalized;
		NS.Lines.MakeArcArrow(ref arcLine, 270, 32, dir, start+new Vector3 (0,0,0.5f), 
			new Vector3(0,0,0), Color.red);
	}

	public void Update () 
	{
		if (Input.GetButtonDown ("Fire1")) {
			ThrowItem();
		}
	}

	public void ThrowItem() {
		Vector3 p = gameObject.transform.position+gameObject.transform.forward;
		Debug.Log (gameObject+" wants to shoot a "+thingToCreate);
		GameObject item = Instantiate (thingToCreate, p, gameObject.transform.rotation);
		Quaternion q = item.transform.rotation;
		Quaternion r = Quaternion.Euler (-angleFromHorizontal, 0, 0);
		GameObject qAxis = null, qArc = null;
		float angle;
		Vector3 axis;
		Debug.Log (q);
		r.ToAngleAxis (out angle, out axis);
		NS.Lines.MakeArrow (ref qAxis, transform.position, transform.position + q*axis);
		NS.Lines.MakeArcArrow (ref qArc, angle, 32, q*axis, 
						transform.forward, transform.position);


		item.transform.rotation = q * r;
		Rigidbody rb = item.GetComponent<Rigidbody> ();
		rb.velocity = item.transform.forward * initialSpeed;
		items.Add (item);
	}

	public void DestroyALl() {
		for (int i = 0; i < items.Count; i++) {
			Destroy (items [i]);
		}
		items.Clear ();
	}
}
