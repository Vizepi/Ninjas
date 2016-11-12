using UnityEngine;
using System.Collections;

public class indicateurConduit : MonoBehaviour {

	private SpriteRenderer r;
	private int counter = 0;
	private GameObject sneakPoint;

	// Use this for initialization
	void Start () {
		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
			if (sr.name == "R") {
				r = sr;
				break;
			}
		}
		r.enabled = false;
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "Player"){
			r.enabled = true;
		}
	}

	void OnTriggerExit2D (Collider2D other){
		if (other.tag == "Player"){
			r.enabled = false;
		}
	}
}
