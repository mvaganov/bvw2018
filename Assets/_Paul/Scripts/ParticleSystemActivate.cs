using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemActivate : MonoBehaviour {

	public GameObject DoctorStrangeParticle;
	public bool isRewinding = false;

	// Use this for initialization
	void Start () {
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (isRewinding = true)
			DoctorStrangeParticle.gameObject.SetActive(true);
		if (isRewinding = false)
			DoctorStrangeParticle.gameObject.SetActive(false);
	}
}
