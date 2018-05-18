using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachTo : MonoBehaviour {
	[System.Serializable]
	public struct IdealParent {
		public string name;
		public Vector3 offset;
	}
	public IdealParent[] idealParent;

	void FixedUpdate () {
		for (int i = 0; i < idealParent.Length; ++i) {
			GameObject go = GameObject.Find (idealParent[i].name);
			if (go != null) {
				transform.parent = go.transform;
				transform.localPosition = idealParent[i].offset;
				Destroy (this);
			}
		}
	}
}
