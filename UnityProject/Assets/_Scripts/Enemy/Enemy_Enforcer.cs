using UnityEngine;
using System.Collections;

public class Enemy_Enforcer : Enemy_Controller {

	private float startTimer = 0.0f;
	private float fireTimer = 0.0f;
	public GameObject bulletPrefab;
	private Transform bulletSpawn;

	[SerializeField]
	private float startTime = 0.1f;
	[SerializeField]
	private float fireRate = 1.0f;

	protected override void Start() {
		base.Start();
		playerDamager = player.GetComponent<PlayerDamager>();
		foreach(Transform t in GetComponentsInChildren<Transform>()) {
			if(t.name == "ShotSpawn") {
					bulletSpawn = t;
					break;
			}
		}
	}

	protected override void BeginAttack()
	{
		startTimer = 0.0f;
		fireTimer = 0.0f;

	}

	protected override void EndAttack()
	{
		
	}

	protected override void Attack() {
		base.Attack();
		
		if(startTimer >= startTime) {
			fireTimer += Time.deltaTime;
			if(fireTimer >= fireRate) {
				fireTimer = 0.0f;
				Fire();
			}
		}
		else {
			startTimer += Time.deltaTime;
		}

	}

	void Fire() {
		GameObject bullet = Instantiate<GameObject>(bulletPrefab);
		bullet.transform.position = bulletSpawn.position;
		bullet.GetComponent<Bullet>().Initialize(bulletSpawn.position, player.transform.position);
	}

	}
