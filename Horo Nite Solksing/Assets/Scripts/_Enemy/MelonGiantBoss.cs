using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonGiantBoss : Enemy
{
	[SerializeField] Transform leftArmPos;
	[SerializeField] Transform rightArmPos;

	[Space] [SerializeField] bool downstrikeChaseL;
	[SerializeField] bool downstrikeChaseR;
	[SerializeField] bool sweepChase;
	[SerializeField] EnemyShockWave shockWave;
	[SerializeField] EnemyShockWave shockWaveMini;
	[SerializeField] Transform shockWaveRightPos;
	[SerializeField] Transform shockWaveRightPos1;
	[SerializeField] Transform shockWaveLeftPos;
	[SerializeField] Transform shockWaveLeftPos1;
	private GameManager gm;
	private bool chased;

	
	[Space] [SerializeField] Transform rubbleMasterT;
	[SerializeField] Transform[] rubblePos;
	[SerializeField] EnemyProjectile rubbleObj;
	[SerializeField] float rubbleFallVel=2;

	[Space] [SerializeField] int setAtk=-1;


	protected override void CallChildOnStart()
	{
		gm = GameManager.Instance;
		if (rubbleMasterT != null)
			rubbleMasterT.parent = null;
	}

	protected override void CallChildOnEarlyUpdate()
	{
		if (sweepChase)
		{
			chased = true;
			int playerDir = (target.self.position.x - 
				(mainPos != null ? mainPos.position.x : self.position.x) 
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
		else if (downstrikeChaseL || downstrikeChaseR)
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
		switch (setAtk)
		{
			case 0: 
				anim.SetTrigger("downstrike L");
				return;
			case 1: 
				anim.SetTrigger("downstrike R");
				return;
			case 2: 
				anim.SetTrigger("rubble");
				return;
			case 3: 
				anim.SetTrigger("sweep R");
				return;
			case 4: 
				anim.SetTrigger("sweep L");
				return;
			default: 
				
				break;
		}

		if (Random.Range(0,3) == 0)
		{
			anim.SetTrigger("rubble");
			return;
		}
		else if (Random.Range(0,2) == 0)
		{
			int rng = Random.Range(0,2);
			anim.SetTrigger(rng == 0 ? "sweep L" : "sweep R");
			if (atPhase2)
				anim.SetTrigger(rng != 0 ? "sweep L" : "sweep R");
			if (atPhase3)
				anim.SetTrigger(rng == 0 ? "sweep L" : "sweep R");
			return;
		}
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

	IEnumerator _RUBBLE_CO()
	{
		CinemachineShake.Instance.ShakeCam(10f, 2.5f, 1.5f);
		List<int> temp = new List<int>();
		for (int i=0 ; i<rubblePos.Length ; i++)
			temp.Add(i);
		
		for (int i=0 ; i<rubblePos.Length ; i++)
		{
			yield return new WaitForSeconds( Random.Range(0.1f, 0.25f) );
			int rng = temp[ Random.Range(0, temp.Count) ];
			var obj = Instantiate(rubbleObj, rubblePos[rng].position, Quaternion.identity);
			obj.rb.velocity = Vector2.down * rubbleFallVel;
			temp.Remove(rng);
		}
	}
}
