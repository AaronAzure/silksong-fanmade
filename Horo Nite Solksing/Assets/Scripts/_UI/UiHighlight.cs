using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHighlight : MonoBehaviour
{
	[SerializeField] RectTransform rect;
	public RectTransform selected; // target
	
	
	[Space] [SerializeField] Animator anim;
	[SerializeField] float lerpDuration=0.5f;
	private bool isMoving;
	private Vector3 startPos;
	private float timeElapsed;

	public void Select()
	{
		if (anim != null)
			anim.SetTrigger("select");
	}

	public void MoveToButton()
	{
		timeElapsed = 0;
		startPos = rect.position;
		isMoving = true;
	}

	Vector3 GetCurrentSelectedButton()
	{
		if (selected == null)
			return rect.position;
		return selected.position;
	}

	// Update is called once per frame
	void Update()
	{
		if (rect != null)
		{
			if (isMoving)
			{
    			timeElapsed += Time.unscaledDeltaTime;
				rect.position = Vector3.Lerp(startPos, selected.position, timeElapsed / lerpDuration);
				if (timeElapsed > lerpDuration)
					isMoving = false;
			}
		}
	}
}
