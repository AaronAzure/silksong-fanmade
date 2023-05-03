using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : Enemy
{
	[SerializeField] Enemy otto;
	[SerializeField] Enemy edulitoh;
	[SerializeField] Transform spawnPos;
	[SerializeField] bool canSpawnEdulitoh;


	[Space] [SerializeField] int nSpawn;


	public override void CallChildOnIsSpecial()
	{
		canSpawnEdulitoh = true;
	}
	protected override void IdleAction() { }

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
		// Debug.Log($"{gameObject.name} = {nSpawn} ");
		if (nSpawn % 4 == 0)
		{
			int rng = (canSpawnEdulitoh ? Random.Range(0,4) : 1);
			var obj = Instantiate(rng == 0 ? edulitoh : otto, spawnPos.position, Quaternion.identity);
			obj.SpawnIn();
			if (room != null)
			{
				obj.room = this.room;
				room.nExtras++;
			}
		}
		nSpawn++;
	}

	public void CRASH_GAME()
	{
		Time.timeScale = 0;
	}
}
