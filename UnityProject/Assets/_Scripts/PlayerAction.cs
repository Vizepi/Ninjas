using UnityEngine;
using System.Collections;

public class PlayerAction : MonoBehaviour {

	float eggplantTimer = 0.0f;
	float cloakTimer = 0.0f;
	float decoyTimer = 0.0f;
	float dashTimer = 0.0f;
	float counterTimer = 0.0f;
	float shurikenTimer = 0.0f;

	[SerializeField]
	int eggplantCooldown = 20;

	[SerializeField]
	int cloakCooldown = 10;

	[SerializeField]
	int decoyCooldown = 10;

	[SerializeField]
	int dashCooldown = 3;

	[SerializeField]
	int counterCooldown = 5;

	[SerializeField]
	int shurikenCooldown = 5;

	public int firstJutsu = -1;
	public int secondJutsu = -1;

	
	// Update is called once per frame
	void Update () {
		eggplantTimer -= Time.deltaTime;
		cloakTimer -= Time.deltaTime;
		decoyTimer -= Time.deltaTime;
		dashTimer -= Time.deltaTime;
		counterTimer -= Time.deltaTime;
		shurikenTimer -= Time.deltaTime;
		if (Input.GetAxis ("Spell0") != 0.0f) {
			MasterJutsu (firstJutsu);
		}
		if (Input.GetAxis ("Spell1") !=0.0f) {
			MasterJutsu (secondJutsu);
		}

	}

	void MasterJutsu(int selectedJutsu){
		switch (selectedJutsu) {
		case 0:
			Eggplant ();
			break;
		case 1:
			Cloak ();
			break;
		case 2:
			Decoy ();
			break;
		case 3:
			Dash ();
			break;
		case 4:
			Counter ();
			break;
		case 5:
			Shuriken ();
			break;
		}
	}

	void Eggplant(){
		if (eggplantTimer > 0){
			return;
			}
		//do Time Stop
		eggplantTimer = eggplantCooldown;
	}

	void Cloak(){
		if (cloakTimer > 0){
			return;
		}
		//do Invisible
		cloakTimer = cloakCooldown;
	}

	void Decoy(){
		if (decoyTimer > 0){
			return;
		}
		//do Leurre
		decoyTimer = decoyCooldown;
	}

	void Dash(){
		if (dashTimer > 0){
			return;
		}
		//do Dash
		dashTimer = dashCooldown;
	}

	void Counter(){
		if (counterTimer > 0){
			return;
		}
		//do Riposte
		counterTimer = counterCooldown;
	}

	void Shuriken(){
		if (shurikenTimer > 0){
			return;
		}
		//do Shuriken
		shurikenTimer = shurikenCooldown;
	}
}
