namespace VRTK {
	
using UnityEngine;

public class Basketball : VRTK_InteractableObject {

	public Material heldMaterial;
	Material oldMaterial;

	void Start () {
		oldMaterial = GetComponent<Renderer> ().material;	
	}
	/*
	public override void  Grabbed(VRTK_InteractGrab currentGrabbingObject = null) {
		base.Grabbed(currentGrabbingObject = null);
		GetComponent<Renderer> ().material = heldMaterial;
		Debug.Log ("Touched");
	}

	public override void  Ungrabbed(GameObject previousGrabbingObject) {
		base.Ungrabbed(previousGrabbingObject);
		GetComponent<Renderer> ().material = oldMaterial;
		Debug.Log ("Dropped");
	} */

		public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
		{
			GetComponent<Renderer> ().material = heldMaterial;
			Debug.Log ("Touched");
			base.OnInteractableObjectGrabbed (e);

		}

		public override void OnInteractableObjectUntouched(InteractableObjectEventArgs e)
		{
			GetComponent<Renderer> ().material = oldMaterial;
			Debug.Log ("Untouched");
			base.OnInteractableObjectUngrabbed (e);
		}
}
}
