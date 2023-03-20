using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuart : Enemy
{
	private bool canJump;

	protected override void CallChildOnStart()
	{
		// base.CallChildOnStart();
		StartCoroutine( ToggleJump() );
	}

	protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void CallChildOnHurt()
	{
		// if (isGrounded && jumpCo == null)
		if (isGrounded && Random.Range(0,2) == 0 && jumpCo == null && PlayerInFarFront())
			jumpCo = StartCoroutine( JumpCo(1) );
	}

	protected override void AttackingAction()
	{
		// if (jumpCo)
		if (isGrounded)
			ChasePlayer();
		else
			MoveInPrevDirection();
		if (isGrounded && jumpCo == null && target.self.position.y - self.position.y > 1)
			jumpCo = StartCoroutine( JumpCo(1) );
		else if (isGrounded && canJump && jumpCo == null && PlayerInFront())
			jumpCo = StartCoroutine( JumpCo(1) );
	}

	IEnumerator ToggleJump()
	{
		yield return new WaitForSeconds(Random.Range(1,4));
		canJump = !canJump;
		StartCoroutine( ToggleJump() );
	}
}
