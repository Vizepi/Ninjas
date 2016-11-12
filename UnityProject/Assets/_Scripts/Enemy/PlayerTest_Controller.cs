using UnityEngine;
using System.Collections;

public class PlayerTest_Controller : MonoBehaviour
{

	private Rigidbody2D rb;

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{
		rb.MovePosition(rb.position + new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * 30.0f);
	}
}
