using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBackground : Breakable
{
	[Space] [Header("background")] 
	[SerializeField] GameObject visualObj;
	[SerializeField] GameObject hitboxObj;
	[SerializeField] GameObject dmgVfxObj;

	protected override void CallChildOnStart() { }

	protected override void CallChildOnDamage(int dmg)
	{
		if (visualObj != null)
		{
			visualObj.SetActive(false);
		}
		if (hitboxObj != null)
		{
			hitboxObj.SetActive(false);
		}
		if (col != null)
		{
			col.enabled = false;
		}
		if (dmgVfxObj != null)
		{
			dmgVfxObj.SetActive(true);
		}
	}
}
