using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : Tool
{
	[Space] [Header("Saw Blade")] 
	[SerializeField] Animator anim;
	[SerializeField] float speed=8;
	[SerializeField] float hitSpeed=0.1f;
	private bool isMoving;
	private bool isColliding;
	private int moveDir=1;
	[SerializeField] Transform wallDetect;


	protected override void CallChildOnEnemyHit(Collider2D other)
	{
	}

	private void OnDrawGizmosSelected() 
	{

	}
	

	public void HitEnemy()
	{
		if (anim != null)
		{
			anim.SetBool("isColliding", true);
			isColliding = true;
		}
	}
	public void ExitEnemy()
	{
		if (anim != null)
		{
			anim.SetBool("isColliding", false);
			isColliding = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!isMoving && other.gameObject.CompareTag("Ground") && rb.velocity.y <= 0)
		{
			if (anim != null)
				anim.SetTrigger("open");
		}
	}

	private void FixedUpdate() 
	{
		if (isMoving)
		{
			rb.velocity = new Vector2(
				moveDir * (isColliding ? hitSpeed : speed), 
				rb.velocity.y
			);
		}
	}

	public void MOVE()
	{
		if (transform.localScale.x < 0)
		{
			moveDir = -1;
		}
		isMoving = true;
	}

	protected override IEnumerator DestroyAfterCo()
	{
		yield return new WaitForSeconds(destroyAfter);
		if (anim != null)
			anim.SetTrigger("destroy");
	}
}
