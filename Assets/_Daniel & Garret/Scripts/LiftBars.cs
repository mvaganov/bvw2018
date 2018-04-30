using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftBars : MonoBehaviour {
	public GameObject[] bars;
	public int wait;
	float timePassed = 0;
	double moved;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timePassed += Time.deltaTime;
		if (timePassed >= wait && moved <= 10) {
			for (int i = 0; i < bars.Length; i++) {
				bars [i].transform.Translate (0f, 0.1f, 0f);
				moved += 0.025;
			}
		}
	}
}
