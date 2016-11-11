using UnityEngine;
using System.Collections;


public class GameController : MonoBehaviour {

	public int scoreValue;
	public int comboValue;
	public int livesValue;
	public GUIText livesText;
	public GUIText comboText;

	private GameObject Player;
	private bool isWounded = false;
	private int nextRespawnPoint;

	public GameObject[] spawnPointsArray;


	[SerializeField]
	private float timerTick = 5.0f;

	private float comboTimer = 0.0f;


	void Start()
	{
		GameObject PlayerObject = GameObject.FindWithTag ("Player");
		Player = PlayerObject.GetComponent<GameObject> ();
	}
		
	void Respawn (){
		nextRespawnPoint = Random.Range(0, 4);
		//Instantiate(Player, spawnPointsArray[nextRespawnPoint].transform, Quaternion.identity);
	}



	void PlayerDeath() {
		Destroy (Player);
		livesValue--;
		if (livesValue > 0) {
			Respawn ();
		} else {
			GameOver ();
		}
	}

	void TakeDamage (bool isLethal){
		if (isLethal==true && isWounded == false){
			isWounded = true;
	}
		else{
			PlayerDeath();
		}
	}


	void Update() {
		comboTimer += Time.deltaTime;
		while (comboTimer >= timerTick) {
			comboTimer -= timerTick;
			comboValue = 0;
		}
	}

	public void UpdateScore (int bonusScore, int bonusCombo){
		scoreValue += bonusScore + bonusScore * comboValue / 4;
		comboValue += bonusCombo;
		comboTimer = 0.0f;
		comboText.text = comboValue + "-hit combo !!!"; 
	}

	void GameOver(){
	
	}
		
		
	//addScore
	//Vies
	//SpawnPlayer
	//SpawnEnnemy
	//SpawnRelics

}
