using UnityEngine;
using System.Collections;

public class PlayerV2GroundDetector : MonoBehaviour {

	private PlayerV2 player;
	private int contactCounter;
	public enum PlayerV2GroundDetectorType {
		LEFT, RIGHT, BOTTOM
	}
	[SerializeField]
	private PlayerV2GroundDetectorType type = PlayerV2GroundDetectorType.BOTTOM;

	void Start () {
		player = transform.parent.GetComponent<PlayerV2>();
		contactCounter = 0;
	}
	
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D c) {
		contactCounter++;
		player.SetGroundContact(type, true);
	}

	void OnTriggerExit2D(Collider2D c) {
		contactCounter--;
		if(contactCounter <= 0) {
			contactCounter = 0;
			player.SetGroundContact(type, false);
		}
	}
}
