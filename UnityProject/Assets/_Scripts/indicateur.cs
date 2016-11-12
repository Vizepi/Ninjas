using UnityEngine;
using System.Collections;

public class indicateur : MonoBehaviour {

	private SpriteRenderer a;
	private int counter = 0;
	private Acrocatic.Player player;
	private scoring score;


	// Use this for initialization
	void Start () {
		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
			if (sr.name == "A") {
				a = sr;
				break;
			}
		}
		a.enabled = false;
		player = GetComponent<Acrocatic.Player>();

		score = GameObject.Find("score").GetComponent<scoring> ();
	}

	void Update(){
		if (counter < 0)
			a.enabled = false;
		counter = 0;
	}

	// Update is called once per frame
	void OnTriggerExit2D (Collider2D other) {
		/*if (other.tag == "Ennemy_back") {
			counter--;
			if (counter <= 0)
				a.enabled = false;
		}*/
		a.enabled = false;
	}
	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Ennemy_back") {
			if (other.gameObject.transform.parent.transform.localScale.x < 0 == player.facingRight) {
				counter++;
				a.enabled = true;
			}
		}
		if (other.tag == "Relique") {
			if (other.gameObject.activeSelf) {
				other.gameObject.SetActive (false);
				Destroy (other);
				score.AddScore ("relique");
			}
		}
	}
	void OnTriggerStay2D (Collider2D other){
		Transform p = other.gameObject.transform.parent;
		if ((p != null && p.transform.localScale.x > 0 == player.facingRight) || other.tag == "sneakPoint") {
			counter++;
			a.enabled = false;
		} else {
			a.enabled = true;
		}
	}
	public void HideA(){
		counter = -1;
		a.enabled = false;
		
	}

}
