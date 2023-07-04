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
	[SerializeField] float horzFactor=1.5f;
	[SerializeField] int horzOffset;
	[SerializeField] float upOffset=5f;
	[SerializeField] float capHorzForce=10;
	[SerializeField] float capVertForce=10;
	[SerializeField] float fallForce=5;
	[SerializeField] bool isFallingAnim;
	private GameManager gm;

	// [Space] [SerializeField] bool keepFacingPlayer;
	protected override void CallChildOnStart()
	{
		gm = GameManager.Instance;
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
				anim.SetBool("nextAttackIsJump", Random.Range(0,3) != 0);
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
		CinemachineShake.Instance.ShakeCam(10f, 0.5f, 2);
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
		CinemachineShake.Instance.ShakeCam(15f, 0.5f, 2);
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
