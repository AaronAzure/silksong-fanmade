using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpawnArea : MonoBehaviour
{
	private bool done;
	public Enemy[] enemies;
	[SerializeField] bool isSpecial;
	[SerializeField] bool alwaysInRange;
	[SerializeField] bool neverLoseSight;


    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!done && other.CompareTag("Player"))
		{
			done = true;
			// for (int i=0 ; i<enemies.Length ; i++)
			foreach (Enemy e in enemies)
			{
				e.gameObject.SetActive(true);
				// var e = Instantiate(enemies[i], pos[Mathf.Min(pos.Length-1, i)].position, Quaternion.identity, transform);
				// enemies[i].transform.position = pos[Mathf.Clamp(0, pos.Length-1, i)].position;
				
				if (isSpecial)
					e.CallChildOnIsSpecial();
				if (alwaysInRange && !e.idleActionOnly)
					e.alwaysInRange = true;
				if (neverLoseSight && !e.idleActionOnly)
					e.neverLoseSight = true;
				e.SpawnIn(1.5f);
			}
		}
	}
}
