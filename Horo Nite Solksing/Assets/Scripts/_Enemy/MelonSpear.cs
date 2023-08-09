using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonSpear : Enemy
{
	[SerializeField] bool inAlertA;
	[SerializeField] bool inBackDashA;
	[SerializeField] bool inAtkA;
	[SerializeField] bool inLungeA;
	[SerializeField] float lungeForce=10;
	[SerializeField] float closeCounter;
	[SerializeField] float closeLimit=0.3f;
	[SerializeField] Transform groundBehindDetect;
	[SerializeField] float backDashSpeed=6;
	private bool justBackDashed;


	protected bool CheckForGround()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundDetect.position, 
			groundDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		return (groundInfo.collider != null);
	}
	// return true if there is ground
	protected virtual bool CheckBehindForGround()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundBehindDetect.position, 
			groundBehindDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		return (groundInfo.collider != null);
	}
	// return true if there is wall behind
	protected virtual bool CheckBehindForWall()
	{
		RaycastHit2D wallInfo = Physics2D.Linecast(
			eyes.position, 
			groundBehindDetect.position, 
			whatIsGround
		);
		return (wallInfo.collider != null);
	}

	protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void AttackingAction()
	{
		if (!inBackDashA && !inAtkA && !inAlertA)
		{
			if (isSuperClose && closeCounter < closeLimit/2 && CheckBehindForGround())
			{
				closeCounter = closeLimit/2;
				anim.SetTrigger("backDash");
			}
			else if (isClose && closeCounter < closeLimit)
			{
				closeCounter += Time.fixedDeltaTime;
				if (closeCounter >= closeLimit)
				{
					closeCounter = 0;
					anim.SetTrigger("attack");
				}
			}
			if (!isClose && closeCounter > 0)
			{
				closeCounter -= (Time.fixedDeltaTime/2);
			}
			
			// chasing
			if (!receivingKb)
			{
				if (!isSuperClose && CheckForGround())
					ChasePlayer();
				else
				{
					rb.velocity = new Vector2(0, rb.velocity.y);
					anim.SetBool("isMoving", false);
				}
			}
			FacePlayer();
		}
		// using spear
		else if (!receivingKb)
		{
			if (inBackDashA)
			{
				justBackDashed = true;
				if (CheckBehindForGround())
					rb.velocity = new Vector2(model.localScale.x * -backDashSpeed, rb.velocity.y);
				else
					rb.velocity = new Vector2(0, rb.velocity.y);
			}
			// one frame only
			else if (justBackDashed && !inBackDashA)
			{
				justBackDashed = false;
				rb.velocity = new Vector2(0, rb.velocity.y);
			}
			else if (inLungeA && CheckForGround())
				rb.velocity = new Vector2(lungeForce * model.localScale.x, rb.velocity.y);
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	protected override void ChasePlayer()
	{
		int playerDir = (target.self.position.x - self.position.x > 0) ? 1 : -1;
		FacePlayer( playerDir );
		if (!receivingKb)
		{
			rb.AddForce(new Vector2(moveSpeed * playerDir * 5, 0), ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -chaseSpeed, chaseSpeed), 
				rb.velocity.y 
			);
		}
		
		if (anim != null) 
		{
			if (hasMoveSpeedAnim && !isFlying)
				anim.SetFloat("moveSpeed", Mathf.Abs(rb.velocity.x));
			if (hasIsMovingAnim)
				anim.SetBool("isMoving", true);
		}
	}

	private bool sighted;
	
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			rb.velocity = new Vector2(0, rb.velocity.y);
			sighted = true;
			currentAction = CurrentAction.none;
			anim.SetTrigger("alert");
		}
	}
	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			sighted = false;
			idleCounter = 0;
		}
	}

}
