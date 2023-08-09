using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonCircus : Enemy
{
	private float momentumSpeed;
	[SerializeField] bool dontFallOff;
	[SerializeField] Transform behindDetect;

	[Space] [SerializeField] GameObject melonBallObj;
	[SerializeField] Animator ballAnim;
	[SerializeField] Collider2D ballCol;
	[SerializeField] Rigidbody2D ballRb;
	private float ballSpeed;
	[SerializeField] bool inAttackAnim;
	private bool chased=true;


	[Space] [SerializeField] Rigidbody2D mainBody;



	protected override void CallChildOnStart()
	{
		if (melonBallObj != null)
		{
			melonBallObj.transform.parent = null;
			transform.parent = melonBallObj.transform;
		}
	}

	public override void CallChildOnIsSpecial()
	{
		dontFallOff = true;
	}

	protected bool CheckCliffBehind()
	{
		if (behindDetect == null)
			return false;
		RaycastHit2D groundInfo = Physics2D.Linecast(
			behindDetect.position, 
			behindDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		return (groundInfo.collider != null);
	}


	protected override void CallChildOnFixedUpdate()
	{
		if (ballAnim != null)
		{
			if (!CheckWall())
				ballSpeed = Mathf.Abs(rb.velocity.x);
			ballAnim.SetFloat("moveSpeed", ballSpeed);
		}
	}

	protected override void CallChildOnInAreaSwap()
	{
		inArea.SwapParent();
		if (!died)
			melonBallObj.SetActive(false);
	}

	protected override bool CallChildOnIsPlayerInSight() 
	{ 
		if (target == null || (!inRange && !alwaysInRange) || CheckWall() || (dontFallOff && !CheckCliff())) return false;
		
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

    protected override void IdleAction()
	{
		WalkAround();
		if (chased && anim != null)
		{
			chased = false;
			anim.SetBool("isChasing", false);
			anim.SetFloat("moveSpeed", 1);
		}
	}

	protected override void AttackingAction()
	{
		if (inAttackAnim)
		{
			if (isGrounded)
				ChasePlayer();
			// cannot change direction whilst falling
			else
				RetainMomentum();
		}
		else if (!receivingKb)
		{
			if (!chased && anim != null)
			{
				chased = true;
				anim.SetBool("isChasing", true);
				anim.SetFloat("moveSpeed", chaseSpeed * 2);
			}
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	protected override void CallChildOnInSight()
	{
		currentAction = CurrentAction.none;
	}
	protected override void CallChildOnLostSight()
	{
		ballSpeed = 0;
		ChooseNextAction();
	}
	protected override void CallChildOnIdleStop()
	{
		ballSpeed = 0;
	}

	protected override void CallChildOnHurt(int dmg, Vector2 forceDir)
	{
		momentumSpeed = 0;
	}

	protected override void CallChildOnDeath()
	{
		if (mainBody != null)
			mainBody.bodyType = RigidbodyType2D.Dynamic;
		if (ballRb != null)
			ballRb.bodyType = RigidbodyType2D.Static;
		if (ballCol != null)
			ballCol.enabled = false;
	}

	protected override void ChasePlayer()
	{
		int playerDir = (target.self.position.x - self.position.x > 0) ? 1 : -1;
		FacePlayer( playerDir );
		if (!receivingKb)
		{
			if (dontFallOff && (!CheckCliff() || (!CheckCliffBehind() && Mathf.Abs(rb.velocity.x) > 1)))
			{
				rb.velocity = new Vector2(rb.velocity.x / 2, rb.velocity.y);
			}
			else
			{
				rb.AddForce(new Vector2(chaseSpeed * playerDir * 5, 0), ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), 
					rb.velocity.y 
				);
				momentumSpeed = rb.velocity.x;
			}

		}
		
		if (anim != null) 
		{
			anim.SetBool("isMoving", true);
		}
	}

	protected void RetainMomentum()
	{
		if (!receivingKb)
		{
			rb.velocity = new Vector2(
				momentumSpeed, 
				rb.velocity.y 
			);
		}
	}
}
