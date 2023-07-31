using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonJumper : Enemy
{
	[Space] [SerializeField] float closeCounter;
	[SerializeField] float jumpSpeedX=3.5f;
	[SerializeField] float jumpSpeedY=8f;
	[SerializeField] float closeLimit=0.5f;
	[SerializeField] bool inAtkA;
	[SerializeField] bool inJumpA;
	private float jumpTimer;
	private float jumpThres=0.1f;


	protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void AttackingAction()
	{
		if (!inAtkA && !inJumpA && isSuperClose && closeCounter < closeLimit)
		{
			closeCounter += Time.fixedDeltaTime;
			if (closeCounter >= closeLimit)
			{
				closeCounter = 0;
				anim.SetTrigger("attack");
			}
		}

		if (inJumpA)
		{
			if (jumpTimer >= jumpThres && isGrounded)
			{
				jumpTimer = 0;
				anim.SetTrigger("landed");
			}
			else
				jumpTimer += Time.fixedDeltaTime;
		}
		else if (!receivingKb)
		{
			if (inAtkA || isSuperClose)
			{
				anim.SetBool("isMoving", false);
				rb.velocity = new Vector2(0, rb.velocity.y);
			}
			else if (!isSuperClose)
			{
				anim.SetBool("isMoving", true);
				ChasePlayer();
			}
		}
	}

	public void _JUMP_ATTACK()
	{
		FacePlayer();
		rb.velocity = new Vector2(jumpSpeedX * model.localScale.x, jumpSpeedY);
	}

	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			currentAction = CurrentAction.none;
			sighted = true;
			anim.SetTrigger("alert");
			anim.SetBool("isChasing", true);
		}

	}
	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			idleCounter = 0;
			sighted = false;
			anim.SetBool("isChasing", false);
		}
	}

}
