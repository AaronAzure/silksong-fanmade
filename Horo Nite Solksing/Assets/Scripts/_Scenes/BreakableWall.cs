using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Breakable
{
	protected override void CallChildOnStart() 
	{
		gm = GameManager.Instance;
		if (gm != null && gm.CheckDestroyedList(gameObject.name))
			Destroy(gameObject);
		if (dmgFx != null)
			dmgFx.transform.parent = null;
		if (destroyedObj != null)
			destroyedObj.transform.parent = null;
	}
}
