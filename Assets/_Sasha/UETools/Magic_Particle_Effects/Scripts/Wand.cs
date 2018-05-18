using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour {
	VRTK.VRTK_InteractableObject io;
	public ParticleSystem particles; 
	// Use this for initialization
	void Start () {

		io = GetComponent < VRTK.VRTK_InteractableObject> ();
		if (io == null) {
			io = gameObject.AddComponent<VRTK.VRTK_InteractableObject> ();
		}
		io.isUsable = io.isGrabbable = true;
		if (particles == null) {
			particles = GetComponentInChildren<ParticleSystem> ();
		}
		particles.Stop ();
		io.InteractableObjectUsed += (sender,EventArgs) => {particles.Play(); };
		io.InteractableObjectUnused += (sender,EventArgs) => {particles.Stop(); };
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
