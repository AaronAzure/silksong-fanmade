using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
	public Room room;
	[SerializeField] GameObject rustlingObj;

    [Space] [SerializeField] bool isSpecial;
    [SerializeField] bool alwaysInRange=true;
    [SerializeField] bool neverLoseSight=true;
	public Enemy[] enemies;


	
	public bool CheckIfHasMoreSpawns(int x)
	{
		return (x < enemies.Length && enemies[x] != null);
	}

	public void SpawnEnemy(int x)
	{
		if (x < enemies.Length && enemies[x] != null)
		{
			if (enemies[x].isFlying)
				StartCoroutine( SpawnEnemyCo(enemies[x], -1, true) );
			else
				StartCoroutine( SpawnEnemyCo(enemies[x], 1.5f) );
		}
		else
		{
			// Debug.Log($"{gameObject.name} nothing to spawn");
			room.Defeated(true);
		}
	}

	IEnumerator SpawnEnemyCo(Enemy enemy, float waitTime, bool skipRustling=false)
	{
		if (!skipRustling && rustlingObj != null)
		{
			rustlingObj.SetActive(false);
			rustlingObj.SetActive(true);
		}
		if (waitTime > 0)
		{
			yield return new WaitForSeconds(waitTime);
		}
		var e = Instantiate(enemy, transform.position, Quaternion.identity, transform);
		e.room = this.room;
		if (isSpecial)
			e.CallChildOnIsSpecial();
		if (alwaysInRange && !e.idleActionOnly)
			e.alwaysInRange = true;
		if (neverLoseSight && !e.idleActionOnly)
			e.neverLoseSight = true;
		e.SpawnIn();
	}
}
