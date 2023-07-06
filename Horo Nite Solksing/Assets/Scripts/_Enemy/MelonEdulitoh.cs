using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonEdulitoh : Enemy
{
	private bool chase;
	[SerializeField] float distToPlayer=3;
	[SerializeField] bool alertAnim;
	[SerializeField] bool attackingAnim;
	[SerializeField] float closeDistTimer;
	[SerializeField] float closeDistTotal=1;
	
	[Space] [SerializeField] Transform spawnPos;
	[SerializeField] Rope melonFlail;

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
		if (!receivingKb)
		{
			if (!attackingAnim && !alertAnim)
			{
				RaycastHit2D targetInfo = Physics2D.Raycast(
					transform.position, 
					new Vector2((PlayerIsToTheRight() ? -1 : 1), 0.5f),  
					distToPlayer,
					whatIsGround
				);
				Vector2 dest = (targetInfo.collider != null) ? 
					targetInfo.point : 
					new Vector2((PlayerIsToTheRight() ? -1 : 1), 0.5f) * distToPlayer + (Vector2) target.self.position;

				Vector2 dir = ((Vector3) dest - transform.position).normalized;
				rb.AddForce(dir * chaseSpeed * 5, ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -chaseSpeed, chaseSpeed),
					Mathf.Clamp(rb.velocity.y, -chaseSpeed, chaseSpeed)
				);

				// chasing
				FacePlayer();
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
				else if (isClose)
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

	public void _THROW_MELON_FLAIL()
	{
		if (melonFlail != null)
		{
			melonFlail.gameObject.SetActive(false);
			melonFlail.gameObject.SetActive(true);
			melonFlail.endPos = target.self.position;
		}
	}
}
