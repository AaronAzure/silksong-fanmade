using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuart : Enemy
{
	protected override void IdleAction()
	{
		// WalkAround();
	}

	protected override void AttackingAction()
	{
		// rb.AddForce(new Vector2(target.transform.position.x - self.position.x, 20), ForceMode2D.Impulse);
	}
}
