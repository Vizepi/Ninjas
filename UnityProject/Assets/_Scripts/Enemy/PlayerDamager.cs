using UnityEngine;
using System.Collections;

public class PlayerDamager : MonoBehaviour {

	private bool damaged = false;
	private bool dead = false;

	public void OnDamage(bool lethal) {
		dead = lethal || damaged;
		damaged = !lethal;
		if (dead) {
			// TODO : call gamecontroller to kill player
		}
	}

}
