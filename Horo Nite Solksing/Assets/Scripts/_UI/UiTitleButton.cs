using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiTitleButton : MonoBehaviour
{
	[SerializeField] CanvasGroup canvasGroup;
	public static UiTitleButton Instance;
	[SerializeField] GameObject eventSystem;
	[SerializeField] TextMeshProUGUI difficultyTxt;
	[SerializeField] TextMeshProUGUI iFrameTxt;
	[SerializeField] TextMeshProUGUI showDmgTxt;
	[SerializeField] GameObject gmObj;
	[SerializeField] UiMouseSupport mouseSupport;
	private GameManager gm;


	private void Awake() 
	{
		Instance = this;
	}

	private void Start() 
	{
		gm = GameManager.Instance;
		if (gm != null && gm.easyMode)
			difficultyTxt.text = "Difficulty: Easy";
		else
			difficultyTxt.text = "Difficulty: Gamer";
		iFrameTxt.text = $"Invincibility: {gm.invincibilityDuration}s";
		showDmgTxt.text = gm.showDmg ? "Show Dmg: On" : "Show Dmg: Off";
		MusicManager m = MusicManager.Instance;
		m.PlayMusic(m.mainThemeMusic, m.mainThemeMusicVol);
	}

	public void START_GAME()
	{
		if (mouseSupport != null)
		{
			mouseSupport.RevertToOriginalSortingOrder();
		}
		if (gm != null)
		{
			gm.Restart();
			DisableInteractable();
		}
	}

	public void CONTROLS()
	{
		if (gm != null)
		{
			gm.OpenRemapControls();
			DisableInteractable();
		}
	}

	public void DIFFICULTY()
	{
		if (gm != null)
		{
			if (gm.ToggleEasyMode())
				difficultyTxt.text = "Difficulty: Easy";
			else
				difficultyTxt.text = "Difficulty: Gamer";
			iFrameTxt.text = $"Invincibility: {gm.invincibilityDuration}s";
		}
		else if (gmObj != null)
		{
			gmObj.SetActive(!gmObj.activeSelf);
		}
	}

	public void I_FRAME()
	{
		if (gm != null)
		{
			gm.ChangeInvincibilityDuration();
			iFrameTxt.text = $"Invincibility: {gm.invincibilityDuration}s";
		}
	}

	public void SHOW_DMG()
	{
		if (gm.ToggleDmgIndicator())
		{
			showDmgTxt.text = "Show Dmg: On";
		}
		else
		{
			showDmgTxt.text = "Show Dmg: Off";
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
