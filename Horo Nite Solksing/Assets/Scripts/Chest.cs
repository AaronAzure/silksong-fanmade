using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Breakable
{
	[SerializeField] GameObject openStateObj;
	[SerializeField] GameObject closedStateObj;
	[SerializeField] Loot loot;

	protected override void CallChildOnStart()
	{
		gm = GameManager.Instance;
		if (gm != null && gm.CheckDestroyedList(name))
		{
			if (openStateObj != null)
			{
				openStateObj.SetActive(true);
			}
			if (closedStateObj != null)
			{
				closedStateObj.SetActive(false);
			}
			Destroy(this);
		}
	}

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
		if (gm != null)
		{
			gm.RegisterDestroyedList(name);
		}
		Destroy(this);
	}
}
