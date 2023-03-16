using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parryable : MonoBehaviour
{
	[SerializeField] Collider2D col;
	[SerializeField] GameObject parryEffect;
	public static bool parried;

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
			if (parryEffect != null)
			{
				Vector2 orig = Vector2.Lerp(other.transform.position, transform.position, 0.5f);
				Instantiate(parryEffect, orig, Quaternion.identity);
				StartCoroutine( ParryCo() );
			}
		}
	}

	IEnumerator ParryCo()
	{
		if (parried) 
			yield break;
		Time.timeScale = 0;
		parried = true;

		yield return new WaitForSecondsRealtime(0.25f);
		parried = false;
		Time.timeScale = 1;
	}
}
