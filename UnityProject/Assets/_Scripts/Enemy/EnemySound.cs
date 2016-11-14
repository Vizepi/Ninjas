using UnityEngine;
using System.Collections;

public class EnemySound : MonoBehaviour
{

	public AudioClip[] stepSounds;
	public AudioClip[] shotSounds;
	public AudioClip[] fireSounds;
	public AudioClip[] noticeSounds;
	public AudioClip[] detectSounds;

	private float timer, stepTime;
	private bool walk;

	private EnemyState state;
	private bool noticed;
	private Enemy_Controller enemy;
	private AudioSource source;
	private AudioSource source2;

	// Use this for initialization
	void Start()
	{
		enemy = GetComponent<Enemy_Controller>();
		AudioSource[] sources = GetComponents<AudioSource>();
		source = sources[0];
		source.volume = 0.7f;
		source2 = sources[1];
		source2.volume = 0.85f;
		state = enemy.GetState();
	}

	// Update is called once per frame
	void Update()
	{
		EnemyState newState = enemy.GetState();
		bool n = enemy.Noticed();

		switch (newState)
		{
			case EnemyState.IDLING:
				source.Stop();
				walk = false;
				if(n && !noticed) {
					PlaySound2(noticeSounds, false, 1);
				}
				break;
			case EnemyState.ROAMING:
				if (state != EnemyState.ROAMING) {
					PlaySound(stepSounds, false, 1);
					walk = true;
					stepTime = 0.4f;
					timer = 0.0f;
				}
				if (n && !noticed) {
					PlaySound2(noticeSounds, false, 1);
				}
				break;
			case EnemyState.FIRING:
				if (state != EnemyState.FIRING) {
					PlaySound(fireSounds, true, 1);
					walk = false;
				}
				break;
			case EnemyState.CHASING:
			case EnemyState.SEARCHING:
				if (state != EnemyState.CHASING && state != EnemyState.SEARCHING) {
					PlaySound(stepSounds, false, 1);
					walk = true;
					stepTime = 0.2f;
					timer = 0.0f;
				}
				break;
		}

		if(walk) {
			timer += Time.deltaTime;
			if(timer >= stepTime) {
				timer = 0.0f;
				PlaySound(stepSounds, false, 1);
			}
		}

		state = newState;
		noticed = n;
	}

	void PlaySound(AudioClip[] array, bool loop, float pitch)
	{
		source.clip = array[Random.Range(0, array.Length)];
		source.loop = loop;
		source.pitch = pitch;
		source.Play();
	}

	void PlaySound2(AudioClip[] array, bool loop, float pitch)
	{
		source2.clip = array[Random.Range(0, array.Length)];
		source2.loop = loop;
		source2.pitch = pitch;
		source2.Play();
	}
}
