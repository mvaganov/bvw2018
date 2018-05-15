using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewtonsCradleCollider : MonoBehaviour {

	Rigidbody rb;

	Vector3 transferForce;
	Vector3 nextVelcity;
	NewtonsCradleCollider whoToTransferTo;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.velocity = Vector3.zero;
	}

	void Update () {
		if (rb.velocity.magnitude < 0.1 && transform.localPosition.y < -5.8) {
			transform.localPosition = new Vector3 (0, -6, 0);
			rb.velocity = Vector3.zero;
		}
	}
	/*

	void OnCollisionEnter(Collision col) {
		if (rb != null) {
			ParticleSystem ps = Singleton.Get ("particle").GetComponent<ParticleSystem>();
			Debug.Log ("collided");
			NewtonsCradleCollider otherSphere = col.gameObject.GetComponent<NewtonsCradleCollider> ();
			//if (otherSphere.whoToTransferTo == this) { // don't transfer if the other one just transfered into me
			//	return;
			//}
			whoToTransferTo = otherSphere;
//			Vector3 currentVelocity = rb.velocity;
			transferForce = rb.velocity;
			Color32 red = new Color32 (255, 0, 0, 255);

			ps.Emit (col.contacts [0].point
				//transform.position
				, transferForce, 1, 1, red);
//			col.gameObject.GetComponent<Rigidbody> ().velocity = currentVelocity;
			//whoToTransferTo = col.gameObject.GetComponent<Rigidbody> ();
			rb.velocity = Vector3.zero;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (whoToTransferTo != null) {
			whoToTransferTo.GetComponent<Rigidbody>().velocity = transferForce;
			whoToTransferTo = null;
		}
	}
	*/
}
