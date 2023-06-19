using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonCircus : Enemy
{
	private float momentumSpeed;
	[SerializeField] GameObject melonBallObj;


	protected override void CallChildOnStart()
	{
		if (melonBallObj != null)
		{
			melonBallObj.transform.parent = null;
			transform.parent = melonBallObj.transform;
		}
	}

    protected override void IdleAction()
	{
		WalkAround();
	}


	protected override void AttackingAction()
	{
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
