using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class UiHighlight : MonoBehaviour
{
	[SerializeField] RectTransform rect;
	[SerializeField] float speed;
	public RectTransform selected; // target
	[Space] 
	[SerializeField] Animator anim;
	[SerializeField] float lerpDuration=0.5f;
	bool isMoving;
	Vector3 startPos;
	float timeElapsed;


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
