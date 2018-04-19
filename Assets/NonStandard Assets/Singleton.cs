using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// author: mvaganov@hotmail.com
// license: Copyfree, public domain. This is free code! Great artists, steal this code!
// latest version at: https://pastebin.com/raw/JMme8A2X
// works well with SceneCombiner: https://pastebin.com/raw/8cZ5yKSd
namespace NS {
	public class Singleton : MonoBehaviour {
		[Tooltip("If another Singleton object with this onlyAllowOne is added to the scene, it will Destroy itself after starting.\nWorks well with SceneCombiner.")]
		public string onlyAllowOne = "";
		[Tooltip("Mark true to keep this object in the scene when changing to another scene.\nThis can be a tricky design, be careful!")]
		public bool dontDestroyOnLoad = true;
		void Start () {
			bool alreadyHere = false;
			GameObject[] objs = FindObjectsOfType<GameObject> ();
			for (int i = 0; i < objs.Length && !alreadyHere; ++i) {
				Singleton[] singles = objs [i].GetComponents<Singleton> ();
				if (singles != null && singles.Length > 0) {
					for (int d = 0; d < singles.Length; ++d) {
						// if one of me already exists...
						if (singles [d] != this && ((onlyAllowOne != "" && singles [d].onlyAllowOne == onlyAllowOne)
												|| (onlyAllowOne == "" && singles [d].gameObject.name == gameObject.name))) {
							alreadyHere = true;
							break;
						}
					}
				}
			}
			if (alreadyHere) {
				Debug.Log("\'"+onlyAllowOne+"\' Singleton already exists, removing \'"+gameObject+"\'");
				gameObject.SetActive (false);
				onlyAllowOne = null;
				Destroy (gameObject);
			} else if(dontDestroyOnLoad) {
				DontDestroyOnLoad (transform.gameObject);
			}
		}

		private static Dictionary<System.Type, Component> instances = new Dictionary<System.Type, Component>();
		public static T GetComponentInstance<T>() where T : Component {
			Component instance = null;
			if(!instances.TryGetValue(typeof(T), out instance)) {
				T[] searchInstance = GameObject.FindObjectsOfType<T> ();
				if (searchInstance != null) {
					if (searchInstance.Length > 1) {
						throw new System.Exception ("multiple instances of " + typeof(T).Name + " found, should be singleton");
					} else if (searchInstance.Length != 0) {
						instance = searchInstance [0];
					}
				}
				if(instance == null) {
					GameObject g = new GameObject();
					instance = g.AddComponent<T>();
					g.name = "<" + instance.GetType().Name + ">";
					instances [typeof(T)] = instance;
				}
			}
			return instance as T;
		}
	}
}