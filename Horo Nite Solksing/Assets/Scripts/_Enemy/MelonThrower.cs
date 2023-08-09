using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonThrower : Enemy
{
	[SerializeField] bool inAtkAnim;
	[SerializeField] Transform projectilePos;
	[SerializeField] EnemyProjectile projectile;
	[SerializeField] float horzFactor=1.5f;
	[SerializeField] int horzOffset;
	[SerializeField] float upOffset=5f;
	[SerializeField] float capHorzForce=10;
	[SerializeField] float capVertForce=10;

	[Space] [SerializeField] bool isScouting;
	[SerializeField] bool isBackDashing;
	private bool justBackDashed;
	[SerializeField] float backDashSpeed=3f;
	[field: SerializeField] protected Transform groundBehindDetect;

	[Space] [SerializeField] bool keepFacingPlayer;


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
		if (isScouting)
		{
			AttackStrategy();
		}
		if (keepFacingPlayer)
		{
			FacePlayer();
		}

		if (isBackDashing && !receivingKb)
		{
			justBackDashed = true;
			if (CheckBehindForGround())
				rb.velocity = new Vector2(model.localScale.x * -backDashSpeed, rb.velocity.y);
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
		else if (!receivingKb && justBackDashed && !isBackDashing)
		{
			justBackDashed = false;
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			currentAction = CurrentAction.none;
			sighted = true;
			anim.SetBool("isAttacking", true);
			rb.velocity = new Vector2(0, rb.velocity.y);
		}

	}
	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			idleCounter = 0;
			sighted = false;
			anim.SetBool("isAttacking", false);
		}
	}

	public void AttackStrategy()
	{
		if (isSuperClose && CheckBehindForGround() && !CheckBehindForWall())
		{
			anim.SetTrigger("backDash");
		}
	}

	public void THROW_MELON()
	{
		if (projectile != null)
		{
			var obj = Instantiate(projectile, projectilePos.position, Quaternion.identity);
			obj.rb.velocity = new Vector2(
				Mathf.Clamp(
					horzFactor * (target.self.position.x - transform.position.x) + Random.Range(-horzOffset, horzOffset + 1), 
					-capHorzForce,
					capHorzForce
				),
				Mathf.Clamp(
					(target.self.position.y - transform.position.y) + upOffset,
					0,
					capVertForce
				)
			);
		}
	}


}
