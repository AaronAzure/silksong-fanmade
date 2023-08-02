using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonBilly : Enemy
{
	[SerializeField] bool upsideDown;
	[SerializeField] Transform flippableObj;
	[SerializeField] Transform playerCheckPos;
	[SerializeField] float distCheck=6;
	private bool isFalling;


	protected override void CallChildOnHurt(int dmg, Vector2 forceDir)
	{
		if (upsideDown)
			DropDown();
	}

	protected override void CallChildOnGizmosSelected()
	{
		if (playerCheckPos != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(playerCheckPos.position, playerCheckPos.position + (Vector3.down * distCheck));
		}
	}

	protected override void CallChildOnStart() 
	{ 
		if (upsideDown)
		{
			rb.gravityScale = 0;
			RaycastHit2D hit = Physics2D.Raycast(model.position, Vector2.up, 1f, whatIsGround);
			if (hit)
				transform.position = hit.point;
		}
	}

	protected override void CheckIsGrounded()
	{
		isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);
	}

	protected override bool CheckSurrounding()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundDetect.position, 
			groundDetect.position + new Vector3(0, upsideDown ? groundDistDetect : -groundDistDetect), 
			whatIsGround
		);
		RaycastHit2D wallInfo = Physics2D.Linecast(
			wallDetect.position, 
			wallDetect.position + new Vector3(model.localScale.x * wallDistDetect, 0), 
			whatIsGround
		);
		return (groundInfo.collider == null || wallInfo.collider != null);
	}

    protected override void IdleAction()
	{
		if (isFalling)
		{
			if (!receivingKb)
				rb.velocity = new Vector2(0, rb.velocity.y);
			if (isGrounded)
			{
				isFalling = false;
				anim.SetFloat("moveSpeed", 1);
			}
		}
		else
		{
			WalkAround();
		}

		if (upsideDown)
		{
			RaycastHit2D hit = Physics2D.Raycast(playerCheckPos.position, Vector2.down, distCheck, finalMask);
			RaycastHit2D ceilingHit = Physics2D.Raycast(model.position, Vector2.up, 1f, whatIsGround);

			if (hit && hit.collider.CompareTag("Player"))
				DropDown();

			if (ceilingHit.collider == null)
				DropDown();
		}
	}

	protected override void AttackingAction()
	{
		if (isFalling)
		{
			if (!receivingKb)
				rb.velocity = new Vector2(0, rb.velocity.y);
			if (isGrounded)
			{
				isFalling = false;
				anim.SetFloat("moveSpeed", 1);
			}
		}
		else
		{
			WalkAround();
		}

		if (upsideDown)
		{
			RaycastHit2D hit = Physics2D.Raycast(playerCheckPos.position, Vector2.down, distCheck, finalMask);
			RaycastHit2D ceilingHit = Physics2D.Raycast(model.position, Vector2.up, 1f, whatIsGround);

			if (hit && hit.collider.CompareTag("Player"))
				DropDown();

			if (ceilingHit.collider == null)
				DropDown();
		}
	}

	protected override void CallChildOnDeath()
	{
		if (upsideDown && flippableObj != null)
		{
			upsideDown = false;
			flippableObj.localScale = Vector3.one;
			flippableObj.localPosition = Vector3.zero;
		}
	}

	protected void DropDown()
	{
		if (upsideDown && flippableObj != null)
		{
			upsideDown = false;
			isFalling = true;
			anim.SetFloat("moveSpeed", 0);
			flippableObj.localScale = Vector3.one;
			flippableObj.localPosition = Vector3.zero;
		}
		rb.gravityScale = 1;
	}
}
