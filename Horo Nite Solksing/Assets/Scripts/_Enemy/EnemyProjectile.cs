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
	[SerializeField] bool canBreak;
	[SerializeField] GameObject breakVfx;

	private void Start() 
	{
		Invoke("Thrown", 0.1f);
	}

	void Thrown()
	{
		thrown = true;
	}


	private void FixedUpdate() 
	{
		if (isGrounded)
			Deactivate();
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
				breakVfx.transform.parent = null;
				Destroy(gameObject);
			}
		}
	}

	// private void OnCollisionExit2D(Collision2D other) 
	// {
	// 	if (other.gameObject.CompareTag("Ground"))
	// 		isGrounded = false;
	// }

	private void OnTriggerEnter2D(Collider2D other) 
	{
		// if (other.CompareTag("Player"))
		// 	Deactivate();

		if (canHitback && other.CompareTag("Finish"))
		{
			if (canBreak)
			{
				breakVfx.transform.parent = null;
				Destroy(gameObject);
			}
			int x = (other.transform.position.x - transform.position.x > 0) ? -1 : 1;
			rb.AddForce(
				new Vector2(x * hitBackForce, 1),
				ForceMode2D.Impulse
			);
			Deactivate();
		}	
	}
}
