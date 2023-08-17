using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonBee : Enemy
{
	private bool chase;
	[SerializeField] bool canChaseAnim;

	
	protected override void IdleAction()
	{
		FlyAround();
	}

	protected override void AttackingAction()
	{
		if (!receivingKb)
		{
			if (canChaseAnim)
			{
				Vector2 dir = (target.self.position + new Vector3(0,0.5f) - transform.position).normalized;
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
			}
			else
			{
				rb.velocity = Vector2.zero;
			}
		}

		// if (closeDistTimer > closeDistTotal)
		// {
		// 	closeDistTimer = 0;
		// 	rb.velocity = Vector2.zero;
		// 	anim.SetTrigger("attack");
		// }
		// else if (isClose)
		// 	closeDistTimer += Time.fixedDeltaTime;
		// else if (closeDistTimer > 0)
		// 	closeDistTimer -= (Time.fixedDeltaTime * 0.5f);
	}

	private bool sighted;

	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			sighted = true;
			currentAction = CurrentAction.none;
			anim.SetTrigger("alert");
			anim.SetBool("isChasing", true);
		}
	}
	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			sighted = false;
			anim.SetBool("isChasing", false);
		}
	}
}
