using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableColliderAfter : MonoBehaviour
{
	[SerializeField] Collider2D col;
	[SerializeField] float timer;

	private void Start() 
	{
		if (timer > 0 && col != null)
			StartCoroutine(DisableAfterCo());
	}


	IEnumerator DisableAfterCo()
	{
		yield return new WaitForSeconds(timer);
		if (col != null)
			col.enabled = false;
	}
}
