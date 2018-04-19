using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakApartIndividualColliders : MonoBehaviour {

	static void GatherChildren(Transform t, List<Transform> childCollidersToBreak) {
		Collider[] c = t.GetComponents<Collider> ();
		if (c != null && c.Length > 0) {
			childCollidersToBreak.Add (t);
		}
		for (int i = 0; i < t.childCount; ++i) {
			GatherChildren (t.GetChild(i), childCollidersToBreak);
		}
	}
	static void Collapse(Transform t) {
		t.SetParent (null);
		Collider[] c = t.GetComponents<Collider> ();
		Rigidbody rb = t.GetComponent<Rigidbody> ();
		if (rb == null) {
			t.gameObject.AddComponent<Rigidbody> ();
		}
		for (int i = 0; i < c.Length; ++i) {
			c [i].enabled = true;
		}
	}

	public static void ExecuteOn(Transform t){
		List<Transform> childrenToDisconnect = new List<Transform>();
		GatherChildren (t, childrenToDisconnect);
		System.Action freedom = null;
		freedom = () => {
			if(childrenToDisconnect.Count > 0){
				int lastIndex = childrenToDisconnect.Count-1;
				Collapse(childrenToDisconnect[lastIndex]);
				childrenToDisconnect.RemoveAt(lastIndex);
				NS.Timer.setTimeout (freedom, 1);
			}
		};
		NS.Timer.setTimeout (freedom, 1);
	}

	public void DoActivateTrigger() {
		ExecuteOn (transform);
	}
}
