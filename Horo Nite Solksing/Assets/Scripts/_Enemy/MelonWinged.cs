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
	[SerializeField] float distToPlayer=2.5f;
	[SerializeField] float slowChaseSpeed=1.5f;
	private Vector2 destPos;

	protected override void CallChildOnStart()
	{
		// targetDest = target.aboveTarget;
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
				RaycastHit2D targetInfo = Physics2D.Raycast(
					target.self.position, 
					Vector2.up,  
					distToPlayer,
					whatIsGround
				);
				destPos = (targetInfo.collider != null) ? 
					targetInfo.point + new Vector2(0, -0.5f) : 
					Vector2.up * distToPlayer + (Vector2) target.self.position;

				Vector2 dir = (destPos - (Vector2) transform.position).normalized;
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
			else if (DistanceToPlayer() > slowChaseSpeed)
			{
				FacePlayer();
				rb.velocity = new Vector2(PlayerIsToTheRight() ? slowChaseSpeed : -slowChaseSpeed, 0);
			}

		}
	}

	public void THROW_MELONS()
	{
		if (melonObj != null)
		{
			FacePlayer();
			float forceOffset = (model.localScale.x > 0) ? 0.5f : -0.5f;
			if (DistanceToPlayer() < 1)
				forceOffset = 0;
			if (GameManager.Instance.hardMode)
			{
				var obj3 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
				obj3.rb.AddForce( new Vector2(horzForce / 2 + forceOffset, throwForce), ForceMode2D.Impulse);
				var obj4 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
				obj4.rb.AddForce( new Vector2(-horzForce / 2 + forceOffset, throwForce), ForceMode2D.Impulse);
			}
			var obj = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
			obj.rb.AddForce( new Vector2(0 + forceOffset, throwForce), ForceMode2D.Impulse);
			var obj1 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
			obj1.rb.AddForce( new Vector2(horzForce + forceOffset, throwForce), ForceMode2D.Impulse);
			var obj2 = Instantiate(melonObj, spawnPos.position, Quaternion.identity);
			obj2.rb.AddForce( new Vector2(-horzForce + forceOffset, throwForce), ForceMode2D.Impulse);
		}
	}


	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			currentAction = CurrentAction.none;
			sighted = true;
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
