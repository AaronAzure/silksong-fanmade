using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{

	public int dmg;
	[SerializeField] float kbForce; 
	[SerializeField] Vector2 kbDir; 
	[SerializeField] bool overrideShake;
	[SerializeField] bool canParry=true;
	[Space] [SerializeField] protected GameObject strikePs;
	[SerializeField] protected float offset=15;

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy"))
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
					kbForce,
					overrideShake,
					canParry
				);
				if (isSawBlade && sawBlade != null)
				{
					sawBlade.HitEnemy();
				}
			}
		}
		if (other.CompareTag("Breakable"))
		{
			Breakable target = other.GetComponent<Breakable>();
			if (target != null) 
			{
				target.Damage(dmg);
				if (isSawBlade && sawBlade != null)
				{
					sawBlade.HitEnemy();
				}
			}
		}
		if (other.CompareTag("Shield"))
		{
			other.GetComponent<EnemyShield>().Attacked(false);
		}
	}


	[SerializeField] bool isSawBlade;
	[SerializeField] SawBlade sawBlade;

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (isSawBlade && sawBlade != null && other.CompareTag("Enemy"))
		{
			sawBlade.ExitEnemy();
		}
	}
}
