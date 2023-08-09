using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	[SerializeField] Collider2D hurtBox;
	public Rigidbody2D rb;
	public Animator anim;
	[SerializeField] SpriteRenderer sr;
	[SerializeField] bool canHitback=true;
	[SerializeField] float hitBackForce=5;
	private bool isGrounded;
	private bool thrown;
	
	
	[Space] [SerializeField] bool canBreak;
	[SerializeField] bool canDestroy;
	[SerializeField] bool deactivateOnGrounded=true;
	[SerializeField] bool destroyOnStop;
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

	bool done;
	private void FixedUpdate() 
	{
		if (isGrounded && deactivateOnGrounded)
			Deactivate();
		if (rotateInDir)
		{
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		if (destroyOnStop && !done && rb.velocity.x == 0)
		{
			done = true;
			if (breakVfx != null)
				breakVfx.transform.parent = null;
			Destroy(gameObject);
		}
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
		if (other.CompareTag("Finish") || other.CompareTag("Respawn"))
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
