using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour {
	static Dictionary<string, Singleton> allTheThings = new Dictionary<string, Singleton> ();

	public static Singleton Get(string named){
		return allTheThings [named];
	}

	// Use this for initialization
	void Start () {
		allTheThings [name] = this;
	}
	
}
