using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edulitoh : Enemy
{
	[Header("Edulitoh")]
	private float closeDistTimer;
	private float closeDistTotal=2;
	[SerializeField] float lungeForce=5;
	[SerializeField] float verticalLungeForce=1;
	[SerializeField] bool inAttackAnim; // assigned by animation
	[SerializeField] bool lungeForward; // assigned by animation
	private bool chase;


    protected override void IdleAction()
	{
		FlyAround();
		if (chase)
		{
			chase = false;
			anim.SetFloat("moveSpeed", 1);
		}
	}


	protected override void AttackingAction()
	{
		if (!inAttackAnim)
		{
			Vector2 dir = (target.self.position - transform.position).normalized;
			rb.AddForce(dir * chaseSpeed * 5, ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -chaseSpeed, chaseSpeed),
				Mathf.Clamp(rb.velocity.y, -chaseSpeed, chaseSpeed)
			);
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
				closeDistTimer -= Time.fixedDeltaTime;
		}
		else if (!beenHurt)
		{
			if (!lungeForward)
				rb.velocity = new Vector2(0, 0);
			else 
			{
				rb.velocity = new Vector2(
					lungeForce * model.localScale.x, 
					(target.transform.position.y - self.position.y < 0) ?
						-verticalLungeForce : verticalLungeForce
				);
			}
		}
	}
}
