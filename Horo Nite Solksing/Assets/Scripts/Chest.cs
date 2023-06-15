using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Breakable
{
	[SerializeField] GameObject openStateObj;
	[SerializeField] GameObject closedStateObj;
	[SerializeField] Loot loot;


	protected override void CallChildOnDamage(int dmg)
	{
		if (openStateObj != null)
		{
			openStateObj.SetActive(true);
		}
		if (closedStateObj != null)
		{
			closedStateObj.SetActive(false);
		}
		if (loot != null)
		{
			loot.SpawnLoot();
		}
		Destroy(this);
	}
}
