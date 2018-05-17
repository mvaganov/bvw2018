using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLogic : MonoBehaviour {

	public float shotTimer;
	private bool timerrunning;
	public TMPro.TextMeshPro[] output;
	public TMPro.TextMeshPro home, guest;
	public TMPro.TextMeshPro highscore;
	int currenthighscore = 2;

//	void AddToTimer() {
//		shotTimer += 1;
//		if (timerrunning) {
//			NS.Timer.setTimeout (AddToTimer, 1000);
//		}
//
//	}
	public void Start()
	{
		highscore.SetText ("Highscore: " + currenthighscore);
	}

	public void StartGame()
	{
		timerrunning = true;
		shotTimer = 0;
		//AddToTimer ();
		home.SetText("Home: 0");
		guest.SetText("Guest: 0");
	}

	void EndGame()
	{
		Debug.Log(home.text.Substring (6)); Debug.Log(guest.text.Substring (7));
		int totalscore = int.Parse (home.text.Substring (6)) + int.Parse (guest.text.Substring (7));
		home.text = "Home: 0";
		guest.text = "Guest: 0";
		if (totalscore > currenthighscore)
		{
			currenthighscore = totalscore;
			highscore.SetText ("Highscore: " + currenthighscore);
		}
		foreach (TMPro.TextMeshPro t in output)
		{
			t.text = "" + 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (timerrunning) {
			shotTimer += Time.deltaTime;		
		}
		foreach (TMPro.TextMeshPro t in output)
		{
			t.text = "" + (int)shotTimer;
		}
		if (shotTimer > 60) {
			timerrunning = false;
			EndGame ();
		}
	}
}
