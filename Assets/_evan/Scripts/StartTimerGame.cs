using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTimerGame : MonoBehaviour {
	public ScoreLogic scorelogic;

	public void StartTheGame()
	{
		scorelogic.StartGame ();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
