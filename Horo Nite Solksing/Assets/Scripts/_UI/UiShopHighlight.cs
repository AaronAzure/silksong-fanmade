using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopHighlight : MonoBehaviour
{
	[SerializeField] RectTransform rect;
	[SerializeField] Canvas canvas;
	[HideInInspector] public float offset;
	private Vector2 prevAnchor=new Vector2(0.5f,0.1f);
	
	
	// [Space] [SerializeField] Animator anim;
	[SerializeField] float lerpDuration=0.5f;
	private bool isMoving;
	private Vector2 anchorMin;
	private Vector2 anchorMax;
	private Vector2 destMin;
	private Vector2 destMax;
	private float timeElapsed;
	[SerializeField] bool inverse;

	private void OnEnable() 
	{
		rect.anchorMin = prevAnchor;
		rect.anchorMax = prevAnchor;
	}

	public void MoveToButton()
	{
		timeElapsed = 0;
		anchorMin = rect.anchorMin;
		anchorMax = rect.anchorMax;
		destMin = new Vector2(rect.anchorMin.x, offset);
		destMax = new Vector2(rect.anchorMax.x, offset);
		prevAnchor = destMin;
		isMoving = true;
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
}
