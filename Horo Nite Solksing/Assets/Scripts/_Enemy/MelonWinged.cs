using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonWinged : Enemy
{
    private bool chase;
	[SerializeField] bool alertAnim;
	[SerializeField] bool attackingAnim;
	[SerializeField] float closeDistTimer;
	[SerializeField] float closeDistTotal=1;
	
	[Space] [SerializeField] Transform spawnPos;
	[SerializeField] float throwForce=8;
	[SerializeField] float horzForce=5;
	[SerializeField] EnemyProjectile melonObj;

	protected override void CallChildOnStart()
	{
		targetDest = target.aboveTarget;
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
		if (!receivingKb)
		{
			if (!attackingAnim && !alertAnim)
			{
				Vector2 dir = (targetDest.position - transform.position).normalized;
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

	public void THROW_MELONS()
	{
		if (melonObj != null)
		{
			FacePlayer();
			if (GameManager.Instance.hardMode)
			{
				var obj3 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
				obj3.rb.AddForce( new Vector2(horzForce / 2, throwForce), ForceMode2D.Impulse);
				var obj4 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
				obj4.rb.AddForce( new Vector2(-horzForce / 2, throwForce), ForceMode2D.Impulse);
			}
			var obj = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
			obj.rb.AddForce( new Vector2(0, throwForce), ForceMode2D.Impulse);
			var obj1 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
			obj1.rb.AddForce( new Vector2(horzForce, throwForce), ForceMode2D.Impulse);
			var obj2 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
			obj2.rb.AddForce( new Vector2(-horzForce, throwForce), ForceMode2D.Impulse);
		}
	}

	protected override void CallChildOnInSight()
	{
		currentAction = CurrentAction.none;
		anim.SetBool("isChasing", true);
	}
	protected override void CallChildOnLostSight()
	{
		anim.SetBool("isChasing", false);
	}
}
