using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMouseSupport : MonoBehaviour
{
	public static UiMouseSupport Instance;
	[field: SerializeField] Canvas canvas;
	[SerializeField] int origSortOrder;
	[field: SerializeField] Canvas canvas2;
	[SerializeField] int origSortOrder2;


    void Awake()
    {
        if (canvas != null)
		{
			origSortOrder = canvas.sortingOrder;
		}
        if (canvas2 != null)
		{
			origSortOrder2 = canvas2.sortingOrder;
		}
    }

	private void OnEnable() 
	{
		if (UiMouseSupport.Instance != null)
		{
			UiMouseSupport.Instance.OverrideSortingOrder();
		}
		if (canvas != null)
		{
			canvas.sortingOrder = 32767;
			UiMouseSupport.Instance = this;
		}
		if (canvas2 != null)
		{
			canvas2.sortingOrder = 32766;
		}
	}

	private void OnDisable() 
	{
		if (canvas != null)
		{
			canvas.sortingOrder = origSortOrder;
		}	
		if (canvas2 != null)
		{
			canvas2.sortingOrder = origSortOrder2;
		}	
		UiMouseSupport.Instance = null;
	}

	public void RevertToOriginalSortingOrder() 
	{
		OnDisable();
	}

	public void OverrideSortingOrder()
	{
		if (canvas != null)
		{
			canvas.sortingOrder -= 10;
		}	
		if (canvas2 != null)
		{
			canvas2.sortingOrder -= 10;
		}	
	}
}
