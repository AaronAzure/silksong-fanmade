using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonBilly : Enemy
{
	[SerializeField] bool upsideDown;
	[SerializeField] Transform flippableObj;


	protected override void CallChildOnHurt(int dmg, Vector2 forceDir)
	{
		if (upsideDown)
			DropDown();
	}

	protected override void CallChildOnStart() 
	{ 
		if (upsideDown)
		{
			rb.gravityScale = 0;
			RaycastHit2D hit = Physics2D.Raycast(model.position, Vector2.up, 1f, whatIsGround);
			if (hit)
				transform.position = hit.transform.position;
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
		WalkAround();
		if (upsideDown)
		{
			RaycastHit2D hit = Physics2D.Raycast(model.position, Vector2.down, 7f, finalMask);
			if (hit)
				Debug.Log(hit.collider.tag);

			if (hit && hit.collider.CompareTag("Player"))
				DropDown();
		}
	}

	protected override void AttackingAction()
	{
		WalkAround();
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
			flippableObj.localScale = Vector3.one;
			flippableObj.localPosition = Vector3.zero;
		}
		rb.gravityScale = 1;
	}
}
