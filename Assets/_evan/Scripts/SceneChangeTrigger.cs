using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour {
	public string nextScene;

	/*void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			SceneManager.LoadScene (nextScene);
		}
	}*/

	public void GoToNextScene(){
		SceneManager.LoadScene (nextScene);
	}
}
