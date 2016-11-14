using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour {

	public AudioClip[] stepSounds;
	public AudioClip[] swordSounds;
	public AudioClip[] jumpSounds;
	public AudioClip[] doublejumpSounds;
	public AudioClip[] landingSounds;
	public AudioClip backgroundMusic;

	private PlayerV2State state;
	private PlayerV2 player;
	private bool doubleJumping;
	private AudioSource source;
	private AudioSource music;

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerV2>();
		AudioSource[] sources = GetComponents<AudioSource>();
		source = sources[0];
		source.volume = 0.75f;
		music = sources[1];
		music.volume = 0.5f;
		state = player.GetState();
		music.clip = backgroundMusic;
		music.loop = true;
		music.Play();
	}

	void OnDestroy() {
		music.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		PlayerV2State newState = player.GetState();
		bool dj = player.IsDoubleJumping();

		switch(newState) {
			case PlayerV2State.IDLING:
				if(state != PlayerV2State.IDLING && state != PlayerV2State.RUNING) {
					PlaySound(landingSounds, false, 2);
				}
				else {
					source.Stop();
				}
				break;
			case PlayerV2State.JUMPING:
				if(state != PlayerV2State.JUMPING) {
					PlaySound(jumpSounds, false, 0.7f);
				}
				else if (!doubleJumping && dj) {
					PlaySound(doublejumpSounds, false, 0.7f);
				}
				break;
			case PlayerV2State.RUNING:
				if(state != PlayerV2State.RUNING) {
					PlaySound(stepSounds, true, 2);
				}
				break;
			case PlayerV2State.SWORDING:
				if(state != PlayerV2State.SWORDING) {
					PlaySound(swordSounds, false, 1);
				}
				break;
		}

		state = newState;
		doubleJumping = dj;
	}

	void PlaySound(AudioClip [] array, bool loop, float pitch) {
		source.clip = array[Random.Range(0, array.Length)];
		source.loop = loop;
		source.pitch = pitch;
		source.Play();
	}
}
