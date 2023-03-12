using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kevin : Enemy
{
	private float closeDistTimer;
	private float closeDistTotal=1f;
	[SerializeField] float lungeForce=5;
	[SerializeField] bool inAttackAnim; // assigned by animation
	[SerializeField] bool lungeForward; // assigned by animation


    protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void AttackingAction()
	{
		if (!inAttackAnim)
		{
			if (isGrounded)
				ChasePlayer();
			else
				MoveInPrevDirection();

			if (closeDistTimer > closeDistTotal)
			{
				anim.SetTrigger("attack");
				closeDistTimer = 0;
			}
			else if (isClose)
				closeDistTimer += Time.fixedDeltaTime;
			else if (closeDistTimer > 0)
				closeDistTimer -= Time.fixedDeltaTime;
		}
		else if (!beenHurt)
		{
			if (!lungeForward)
				rb.velocity = new Vector2(0, rb.velocity.y);
			else
				rb.velocity = new Vector2(lungeForce * model.localScale.x, rb.velocity.y);
		}
	}
}
