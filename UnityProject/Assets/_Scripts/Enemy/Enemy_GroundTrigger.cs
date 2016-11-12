using UnityEngine;
using System.Collections;

public class Enemy_GroundTrigger : MonoBehaviour {

	private GameObject enemy = null;
	private Enemy_Controller enemyController = null;
	private int platformCounter = 0;

	// Use this for initialization
	void Start () {
		enemy = transform.parent.gameObject;
		enemyController = enemy.GetComponent<Enemy_Controller>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D c) {
		if(c.tag.Equals("SolidPlatform")) {
			enemyController.SetGroundTriggering(true);
			platformCounter++;
			Debug.Log("++" + platformCounter);
		}
	}

	void OnTriggerExit2D(Collider2D c)
	{
		if (c.tag.Equals("SolidPlatform")) {
			platformCounter--;
			Debug.Log("--" + platformCounter);
			if (platformCounter <= 0) {
				enemyController.SetGroundTriggering(false);
				platformCounter = 0;
				Debug.Log("out");
			}
		}
	}
}
