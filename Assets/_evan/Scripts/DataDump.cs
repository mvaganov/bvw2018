using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDump : MonoBehaviour {

	// Use this for initialization
	void Start () {
		NS.Timer.setTimeout (DoTest, 3000);
	}

	void DoTest()
	{
		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();
		foreach (object g in allObjects) {
			GameObject go = g as GameObject;
			if (go != null && go.activeInHierarchy) {
				int maxChar = Mathf.Min (20, go.ToString ().Length);
				CmdLine.Log (go.ToString ().Substring (0, maxChar));
				Component[] comps = go.transform.GetComponents<Component> ();
				for (int c = 0; c < comps.Length; ++c) {
					CmdLine.Log ("  " + comps [c].GetType ().Name);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
