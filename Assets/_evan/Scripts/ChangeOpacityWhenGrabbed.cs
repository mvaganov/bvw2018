using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOpacityWhenGrabbed : VRTK.GrabAttachMechanics.VRTK_FixedJointGrabAttach {
	public Material heldMaterial;
	Material oldMaterial;

	protected override void Initialise(){
		Debug.Log ("Initialise");
		oldMaterial = GetComponent<Renderer> ().material;

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint) {
		GetComponent<Renderer> ().material = heldMaterial;
		Debug.Log ("Grabbed");
		return base.StartGrab (grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
	}
	public override void StopGrab(bool applyGrabbingObjectVelocity) {
		base.StopGrab (applyGrabbingObjectVelocity);
		GetComponent<Renderer> ().material = oldMaterial;
		Debug.Log ("dropped");
	}
}
