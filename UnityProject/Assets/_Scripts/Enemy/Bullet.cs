using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	private Rigidbody2D rb;
	private Vector2 direction;
	private Vector2 spawnPosition;

	[SerializeField]
	private float speed = 20.0f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}

	public void Initialize(Vector2 position, Vector2 target)
	{
		transform.position = position;
		spawnPosition = position;
		direction = target - new Vector2(transform.position.x, transform.position.y);
		direction.Normalize();
		direction *= speed;
		transform.Rotate(0.0f, 0.0f, 180.0f + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
	}
	
	// Update is called once per frame
	void Update ()
	{
		rb.AddForce(direction * 0.001f);
		if (Vector2.Distance(transform.position, spawnPosition) >= 100.0f) {
			enabled = false;
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		if(c.tag == "Player") {
			c.gameObject.GetComponent<PlayerDamager>().OnDamage(false);
		}
		else if(c.transform.parent != null && c.transform.parent.tag == "Enemy") {
			return;
		}
		enabled = false;
		Destroy(gameObject);
	}

}
