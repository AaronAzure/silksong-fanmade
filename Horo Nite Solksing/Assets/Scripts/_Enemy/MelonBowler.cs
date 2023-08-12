using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonBowler : Enemy
{
	[SerializeField] Transform projectilePos;
	[SerializeField] EnemyProjectile projectile;
	[SerializeField] float horzForce=1.5f;
	[SerializeField] float upForce=4f;

	private bool justBackDashed;
	[SerializeField] float backDashSpeed=3f;
	[field: SerializeField] protected Transform groundBehindDetect;

	[Space] [SerializeField] bool keepFacingPlayer;
    [SerializeField] bool inSuperCloseA;
    [SerializeField] bool inAtkA;
	[SerializeField] bool isBackDashingA;


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
		if (inSuperCloseA)
		{
			if (isSuperClose && CheckBehindForGround() && !CheckBehindForWall())
			{
				anim.SetTrigger("backDash");
			}
		}
		if (inAtkA)
		{
			if (keepFacingPlayer)
			{
				FacePlayer();
				anim.SetFloat("atkDir", model.localScale.x > 0 ? 1 : -1);
			}
			if (!receivingKb)
			{
				if (isBackDashingA)
				{
					justBackDashed = true;
					if (CheckBehindForGround())
						rb.velocity = new Vector2(model.localScale.x * -backDashSpeed, rb.velocity.y);
					else
						rb.velocity = new Vector2(0, rb.velocity.y);
				}
				else if (justBackDashed && !isBackDashingA)
				{
					justBackDashed = false;
					rb.velocity = new Vector2(0, rb.velocity.y);
				}
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

	public void _THROW_MELON()
	{
		if (projectile != null)
		{
			var obj = Instantiate(projectile, projectilePos.position, Quaternion.identity);
			obj.rb.velocity = new Vector2(horzForce * model.localScale.x, upForce);
			if (model.localScale.x < 0)
				obj.transform.localScale = new Vector3(
					-obj.transform.localScale.x, 
					obj.transform.localScale.y,
					obj.transform.localScale.z);
			obj.anim.SetFloat("moveSpeed", 6);
		}
	}


}
