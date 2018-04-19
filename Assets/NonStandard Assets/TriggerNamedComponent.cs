using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NS {
	public class TriggerNamedComponent : MonoBehaviour {
		public string componentName;
		public float delayInSeconds = 0;
		void OnTriggerEnter (Collider col) {
			GameObject go = col.gameObject;
			Object comp = go.GetComponent (componentName);
			if (comp != null) {
				Trigger.DoActivateTrigger (go, comp, gameObject, true, delayInSeconds);
//			} else {
//				Debug.Log (go + " does not have a \"" + componentName + "\" component");
			}
		}
	}
}