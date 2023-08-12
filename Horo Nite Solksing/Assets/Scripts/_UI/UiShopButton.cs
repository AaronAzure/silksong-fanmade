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
	[SerializeField] GameObject upgradeIcon;
	[SerializeField] TextMeshProUGUI costTxt;
	[SerializeField] TextMeshProUGUI titleTxt;
	[SerializeField] TextMeshProUGUI extraTxt;
	[SerializeField] TextMeshProUGUI descTxt;
	private int nPurchased;
	[SerializeField] int maxPurchases=3;
	[SerializeField] bool mustUnlockFirst;
	[SerializeField] int unlockCost=10;

	[Space] [SerializeField] string title;
	[SerializeField] [TextArea(2,4)] string desc;


	public enum Upgrade {
		pin,
		pimpillo,
		caltrop,
		sawblade,
		shield,
		extraSpool,
		lootCharm,
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
			if (mustUnlockFirst)
				costTxt.text = $"{unlockCost}";
			else
				costTxt.text = $"{PlayerControls.Instance.GetCost(upgrade)}";
		}	
	}

	private bool CheckIsTool()
	{
		return (upgrade != Upgrade.health && upgrade != Upgrade.spool);
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
		if (titleTxt != null)
			titleTxt.text = title;
		if (extraTxt != null)
		{
			if (mustUnlockFirst)
				extraTxt.text = $"{unlockCost}";
			else
				extraTxt.text = $"{PlayerControls.Instance.GetCost(upgrade)}";
		}
		if (descTxt != null)
		{
			if (mustUnlockFirst)
				descTxt.text = $"Unlock {title} tool";
			else
				descTxt.text = desc;
		}
	}

	public void _PURCHASE()
	{
		// unlock tool
		if (mustUnlockFirst)
		{
			if (PlayerControls.Instance.CanAffordDirectPurchase(unlockCost))
			{
				mustUnlockFirst = false;
				PlayerControls.Instance.UnlockPurchase(upgrade, unlockCost);
				PlayerControls.Instance.FullRestore();
				SetCostText();
				if (upgradeIcon != null)
					upgradeIcon.SetActive(true);
				if (descTxt != null)
				{
					if (mustUnlockFirst)
						descTxt.text = $"Unlock {title} tool";
					else
						descTxt.text = desc;
				}
			}
		}
		else
		{
			// Is able to purchase
			if (nPurchased < maxPurchases && PlayerControls.Instance.CanAffordPurchase(upgrade))
			{
				PlayerControls.Instance.MakePurchase(upgrade);
				PlayerControls.Instance.FullRestore();
				nPurchased++;
				SetCostText();
			}
			if (nPurchased >= maxPurchases)
			{
				gameObject.SetActive(false);
				master.SelectNewButton();
			}
		}
	}
}
