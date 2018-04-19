using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAfterFall : MonoBehaviour {
	public Vector3 startLocation;
	public float minY = -20;

	// Use this for initialization
	void Start () {
		startLocation = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < minY) {
			DoFallBehavior ();
		}
	}

	public void DoFallBehavior() {
		transform.position = startLocation;
	}

}
