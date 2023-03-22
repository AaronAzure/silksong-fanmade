using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager Instance;

	public AudioSource bgMusic;
	// [SerializeField] float bgMusicVol;
	public AudioSource bossMusic;
	// [SerializeField] float bossMusicVol;
	[SerializeField] AudioSource parrySfx;
	[SerializeField] AudioSource hurtSfx;
	[SerializeField] AudioSource hornetAtkSfx;
	[SerializeField] AudioSource hornetAtk2Sfx;

	private AudioSource currentMusic;
	private AudioSource nextMusic;
	public AudioSource prevMusic {get; private set;}

	private float timer;
	private float incre;
	[SerializeField] float duration=0.5f;

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
		// DontDestroyOnLoad(this);

		if (bgMusic != null) 
			PlayMusic(this.bgMusic);
	}

    public void PlayMusic(AudioSource a, bool remember=false)
	{
		this.enabled = true;
		if (a != null)
		{
			nextMusic = a;
			a.volume = 0;
			a.Play();
		}
		if (remember)
			prevMusic = currentMusic;
		StartCoroutine( PlayMusicCo(a) );
		// timer = 0;
		// incre = 0.5f * duration;
	}

    public void PlayParrySFX()
	{
		if (parrySfx != null)
			parrySfx.Play();
	}
    public void PlayHurtSFX()
	{
		if (hurtSfx != null)
			hurtSfx.Play();
	}
    public void PlayHornetAtkSfx(bool atk1)
	{
		if (atk1 && hornetAtkSfx != null)
			hornetAtkSfx.Play();
		else if (!atk1 && hornetAtk2Sfx != null)
			hornetAtk2Sfx.Play();
	}

	IEnumerator PlayMusicCo(AudioSource a)
	{
		while (currentMusic != null && currentMusic.volume > 0)
		{
			yield return new WaitForSeconds(0.05f);
			currentMusic.volume -= 0.05f;
		}
		if (currentMusic != null)
			currentMusic.Stop();
		while (a != null && a.volume < 0.5f)
		{
			a.volume += 0.05f;
			yield return new WaitForSeconds(0.05f);
		}
		currentMusic = a;
	}
}
