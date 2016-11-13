using UnityEngine;
using System.Collections;

public class indicateur : MonoBehaviour {
    
	private SpriteRenderer a;
	private int counter = 0;
//	private GameObject player;


	// Use this for initialization
	void Start () {/*
		foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
			if (sr.name == "A") {
				a = sr;
				break;
			}
		}
		a.enabled = false;*/
	}

	void Update(){/*
		if (counter < 0)
			a.enabled = false;
		counter = 0;*/
	}

	// Update is called once per frame
	void OnTriggerExit2D (Collider2D other) {/*
		a.enabled = false;*/
	}

<<<<<<< HEAD
/*	void OnTriggerEnter2D (Collider2D other) {
=======
	void OnTriggerEnter2D (Collider2D other) {/*
>>>>>>> origin/master
		if (other.tag == "Ennemy_back") {
			if (other.gameObject.transform.parent.transform.localScale.x < 0 == GetComponent<Player> ().facingRight) {
				counter++;
				a.enabled = true;
			}
		}*/
	}

	void OnTriggerStay2D (Collider2D other){/*
		if (other.tag == "Ennemy_back") {
			if (other.gameObject.transform.parent.transform.localScale.x > 0 == GetComponent<Player> ().facingRight) {
				counter++;
				a.enabled = false;
			} else {
				a.enabled = true;
			}
<<<<<<< HEAD
		}
	}*/
	public void HideA(){
=======
		}*/
	}
	public void HideA(){/*
>>>>>>> origin/master
		counter = -1;
		a.enabled = false;	*/
	}
}