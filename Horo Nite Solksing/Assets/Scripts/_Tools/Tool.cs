using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
	// [SerializeField] protected int uses=4;
	[SerializeField] public int totaluses=4;

	[Space] [SerializeField] protected int dmg;
	public bool toRight=true;
	[SerializeField] protected Vector2 dir;
	[SerializeField] protected float force;
	[SerializeField] protected Vector2 kbDir;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] GameObject strikePs;
	[SerializeField] float offset=15;

	[Space] [SerializeField] bool destroyOnWallHit;
	[Space] [SerializeField] bool destroyOnEnemyHit;

	private void Start() 
	{
		if (!toRight)
		{
			rb.velocity = new Vector2(-dir.x, dir.y);
			kbDir = new Vector2(-kbDir.x, kbDir.y);
			gameObject.transform.localScale = new Vector3(
				-transform.localScale.x,
				transform.localScale.y
			);
		}
		else
		{
			rb.velocity = dir;
		}
		CallChildOnStart();
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy"))
		{
			CallChildOnEnemyHit(other);
			if (destroyOnEnemyHit)
			{
				CallChildOnHit();
			}
		}
		if (destroyOnWallHit && other.CompareTag("Ground"))
		{
			CallChildOnHit();
		}
	}

	protected virtual void CallChildOnEnemyHit(Collider2D other)
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
				force
			);
		}
	}
	protected virtual void CallChildOnHit() 
	{
		Destroy(gameObject);
	}
	protected virtual void CallChildOnStart() {}
}
