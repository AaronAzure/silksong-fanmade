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

	protected override void CallChildOnFixedUpdate()
	{
		if (anim != null)
			anim.SetBool("inAttackAnim", inAttackAnim);
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
				CHECK_PLAYER_LOCATION();

				anim.SetTrigger("attack");
				closeDistTimer = 0;
			}
			else if (isClose)
				closeDistTimer += Time.fixedDeltaTime;
			else if (closeDistTimer > 0)
				closeDistTimer -= (Time.fixedDeltaTime * 0.5f);
		}
		else if (!receivingKb)
		{
			if (!lungeForward || !CheckCliff())
				rb.velocity = new Vector2(0, rb.velocity.y);
			else 
				rb.velocity = new Vector2(lungeForce * model.localScale.x, rb.velocity.y);
		}
	}

	public void CHECK_PLAYER_LOCATION()
	{
		if (Mathf.Abs(target.transform.position.x - self.position.x) < 2.5f &&
			(target.transform.position.y - self.position.y) > 1.75f)
		{
			anim.SetFloat("nAttack", 1);
		}
		else
			anim.SetFloat("nAttack", 0);
	}
}
