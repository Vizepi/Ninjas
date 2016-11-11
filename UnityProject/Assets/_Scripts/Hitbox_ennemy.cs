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

	void OnTriggerEnter2D (Collider2D other) {
		Destroy (ennemy.gameObject);	
	}
}
