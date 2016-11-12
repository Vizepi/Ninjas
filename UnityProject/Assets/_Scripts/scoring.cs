using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class scoring : MonoBehaviour {

	[HideInInspector]
	public int score;

	private string text;

	private Text canvas;
	public float time_combo;
	public float timer;

	// Use this for initialization
	void Start () {
		score = 0;
		canvas = GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () {
		text = score.ToString();
		canvas.text = "Score : " + text;

	}
	public void AddScore(ScoringType type){
		switch(type) {
			case ScoringType.KILL:
				score += 100;
				break;
			case ScoringType.RELIC:
				score += 1000;
				break;
		}
	}

	/*
	public void Launchcombo(){
		time_combo += Time.deltaTime;
		while (time_combo >= timer) {
			timer = time_combo;
			}
		}
	*/
}
