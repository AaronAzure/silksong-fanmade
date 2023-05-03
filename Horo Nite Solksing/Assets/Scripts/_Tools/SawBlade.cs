using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : Tool
{
	[Space] [Header("Saw Blade")] 
	[SerializeField] Animator anim;
	[SerializeField] float speed=8;
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

	protected override void CallChildOnStart()
	{
		if (transform.localScale.x < 0)
		{
			moveDir = -1;
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
		if (anim != null)
		{
			anim.SetBool("isColliding", false);
			isColliding = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!isMoving && !isOpening && other.gameObject.CompareTag("Ground") && rb.velocity.y <= 0)
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
				moveDir * (isColliding ? hitSpeed : speed), 
				rb.velocity.y
			);

			if (!isStuck)
			{
				if (Physics2D.OverlapBox(wallDetect.position, wallRect, 0, whatIsGround))
				{
					isStuck = true;
					if (destroyAfterCo != null) StopCoroutine(destroyAfterCo);
					destroyAfter = 0.5f;
					destroyAfterCo = StartCoroutine( DestroyAfterCo() );
					// Debug.Log("<color=magenta>stuck!!</color>");
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
