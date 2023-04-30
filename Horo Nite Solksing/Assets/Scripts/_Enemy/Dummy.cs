using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Dummy : Enemy
{
	protected override void CallChildOnHurt(int dmg, Vector2 forceDir)
	{
		// Assert.IsNotNull(other, "opponent is missing");
		Assert.IsNotNull(anim, "anim is missing");
		anim.SetTrigger((forceDir.x) > 0 ? "hurtFromLeft" : "hurtFromRight");
	}

}
