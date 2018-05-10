using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemActivate : MonoBehaviour {

	public ParticleSystem RewindParticle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void ActivateParticleSystem()
	{
		if (RewindParticle.isPlaying) {
			RewindParticle.Stop ();
		} else {
			RewindParticle.Play ();
		}
	}
}
