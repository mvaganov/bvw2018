using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableParticleSystemObject : VRTK.VRTK_InteractableObject {
	public ParticleSystem ps;
	public int triggerBurst = 50;

	void Start() {
		if (ps == null) {
			ps = GetComponent<ParticleSystem> ();
			if (ps == null) {
				ps = GetComponentInChildren<ParticleSystem> ();
			}
		}
		ps.Stop ();
		this.isGrabbable = true;
		this.isUsable = true;
	}

	public override void StartUsing(VRTK.VRTK_InteractUse usingObject) {
		base.StartUsing(usingObject);
		ps.Play ();
		ps.Emit (triggerBurst);
	}

	public override void StopUsing(VRTK.VRTK_InteractUse usingObject) {
		base.StopUsing(usingObject);
		ps.Stop ();
	}

	public override void Grabbed(VRTK.VRTK_InteractGrab currentGrabbingObject = null) {
		base.Grabbed (currentGrabbingObject);
		VRTK.VRTK_InteractUse iu = currentGrabbingObject.gameObject.GetComponent<VRTK.VRTK_InteractUse> ();
		if (iu == null) {
			iu = currentGrabbingObject.gameObject.AddComponent<VRTK.VRTK_InteractUse> ();
		}
	}

	public override void Ungrabbed(VRTK.VRTK_InteractGrab previousGrabbingObject = null) {
		base.Ungrabbed (previousGrabbingObject);
	}

	GameObject predictionLine;

	protected override void Update() {
		base.Update ();
		if (IsGrabbed ()) {
			Vector3[] prediction = new Vector3[32];
			float tdelta = 0.125f;
//			float t = tdelta;
			Rigidbody rb = GetComponent<Rigidbody> ();
			Vector3 p = transform.position;
			for (int i = 0; i < prediction.Length; ++i) {
				p += rb.velocity * tdelta;
				if (rb.useGravity) {
					p += Physics.gravity * tdelta;
				}
				prediction [i] = p;
			}
			NS.Lines.MakeArrow (ref predictionLine, prediction, prediction.Length, Color.red);
		}
	}
}