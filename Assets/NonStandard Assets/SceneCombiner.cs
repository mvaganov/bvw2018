#if UNITY_EDITOR
// TODO create a non UnityEditor dependent class to handle SceneAssets
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// author: mvaganov@hotmail.com
// license: Copyfree, public domain. This is free code! Great artists, steal this code!
// latest version at: https://pastebin.com/raw/8cZ5yKSd
// works well with Singleton: https://pastebin.com/raw/JMme8A2X
namespace NS {
	public class SceneCombiner : MonoBehaviour {
		[Tooltip("What scenes to combine together.\nConsider having objects with 'Singleton' in the Scenes to prevent duplication.")]
		public UnityEditor.SceneAsset[] scenesToAdd;
		// Use this for initialization
		void Start () {
			for (int i = 0; i < scenesToAdd.Length; ++i) {
				SceneManager.LoadScene (scenesToAdd [i].name, LoadSceneMode.Additive);
			}
		}
	}
}
#endif