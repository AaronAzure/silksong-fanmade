using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UiShopButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
	[HideInInspector] public Button self;
	[SerializeField] UiShopHighlight master;
	public float offset;
	[SerializeField] TextMeshProUGUI costTxt;
	private int nPurchased;


	public enum Upgrade {
		pin,
		pimpillo,
		caltrop,
		sawblade,
		shield,
		extraSpool,
		health,
		spool
	}
	[SerializeField] protected Upgrade upgrade=0;

	private void Awake() 
	{
		self = GetComponent<Button>();	
	}

	private void OnEnable() 
	{
		SetCostText();
	}

	private void SetCostText() 
	{
		if (costTxt != null)
		{
			costTxt.text = $"{PlayerControls.Instance.GetCost(upgrade)}";
		}	
	}

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
			master.MoveToButton(this);
		}
	}

	public void _PURCHASE()
	{
		// Is able to purchase
		if (nPurchased < 3 && PlayerControls.Instance.CanAffordPurchase(upgrade))
		{
			PlayerControls.Instance.MakePurchase(upgrade);
			nPurchased++;
			SetCostText();
		}
		if (nPurchased >= 3)
		{
			gameObject.SetActive(false);
			master.SelectNewButton();
		}
	}
}
