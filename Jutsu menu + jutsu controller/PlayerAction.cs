using UnityEngine;
using System.Collections;

public class PlayerAction : MonoBehaviour {

	//private PlayerV2 player;

	float eggplantTimer = 0.0f;
	float cloakTimer = 0.0f;
	float decoyTimer = 0.0f;
	float dashTimer = 0.0f;
	float counterTimer = 0.0f;
	float shurikenTimer = 0.0f;

	// eggPlant
	private float eggPlant_timer = 0.0f;
	[SerializeField]
	private float eggPlant_duration = 5.0f;
	private bool eggPlant_playing = false;
	public bool IsEggPlantPlaying() { return eggPlant_playing; }

	// cloak
	private float cloak_timer = 0.0f;
	[SerializeField]
	private float cloak_duration = 5.0f;
	private bool cloak_playing = false;
	public bool cloakPlaying() { return cloak_playing; }

	// shuriken


	[SerializeField]
	float eggplantCooldown = 25.0f;

	[SerializeField]
	float cloakCooldown = 10.0f;

	[SerializeField]
	float decoyCooldown = 10.0f;

	[SerializeField]
	float dashCooldown = 3.0f;

	[SerializeField]
	float counterCooldown = 5.0f;

	[SerializeField]
	float shurikenCooldown = 5.0f;

	public int firstJutsu = -1;
	public int secondJutsu = -1;

	void Start(){
		//player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerV2> ();
	}

	
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

		if(eggPlant_playing) {
			eggPlant_Update();
		}

		if (cloak_playing) {
			cloak_Update ();
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
		eggPlant_playing = true;
		eggPlant_timer = 0.0f;
	}

	void Cloak(){
		if (cloakTimer > 0){
			return;
		}
		//do Invisible
		cloakTimer = cloakCooldown;
		cloak_playing = true;
		cloak_timer = 0.0f;
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
		//player.Dash();
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

	// eggPlant
	void eggPlant_Update() {
		eggPlant_timer += Time.deltaTime;
		if(eggPlant_timer >= eggPlant_duration) {
			eggPlant_playing = false;
		}
	}

	//cloak
	void cloak_Update(){
		cloak_timer += Time.deltaTime;
		if (cloak_timer >= cloak_duration) {
			cloak_playing = false;
		}
	}
}
