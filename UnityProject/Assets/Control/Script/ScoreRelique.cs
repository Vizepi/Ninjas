using UnityEngine;
using System.Collections;

public class ScoreRelique : MonoBehaviour {

	public Gamecontrolleur gamecontrol;
	public float lifetime = 20.0f;
	private float timer;

	// Use this for initialization
	void Start () {
		gamecontrol = GameObject.FindGameObjectWithTag("GameController").GetComponent<Gamecontrolleur>();
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > lifetime) {
			enabled = false;
			Destroy (gameObject);
			timer = 0;
		}
	}

	void OnTriggerEnter2D (Collider2D other){
		if (gameObject.activeSelf && other.tag == "Player") {
			gameObject.SetActive (false);
			Destroy (gameObject);
			gamecontrol.UpdateScore (1000, 3);
		}
	}
}
