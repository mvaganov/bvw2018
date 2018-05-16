using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DidScoreTrigger : MonoBehaviour {

	public TMPro.TextMeshPro output;
	public string HomeGuest;
	private int score = 0;
	public ParticleSystem confetti;

	void Start() {
		output.text = HomeGuest + " :" + (score);
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<VRTK.Basketball> () != null) {
			Debug.Log (HomeGuest + " Scored!");
			output.text = HomeGuest + " :" + (++score);
			confetti.Emit (50);
		}
	}
}
