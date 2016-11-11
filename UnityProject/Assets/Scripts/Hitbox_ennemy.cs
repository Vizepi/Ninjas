using UnityEngine;
using System.Collections;

public class Hitbox_ennemy : MonoBehaviour {
	private BoxCollider2D hitbox;
	private GameObject ennemy;
	private Acrocatic.Player player;

	// Use this for initialization
	void Start () {
		hitbox = GetComponent<BoxCollider2D>();
		player = GetComponent<Acrocatic.Player> ();
		ennemy = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D (Collider2D other) {
		if (Input.GetKeyDown ("a")) {
			Destroy (ennemy.gameObject);
		}
	}
}
