using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTest : MonoBehaviour
{
    [SerializeField] GameObject obj;
    private Transform master;

	private void OnEnable() 
	{
		if (obj == null)
			this.gameObject.SetActive(false);	
		else
			master = obj.transform;
	}

	public void SwapParent(Transform o, bool thisIsParent=true)
	{
		if (!thisIsParent)
			o.parent = null;
		transform.parent = thisIsParent ? null : o;
		if (thisIsParent)
		{
			transform.position = o.position;
			o.parent = transform;
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (obj != null && other.CompareTag("MainCamera"))	
		{
			obj.SetActive(true);	
			SwapParent(master, false);
		}
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (obj != null && other.CompareTag("MainCamera2"))	
		{
			obj.SetActive(false);
			SwapParent(master, true);
		}
	}
}
