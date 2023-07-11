using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Rewired.Integration.UnityUI;

public class UiShopButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
	[SerializeField] UiShopHighlight master;
	[SerializeField] float offset;
	[SerializeField] RewiredEventSystem res;

	public void OnPointerEnter(PointerEventData eventData)
	{
		// if (master != null)
		// {
		// 	master.offset = this.offset;
		// 	master.MoveToButton();
		// }
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (master != null)
		{
			master.offset = this.offset;
			master.MoveToButton();
		}
		if (res != null)
		{
			res.firstSelectedGameObject = this.gameObject;
		}
	}
}
