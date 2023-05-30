using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuart : Enemy
{
	private bool canJump;
	private bool engaged;
	public bool jumping;

	protected override void CallChildOnStart()
	{
		// base.CallChildOnStart();
		StartCoroutine( ToggleJump() );
	}

	protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void CallChildOnHurtAfter()
	{
		// if (isGrounded && jumpCo == null)
		if (isGrounded && Random.Range(0,2) == 0 && jumpCo == null && PlayerInFarFront())
			anim.SetTrigger("jump");
			// jumpCo = StartCoroutine( JumpCo(1) );
	}

	protected override void AttackingAction()
	{
		// if (jumpCo)
		// if (!jumping)
		// {
			// chasing
			if (isGrounded)
				ChasePlayer();
			// cannot change direction whilst falling
			else
				MoveInPrevDirection();
		// }

		// player jumped
		if (isGrounded && engaged && jumpCo == null && target.self.position.y - self.position.y > 1)
			anim.SetTrigger("jump");
			// jumpCo = StartCoroutine( JumpCo(1) );
		else if (isGrounded && engaged && canJump && jumpCo == null && PlayerInFront())
			anim.SetTrigger("jump");
			// jumpCo = StartCoroutine( JumpCo(1) );
	}

	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			sighted = true;
			StartCoroutine( EngageCo() );
		}
	}
	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			engaged = sighted = false;
		}
	}
	
	IEnumerator EngageCo()
	{
		yield return new WaitForSeconds(0.5f);
		engaged = true;
	}


	IEnumerator ToggleJump()
	{
		yield return new WaitForSeconds(Random.Range(1,4));
		canJump = !canJump;
		StartCoroutine( ToggleJump() );
	}
}
