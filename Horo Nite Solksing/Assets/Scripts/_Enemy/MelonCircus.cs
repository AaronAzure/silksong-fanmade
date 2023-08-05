using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonCircus : Enemy
{
	private float momentumSpeed;
	[SerializeField] GameObject melonBallObj;
	[SerializeField] Animator ballAnim;
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


	protected override void CallChildOnFixedUpdate()
	{
		if (ballAnim != null)
		{
			ballAnim.SetFloat("moveSpeed", Mathf.Abs(rb.velocity.x));
		}
	}

	protected override void CallChildOnInAreaSwap()
	{
		inArea.SwapParent();
		melonBallObj.SetActive(false);
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
		else
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

	protected override void CallChildOnHurt(int dmg, Vector2 forceDir)
	{
		momentumSpeed = 0;
	}

	protected override void CallChildOnDeath()
	{
		if (melonBallObj != null)
		{
			melonBallObj.SetActive(false);
			if (mainBody != null)
				mainBody.bodyType = RigidbodyType2D.Dynamic;
		}
	}

	protected override void ChasePlayer()
	{
		int playerDir = (target.self.position.x - self.position.x > 0) ? 1 : -1;
		FacePlayer( playerDir );
		Debug.Log("chasing");
		if (!receivingKb)
		{
			rb.AddForce(new Vector2(chaseSpeed * playerDir * 5, 0), ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), 
				rb.velocity.y 
			);
			momentumSpeed = rb.velocity.x;
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
