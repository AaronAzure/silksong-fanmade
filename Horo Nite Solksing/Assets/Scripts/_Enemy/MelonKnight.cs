using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonKnight : Enemy
{
	private bool usedShield;
	[SerializeField] bool usingShield;
	[SerializeField] float playerAboveVal=2.5f;

	[Space] [SerializeField] float closeCounter;
	[SerializeField] float closeLimit=1f;
	[SerializeField] float atkMomentum=3f;
	[SerializeField] bool lungeAnim;
	[SerializeField] bool inAtkAnim;


	protected bool CheckForGround()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundDetect.position, 
			groundDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		return (groundInfo.collider != null);
	}

	private void UseShield(bool equip)
	{
		usedShield = equip;
		anim.SetBool("usingShield", equip);
	}

	protected override void IdleAction()
	{
		if (usedShield)
		{
			UseShield(false);
			anim.SetFloat("moveSpeed", 1);
		}
		WalkAround();
	}
	
	protected override void AttackingAction()
	{
		if (!usedShield)
		{
			UseShield(true);
		}
		if (!inAtkAnim)
		{
			if (inParryState)
			{
				anim.SetFloat(
					"shieldDir", 
					((target.transform.position.y - self.transform.position.y) > playerAboveVal) ? 1 : 0
				);
			}
			if (isSuperClose && closeCounter < closeLimit)
			{
				closeCounter += Time.fixedDeltaTime;
				if (closeCounter >= closeLimit)
				{
					closeCounter = 0;
					anim.SetTrigger("attack");
				}
			}
			
			// chasing and not shield
			if (!isSuperClose)
			{
				ChasePlayer();
				if (usedShield)
				{
					anim.SetFloat("moveSpeed", chaseSpeed);
					UseShield(false);
				}
			}
			// shielding and not chasing
			else if (!receivingKb)
			{
				rb.velocity = new Vector2(0, rb.velocity.y);
				if (!usedShield)
				{
					anim.SetFloat("moveSpeed", 1);
					UseShield(true);
				}
			}
			FacePlayer();
		}
		else if (!receivingKb)
		{
			if (lungeAnim && CheckForGround())
				rb.velocity = new Vector2(atkMomentum * model.localScale.x, rb.velocity.y);
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	private bool sighted;
	
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			sighted = true;
			currentAction = CurrentAction.none;
			anim.SetBool("isChasing", true);
		}
	}
	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			sighted = false;
			idleCounter = 0;
			anim.SetBool("isChasing", false);
		}
	}
}
