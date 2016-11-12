using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gamecontrolleur : MonoBehaviour {

	//Life Value
	public int nb_vie;
	private string text;
	private Text canvas_vie;

	//Combo Value
	public float combotime;
	private bool in_combo;
	private float timer = 0.0f;
	private Text canvas_combo;
	private Image barre_combo;

	//Score Value
	[HideInInspector]
	public int scoreValue;
	[HideInInspector]
	public int comboValue;
	private string text_score;
	private Text canvas_score;

	//Spawn Reliques
	public GameObject[] relique_spawn;
	private float random_time;
	private int random_number;
	private float timer_relique = 0.0f;
	private bool relique_state;
	public GameObject [] relique;


	void Start () {
		nb_vie = 2;
		combotime = 5;
		scoreValue = 0;
		canvas_vie = GameObject.Find("vie").GetComponent<Text> ();
		canvas_score = GameObject.Find("score").GetComponent<Text> ();
		canvas_combo = GameObject.Find("combo").GetComponent<Text> ();
		barre_combo = GameObject.Find("barre_combo").GetComponent<Image> ();
		in_combo = false;

		random_time = 1;
		random_number = Random.Range(1, 5);

		//relique = GameObject.FindGameObjectWithTag ("Relique");
		Debug.Log(random_time);
	
	}
	
	// Update is called once per frame
	void Update () {
		//Life
		text = nb_vie.ToString();
		canvas_vie.text = "Vie restante : " + text;

		//Combo
		timer += Time.deltaTime; //Temps depuis le début du jeu
		if (timer > combotime) {
			in_combo = false;
		}
		if (in_combo == false) {
			canvas_combo.enabled = false;
			barre_combo.enabled = false;
			comboValue = 0;
		} else {
			barre_combo.transform.localScale = new Vector2 (0.7f * (combotime - timer) / (float)combotime, 0.05f);
		}

		//Spawn des reliques
		//Debug.Log(random_time);
		timer_relique += Time.deltaTime; //Temps depuis le début du jeu
		if (timer_relique > random_time) {
			if (relique_state == false) {
				int random_number = Random.Range (0, 1);//relique_spawn.Length);
				int random_relique = Random.Range (0, relique.Length);

				Instantiate (relique[random_relique], relique_spawn[random_number].transform.position, Quaternion.identity);
				relique_state = true;
			
			}
		}
	
	}

	public void die(int vie){
		if (vie == 0) {
			print ("gameover ();");
		} else {
			nb_vie -= 1;
		}
	}
  
	public void UpdateScore(int scoreBonus,int comboBonus){

		scoreValue += scoreBonus + (scoreBonus * comboValue /4);
		comboValue += comboBonus;

		text_score = scoreValue.ToString();
		canvas_score.text = "Score : " + text_score;
		timer = 0;
		canvas_combo.enabled = true;
		barre_combo.enabled = true;

		in_combo = true;
		canvas_combo.text = "Combo : " + comboValue;
	}
}