using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NamedEvents : MonoBehaviour {
	[System.Serializable]
	public struct NamedEvent {
		public string name;
		public UnityEvent Event;
	}

	public NamedEvent[] events;

	public int DoEvent(string named) {
		int eventsDone = 0;
		if (events != null) {
			// TODO sort events by name, then use binary search
			for (int i = 0; i < events.Length; ++i) {
				if (events [i].name == named) {
					events [i].Event.Invoke ();
					eventsDone++;
				}
			}
		}
		return eventsDone;
	}
}
