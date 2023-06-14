using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	private bool done;


	private void OnEnable() {
		done = false;
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!done && gameObject.CompareTag("Finish") && other.CompareTag("EnemyAttack"))
		{
			done = true;
			p.ShawRetreat(false);
		}
		else if (!done && other.CompareTag("Enemy"))
		{
			done = true;
			p.ShawRetreat(false);
		}
		else if (!done && other.CompareTag("Breakable"))
		{
			done = true;
			p.ShawRetreat(false);
		}
	}
}
