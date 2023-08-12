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


	[Space] [Header("DESCRIPTION")]
	[SerializeField] string title;
	[SerializeField] bool GetTotalUses;
	[SerializeField] string extraDesc;
	[SerializeField] bool hasDifferentDesc;
	[SerializeField] [TextArea(3,5)] string desc;
	[SerializeField] [TextArea(3,5)] string desc1;
	[SerializeField] [TextArea(3,5)] string desc2;
	[SerializeField] [TextArea(3,5)] string desc3;

	
	[Space] [SerializeField] TextMeshProUGUI titleTxt;
	[Space] [SerializeField] TextMeshProUGUI verTxt;
	[Space] [SerializeField] TextMeshProUGUI extraTxt;
	[Space] [SerializeField] TextMeshProUGUI descTxt;

	[SerializeField] bool isShield;
	[SerializeField] bool isExtraSpool;
	[SerializeField] bool isLootCharm;


	public void OnEnable() 
	{
		if (verTxt != null)
		{
			if (isShield)
			{
				switch (PlayerControls.Instance.nShieldBonus)
				{
					case 0: verTxt.text = "I"; break;
					case 1: verTxt.text = "II"; break;
					case 2: verTxt.text = "III"; break;
					case 3: verTxt.text = "IV"; break;
				}
			}
			else if (isExtraSpool)
			{
				switch (PlayerControls.Instance.nExtraSpoolBonus)
				{
					case 0: verTxt.text = "I"; break;
					case 1: verTxt.text = "II"; break;
					case 2: verTxt.text = "III"; break;
					case 3: verTxt.text = "IV"; break;
				}
			}
			else if (isLootCharm)
			{
				switch (PlayerControls.Instance.nLootCharmBonus)
				{
					case 0: verTxt.text = "I"; break;
					case 1: verTxt.text = "II"; break;
					case 2: verTxt.text = "III"; break;
					case 3: verTxt.text = "IV"; break;
				}
			}
			else if (tool != null)
			{
				switch (tool.level)
				{
					case 0: verTxt.text = "I"; break;
					case 1: verTxt.text = "II"; break;
					case 2: verTxt.text = "III"; break;
					case 3: verTxt.text = "IV"; break;
				}
			}
		}	
	}

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

		if (PlayerControls.Instance != null && PlayerControls.Instance.isResting)
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

		if (PlayerControls.Instance != null && PlayerControls.Instance.isResting)
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
		if (PlayerControls.Instance != null && PlayerControls.Instance.isResting)
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
			extraTxt.text = GetTotalUses ? $"{tool.GetTotalUses()} uses" : extraDesc;
		}
		if (descTxt != null)
		{
			// doesn't change with tool level
			if (!hasDifferentDesc)
			{
				descTxt.text = desc;
			}
			// has different for each tool level
			else
			{
				descTxt.text = GetDesc();
			}
		}
		if (eventSystem != null)
		{
			eventSystem.firstSelectedGameObject = this.gameObject;
		}
	}

	public string GetDesc(int x=0)
	{
		PlayerControls p = PlayerControls.Instance;
		if (isShield)
		{
			switch (p.nShieldBonus + x)
			{
				case 1: 	return desc1;
				case 2: 	return desc2;
				case 3: 	return desc3;
				default: 	return desc;
			}
		}
		else if (isExtraSpool)
		{
			switch (p.nExtraSpoolBonus + x)
			{
				case 1: 	return desc1;
				case 2: 	return desc2;
				case 3: 	return desc3;
				default: 	return desc;
			}
		}
		else if (isLootCharm)
		{
			switch (p.nLootCharmBonus + x)
			{
				case 1: 	return desc1;
				case 2: 	return desc2;
				case 3: 	return desc3;
				default: 	return desc;
			}
		}
		else
		{
			switch (tool.level + x)
			{
				case 1: 	return desc1;
				case 2: 	return desc2;
				case 3: 	return desc3;
				default: 	return desc;
			}
		}
	}
}
