using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
	// [SerializeField] protected int uses=4;
	[SerializeField] public int totaluses=4;
	[SerializeField] public Sprite icon;

	[Space] [SerializeField] protected int dmg;
	public bool toRight=true;
	[SerializeField] protected Vector2 dir;
	[HideInInspector] public float velocityMultiplier=1;
	[SerializeField] protected float force;
	[SerializeField] protected Vector2 kbDir;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] public bool isMultiple;
	[SerializeField] public int nCopies=4;
	[Space] [SerializeField] protected GameObject strikePs;
	[SerializeField] protected float offset=15;

	[Space] [SerializeField] bool destroyOnWallHit;
	[SerializeField] bool destroyOnEnemyHit;
	[HideInInspector] public bool justThrown;
	[Space] [SerializeField] bool noStart;
	[SerializeField] protected float destroyAfter;
	[SerializeField] protected Coroutine destroyAfterCo;

	private void Start() 
	{
		if (!noStart)
		{
			if (!toRight)
			{
				rb.velocity = new Vector2(-dir.x * velocityMultiplier, dir.y);
				kbDir = new Vector2(-kbDir.x, kbDir.y);
				gameObject.transform.localScale = new Vector3(
					-transform.localScale.x,
					transform.localScale.y
				);
			}
			else
			{
				rb.velocity = dir * velocityMultiplier;
			}
			CallChildOnStart();

			if (destroyAfter > 0)
				destroyAfterCo = StartCoroutine( DestroyAfterCo() );
		}
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

	protected virtual IEnumerator DestroyAfterCo()
	{
		yield return new WaitForSeconds(destroyAfter);
		Destroy(gameObject);
	}

	public void DESTROY() 
	{
		Destroy(gameObject);
	}
}
