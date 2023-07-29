using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTest : MonoBehaviour
{
    [SerializeField] GameObject obj;
    private Transform master;
	private Transform mainHolder;

	[Space] [SerializeField] bool alwaysActive;

	private void Awake() 
	{
		mainHolder = transform.parent.parent;
	}

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
			if (master != null && transform.parent != null)
				transform.parent = thisIsParent ? mainHolder : master;

			// enemy active
			if (!thisIsParent)
				master.parent = mainHolder;

			// enemy hidden
			else
			{
				transform.position = master.position;
				if (master != null && transform.parent != null)
					master.parent = this.transform;
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
