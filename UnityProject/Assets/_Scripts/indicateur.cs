using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class indicateur : MonoBehaviour {

	private SpriteRenderer a;
	private PlayerV2 player;
	private static Gamecontrolleur gamecontrol;


	// Use this for initialization
	void Start () {
		foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
			if (sr.name == "A") {
				a = sr;
				break;
			}
		}
		player = GetComponent<PlayerV2>();
		a.enabled = false;
		gamecontrol = GameObject.FindGameObjectWithTag("GameController").GetComponent<Gamecontrolleur>();
	}

	void Update(){
		a.enabled = s_enemies.Count > 0;
	}

	// Update is called once per frame
	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Ennemy_back") {
			RemoveEnemy(other.gameObject.transform.parent.gameObject);
		}
	}

	void OnTriggerStay2D (Collider2D other){
		if (other.tag == "Ennemy_back") {
			if (other.gameObject.transform.parent.transform.localScale.x < 0 == player.FacingLeft()) {
				AddEnemy(other.gameObject.transform.parent.gameObject);
				if (Input.GetKeyDown("a")) {
					KillAll();
					player.Sword();
				}
			}
			else {
				RemoveEnemy(other.gameObject.transform.parent.gameObject);
			}
		}
	}

	static List<GameObject> s_enemies = new List<GameObject>();
	static void AddEnemy(GameObject e) {
		if(!s_enemies.Contains(e)) {
			s_enemies.Add(e);
		}
	}
	static void RemoveEnemy(GameObject e) {
		if(s_enemies.Contains(e)) {
			s_enemies.Remove(e);
		}
	}
	static void KillAll() {
		foreach(GameObject go in s_enemies) {
			if (go.activeSelf) {
				go.SetActive(false);
				Destroy(go);
				gamecontrol.UpdateScore(100, 1);
			}
		}
		s_enemies.Clear();
	}
}