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
	[SerializeField] float lungeMultiplier=1f;
	[SerializeField] bool chasingAnim;
	[SerializeField] bool lungeAnim;
	[SerializeField] bool inAtkAnim;
	[SerializeField] bool dontFallOff;


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
		anim.SetFloat(
			"shieldDir", 
			((target.transform.position.y - self.transform.position.y) > playerAboveVal) ? 1 : 0
		);
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
		if (!usedShield && isGrounded)
		{
			UseShield(true);
		}

		if (!inAtkAnim)
		{
			if (isShielding)
			{
				anim.SetFloat(
					"shieldDir", 
					((target.transform.position.y - self.transform.position.y) > playerAboveVal) ? 1 : 0
				);
			}
			if (isSuperClose && !toolSuperClose && closeCounter < closeLimit)
			{
				closeCounter += Time.fixedDeltaTime;
				if (closeCounter >= closeLimit)
				{
					closeCounter = 0;
					anim.SetTrigger("attack");
					anim.SetFloat("attackSpeed", 1);
				}
			}
			if (!isSuperClose && !toolSuperClose && closeCounter > 0)
			{
				closeCounter -= (Time.fixedDeltaTime/2);
			}
			
			// chasing and not shield
			if (!isSuperClose && !toolSuperClose)
			{
				if (!isShielding && chasingAnim && isGrounded && !receivingKb)
					ChasePlayer();
				if (usedShield)
				{
					// anim.SetFloat("moveSpeed", chaseSpeed);
					UseShield(false);
				}
			}
			// shielding and not chasing
			else if (!chasingAnim)
			{
				rb.velocity = new Vector2(0, rb.velocity.y);
				if (!usedShield && !isGrounded)
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
				rb.velocity = new Vector2(atkMomentum * model.localScale.x * lungeMultiplier, rb.velocity.y);
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	protected override void ChasePlayer()
	{
		int playerDir = (target.self.position.x - self.position.x > 0) ? 1 : -1;
		FacePlayer( playerDir );
		if (!receivingKb)
		{
			if (!dontFallOff || CheckCliff())
			{
				rb.AddForce(new Vector2(moveSpeed * playerDir * 5, 0), ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), 
					rb.velocity.y 
				);
			}
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
		
		if (anim != null) 
		{
			if (hasMoveSpeedAnim && !isFlying)
				anim.SetFloat("moveSpeed", Mathf.Abs(rb.velocity.x));
			if (hasIsMovingAnim)
				anim.SetBool("isMoving", true);
		}
	}

	protected override bool CallChildOnIsPlayerInSight() 
	{ 
		if (target == null || (!inRange && !alwaysInRange) || CheckWall()) return false;
		
		RaycastHit2D playerInfo = Physics2D.Linecast(
			(eyes != null) ? eyes.position : self.position, 
			target.self.position, 
			finalMask
		);
		bool canSeePlayer = (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"));
		if (canSeePlayer)
		{
			CallChildOnInSight();
			attackingPlayer = true;
			searchCounter = 0;
		} 
		return canSeePlayer;
	}

	protected override void CallChildOnShielded()
	{
		if (isSuperClose)
		{
			closeCounter = 0;
			anim.SetTrigger("blocked");
			anim.SetFloat("attackSpeed", 1.5f);
		}
	}

	private bool sighted;
	
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			rb.velocity = new Vector2(0, rb.velocity.y);
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
			idleCounter = 0;
			anim.SetBool("isChasing", false);
		}
	}

	public void _RESET_ATTACK_SPEED()
	{
		anim.SetFloat("attackSpeed", 1f);
	}
}
