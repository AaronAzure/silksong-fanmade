using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using Rewired;

public class UiSelectable : MonoBehaviour, ISelectHandler
{
	[SerializeField] UiHighlight master;
	[Space] [SerializeField] Rewired.Integration.UnityUI.RewiredEventSystem eventSystem;
	[SerializeField] RectTransform self;
	[SerializeField] Image img;
	public Tool tool;

	
	[Space][SerializeField] Image skillHolderImg;
	[SerializeField] Sprite skillSpr;

	
	[Space][SerializeField] Image passiveHolderImg;
	[SerializeField] Sprite passiveSpr;
	[SerializeField] Sprite emptySpr;


	[Space] [Header("Description")]
	[SerializeField] string title;
	[SerializeField] string extraDesc;
	[SerializeField] [TextArea(3,5)] string desc;

	
	[Space] [SerializeField] TextMeshProUGUI titleTxt;
	[Space] [SerializeField] TextMeshProUGUI extraTxt;
	[Space] [SerializeField] TextMeshProUGUI descTxt;


	public void SET_TOOL()
	{
		Assert.IsNotNull(PlayerControls.Instance, "PlayerControls.Instance == null");

		if (tool != null && PlayerControls.Instance != null && PlayerControls.Instance.isResting)
		{
			if (PlayerControls.Instance.EquipTool(tool, this))
				img.enabled = true;
			else
				img.enabled = false;
			if (master != null) master.Select();
		}
	}
	public void SET_SKILL(int n)
	{
		Assert.IsNotNull(PlayerControls.Instance, "PlayerControls.Instance == null");

		if (tool != null && PlayerControls.Instance != null && PlayerControls.Instance.isResting)
		{
			PlayerControls.Instance.EquipSkill(n);
			img.enabled = true;
			if (skillHolderImg != null && skillSpr != null)
				skillHolderImg.sprite = skillSpr;
			if (master != null) master.Select();
		}
	}
	public void SET_PASSIVE(int n)
	{
		Assert.IsNotNull(PlayerControls.Instance, "PlayerControls.Instance == null");

		if (tool != null && PlayerControls.Instance != null && PlayerControls.Instance.isResting)
		{
			if (PlayerControls.Instance.EquipPassive(n))
			{
				if (passiveHolderImg != null && passiveSpr != null)
					passiveHolderImg.sprite = passiveSpr;
				img.enabled = true;
			}
			else
			{
				if (passiveHolderImg != null && emptySpr != null)
					passiveHolderImg.sprite = emptySpr;
				img.enabled = false;
			}
			if (master != null) master.Select();
		}
	}
	public void UNEQUIP_SKILL()
	{
		img.enabled = false;
	}
	public void SET_CREST(int n)
	{
		Assert.IsNotNull(PlayerControls.Instance, "PlayerControls.Instance == null");

		if (PlayerControls.Instance != null && PlayerControls.Instance.isResting)
		{
			PlayerControls.Instance.EquipCrest(n);
			if (master != null) master.Select();
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (master != null && self != null) 
		{
			master.selected = self;
			master.MoveToButton();
		}
		if (titleTxt != null)
		{
			titleTxt.text = title;
		}
		if (extraTxt != null)
		{
			extraTxt.text = extraDesc;
		}
		if (descTxt != null)
		{
			descTxt.text = desc;
		}
		if (eventSystem != null)
		{
			eventSystem.firstSelectedGameObject = this.gameObject;
		}
	}
}
