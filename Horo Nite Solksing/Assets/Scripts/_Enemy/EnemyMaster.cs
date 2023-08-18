using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
	public static EnemyMaster Instance;
    [SerializeField] List<Enemy> enemies;

	private void Awake() 
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
		enemies = new List<Enemy>();
	}

	private void FixedUpdate() 
	{
		foreach (Enemy e in enemies)
		{
			if (e != null && e.gameObject.activeInHierarchy)
			{
				e.EnemyAction();
			}
		}	
	}

	public void AddEnemy(Enemy newEnemy)
	{
		if (enemies == null)
			enemies = new List<Enemy>();
		
		enemies.Add(newEnemy);
	}

	public void RemoveEnemy(Enemy newEnemy)
	{
		if (enemies != null && enemies.Contains(newEnemy))
		{
			enemies.Remove(newEnemy);
		}
	}
}
