using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired.Integration.UnityUI;

public class UiShopHighlight : MonoBehaviour
{
	[SerializeField] RectTransform rect;
	[SerializeField] Canvas canvas;
	[HideInInspector] public float offset;
	[SerializeField] float yOffset=0.25f;
	private Vector2 prevAnchor=new Vector2(0.5f,0.1f);
	[SerializeField] List<UiShopButton> shopButtons;
	[SerializeField] UiShopButton currButton;
	[SerializeField] UiShopButton prevButton;
	
	
	[Space] [SerializeField] RewiredEventSystem res;
	
	
	// [Space] [SerializeField] Animator anim;
	[SerializeField] float lerpDuration=0.5f;
	private bool isMoving;
	private Vector2 anchorMin;
	private Vector2 anchorMax;
	private Vector2 destMin;
	private Vector2 destMax;
	private float timeElapsed;
	[SerializeField] bool inverse;

	private void Awake() 
	{
		if (shopButtons == null)
			shopButtons = new List<UiShopButton>();	
		else
			SetButtonOffset();
	}

	private void OnEnable() 
	{
		rect.anchorMin = prevAnchor;
		rect.anchorMax = prevAnchor;
		if (currButton != null && !currButton.gameObject.activeSelf)
		{
			Debug.Log("SOLD OUT - Select New Button");
			SelectNewButton();
		}
	}

	public void MoveToButton(UiShopButton o)
	{
		timeElapsed = 0;
		anchorMin = rect.anchorMin;
		anchorMax = rect.anchorMax;
		destMin = new Vector2(rect.anchorMin.x, offset);
		destMax = new Vector2(rect.anchorMax.x, offset);
		prevAnchor = destMin;
		isMoving = true;
		if (res != null)
		{
			prevButton = currButton;
			currButton = o;
			res.firstSelectedGameObject = o.gameObject;
			if (prevButton == currButton)
				prevButton = null;
		}
	}

	private void SetButtonOffset()
	{
		float y = 0.1f;
		foreach (UiShopButton b in shopButtons)
		{
			b.offset = y;
			y += yOffset;
		}
	}

	public void SelectNewButton()
	{
		// no previous button registered
		if (prevButton == null)
		{
			// has more buttons
			if (shopButtons.Count > 1)
			{
				int ind = shopButtons.IndexOf(currButton);
				shopButtons.Remove(currButton);
				if (ind == shopButtons.Count)
					prevButton = shopButtons[ind-1];
				else if (shopButtons[ind] != null && shopButtons[ind].gameObject.activeSelf)
					prevButton = shopButtons[ind];
				else
					prevButton = shopButtons[ind-1];
			}
		}
		else
			shopButtons.Remove(currButton);

		// has button to select from
		if (shopButtons.Count > 0)
		{
			currButton = prevButton;
			SetButtonOffset();
			if (prevButton != null && prevButton.self != null)
				prevButton.self.Select();
			prevButton = null;
		}
	}

    void Update()
	{
		if (rect != null)
		{
			if (isMoving)
			{
    			timeElapsed += Time.unscaledDeltaTime;
				
				rect.anchorMin = Vector2.Lerp(anchorMin, destMin, timeElapsed / lerpDuration); 
				rect.anchorMax = Vector2.Lerp(anchorMax, destMax, timeElapsed / lerpDuration); 
				
				if (timeElapsed > lerpDuration)
					isMoving = false;
			}
		}
	}

	public bool HasStuffToPurchase()
	{
		// at least one button is active
		if (shopButtons.Count > 0)
		{
			foreach (UiShopButton btn in shopButtons)
			{
				if (btn.gameObject.activeSelf)
					return true;
			}
		}
		return false;
	}
}
