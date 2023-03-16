using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parryable : MonoBehaviour
{
	[SerializeField] Collider2D col;

	private void OnEnable() 
	{
		if (col != null)
			col.enabled = true;
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (col != null && other.CompareTag("Finish"))
		{
			col.enabled = false;
		}
	}
}
