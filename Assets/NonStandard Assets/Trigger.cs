using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// author: mvaganov@hotmail.com
// license: Copyfree, public domain. This is free code! Great artists, steal this code!
// latest version at: https://pastebin.com/raw/8cZ5yKSd
// requires Timer at: https://pastebin.com/raw/h61nAC3E
namespace NS {
	public class Trigger : TriggerBase {
		public float delayInSeconds = 0;
		[Tooltip("* Transform: teleport triggering object to the Transform\n"+
			"* SceneAsset: load the scene\n"+
			"* AudioClip: play audio here\n"+
			"* GameObject: SetActivate(true)\n"+
			"* Material: set this Renderer's .material property, make a note of what the previous material was\n"+
			"* <any other>: activate a \'DoActivateTrigger()\' method (if available)\n"+
			"* IEnumerable: trigger each element in the list")]
		public Object whatToTrigger;
		[Tooltip("If false, *deactivate* whatToTrigger instead")]
		public bool activate = true;

		void Start () { AddTriggers (gameObject, whatToTrigger, kind, triggerTag, activate, delayInSeconds); }

		public void DoActivateTrigger (){ DoActivateTrigger (gameObject, whatToTrigger, gameObject, activate, delayInSeconds); }
	}
	[RequireComponent(typeof(Collider))]
	public class TriggerBase : MonoBehaviour {

		public enum TKind {
			scriptedOnly,
			onTriggerEnter, onTriggerExit, onTriggerStay, 
			onCollisionEnter, onCollisionExit, onCollisionStay, 
			onControllerColliderHit,
			onStart, onDestroy, onEnable, onDisable, onApplicationPause, onApplicationUnpause, onApplicationQuit, 
			onMouseEnter, onMouseOver, onMouseExit,
			onMouseDown, onMouseDrag, onMouseUp, onMouseUpAsButton,
			onBecameInvisible, onBecameVisible,
			// TODO add: onVREnter (can be hands or head), onVRExit, onVRPress (can be any hand control button), onVRRelease, onVRPointStart, onVRPointStop, onVRSwipe
		}
		[Tooltip("What triggers the object above")]
		public TKind kind = TKind.onTriggerEnter;
		[Tooltip("For colliders, if not empty, only trigger if the triggering collider is tagged with this value")]
		public string triggerTag;

