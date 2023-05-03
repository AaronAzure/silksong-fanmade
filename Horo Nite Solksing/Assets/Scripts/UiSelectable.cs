using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Assertions;

public class UiSelectable : MonoBehaviour, ISelectHandler
{
	[SerializeField] UiHighlight master;
	[SerializeField] RectTransform self;
	[SerializeField] Image img;
	public Tool tool;

	
	[Space][SerializeField] Image skillHolderImg;
	[SerializeField] Sprite skillSpr;


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
	}
}
