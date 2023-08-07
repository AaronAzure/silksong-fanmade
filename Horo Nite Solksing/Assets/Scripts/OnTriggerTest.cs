using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTest : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] Transform master;
	[SerializeField] Transform mainHolder;

	[Space] [SerializeField] bool alwaysActive;

	[Space] [SerializeField] bool isMelonCircus;
	

	private void Awake() 
	{
		mainHolder = GetMasterParent();
	}

	private Transform GetMasterParent()
	{
		return isMelonCircus ? transform.parent.parent : transform.parent.parent;
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
			if (master != null && mainHolder != null && transform.parent != null)
				transform.parent = thisIsParent ? mainHolder : master;

			// enemy active
			if (!thisIsParent)
			{
				if (mainHolder != null)
					master.parent = mainHolder;
				transform.parent = master;
			}

			// enemy hidden
			else
			{
				transform.position = master.position;
				if (master != null && transform.parent != null)
					master.parent = this.transform;
			}
		}
	}

	public void OnMasterDeath()
	{
		if (master != null && mainHolder != null)
			master.parent = mainHolder;
		transform.parent = null;
		Destroy(this);
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