		public static void DoActivateTrigger(object triggeringObject, object whatToTrigger, GameObject triggeredGameObject, bool activate, float delayInSeconds) {
			if (delayInSeconds <= 0) {
				DoActivateTrigger (triggeringObject, whatToTrigger, triggeredGameObject, activate);
			} else {
				NS.Timer.setTimeout (() => {
					DoActivateTrigger (triggeringObject, whatToTrigger, triggeredGameObject, activate);
				}, (long)(delayInSeconds * 1000));
			}
		}
		public static void DoActivateTrigger(object triggeringObject, object whatToTrigger, GameObject triggeredGameObject, bool activate) {
			if (whatToTrigger == null) { Debug.LogError ("Don't know how to trigger null"); return; }
			System.Type type = whatToTrigger.GetType ();
			if (typeof(System.Action).IsAssignableFrom (type)) {
				System.Action a = whatToTrigger as System.Action;
				a.Invoke ();
			} else if (typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom (type)) {
				UnityEngine.Events.UnityEvent a = whatToTrigger as UnityEngine.Events.UnityEvent;
				a.Invoke ();
			} else if (type == typeof(Transform)) {
				Transform t = whatToTrigger as Transform;
				Transform toMove = triggeringObject as Transform;
				if (toMove == null) {
					GameObject go = triggeringObject as GameObject;
					if (go != null) {
						toMove = go.transform;
					}
				}
				if (toMove != null) {
					toMove.position = t.position;
				}
			} else if (type == typeof(UnityEditor.SceneAsset)) {
				UnityEditor.SceneAsset scene = whatToTrigger as UnityEditor.SceneAsset;
				LoadLevel (scene.name, activate);
			} else if (type == typeof(AudioClip) || type == typeof(AudioSource)) {
				AudioSource asource = null;
				if (type == typeof(AudioSource)) {
					asource = whatToTrigger as AudioSource;
				}
				if (asource == null) {
					asource = triggeredGameObject.AddComponent<AudioSource> ();
				}
				if (type == typeof(AudioClip)) {
					asource.clip = whatToTrigger as AudioClip;
				}
				if (activate) {
					asource.Play ();
				} else {
					asource.Stop ();
				}
			} else if (type == typeof(ParticleSystem)) {
				ParticleSystem ps = whatToTrigger as ParticleSystem;
				if (activate) {
					Transform t = ps.transform;
					t.position = triggeredGameObject.transform.position;
					t.rotation = triggeredGameObject.transform.rotation;
					ParticleSystem.ShapeModule sm = ps.shape;
					if (sm.shapeType == ParticleSystemShapeType.Mesh) {
						sm.mesh = triggeredGameObject.GetComponent<MeshFilter> ().mesh;
						sm.scale = triggeredGameObject.transform.lossyScale;
					} else if (sm.shapeType == ParticleSystemShapeType.MeshRenderer) {
						sm.meshRenderer = triggeredGameObject.GetComponent<MeshRenderer> ();
					}
					ps.Play ();
				} else {
					ps.Stop ();
				}
			} else if (type == typeof(GameObject)) {
				(whatToTrigger as GameObject).SetActive (activate);
			} else if (type == typeof(UnityEngine.Material)) {
				RememberedOriginalMaterial r = triggeredGameObject.GetComponent<RememberedOriginalMaterial> ();
				if (activate) {
					Material m = whatToTrigger as Material;
					if (r == null) {
						r = triggeredGameObject.AddComponent<RememberedOriginalMaterial> ();
						r.oldMaterial = triggeredGameObject.GetComponent<Renderer> ().material;
						triggeredGameObject.GetComponent<Renderer> ().material = m;
					}
				} else {
					if (r != null) {
						triggeredGameObject.GetComponent<Renderer> ().material = r.oldMaterial;
						Destroy (r);
					}
				}
			} else if (typeof(IEnumerable).IsAssignableFrom (type)) {
				IEnumerable ienum = whatToTrigger as IEnumerable;
				IEnumerator iter = ienum.GetEnumerator ();
				while (iter.MoveNext ()) {
					DoActivateTrigger (triggeringObject, iter.Current, triggeredGameObject, activate);
				}
			} else if (type == typeof(Animation)) {
				if (activate) {
					(whatToTrigger as Animation).Play ();
				} else {
					(whatToTrigger as Animation).Stop();
				}
			} else {
				System.Reflection.MethodInfo[] m = type.GetMethods ();
				bool invoked = false;
				for (int i = 0; i < m.Length; ++i) {
					if (m[i].Name == "DoActivateTrigger" && m[i].GetParameters().Length == 0) {
						m[i].Invoke (whatToTrigger, new object[]{ });
						invoked = true;
						break;
					}
				}
				if(!invoked) {
					Debug.LogError ("Don't know how to "+((activate)?"activate":"deactivate")+" a \'" + type + "\'");
				}
			}
		}
		/// <summary>Loads or unloads the level.</summary>
		/// <param name="sceneName">scene name.</param>
		/// <param name="load">If set to <c>true</c> load (or unload current and reload), otherwise unload (if there are multiple levels)</param>
		public static void LoadLevel(string sceneName, bool load = true) {
			if (load) {
				int currentSceneIndex = -1;
				for (int i = 0; i < SceneManager.sceneCount; ++i) {
					if (SceneManager.GetSceneAt (i).name == sceneName) {
						currentSceneIndex = i; break;
					}
				}
				if (SceneManager.sceneCount > 1 && currentSceneIndex >= 0) {
					// if the scene is already loaded, unload it. then load it once it is unloaded
					AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
					if (op == null) {
						Debug.LogError ("Error with scene named \"" + sceneName + "\"");
						SceneManager.LoadScene (sceneName);
					} else {
						op.completed += (AsyncOperation o)=>{ SceneManager.LoadScene (sceneName, LoadSceneMode.Additive); };
					}
				} else {
					SceneManager.LoadScene (sceneName);
				}
			} else {
				SceneManager.UnloadSceneAsync (sceneName);
			}
		}
		public class RememberedOriginalMaterial : MonoBehaviour { public Material oldMaterial; }

		private static void ColToTrig(GameObject g) {
			Collider c = g.GetComponent<Collider> ();      if (c) { c.isTrigger = true; }
			Collider2D c2 = g.GetComponent<Collider2D> (); if (c2) { c2.isTrigger = true; }
		}

