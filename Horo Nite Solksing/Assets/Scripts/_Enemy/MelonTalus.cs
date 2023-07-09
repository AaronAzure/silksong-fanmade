using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonTalus : Enemy
{
    [SerializeField] bool inAtkAnim;
	[SerializeField] EnemyShockWave shockWave;
	[SerializeField] EnemyShockWave shockWaveMini;
	[SerializeField] Transform shockWavePos;
	[SerializeField] Transform shockWaveBackPos;
	[SerializeField] float fallForce=5;
	[SerializeField] bool isJumpingAnim;
	[SerializeField] bool isFallingAnim;
	[SerializeField] float atkMomentum=10f;
	[SerializeField] bool lungeAnim;
	private GameManager gm;

	// [Space] [SerializeField] bool keepFacingPlayer;
	protected override void CallChildOnStart()
	{
		gm = GameManager.Instance;
	}

	protected bool CheckForGround()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundDetect.position, 
			groundDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		return (groundInfo.collider != null);
	}


    protected override void IdleAction()
	{
		// WalkAround();
	}


	protected override void AttackingAction()
	{
		if (isFallingAnim && isGrounded)
		{
			anim.SetTrigger("slam");
		}
		else if (!isJumpingAnim && !receivingKb)
		{
			if (lungeAnim && CheckForGround())
				rb.velocity = new Vector2(atkMomentum * model.localScale.x, rb.velocity.y);
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	protected override void CallChildOnDeath()
	{
		// rb.gravityScale = 1;
	}

	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			currentAction = CurrentAction.none;
			sighted = true;
			anim.SetBool("isAttacking", true);
			rb.velocity = Vector2.zero;
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

	public void NEXT_ATTACK()
	{
		if (sighted)
		{
			FacePlayer();
			if (isClose)
			{
				switch (Random.Range(0,5))
				{
					// shockwave
					case 0: 
						anim.SetBool("nextAttackIsJump", false);
						break;
					// jump attack
					case 1: 
						anim.SetBool("nextAttackIsJump", true);
						break;
					// jump attack
					case 2: 
						anim.SetBool("nextAttackIsJump", true);
						break;
					default: 
						anim.SetTrigger("closeCombat");
						break;
				}
			}
			else
			{
				anim.SetBool("nextAttackIsJump", false);
			}
			anim.SetTrigger("attack");
		}
	}

	public void FACE_PLAYER()
	{
		if (sighted)
			FacePlayer();
	}

	public void SHOCKWAVE()
	{
		CinemachineShake.Instance.ShakeCam(10f, 0.5f, 1.5f);
		if (gm != null && gm.easyMode)
		{
			if (shockWaveMini != null)
			{
				var obj = Instantiate(shockWaveMini, shockWavePos.position, Quaternion.identity);
				if (model.localScale.x < 0)
					obj.Flip();
			}
		}
		else
		{
			if (shockWave != null)
			{
				var obj = Instantiate(shockWave, shockWavePos.position, Quaternion.identity);
				if (model.localScale.x < 0)
					obj.Flip();
			}
		}
	}

	public void _SHAKECAM()
	{
		CinemachineShake.Instance.ShakeCam(7.5f, 0.5f, 0.75f);
	}

	public void _JUMP()
	{
		if (rb != null)
		{
			FacePlayer();
			rb.AddForce(new Vector2(DirectionToPlayer(), jumpForce), ForceMode2D.Impulse);
		}
	}

	public void FALL()
	{
		if (rb != null)
		{
			rb.velocity = new Vector2(rb.velocity.x, 0);
			rb.AddForce(Vector2.down * fallForce, ForceMode2D.Impulse);
		}
	}

	public void SLAM()
	{
		rb.velocity = Vector2.zero;
		CinemachineShake.Instance.ShakeCam(15f, 0.5f, 1.5f);
		if (gm != null && gm.easyMode)
		{
			if (shockWaveMini != null)
			{
				var obj = Instantiate(shockWaveMini, shockWavePos.position, Quaternion.identity);
				if (model.localScale.x < 0)
					obj.Flip();
			}
			if (shockWaveMini != null)
			{
				var obj = Instantiate(shockWaveMini, shockWaveBackPos.position, Quaternion.identity);
				if (model.localScale.x > 0)
					obj.Flip();
			}
		}
		else
		{
			if (shockWave != null)
			{
				var obj = Instantiate(shockWave, shockWavePos.position, Quaternion.identity);
				if (model.localScale.x < 0)
					obj.Flip();
			}
			if (shockWave != null)
			{
				var obj = Instantiate(shockWave, shockWaveBackPos.position, Quaternion.identity);
				if (model.localScale.x > 0)
					obj.Flip();
			}
		}
	}
}
