using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleList : MonoBehaviour {
	public TimeBody isRewinding;
	public List<GameObject> RewindableObjects;
	public void TurnThemOff() {
		for (int i = 0; i < RewindableObjects.Count; i++) {
			TimeBody t = RewindableObjects [i].GetComponent<TimeBody> ();
			t.StopRewind ();
		}
	}
	public void TurnThemOn(){
		for(int i = 0; i < RewindableObjects.Count; i++) {
			TimeBody t = RewindableObjects [i].GetComponent<TimeBody> ();
			t.StartRewind();
		}
	}
	void Update (){
	}
}
