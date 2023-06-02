using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Tool
{
	[SerializeField] float duration;
	[SerializeField] Collider2D col;
	private HashSet<GameObject> alreadyHit;
	[SerializeField] bool hasCamShake=true;
	[SerializeField] int c=0;


	protected override void CallChildOnStart()
	{
		if (alreadyHit == null)
			alreadyHit = new HashSet<GameObject>();	
		if (hasCamShake)
			CinemachineShake.Instance.ShakeCam(11.5f, 0.75f, 2);
		// StartCoroutine( DeactivateCo() );
	}
	// IEnumerator DeactivateCo()
	// {
	// 	yield return new WaitForSeconds(duration);
	// 	if (col != null) col.enabled = false;
	// }

	protected override void CallChildOnEnemyHit(Collider2D other)
	{
		Enemy target = other.GetComponent<Enemy>();
		// Vector2 temp = (other.transform.position - transform.position).normalized;
		// float angleZ = 
		// 	Mathf.Atan2(Mathf.Abs(temp.y), temp.x) * Mathf.Rad2Deg;

		if (target != null) 
		{
			if (alreadyHit.Contains(other.gameObject))
				return;
			else
				alreadyHit.Add(other.gameObject);
			
			target.TakeDamage(
				dmg, 
				transform, 
				kbDir, 
				force,
				false,
				false,
				gameObject
			);
		}
	}
}
