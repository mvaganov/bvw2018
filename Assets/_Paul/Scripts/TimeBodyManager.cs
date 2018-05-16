using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TimeBodyManager : MonoBehaviour {

	VRTK.VRTK_InteractableObject io;
	public UnityEvent whatToDoWhenUsed;

	public UnityEvent whatToDoWhenUnused;

	// Use this for initialization
	void Start () {
		io = GetComponent<VRTK.VRTK_InteractableObject> ();
		if (io == null) {
			io = gameObject.AddComponent<VRTK.VRTK_InteractableObject> ();
		}
		io.isGrabbable = io.isUsable = true;
		io.InteractableObjectUsed += (sender, args) => {
			//Debug.Log("use");
			whatToDoWhenUsed.Invoke ();
		};
		io.InteractableObjectUnused += (sender, args) => {
			//Debug.Log("un use");
			whatToDoWhenUnused.Invoke ();
		};
	}
}
