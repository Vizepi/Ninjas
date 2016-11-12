using UnityEngine;
using System.Collections;

public class Hitbox_ennemy : MonoBehaviour {
	private GameObject ennemy;
	private Acrocatic.Player player;
	private indicateur indic;
	[HideInInspector]
	public bool direction_player;
	private scoring score;

	// Use this for initialization
	void Start () {
		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		ennemy = transform.parent.gameObject;
		indic = playerObject.GetComponent<indicateur> ();
		player = playerObject.GetComponent<Acrocatic.Player> ();
		score = GameObject.Find("score").GetComponent<scoring> ();
	}
	
	// Update is called once per frame
	void Update () {
		direction_player = player.facingRight;
	}

	void OnTriggerStay2D (Collider2D other) {
		if (direction_player == ennemy.transform.localScale.x < 0.0f) {
			if (Input.GetKeyDown ("a")) {
				indic.HideA ();
				if (ennemy.gameObject.activeSelf) {
					ennemy.gameObject.SetActive (false);
					Destroy (ennemy.gameObject);	
					score.AddScore ("kill");
				}
			}
		}
	}
}
