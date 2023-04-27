using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caltrops : Tool
{
	protected override void CallChildOnEnemyHit(Collider2D other)
	{
		Enemy target = other.GetComponent<Enemy>();
		Vector2 temp = (other.transform.position - transform.position).normalized;
		float angleZ = 
			Mathf.Atan2(Mathf.Abs(temp.y), temp.x) * Mathf.Rad2Deg;

		if (target != null) 
		{
			if (!target.inParryState && strikePs != null)
			{
				Instantiate(
					strikePs, 
					other.transform.position, 
					Quaternion.Euler(0,0,angleZ + offset * temp.x)
				);
			}
			
			target.TakeDamage(
				dmg, 
				transform, kbDir, 
				force,
				true,
				false
			);
		}
	}
}
