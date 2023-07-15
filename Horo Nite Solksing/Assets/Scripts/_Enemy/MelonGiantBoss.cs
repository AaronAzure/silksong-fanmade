using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonGiantBoss : Enemy
{
	[SerializeField] Transform leftArmPos;
	[SerializeField] Transform rightArmPos;

	[Space] [SerializeField] bool downstrikeChaseL;
	[SerializeField] bool downstrikeChaseR;
	[SerializeField] EnemyShockWave shockWave;
	[SerializeField] EnemyShockWave shockWaveMini;
	[SerializeField] Transform shockWaveRightPos;
	[SerializeField] Transform shockWaveRightPos1;
	[SerializeField] Transform shockWaveLeftPos;
	[SerializeField] Transform shockWaveLeftPos1;
	private GameManager gm;
	private bool chased;


	protected override void CallChildOnStart()
	{
		gm = GameManager.Instance;
	}

	protected override void CallChildOnEarlyUpdate()
	{
		if (downstrikeChaseL || downstrikeChaseR)
		{
			chased = true;
			int playerDir = (target.self.position.x - 
				(downstrikeChaseL ? leftArmPos.position.x : rightArmPos.position.x) 
				> 0) ? 1 : -1;
			Debug.Log("chasing");
			if (!receivingKb)
			{
				rb.AddForce(new Vector2(chaseSpeed * playerDir * 5, 0), ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), 
					rb.velocity.y 
				);
			}
		}
		else if (chased)
		{
			chased = false;
			rb.velocity = Vector2.zero;
		}
	}

	public void _CHOOSE_ATTACK()
	{
		float distFromLeftHand = Vector2.Distance(target.self.position, leftArmPos.position);
		float distFromRightHand = Vector2.Distance(target.self.position, rightArmPos.position);
		if (distFromLeftHand < distFromRightHand)
			anim.SetTrigger("downstrike L");
		else
			anim.SetTrigger("downstrike R");
	}

	
    public void _DOWNSTRIKE_R()
	{
		rb.velocity = Vector2.zero;
		CinemachineShake.Instance.ShakeCam(15f, 0.75f, 1.5f);
		if (gm != null && gm.easyMode)
		{
			if (shockWaveMini != null)
			{
				var obj = Instantiate(shockWaveMini, shockWaveRightPos.position, Quaternion.identity);
				obj.Flip();
			}
			if (shockWaveMini != null)
			{
				var obj = Instantiate(shockWaveMini, shockWaveRightPos1.position, Quaternion.identity);
			}
		}
		else
		{
			if (shockWave != null)
			{
				var obj = Instantiate(shockWave, shockWaveRightPos.position, Quaternion.identity);
				obj.Flip();
			}
			if (shockWave != null)
			{
				var obj = Instantiate(shockWave, shockWaveRightPos1.position, Quaternion.identity);
			}
		}
	}
    public void _DOWNSTRIKE_L()
	{
		rb.velocity = Vector2.zero;
		CinemachineShake.Instance.ShakeCam(15f, 0.75f, 1.5f);
		if (gm != null && gm.easyMode)
		{
			if (shockWaveMini != null)
			{
				var obj = Instantiate(shockWaveMini, shockWaveLeftPos.position, Quaternion.identity);
			}
			if (shockWaveMini != null)
			{
				var obj = Instantiate(shockWaveMini, shockWaveLeftPos1.position, Quaternion.identity);
				obj.Flip();
			}
		}
		else
		{
			if (shockWave != null)
			{
				var obj = Instantiate(shockWave, shockWaveLeftPos.position, Quaternion.identity);
			}
			if (shockWave != null)
			{
				var obj = Instantiate(shockWave, shockWaveLeftPos1.position, Quaternion.identity);
				obj.Flip();
			}
		}
	}
}
