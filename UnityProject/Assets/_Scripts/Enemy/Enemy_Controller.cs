using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Controller : MonoBehaviour {

	private Animator animator = null;
	private Rigidbody2D rb = null;
	private EnemyState state = EnemyState.IDLING;
	private float direction = 1.0f;
	private bool triggering = true;
	private float idleTimer = 0.0f;
	private float swapTimer = 0.0f;
	private Transform lineThrower = null;
	private bool playerInView = false;
	private float playerViewTimer = 0.0f;
	private RaycastHit2D[] raycastHits = null;
	private bool playerTriggering = false;

	protected bool left = false;
	protected GameObject player = null;
	protected Segment viewLine = null;
	protected PlayerDamager playerDamager;

	// ENFORCER
	/*private float enforcer_StartTimer = 0.0f;
	private float enforcer_FireTimer = 0.0f;
	public GameObject enforcer_BulletPrefab;
	private Transform enforcer_BulletSpawn;*/

	// KATANA

	// DEBUGGING
	// DEBUGGING
	// DEBUGGING
	/*private LineRenderer lineViewer = null;*/
	// END DEBUGGING
	// END DEBUGGING
	// END DEBUGGING

	[SerializeField]
	private float waitTime = 1.2f;
	[SerializeField]
	private float speed = 2.0f;
	[SerializeField]
	private float chasingSpeed = 4.0f;
	[SerializeField]
	private float playerDetectionTime = 1.5f;
	[SerializeField]
	private float playerAbandonTime = 3.0f;
	[SerializeField]
	private float playerAngleOfView = 60.0f;
	[SerializeField]
	private float playerViewDistance = 7.5f;
	[SerializeField]
	private float playerShotDistance = 3.5f;
	[SerializeField]
	private bool startFacingLeft = false;
	[SerializeField]
	private float swapSideTime = 0.3f;
	[SerializeField]
	private float alertDistance = 5.0f;
	/*[SerializeField]
	private EnemyType type = EnemyType.ENFORCER;*/

	// ENFORCER
	/*[Header("Enforcer Settings")]
	[SerializeField]
	private float enforcer_StartTime = 0.1f;
	[SerializeField]
	private float enforcer_FireRate = 1.0f;*/

	// KATANA

    //AudioClip Array :

    public AudioClip[] walkSounds;
    private float soundTimer = 0.0f;
    private float waitSoundTime = 0.4f;

	// Use this for initialization
	protected virtual void Start () {
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		player = GameObject.FindGameObjectWithTag("Player");
		foreach(Transform t in GetComponentsInChildren<Transform>()) {
			if(t.name == "LineThrower") {
				lineThrower = t;
			}
		}
		viewLine = new global::Segment(lineThrower.position, player.transform.position);
		raycastHits = new RaycastHit2D[10];
		playerViewDistance *= playerViewDistance;
		playerShotDistance *= playerShotDistance;
		if(startFacingLeft) {
			direction = -1.0f;
		}
		left = startFacingLeft;
		SetDirection(startFacingLeft);
		SetState(EnemyState.IDLING);

		AddEnemy(this);

		playerDamager = player.GetComponent<PlayerDamager>();

		//StartType();

		// DEBUGGING
		// DEBUGGING
		// DEBUGGING
		/*lineViewer = GetComponent<LineRenderer>();
		lineViewer.SetWidth(0.05f, 0.05f);*/
		// END DEBUGGING
		// END DEBUGGING
		// END DEBUGGING
	}

	void OnDestroy() {
		RemoveEnemy(this);
	}

	void SetDirection(bool setLeft) {
		left = setLeft;
		if(setLeft) {
			direction = -1.0f;
		}
		else {
			direction = 1.0f;
		}
		Vector3 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x);
		scale.x *= left ? 1.0f : -1.0f;
		transform.localScale = scale;
	}

	void SetState(EnemyState newState) {
		switch(newState) {
			case EnemyState.ROAMING:
				animator.SetTrigger("startRoaming");
				break;
			case EnemyState.IDLING:
				animator.SetTrigger("startIdling");
				break;
			case EnemyState.CHASING:
				animator.SetTrigger("startChasing");
				break;
			case EnemyState.SEARCHING:
				animator.SetTrigger("startSearching");
				break;
			case EnemyState.FIRING:
				BeginAttack();
				animator.SetTrigger("startFiring");
				break;
		}
		state = newState;
	}

	public void SetGroundTriggering(bool t)
	{
		triggering = t;
	}

	public void PlayerTrigger(bool inside) {
		playerTriggering = inside;
	}

	protected virtual void BeginAttack() {

	}

	protected virtual void EndAttack() {

	}

	protected virtual void Attack() {

	}

	public void Alert() {
		playerViewTimer = 0.0f;
		state = EnemyState.SEARCHING;
	}

	void Move(float s) {
		rb.AddForce(new Vector2(s * direction * 100.0f, 0.0f), ForceMode2D.Impulse);
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 velocity = rb.velocity;

        //SoundManagement

        if (state == EnemyState.ROAMING)
        {
            soundTimer += Time.deltaTime;
            if(soundTimer >= waitSoundTime){
                soundTimer = 0;
                AudioSource audio = GetComponent<GameObject>().GetComponents<AudioSource>();
                audio.clip = walkSounds[Random.Range(0, walkSounds.Length)];
                audio.Play();
            }
        }

		// Detecting player
		float playerDistance = viewLine.SquareLength();
		viewLine.start = lineThrower.position;
		viewLine.end = player.transform.position;
		float angle = viewLine.Angle() * direction;
		bool inViewField = (angle >= (90.0f - playerAngleOfView)) && (angle <= (90.0f + playerAngleOfView));
		playerInView = (playerDistance <= playerViewDistance) && inViewField;
		Physics2D.RaycastNonAlloc(viewLine.start, viewLine.end - viewLine.start, raycastHits, viewLine.SquareLength());
		foreach (RaycastHit2D rayHit in raycastHits) {
			if (rayHit.collider != null) {
				if(rayHit.collider.tag == "Player") {
					playerInView &= true;
					break;
				}
				else if(!rayHit.collider.isTrigger) {
					playerInView = false;
					break;
				}
			}
			else {
				playerInView = false;
			}
		}
		if(playerTriggering) {
			SetState(EnemyState.FIRING);
		}
		//playerInView &= raycastHits[0].collider.tag == "Player";

		// Changing behavior
		if (playerInView) {
			if (state == EnemyState.ROAMING || state == EnemyState.IDLING) {
				playerViewTimer += Time.deltaTime;
				if(playerViewTimer >= playerDetectionTime) {
					playerViewTimer = 0.0f;
					if (playerDistance > playerShotDistance) {
						SetState(EnemyState.CHASING);
					}
					else {
						SetState(EnemyState.FIRING);
					}
				}
			}
			else if (state == EnemyState.SEARCHING) {
				playerViewTimer = 0.0f;
				if (playerDistance > playerShotDistance) {
					SetState(EnemyState.CHASING);
				}
				else {
					SetState(EnemyState.FIRING);
				}
			}
			else if(state == EnemyState.CHASING && playerDistance <= playerShotDistance) {
				playerViewTimer = 0.0f;
				SetState(EnemyState.FIRING);
			}
			else if(state == EnemyState.FIRING && playerDistance > playerShotDistance + 1.0f) {
				playerViewTimer = 0.0f;
				EndAttack();
				SetState(EnemyState.CHASING);
			}
		}
		else if(state == EnemyState.CHASING) {
			playerViewTimer = 0.0f;
			SetState(EnemyState.SEARCHING);
		}
		else if(state == EnemyState.SEARCHING) {
			playerViewTimer += Time.deltaTime;
			if(playerViewTimer >= playerAbandonTime) {
				playerViewTimer = 0.0f;
				if (Random.Range(0, 2) == 0 || triggering) {
					SetState(EnemyState.ROAMING);
				}
				else {
					SetState(EnemyState.IDLING);
				}
			}
		}
		else if(state == EnemyState.FIRING) {
			EndAttack();
			SetState(EnemyState.SEARCHING);
		}
		else {
			playerViewTimer = 0.0f;
		}

		// Behavior
		switch(state)
		{
			case EnemyState.ROAMING:
				if (!triggering)
				{
					SetState(EnemyState.IDLING);
					idleTimer = 0.0f;
				}
				else {
					Move(speed);
				}
				break;
			case EnemyState.IDLING:
				idleTimer += Time.deltaTime;
				if (idleTimer >= waitTime) {
					triggering = true;
					SetState(EnemyState.ROAMING);
					SetDirection(!left);
				}
				break;
			case EnemyState.CHASING:
			case EnemyState.SEARCHING:
				bool facing = viewLine.end.x - transform.position.x < 0.0f;
				if(facing != left) {
					swapTimer += Time.deltaTime;
					if(swapTimer >= swapSideTime) {
						swapTimer = 0.0f;
						SetDirection(facing);
					}
				}
				if(triggering) {
					Move(chasingSpeed);
				}
				break;
            case EnemyState.FIRING:
                Attack();
                break;



		}

		if(state == EnemyState.CHASING || state == EnemyState.FIRING) {
			AlertAll(this);
		}

		velocity = new Vector2(rb.velocity.x, velocity.y);
		rb.velocity = velocity;


		// DEBUGGING
		// DEBUGGING
		// DEBUGGING
		/*lineViewer.SetPosition(0, new Vector3(viewLine.start.x, viewLine.start.y, -0.5f));
		lineViewer.SetPosition(1, new Vector3(viewLine.end.x, viewLine.end.y, -0.5f));
		lineViewer.SetColors(Color.green, Color.green);
		if (playerInView) lineViewer.SetColors(Color.yellow, Color.yellow);
		if (state == EnemyState.CHASING) lineViewer.SetColors(Color.red, Color.red);
		if (state == EnemyState.SEARCHING) lineViewer.SetColors(Color.cyan, Color.cyan);
		if (state == EnemyState.FIRING) lineViewer.SetColors(Color.white, Color.white);*/
		// END DEBUGGING
		// END DEBUGGING
		// END DEBUGGING
	}

	// STATIC
	// STATIC
	// STATIC
	private static List<Enemy_Controller> s_enemies = new List<Enemy_Controller>();

	static void AddEnemy(Enemy_Controller enemy) {
		if(!s_enemies.Contains(enemy)) {
			s_enemies.Add(enemy);
		}
	}
	
	static void RemoveEnemy(Enemy_Controller enemy) {
		s_enemies.Remove(enemy);
	}

	static void AlertAll(Enemy_Controller enemy) {
		foreach(Enemy_Controller e in s_enemies) {
			if(e != enemy && Vector2.Distance(e.transform.position, enemy.transform.position) < enemy.alertDistance) {
				e.Alert();
			}
		}
	}
	// END STATIC
	// END_STATIC
	// END_STATIC
}
