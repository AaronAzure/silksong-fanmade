using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile2 : MonoBehaviour
{
	[SerializeField] bool rotateInDir;
	public Rigidbody2D rb;
	[SerializeField] GameObject breakVfx;
	[SerializeField] bool dontDestroyOnHit;


	private void FixedUpdate() 
	{
		if (rotateInDir)
		{
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!dontDestroyOnHit)
		{
			if (other.CompareTag("Ground"))
			{
				DestroySequence();
			}
			else if (other.CompareTag("Player"))
			{
				PlayerControls.Instance.CallOnTakeDamage(transform);
				DestroySequence();
			}
		}
	}

	void DestroySequence()
	{
		if (breakVfx != null)
			breakVfx.transform.parent = null;
		Destroy(gameObject);
	}
}
