using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonJumper : Enemy
{
	[Space] [SerializeField] float closeCounter;
	[SerializeField] bool hasHighJump;
	[SerializeField] float highJumpSpeedX=2.5f;
	[SerializeField] float highJumpSpeedY=12f;
	[SerializeField] float longJumpSpeedX=7f;
	[SerializeField] float longJumpSpeedY=7f;
	[SerializeField] float closeDist=6;

	
	[Space] [SerializeField] float closeLimit=0.5f;
	[SerializeField] bool inAtkA;
	[SerializeField] bool inJumpA;
	[SerializeField] bool isFacingPlayerA;
	private float jumpTimer;
	private float jumpThres=0.1f;


	protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void AttackingAction()
	{
		if (isGrounded && !inAtkA && !inJumpA && isSuperClose && closeCounter < closeLimit)
		{
			closeCounter += Time.fixedDeltaTime;
			if (closeCounter >= closeLimit)
			{
				closeCounter = 0;
				anim.SetTrigger("attack");
			}
		}

		if (isFacingPlayerA)
			FacePlayer();

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
			if (isGrounded && (inAtkA || isSuperClose))
			{
				anim.SetBool("isMoving", false);
				rb.velocity = new Vector2(0, rb.velocity.y);
			}
			else if (!isSuperClose)
			{
				anim.SetBool("isMoving", true);
				ChasePlayer();
			}
			anim.SetFloat("jumpVelocity", rb.velocity.y);
		}
	}

	public void _JUMP_ATTACK()
	{
		if (!isGrounded)
			return;
			
		// high jump
		if (hasHighJump && DistanceToPlayer(false) < closeDist)
			rb.velocity = new Vector2(highJumpSpeedX * model.localScale.x, highJumpSpeedY);
		// long jump
		else
			rb.velocity = new Vector2(longJumpSpeedX * model.localScale.x, longJumpSpeedY);
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
