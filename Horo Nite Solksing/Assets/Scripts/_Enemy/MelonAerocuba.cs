using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonAerocuba : Enemy
{
    private bool chase;
    private bool prep;
	[SerializeField] float atkSpeed=8;
	[SerializeField] float distToPlayer=3;
	[SerializeField] float closeDistTimer;
	[SerializeField] float closeDistTotal=1;
	
	[SerializeField] bool inAtkA;
	[SerializeField] bool inPrepA;
	[SerializeField] bool inLungeA;
	[SerializeField] bool lockDirectionA;
	private Vector2 destPos;
	private int atkDir;

	protected override void IdleAction()
	{
		FlyAround();
		if (chase)
		{
			chase = false;
		}
	}

	protected override void AttackingAction()
	{
		if (!lockDirectionA)
			FacePlayer();
		if (!receivingKb)
		{
			if (!inAtkA)
			{
				RaycastHit2D targetInfo = Physics2D.Raycast(
					target.self.position, 
					new Vector2((PlayerIsToTheRight() ? -1 : 1), 0.5f),  
					distToPlayer,
					whatIsGround
				);
				destPos = (targetInfo.collider != null) ? 
					targetInfo.point : 
					new Vector2((PlayerIsToTheRight() ? -1 : 1), 0.5f) * distToPlayer + (Vector2) target.self.position;
				Vector2 dir = ((Vector3) destPos - transform.position).normalized;
				rb.AddForce(dir * chaseSpeed * 5, ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -chaseSpeed, chaseSpeed),
					Mathf.Clamp(rb.velocity.y, -chaseSpeed, chaseSpeed)
				);
					
				// chasing
				if (!chase)
				{
					chase = true;
				}
				if (closeDistTimer > closeDistTotal)
				{
					closeDistTimer = 0;
					rb.velocity = Vector2.zero;
					atkDir = (model.localScale.x > 0) ? 1 : -1;
					anim.SetTrigger("attack");
				}
				else if (isClose)
					closeDistTimer += Time.fixedDeltaTime;
				else if (closeDistTimer > 0)
					closeDistTimer -= (Time.fixedDeltaTime * 0.5f);
			}
			else if (inPrepA)
			{
				bool targetIsAbove = (target.self.position.y - self.position.y) > 0;
				rb.AddForce(new Vector2(0, targetIsAbove ? 10 : -10), ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),
					Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed)
				);
				if (!prep)
					prep = true;
				// rb.velocity = new Vector2(atkSpeed * atkDir, 0);
			}
			else if (inLungeA)
			{
				if (prep)
				{
					prep = false;
					rb.velocity = Vector2.zero;
				}
				bool targetIsAbove = (target.self.position.y - self.position.y) > 0;
				rb.AddForce(new Vector2(atkSpeed * atkDir * 5, targetIsAbove ? 1 : -1), ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),
					Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed)
				);
				// rb.velocity = new Vector2(atkSpeed * atkDir, 0);
			}
		}
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
