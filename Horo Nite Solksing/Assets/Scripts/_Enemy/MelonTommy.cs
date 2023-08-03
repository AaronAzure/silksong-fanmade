using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonTommy : Enemy
{
	[SerializeField] bool hidingA;
	[SerializeField] bool backDashingA;
	[SerializeField] bool inAtkA;
	
	
	[SerializeField] float atkTimer=1.9f;
	[Space] [SerializeField] float atkTimerLimit=2f;

	[Space] [SerializeField] Transform shotPos;
	[SerializeField] float shotForce=5;
	[SerializeField] EnemyProjectile2 projectile;
	
	private bool justBackDashed;
	[SerializeField] float backDashSpeed=3f;
	[field: SerializeField] protected Transform groundBehindDetect;


	protected override void CallChildOnGizmosSelected()
	{
		if (target != null)
			Gizmos.DrawLine((eyes != null) ? eyes.position : self.position, target.self.position);
	}

	protected override void CallChildOnLoseHp(int dmg)
	{
		dmg = backDashingA ? (int)(dmg/2) : dmg;
		if (!cannotTakeDmg)
			hp -= dmg;
		if (GameManager.Instance.showDmg && dmgPopup != null)
		{
			// var obj = Instantiate(dmgPopup, transform.position + Vector3.up, Quaternion.identity);
			if (dmgPopup.gameObject.activeSelf)
			{
				dmgPopup.txt.text = $"{dmg + int.Parse(dmgPopup.txt.text)}";
				dmgPopup.anim.SetTrigger("reset");
			}
			else
				dmgPopup.txt.text = $"{dmg}";
			dmgPopup.gameObject.SetActive(true);
		}
	}

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
			groundBehindDetect.position + new Vector3(model.localScale.x, 0), 
			groundBehindDetect.position, 
			whatIsGround
		);
		return (wallInfo.collider != null);
	}


	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			sighted = true;
			// atkTimer = 0;
			rb.velocity = new Vector2(0, rb.velocity.y);
			currentAction = CurrentAction.none;
			anim.SetTrigger("alert");
		}
	}

	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			sighted = false;
			idleCounter = 0;
		}
	}

	protected override void IdleAction()
	{
		// WalkAround();
	}

	protected override void AttackingAction()
	{
		if (!hidingA && !backDashingA && !stillAttacking && !inAtkA)
		{
			FacePlayer();
			
			if (inRange)
				atkTimer += Time.fixedDeltaTime;
			if (isSuperClose && atkTimer < (atkTimerLimit / 2) && CheckBehindForGround() && !CheckBehindForWall())
			{
				atkTimer = 1.25f;
				anim.SetTrigger("hide");
			}
			else if (atkTimer > atkTimerLimit)
			{
				atkTimer = 0;
				// anim.SetBool("isMoving", false);
				// anim.SetBool("isClose", isSuperClose);
				anim.SetTrigger("attack");
			}
		}
		if (!receivingKb)
		{
			if (backDashingA)
			{
				justBackDashed = true;
				if (CheckBehindForGround() && !CheckBehindForWall())
					rb.velocity = new Vector2(model.localScale.x * -backDashSpeed, rb.velocity.y);
				else
				{
					rb.velocity = new Vector2(0, rb.velocity.y);
					anim.SetTrigger("unhide");
				}
			}
			// first frame of animator backDashingA being unchecked
			else if (justBackDashed && !backDashingA)
			{
				justBackDashed = false;
				rb.velocity = new Vector2(0, rb.velocity.y);
			}
			else 
			{
				rb.velocity = new Vector2(0, rb.velocity.y);
			}
		}
	}

	public void _SHOOT()
	{
		var atk = Instantiate(projectile, shotPos.position, Quaternion.identity);
		Vector2 dir = new Vector2(model.localScale.x, 0);
		atk.rb.AddForce(dir.normalized * shotForce, ForceMode2D.Impulse);
	}
}
