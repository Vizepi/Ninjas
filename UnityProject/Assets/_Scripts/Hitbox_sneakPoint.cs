using UnityEngine;
using System.Collections;

public class Hitbox_sneakPoint : MonoBehaviour {

	private Transform player;
	private indicateur indic;
	public Transform arrivalPoint;


	// Use this for initialization
	void Start () {
		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		player = playerObject.transform;
	}
	
	void OnTriggerStay2D (Collider2D other) {
		if (Input.GetKeyDown ("r")) {
			player.position = arrivalPoint.position;
		}
	}

}
