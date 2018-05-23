using UnityEngine;
namespace NS.Contingency {
	public class ContingentOnKeyDown : ContingentScript {
		public KeyCode key;
		void FixedUpdate() {
			if(Input.GetKeyDown(key)) { DoActivateTrigger(); }
		}
	}
}