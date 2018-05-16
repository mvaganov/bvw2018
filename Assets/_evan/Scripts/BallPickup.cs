using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPickup : MonoBehaviour {

	public ThrowBasketballScript thrower;

	void OnTriggerEnter(Collider col) {
		if (col.GetComponent<Ball>() != null && thrower.canPickUpBall()) {
			thrower.EnableShooting (true);
			Destroy (col.gameObject);
		}
	}
}
