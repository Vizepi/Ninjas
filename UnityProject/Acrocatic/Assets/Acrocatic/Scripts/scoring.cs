using UnityEngine;
using System.Collections;

public class scoring : MonoBehaviour {

	[HideInInspector]
	public int score;

	private string text;
	public float time_combo;
	public float timer;

	// Use this for initialization
	void Start () {
		score = 0;
	}

	// Update is called once per frame
	void Update () {
		text = score.ToString();
		//GetComponent<Canvas> ().gameObject = text;

	}
	public void AddScore(string type){
		if (type == "kill") {
			score = score + 100;
		}
		if (type == "relique") {
			score = score + 1000;
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
