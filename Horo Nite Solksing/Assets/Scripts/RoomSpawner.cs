using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
	public Room room;
    [Space] public Enemy[] enemies;

	
	public bool CheckIfHasMoreSpawns(int x)
	{
		return (x < enemies.Length && enemies[x] != null);
	}

	public void SpawnEnemy(int x)
	{
		if (x < enemies.Length && enemies[x] != null)
		{
			// Debug.Log($"{gameObject.name} spawning");
			var e = Instantiate(enemies[x], transform.position, Quaternion.identity, transform);
			e.room = this.room;
			e.CallChildOnIsSpecial();
			e.SpawnIn();
		}
		else
		{
			// Debug.Log($"{gameObject.name} nothing to spawn");
			room.Defeated(true);
		}
	}
}
