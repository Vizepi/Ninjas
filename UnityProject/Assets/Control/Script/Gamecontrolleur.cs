using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gamecontrolleur : MonoBehaviour {

    //Life Value
    public int nb_vie;
    private string text;
    private Text canvas_vie;

    //Combo Value
    public float combotime = 0.0f;
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
    private float timer_relique = 0.0f;
    private int random_relique;
    public GameObject[] relique;

    //Spawn Ennemy
	public GameObject Player;
    public GameObject[] ennemy_type;
    public GameObject[] ennemy_spawn;
    [HideInInspector]
    private CircleCollider2D Find_Player;
	public float time_to_spawn = 5.0f;
	public float timer_ennemy = 0.0f;
	public int random_spawn_ennemy;
	public int random_ennemy;


	void Start () {
		nb_vie = 2;
		combotime = 5;
		scoreValue = 0;
		Player = GameObject.FindGameObjectWithTag ("Player");
		canvas_vie = GameObject.Find("vie").GetComponent<Text> ();
		canvas_score = GameObject.Find("score").GetComponent<Text> ();
		canvas_combo = GameObject.Find("combo").GetComponent<Text> ();
		barre_combo = GameObject.Find("barre_combo").GetComponent<Image> ();
		in_combo = false;

		random_time = Random.Range (25, 40);
	}
	
	// Update is called once per frame
	void Update () {
		//Life
		text = nb_vie.ToString();
		canvas_vie.text = "Vie : " + text;

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
        timer_relique += Time.deltaTime; //Temps depuis le début du jeu
		if (timer_relique > random_time) {
				int random_number = Random.Range (0, relique_spawn.Length);
				random_relique = Random.Range (0, relique.Length);

				Instantiate (relique[random_relique], relique_spawn[random_number].transform.position, Quaternion.identity);
				timer_relique = 0;
				random_time = Random.Range (25, 40);
		}

		//Spawn des ennemis
		timer_ennemy += Time.deltaTime;
		Vector2 Player_position = new Vector2 (Player.transform.position.x, Player.transform.position.y);
		if (timer_ennemy > time_to_spawn) {


			int random_number = Random.Range (0, ennemy_spawn.Length);
			random_ennemy = Random.Range (0, ennemy_type.Length);

            SpawnPlayer.Spawn_near_player();
            Instantiate (ennemy_type[random_ennemy], ennemy_spawn[random_number].transform.position, Quaternion.identity);
            timer_ennemy = 0;
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