		public static void AddTriggers(GameObject g, object w, TKind kind, string triggerTag, bool a, float s) {
			bool is2D = false;
			switch(kind){
			case TKind.onTriggerEnter: case TKind.onTriggerExit: case TKind.onTriggerStay:
				is2D = g.GetComponent<Collider2D> () != null;
				break;
			}
			switch (kind) {
			case TKind.onTriggerEnter:if(is2D) AddTaggedTrigger<OnTriggerEnter2D_> (g,w,triggerTag,a,s);
			                          else     AddTaggedTrigger<OnTriggerEnter_>   (g,w,triggerTag,a,s); ColToTrig (g); break;
			case TKind.onTriggerExit: if(is2D) AddTaggedTrigger<OnTriggerExit2D_>  (g,w,triggerTag,a,s);
			                          else     AddTaggedTrigger<OnTriggerExit_>    (g,w,triggerTag,a,s); ColToTrig (g); break;
			case TKind.onTriggerStay: if(is2D) AddTaggedTrigger<OnTriggerStay2D_>  (g,w,triggerTag,a,s);
			                          else     AddTaggedTrigger<OnTriggerStay_>    (g,w,triggerTag,a,s); ColToTrig (g); break;
			case TKind.onCollisionEnter:if(is2D) AddTaggedTrigger<OnCollisionEnter2D_> (g,w,triggerTag,a,s);
			                            else     AddTaggedTrigger<OnCollisionEnter_>   (g,w,triggerTag,a,s); ColToTrig (g); break;
			case TKind.onCollisionExit: if(is2D) AddTaggedTrigger<OnCollisionExit2D_>  (g,w,triggerTag,a,s);
			                            else     AddTaggedTrigger<OnCollisionExit_>    (g,w,triggerTag,a,s); ColToTrig (g); break;
			case TKind.onCollisionStay: if(is2D) AddTaggedTrigger<OnCollisionStay2D_>  (g,w,triggerTag,a,s);
			                            else     AddTaggedTrigger<OnCollisionStay_>    (g,w,triggerTag,a,s); ColToTrig (g); break;
			case TKind.onControllerColliderHit:AddTaggedTrigger<OnControllerColliderHit_> (g,w,triggerTag,a,s); break;
			case TKind.onStart: 			AddTrigger<OnStart_>    (g,w,a,s); break;
			case TKind.onDestroy:			AddTrigger<OnDestroy_>  (g,w,a,s); break;
			case TKind.onEnable:			AddTrigger<OnEnable_>   (g,w,a,s); break;
			case TKind.onDisable:			AddTrigger<OnDisable_>  (g,w,a,s); break;
			case TKind.onApplicationPause:	AddTrigger<OnApplicationPause_>   (g,w,a,s); break;
			case TKind.onApplicationUnpause:AddTrigger<OnApplicationUnpause_> (g,w,a,s); break;
			case TKind.onApplicationQuit:	AddTrigger<OnApplicationQuit_>    (g,w,a,s); break;
			case TKind.onMouseEnter:		AddTrigger<OnMouseEnter_>(g,w,a,s); break;
			case TKind.onMouseOver:			AddTrigger<OnMouseOver_> (g,w,a,s); break;
			case TKind.onMouseExit:			AddTrigger<OnMouseExit_> (g,w,a,s); break;
			case TKind.onMouseDown:			AddTrigger<OnMouseDown_> (g,w,a,s); break;
			case TKind.onMouseDrag:			AddTrigger<OnMouseDrag_> (g,w,a,s); break;
			case TKind.onMouseUp:			AddTrigger<OnMouseUp_>   (g,w,a,s); break;
			case TKind.onMouseUpAsButton:	AddTrigger<OnMouseUpAsButton_> (g,w,a,s); break;
			case TKind.onBecameInvisible:	AddTrigger<OnBecameInvisible_> (g,w,a,s); break;
			case TKind.onBecameVisible:		AddTrigger<OnBecameVisible_> (g,w,a,s); break;
			}
		}

		public class _TriggerBase : MonoBehaviour { 
			public object whatToTrigger;
			public float delayInSeconds = 0;
			public bool activate = true;
			public void DoActivateTrigger () {
				Trigger.DoActivateTrigger (gameObject, whatToTrigger, gameObject, activate, delayInSeconds);
			}
			public void DoTriggerMouse() { Trigger.DoActivateTrigger (Input.mousePosition, whatToTrigger, gameObject, activate); }
		}
		private static void AddTrigger<T>(GameObject gameObject, object whatToTrigger, bool activate, float s) where T : _TriggerBase {
			T t = gameObject.AddComponent<T> ();
			t.whatToTrigger = whatToTrigger;
			t.activate = activate;
			t.delayInSeconds = s;
		}

