using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableColliderAfter : MonoBehaviour
{
    [SerializeField] Collider2D col;
	[SerializeField] float timer;

	private void Start() 
	{
		if (timer > 0 && col != null)
			StartCoroutine(EnableAfterCo());
	}


	IEnumerator EnableAfterCo()
	{
		yield return new WaitForSeconds(timer);
		if (col != null)
			col.enabled = true;
	}
}
