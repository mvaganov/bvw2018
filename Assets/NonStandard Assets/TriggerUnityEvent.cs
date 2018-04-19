using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NS {
	public class TriggerUnityEvent : TriggerBase {
		public float delayInSeconds = 0;
		public UnityEngine.Events.UnityEvent whatToTrigger;

		void Start () { AddTriggers (gameObject, whatToTrigger, kind, triggerTag, true, delayInSeconds); }
		public void DoActivateTrigger () {
			DoActivateTrigger (gameObject, whatToTrigger, gameObject, true, delayInSeconds);
		}
	}
}