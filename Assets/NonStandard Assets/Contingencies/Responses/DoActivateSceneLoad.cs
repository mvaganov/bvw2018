using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace NS.Contingency.Response {
	public class DoActivateSceneLoad : _NS.Contingency.Response.DoActivateBasedOnContingency {
		public string sceneName;
		public bool additive = false;
		public EditorGUIObjectReference whatToActivateWhenDone;
		public void DoActivateTrigger () { DoActivateTrigger(null); }
		public void DoActivateTrigger (object whatTriggeredThis) {
			AsyncOperation op = SceneManager.LoadSceneAsync(sceneName,
				additive?LoadSceneMode.Additive:LoadSceneMode.Single);
			if(whatToActivateWhenDone.data != null) {
				op.completed += (AsyncOperation a)=> {
					NS.F.DoActivate(whatToActivateWhenDone, whatTriggeredThis, this, true);
				};
			}
		}
		public void DoDeactivateTrigger(object whatTriggeredThis) {
			AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
			if(whatToActivateWhenDone.data != null) {
				op.completed += (AsyncOperation a)=> {
					NS.F.DoActivate(whatToActivateWhenDone, whatTriggeredThis, this, false);
				};
			}
		}
	}
}
