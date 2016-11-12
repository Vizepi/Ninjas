using UnityEngine;
using System.Collections;

public class ScoreRelique : MonoBehaviour {

	public Gamecontrolleur gamecontrol;

	// Use this for initialization
	void Start () {
		gamecontrol = GameObject.Find("GameControlleur").GetComponent<Gamecontrolleur>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D other){
		if (gameObject.activeSelf && other.tag == "Player") {
			gameObject.SetActive (false);
			Destroy (gameObject);
			gamecontrol.UpdateScore (1000, 3);
		}
	}
}
