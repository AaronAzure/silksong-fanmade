using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTest : MonoBehaviour
{
    [SerializeField] GameObject obj;
    private Transform master;

	[Space] [SerializeField] bool alwaysActive;

	private void OnEnable() 
	{
		if (obj == null)
			this.gameObject.SetActive(false);	
		else
			master = obj.transform;
	}

	public void SwapParent(bool thisIsParent=true)
	{
		if (master != null)
		{
			if (!thisIsParent)
				master.parent = null;
			if (master != null && transform.parent != null)
				transform.parent = thisIsParent ? null : master;
			if (thisIsParent)
			{
				transform.position = master.position;
				if (master != null && transform.parent != null)
					master.parent = transform;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (obj != null && other.CompareTag("MainCamera"))	
		{
			obj.SetActive(true);	
			SwapParent(false);
		}
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (obj != null && other.CompareTag("MainCamera2"))	
		{
			obj.SetActive(false);
			SwapParent(true);
		}
	}
}
