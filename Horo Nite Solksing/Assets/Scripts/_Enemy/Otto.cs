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
			var obj = Instantiate(bellObj, spawnPos.position, Quaternion.identity);
			obj.rb.AddForce( new Vector2(
				(target.self.position.x - obj.transform.position.x) * 1.75f,
				throwForce), 
				ForceMode2D.Impulse
			);
		}
	}
}
