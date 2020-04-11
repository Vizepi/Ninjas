using UnityEngine;
using System.Collections;

public class PlayerDamager : MonoBehaviour {

	private bool damaged = false;
	private bool dead = false;
	private PlayerV2 player;

	void Start() {
		player = transform.parent.GetComponent<PlayerV2>();
	}

	public void OnDamage(bool lethal) {
		dead = lethal || damaged;
		damaged = !lethal;
		Debug.Log("touched");
		if (dead) {
			Debug.Log("dead");

		}
	}

}
