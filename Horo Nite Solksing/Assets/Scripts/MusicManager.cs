using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager Instance;

	public AudioSource bgMusic;
	[field: SerializeField] public float bgMusicVol {get; private set;}

	[Space] public AudioSource bossMusic;
	[field: SerializeField] public float bossMusicVol {get; private set;}

	[Space] public AudioSource arenaMusic;
	[field: SerializeField] public float arenaMusicVol {get; private set;}
	
	[Space] [SerializeField] AudioSource parrySfx;
	[SerializeField] AudioSource parry2Sfx;
	[SerializeField] AudioSource hurtSfx;
	[SerializeField] AudioSource hornetAtkSfx;
	[SerializeField] AudioSource hornetAtk2Sfx;

	private AudioSource currentMusic;
	private AudioSource nextMusic;
	public AudioSource prevMusic {get; private set;}
	public float prevMusicVol {get; private set;}

	private float timer;
	private float incre;
	private float origVolume;
	private Coroutine softenCo;
	[SerializeField] float duration=0.5f;

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
		// DontDestroyOnLoad(this);

		if (bgMusic != null) 
			PlayMusic(this.bgMusic, bgMusicVol);
	}

    public void PlayMusic(AudioSource a, float vol=0.5f, bool remember=false)
	{
		this.enabled = true;
		if (a != null)
		{
			nextMusic = a;
			a.volume = 0;
			a.Play();
		}
		if (remember)
		{
			prevMusic = currentMusic;
			prevMusicVol = currentMusic.volume;
		}
		StartCoroutine( PlayMusicCo(a, vol) );
		// timer = 0;
		// incre = 0.5f * duration;
	}

    public void PlayParrySFX()
	{
		if (parrySfx != null)
			parrySfx.Play();
	}
    public void PlayParry2SFX()
	{
		if (parry2Sfx != null)
			parry2Sfx.Play();
	}
    public void PlayHurtSFX()
	{
		if (hurtSfx != null)
			hurtSfx.Play();
	}
    public void SoftenBgMusic()
	{
		if (softenCo != null)
		{
			StopCoroutine(softenCo);
			if (currentMusic != null)
				currentMusic.volume = origVolume;
		}
		softenCo = StartCoroutine(SoftenMusicCo());
	}
    public void PlayHornetAtkSfx(bool atk1)
	{
		if (atk1 && hornetAtkSfx != null)
			hornetAtkSfx.Play();
		else if (!atk1 && hornetAtk2Sfx != null)
			hornetAtk2Sfx.Play();
	}

	IEnumerator PlayMusicCo(AudioSource a, float vol)
	{
		float inc = 0.05f;
		if (currentMusic != null)
			inc = currentMusic.volume / 10;
		while (currentMusic != null && currentMusic.volume > 0)
		{
			yield return new WaitForSeconds(0.05f);
			currentMusic.volume -= inc;
		}
		if (currentMusic != null)
			currentMusic.Stop();
		
		inc = vol / 10;
		while (a != null && a.volume < vol)
		{
			a.volume += inc;
			yield return new WaitForSeconds(0.05f);
		}
		currentMusic = a;
	}

	IEnumerator SoftenMusicCo()
	{
		if (currentMusic == null) yield break;

		origVolume = currentMusic.volume;
		currentMusic.volume = origVolume / 2f;

		yield return new WaitForSeconds(1);
		while (currentMusic != null && currentMusic.volume < origVolume)
		{
			currentMusic.volume += 0.05f;
			yield return new WaitForSeconds(0.05f);
		}
		softenCo = null;
	}
}
