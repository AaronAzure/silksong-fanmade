using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Otto : Enemy
{
	private float closeDistTimer;
	[SerializeField] float closeDistTotal=2;
	[SerializeField] bool inAttackAnim; // assigned by animation
	[SerializeField] float throwForce=8;
	[SerializeField] EnemyProjectile bellObj;
	[SerializeField] Transform spawnPos;
	[SerializeField] float atkSpeed=1;
	private bool chase;

	[Space] [SerializeField] bool isWatermelon;
	private int throwCount;

	protected override void CallChildOnStart()
	{
		anim.SetFloat("atkSpeed", atkSpeed);
	}

    protected override void IdleAction()
	{
		FlyAround();
		anim.SetFloat("moveSpeed", 1);
		
		if (chase)
		{
			chase = false;
			anim.SetFloat("moveSpeed", 1);
		}
	}


	protected override void AttackingAction()
	{
		if (!inAttackAnim || isWatermelon)
		{
			Vector2 dir = (target.self.position - transform.position).normalized;
			rb.AddForce(dir * chaseSpeed * 5, ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -chaseSpeed, chaseSpeed),
				Mathf.Clamp(rb.velocity.y, -chaseSpeed, chaseSpeed)
			);
		}

		// chasing
		if (!inAttackAnim)
		{
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
			FacePlayer();
			if (!receivingKb)
				rb.velocity = Vector2.zero;
		}
	}

	public void THROW_BELL()
	{
		if (bellObj != null)
		{
			FacePlayer();
			var obj = Instantiate(bellObj, spawnPos.position, Quaternion.identity);
			obj.rb.AddForce( new Vector2(
				(target.self.position.x - obj.transform.position.x) * 1.75f,
				throwForce), 
				ForceMode2D.Impulse
			);
			if (isWatermelon)
			{
				if (atPhase3 && throwCount < 2)
				{
					throwCount++;
					closeDistTimer = 0;
					rb.velocity = Vector2.zero;
					anim.SetTrigger("attack");
				}
				else if (!atPhase3 && atPhase2 && throwCount < 1)
				{
					throwCount++;
					closeDistTimer = 0;
					rb.velocity = Vector2.zero;
					anim.SetTrigger("attack");
				}
				else 
				{
					throwCount = 0;
				}
			}
		}
	}
}
