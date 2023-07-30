using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Breakable
{
	protected override void CallChildOnStart() 
	{
		gm = GameManager.Instance;
		if (gm != null) 
		{
			// GetActiveScene().name + name
			if (exactName == "" && gm.CheckDestroyedList(name))
			{
				foreach (Breakable breakable in breakables)
					if (breakable != null)
						breakable.EnableCanBeHit();
				Destroy(gameObject);
			}
			else if (exactName != "" && gm.CheckDestroyedList(exactName, true))
			{
				foreach (Breakable breakable in breakables)
					if (breakable != null)
						breakable.EnableCanBeHit();
				Destroy(gameObject);
			}
		}
		if (dmgFx != null)
			dmgFx.transform.parent = null;
		if (destroyedObj != null)
			destroyedObj.transform.parent = null;
	}
}
