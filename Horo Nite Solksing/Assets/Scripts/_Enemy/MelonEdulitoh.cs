using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonEdulitoh : Enemy
{
	private bool chase;
	[SerializeField] float distToPlayer=3;
	[SerializeField] bool alertAnim;
	[SerializeField] float closeDistTimer;
	[SerializeField] float closeDistTotal=1;
	
	[Space] [SerializeField] Transform gripPos;
	[SerializeField] Rope melonFlail;
	[SerializeField] float flailReach=5f;
	[SerializeField] bool lockDirectionAnim;
	private Vector2 destPos;
	[SerializeField] bool stillPreping;
	private bool thrown;

	protected override void CallChildOnStart()
	{
		if (melonFlail != null)
		{
			melonFlail.target = target.self;
		}
	}

	protected override void IdleAction()
	{
		FlyAround();
		if (chase)
		{
			chase = false;
			anim.SetFloat("moveSpeed", moveSpeed);
		}
	}

	protected override void AttackingAction()
	{
		if (!lockDirectionAnim)
			FacePlayer();
		if (!receivingKb)
		{
			if (!alertAnim)
			{
				Vector2 dir = ((Vector3) destPos - transform.position).normalized;
				rb.AddForce(dir * chaseSpeed * 5, ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -chaseSpeed, chaseSpeed),
					Mathf.Clamp(rb.velocity.y, -chaseSpeed, chaseSpeed)
				);
			}

			if (!stillAttacking && !alertAnim)
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
					
				// chasing
				if (!chase)
				{
					chase = true;
					anim.SetFloat("moveSpeed", chaseSpeed);
				}
				if (closeDistTimer > closeDistTotal)
				{
					closeDistTimer = 0;
					rb.velocity = Vector2.zero;
					anim.SetTrigger("attack");
				}
				else if (!stillPreping && isClose)
					closeDistTimer += Time.fixedDeltaTime;
				else if (closeDistTimer > 0)
					closeDistTimer -= (Time.fixedDeltaTime * 0.5f);
			}
			else
			{
				rb.velocity = Vector2.zero;
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

	public void BallRetract()
	{
		anim.SetTrigger("retract");
		thrown = false;
	}


	public void _THROW_MELON_FLAIL()
	{
		if (!thrown && melonFlail != null)
		{
			thrown = true;
			Vector2 dir = (target.self.position - gripPos.position).normalized;
			// RaycastHit2D targetInfo = Physics2D.Raycast(
			// 	gripPos.position, 
			// 	dir,  
			// 	flailReach,
			// 	whatIsGround
			// );
			
			// // missed
			// if (targetInfo.collider == null)
			// {
			// 	melonFlail.skipStuckState = true;
			// 	anim.Play("melon_edulitoh_attack_anim", -1, 0.75f);
			// 	melonFlail.endPos = gripPos.position + (Vector3) (dir * flailReach);
			// }
			// // ht something
			// else
			// {
			// 	melonFlail.endPos = targetInfo.point;
			// }
			melonFlail.endPos = target.self.position;
			melonFlail.dir = model.transform.localScale.x > 0 ? 1 : -1;
			melonFlail.gameObject.SetActive(false);
			melonFlail.gameObject.SetActive(true);
		}
	}

	public void _HIDE_ROPE()
	{
		if (melonFlail != null)
		{
			melonFlail.gameObject.SetActive(false);
		}
	}

	public void _RETRACT_MELON_FLAIL()
	{
		if (melonFlail != null)
		{
			melonFlail.isRetracting = true;
		}
	}
}
