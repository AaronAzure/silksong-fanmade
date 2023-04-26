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
	[SerializeField] Vector3 offset;
	public RectTransform selected;
	// [HideInInspector] public RectTransform selected;
	[Space] [SerializeField] bool lerp;
	[SerializeField] Animator anim;
	// private Vector3 startPos;


	public void Select()
	{
		if (anim != null)
			anim.SetTrigger("select");
	}

	Vector3 GetCurrentSelectedButton()
	{
		if (selected == null)
			return transform.position;
		return selected.localPosition + offset;
	}

	// Update is called once per frame
	void Update()
	{
		if (rect != null)
		{
			if (lerp)
				rect.localPosition = Vector3.MoveTowards(rect.localPosition, GetCurrentSelectedButton(), speed);
			else
				rect.localPosition = GetCurrentSelectedButton();
		}
	}
}
