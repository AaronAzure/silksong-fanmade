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
	public Tool tool;

	public void SET_TOOL()
	{
		Assert.IsNotNull(PlayerControls.Instance, "PlayerControls.Instance == null");

		if (tool != null && PlayerControls.Instance != null)
		{
			PlayerControls.Instance.EquipTool(tool);
			if (master != null) master.Select();
		}
	}
	public void SET_CREST(int n)
	{
		Assert.IsNotNull(PlayerControls.Instance, "PlayerControls.Instance == null");

		if (PlayerControls.Instance != null)
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
