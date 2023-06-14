using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonBilly : Enemy
{
    protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void AttackingAction()
	{
		WalkAround();
		
	}
}