		public class OnStart_ : _TriggerBase { void Start() { DoActivateTrigger(); }}
		public class OnDestroy_ : _TriggerBase { void OnDestroy() { DoActivateTrigger(); }}
		public class OnEnable_ : _TriggerBase { void OnEnable() { DoActivateTrigger(); }}
		public class OnDisable_ : _TriggerBase { void OnDisable() { DoActivateTrigger(); }}
		public class OnBecameInvisible_ : _TriggerBase { void OnBecameInvisible() { DoActivateTrigger(); }}
		public class OnBecameVisible_ : _TriggerBase { void OnBecameVisible() { DoActivateTrigger(); }}
		public class OnMouseEnter_ : _TriggerBase { void OnMouseEnter() { DoTriggerMouse (); }}
		public class OnMouseOver_ : _TriggerBase { void OnMouseOver() { DoTriggerMouse (); }}
		public class OnMouseExit_ : _TriggerBase { void OnMouseExit() { DoTriggerMouse (); }}
		public class OnMouseDown_ : _TriggerBase { void OnMouseDown() { DoTriggerMouse (); }}
		public class OnMouseDrag_ : _TriggerBase { void OnMouseDrag() { DoTriggerMouse (); }}
		public class OnMouseUp_ : _TriggerBase { void OnMouseUp() { DoTriggerMouse (); }}
		public class OnMouseUpAsButton_ : _TriggerBase { void OnMouseMouseUpAsButton() { DoTriggerMouse (); }}

		public delegate void BooleanAction(bool b);
		public static void EquateUnityEditorPauseWithApplicationPause(BooleanAction b) {
			#if UNITY_EDITOR
			// This method is run whenever the playmode state is changed.
			UnityEditor.EditorApplication.pauseStateChanged += (UnityEditor.PauseState ps) => {
				b(ps == UnityEditor.PauseState.Paused);
			};
			#endif
		}
		public class OnApplicationPause_ : _TriggerBase { void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus == true) { DoActivateTrigger (); }
		} void Start() { EquateUnityEditorPauseWithApplicationPause (OnApplicationPause); }}
		public class OnApplicationUnpause_ : _TriggerBase { void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus == false) { DoActivateTrigger (); }
		} void Start() { EquateUnityEditorPauseWithApplicationPause (OnApplicationPause); }}

		public class OnApplicationQuit_ : _TriggerBase { void OnApplicationQuit() { DoActivateTrigger(); }}
		//onTriggerEnter, onTriggerExit, onTriggerStay, 
		public class _TriggerAreaBase : _TriggerBase {
			public string triggerTag;
			public bool IsTriggeringObject(GameObject o){ return triggerTag == "" || o.tag == triggerTag || o.tag == ""; }
			public void DoActivateTrigger (GameObject tobj) {
				if (IsTriggeringObject(tobj)) { 
					Trigger.DoActivateTrigger (tobj, whatToTrigger, gameObject, activate, delayInSeconds); }
			}
		}
		private static void AddTaggedTrigger<T>(GameObject gameObject, object whatToTrigger, string triggerTag, bool activate, float s) where T : _TriggerAreaBase {
			T t = gameObject.AddComponent<T> ();
			t.whatToTrigger = whatToTrigger;
			t.triggerTag = triggerTag;
			t.activate = activate;
			t.delayInSeconds = s;
		}

		public class OnTriggerEnter_ : _TriggerAreaBase { void OnTriggerEnter (Collider col) { DoActivateTrigger (col.gameObject); }}
		public class OnTriggerExit_ : _TriggerAreaBase { void OnTriggerExit (Collider col) { DoActivateTrigger (col.gameObject); }}
		public class OnTriggerStay_ : _TriggerAreaBase { void OnTriggerStay (Collider col) { DoActivateTrigger (col.gameObject); }}
		public class OnTriggerEnter2D_ : _TriggerAreaBase { void OnTriggerEnter2D (Collider2D col) { DoActivateTrigger (col.gameObject); }}
		public class OnTriggerExit2D_ : _TriggerAreaBase { void OnTriggerExit2D (Collider2D col) { DoActivateTrigger (col.gameObject); }}
		public class OnTriggerStay2D_ : _TriggerAreaBase { void OnTriggerStay2D (Collider2D col) { DoActivateTrigger (col.gameObject); }}
		public class OnCollisionEnter_ : _TriggerAreaBase { void OnCollisionEnter (Collider col) { DoActivateTrigger (col.gameObject); }}
		public class OnCollisionExit_ : _TriggerAreaBase { void OnCollisionExit (Collider col) { DoActivateTrigger (col.gameObject); }}
		public class OnCollisionStay_ : _TriggerAreaBase { void OnCollisionStay (Collider col) { DoActivateTrigger (col.gameObject); }}
		public class OnCollisionEnter2D_ : _TriggerAreaBase { void OnCollisionEnter2D (Collider2D col) { DoActivateTrigger (col.gameObject); }}
		public class OnCollisionExit2D_ : _TriggerAreaBase { void OnCollisionExit2D (Collider2D col) { DoActivateTrigger (col.gameObject); }}
		public class OnCollisionStay2D_ : _TriggerAreaBase { void OnCollisionStay2D (Collider2D col) { DoActivateTrigger (col.gameObject); }}
		public class OnControllerColliderHit_ : _TriggerAreaBase { void OnControllerColliderHit(CharacterController col) { DoActivateTrigger (col.gameObject); }}
	}
}
