using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTitleButton : MonoBehaviour
{
	[SerializeField] CanvasGroup canvasGroup;
	public static UiTitleButton Instance;
	[SerializeField] GameObject eventSystem;


	private void Awake() 
	{
		Instance = this;
	}

	private void Start() 
	{
		MusicManager m = MusicManager.Instance;
		m.PlayMusic(m.mainThemeMusic, m.mainThemeMusicVol);
	}

	public void START_GAME()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.Restart();
			DisableInteractable();
		}
	}

	public void CONTROLS()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OpenRemapControls();
			DisableInteractable();
		}
	}

	private void DisableInteractable()
	{
		if (canvasGroup != null)
		{
			canvasGroup.interactable = false;
		}
		if (eventSystem != null)
		{
			eventSystem.SetActive(false);
		}
	}

	public void DoneRemapping()
	{
		EnableInteractable();
	}

	private void EnableInteractable()
	{
		if (canvasGroup != null)
		{
			canvasGroup.interactable = true;
		}
		if (eventSystem != null)
		{
			eventSystem.SetActive(true);
		}
	}
}
