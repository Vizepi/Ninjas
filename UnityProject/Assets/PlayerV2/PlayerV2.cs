using UnityEngine;
using System.Collections;

public class PlayerV2 : MonoBehaviour {

	private PlayerV2State state;
	private Rigidbody2D rb;
	private Animator anim;
	private bool facingLeft;
	private float direction;
	private float originalScaleX;
	private bool groundContacting;
	private bool contactBottom, contactLeft, contactRight;
	private bool doubleJumping;
	private Vector2 moving;

	[SerializeField]
	private float speed = 3.0f;
	[SerializeField]
	private float jumpIntensity = 15.0f;
	[SerializeField]
	private float doubleJumpIntensity = 15.0f;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		originalScaleX = Mathf.Abs(transform.localScale.x);
		SetState(PlayerV2State.IDLING);
		SetDirection(true);
		groundContacting = true;
		moving = new Vector2();
		doubleJumping = false;
	}
	
	void Update () {

		float inputX = (Input.GetKey("left") ? -1 : 0) + (Input.GetKey("right") ? 1 : 0);
		bool inputJ = Input.GetKeyDown("space");

		// Inputs
		switch (state)
		{
			case PlayerV2State.IDLING:
				Look(inputX);
				if (inputX != 0.0f) {
					SetState(PlayerV2State.RUNING);
				}
				Jump(inputJ);
				Falling();
				break;
			case PlayerV2State.RUNING:
				if(inputX == 0.0f) {
					SetState(PlayerV2State.IDLING);
				}
				Look(inputX);
				Jump(inputJ);
				break;
			case PlayerV2State.JUMPING:
				Look(inputX);
				Jump(inputJ);
				Grounding(inputX);
				break;
			case PlayerV2State.FALLING:
				Look(inputX);
				Jump(inputJ);
				Grounding(inputX);
				break;
			case PlayerV2State.BINDING:
				break;
			case PlayerV2State.DYING:
				break;
			case PlayerV2State.THROWING:
				break;
			case PlayerV2State.SWORDING:
				break;
		}
		if(inputX != 0.0f) {
			Move(new Vector2(-direction * speed, 0.0f));
		}

		// Behavior
		switch (state) {
			case PlayerV2State.IDLING:
				Falling();
				break;
			case PlayerV2State.RUNING:
				Falling();
				break;
			case PlayerV2State.JUMPING:
				Falling();
				break;
			case PlayerV2State.FALLING:
				break;
			case PlayerV2State.BINDING:
				break;
			case PlayerV2State.DYING:
				break;
			case PlayerV2State.THROWING:
				break;
			case PlayerV2State.SWORDING:
				break;
		}

	}

	void Look(float inputX) {
		if (inputX != 0.0f) {
			SetDirection(inputX > 0.0f);
		}
	}

	void Jump(bool inputJ)
	{
		Debug.Log(rb.velocity.y);
		if (inputJ && (groundContacting || (!doubleJumping && rb.velocity.y > -15.0f)))
		{
			rb.velocity = new Vector2(rb.velocity.x, 0.0f);
			if (groundContacting) {
				groundContacting = false;
				rb.AddForce(new Vector2(0.0f, jumpIntensity), ForceMode2D.Impulse);
			}
			else {
				doubleJumping = true;
				rb.AddForce(new Vector2(0.0f, doubleJumpIntensity), ForceMode2D.Impulse);
			}
			SetState(PlayerV2State.JUMPING);
		}
	}

	void Grounding(float inputX) {
		if(groundContacting) {
			doubleJumping = false;
			if(inputX == 0) {
				SetState(PlayerV2State.IDLING);
			}
			else {
				SetState(PlayerV2State.RUNING);
			}
		}
	}

	void Falling() {
		if(!groundContacting && rb.velocity.y < 0.0f) {
			SetState(PlayerV2State.FALLING);
		}
	}

	void FixedUpdate() {
		rb.velocity = moving + new Vector2(0.0f, rb.velocity.y);
		moving.x = moving.y = 0.0f;
	}

	void Move(Vector2 v) {
		moving += v;
	}

	void SetState(PlayerV2State newState) {
		switch(newState) {
			case PlayerV2State.IDLING:
				anim.SetTrigger("startIdle");
				break;
			case PlayerV2State.RUNING:
				anim.SetTrigger("startRun");
				break;
			case PlayerV2State.JUMPING:
				anim.SetTrigger("startJump");
				break;
			case PlayerV2State.FALLING:
				anim.SetTrigger("startAir");
				break;
			case PlayerV2State.BINDING:
				anim.SetTrigger("startBind");
				break;
			case PlayerV2State.DYING:
				anim.SetTrigger("startDeath");
				break;
			case PlayerV2State.THROWING:
				anim.SetTrigger("startThrow");
				break;
			case PlayerV2State.SWORDING:
				//anim.SetTrigger("startKill");
				break;
		}
		state = newState;
	}

	void SetDirection(bool left) {
		facingLeft = left;
		direction = (left ? -1.0f : 1.0f);
		transform.localScale = new Vector3(originalScaleX * direction, transform.localScale.y, 1.0f);
	}

	public void SetGroundContact(PlayerV2GroundDetector.PlayerV2GroundDetectorType type, bool contact) {
		switch(type) {
			case PlayerV2GroundDetector.PlayerV2GroundDetectorType.BOTTOM:
				contactBottom = contact;
				break;
			case PlayerV2GroundDetector.PlayerV2GroundDetectorType.LEFT:
				/*SetState(PlayerV2State.BINDING);
				SetDirection(false);*/
				contactLeft = contact;
				break;
			case PlayerV2GroundDetector.PlayerV2GroundDetectorType.RIGHT:
				/*SetState(PlayerV2State.BINDING);
				SetDirection(true);*/
				contactRight = contact;
				break;
		}
		groundContacting = contactBottom || contactLeft || contactRight;
	}

}
