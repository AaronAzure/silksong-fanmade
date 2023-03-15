using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : Enemy
{
	[SerializeField] Enemy otto;
	[SerializeField] Transform spawnPos;
	private int x;


	protected override void IdleAction()
	{
		
	}

	protected override void AttackingAction()
	{
		FacePlayer();
	}

	protected override void CallChildOnFixedUpdate()
	{
		anim.SetBool("isAttacking", attackingPlayer);
	}

	public void SPAWN_ALLY()
	{
		if (x % 4 == 0)
		{
			var obj = Instantiate(otto, spawnPos.position, Quaternion.identity);
			obj.SpawnIn();
		}
		x++;
	}

	public void CRASH_GAME()
	{
		Time.timeScale = 0;
	}
}
