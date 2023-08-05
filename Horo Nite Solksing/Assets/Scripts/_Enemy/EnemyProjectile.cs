using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	[SerializeField] Collider2D hurtBox;
	public Rigidbody2D rb;
	[SerializeField] SpriteRenderer sr;
	[SerializeField] bool canHitback=true;
	[SerializeField] float hitBackForce=5;
	private bool isGrounded;
	private bool thrown;
	
	
	[Space] [SerializeField] bool canBreak;
	[SerializeField] bool canDestroy;
	[SerializeField] float breakAfter=-1;
	[SerializeField] bool breakOnPlayerHit;
	[SerializeField] GameObject breakVfx;

	
	[Space] [SerializeField] bool rotateInDir;

	private void Start() 
	{
		Invoke("Thrown", 0.1f);
		if (breakAfter > 0)
			Invoke("BreakAfter", breakAfter);
	}

	void Thrown()
	{
		thrown = true;
	}
	void BreakAfter()
	{
		canBreak = true;
	}


	private void FixedUpdate() 
	{
		if (isGrounded)
			Deactivate();
		if (rotateInDir)
		{
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		// Debug.Log(rb.velocity);	
	}

	void Deactivate()
	{
		// sr.color = Color.red;
		Destroy(gameObject, 5);
		hurtBox.enabled = false;
		this.enabled = false;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (thrown && other.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
			if (canBreak)
			{
				if (breakVfx != null)
					breakVfx.transform.parent = null;
				Destroy(gameObject);
			}
		}
		if (breakOnPlayerHit && other.gameObject.CompareTag("Player"))
		{
			if (breakVfx != null)
				breakVfx.transform.parent = null;
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		// if (other.CompareTag("Player"))
		// 	Deactivate();

		if (other.CompareTag("Finish"))
		{
			if (canBreak || canDestroy)
			{
				if (breakVfx != null)
					breakVfx.transform.parent = null;
				Destroy(gameObject);
			}
			if (canHitback)
			{
				int x = (other.transform.position.x - transform.position.x > 0) ? -1 : 1;
				rb.AddForce(
					new Vector2(x * hitBackForce, 1),
					ForceMode2D.Impulse
				);
				Deactivate();
			}
		}	
		if (breakOnPlayerHit && other.CompareTag("Player"))
		{
			if (breakVfx != null)
				breakVfx.transform.parent = null;
			Destroy(gameObject);
		}
	}
}
