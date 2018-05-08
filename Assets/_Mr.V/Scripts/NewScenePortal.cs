using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewScenePortal : MonoBehaviour {
	[Tooltip("interacting with this portal will change to this scene (restart level if empty)")]
	public string nextScene;
	[Tooltip("how many seconds to wait before going to the next scene")]
	public float delay = 0;
	VRTK.VRTK_InteractableObject io;

	public void GoToNextScene() {
		if(nextScene != null && nextScene.Length > 0) {
			SceneManager.UnloadScene(SceneManager.GetActiveScene().name);
			SceneManager.LoadScene(nextScene);
		} else {
			SceneManager.UnloadScene(SceneManager.GetActiveScene().name);
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	void Start () {
		io = GetComponent<VRTK.VRTK_InteractableObject>();
		VRTK.InteractableObjectEventHandler ev = (sender, args) =>{
			NS.Timer.setTimeout(()=>{ GoToNextScene(); }, 0);
		};
		io.InteractableObjectUsed += ev;
		io.InteractableObjectGrabbed += ev;
	}
}
