using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonJumper : Enemy
{
	protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void AttackingAction()
	{
		if (!receivingKb)
		{
			if (!isSuperClose)
				ChasePlayer();
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			currentAction = CurrentAction.none;
			sighted = true;
			anim.SetTrigger("alert");
			anim.SetBool("isChasing", true);
		}

	}
	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			idleCounter = 0;
			sighted = false;
			anim.SetBool("isChasing", false);
		}
	}
}
