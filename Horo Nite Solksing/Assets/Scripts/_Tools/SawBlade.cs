using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : Tool
{
	[Space] [Header("Saw Blade")] 
	[SerializeField] Animator anim;
	[SerializeField] bool isCollidingAnim;
	[SerializeField] PlayerHitbox hitbox;

	
	[Space] [SerializeField] float speed=8;
	[SerializeField] float hitSpeed=0.1f;
	[SerializeField] float drawbackSpeed=2f;
	private bool isMoving;
	private bool isColliding;
	private bool isOpening;
	private bool isStuck;
	private int moveDir=1;
	[Space] [SerializeField] Transform wallDetect;
	[SerializeField] Vector2 wallRect;
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] LayerMask whatIsPlatform;

	protected override void CallChildOnStart()
	{
		if (transform.localScale.x < 0)
		{
			moveDir = -1;
		}
		switch (level)
		{
			case 1 :
				hitbox.dmg = 7;
				break;
			case 2 :
				hitbox.dmg = 9;
				break;
			case 3 :
				hitbox.dmg = 11;
				break;
			default :
				break;
		}
	}
	protected override void CallChildOnEnemyHit(Collider2D other) {}

	private void OnDrawGizmosSelected() 
	{
		if (wallDetect != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube(wallDetect.position, wallRect);
		}
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
		if (!isCollidingAnim && anim != null)
		{
			anim.SetBool("isColliding", false);
			isColliding = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!isMoving && !isOpening && rb.velocity.y <= 0 &&
			other.gameObject.CompareTag("Ground"))
		{
			isOpening = true;
			if (anim != null)
				anim.SetTrigger("open");
		}
	}

	private void FixedUpdate() 
	{
		if (isMoving)
		{
			rb.velocity = new Vector2(
				moveDir * ((isCollidingAnim || isColliding) ? hitSpeed : speed), 
				rb.velocity.y
			);

			if (!isStuck)
			{
				if (Physics2D.OverlapBox(wallDetect.position, wallRect, 0, whatIsGround | whatIsPlatform))
				{
					isStuck = true;
					if (destroyAfterCo != null) StopCoroutine(destroyAfterCo);
					destroyAfter = 0.2f;
					destroyAfterCo = StartCoroutine( DestroyAfterCo() );
				}
			}
		}
		else if (isOpening)
		{
			rb.velocity = new Vector2(-moveDir * drawbackSpeed, rb.velocity.y);
		}
	}

	public void MOVE()
	{
		isMoving = true;
	}

	protected override IEnumerator DestroyAfterCo()
	{
		yield return new WaitForSeconds(destroyAfter);
		if (anim != null)
			anim.SetTrigger("destroy");
	}
}
