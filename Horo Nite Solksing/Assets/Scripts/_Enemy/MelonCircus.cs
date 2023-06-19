using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonCircus : Enemy
{
	private float momentumSpeed;
	[SerializeField] GameObject melonBallObj;
	[SerializeField] Animator ballAnim;
	private bool chased=true;



	private void OnEnable()
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
			anim.SetFloat("moveSpeed", 1);
		}
	}

	protected override void AttackingAction()
	{
		if (!chased && anim != null)
		{
			chased = true;
			anim.SetFloat("moveSpeed", chaseSpeed * 2);
		}
		if (isGrounded)
			ChasePlayer();
		// cannot change direction whilst falling
		else
			RetainMomentum();
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
