using UnityEngine;
using System.Collections;

public class PlayerDetector : MonoBehaviour {

	private Enemy_Controller enemy;

	void Start() {
		enemy = transform.parent.GetComponent<Enemy_Controller>();
	}
	
	void OnTriggerEnter2D (Collider2D c) {
		if(c.tag == "Player") {
			enemy.PlayerTrigger(true);
		}
	}

	void OnTriggerExit2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			enemy.PlayerTrigger(false);
		}
	}
}
