using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonEntombed : Enemy
{
	[Space] [SerializeField] bool chargingA;
	[SerializeField] bool stopMoving;
	[SerializeField] float atkDir;


	protected override void IdleAction()
	{
		if (!stopMoving)
		{
			if (!chargingA)
				WalkAround();
			else if (!receivingKb)
			{
				if (CheckSurrounding())
				{
					_LOSE_SIGHT();
				}
				else
					rb.velocity = new Vector2(atkDir * chaseSpeed, rb.velocity.y);
			}
		}
		else if (!receivingKb)
		{
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	protected override void AttackingAction()
	{
		
	}

	protected override void CallChildOnHurt(int dmg, Vector2 forceDir)
	{
		// player is to the right
		if (target.self.position.x - transform.position.x > 0)
			currentAction = CurrentAction.right;
		// player is to the left
		else
			currentAction = CurrentAction.left;

		ChooseNextAction();
	}

	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted && !CheckSurrounding())
		{
			currentAction = CurrentAction.none;
			sighted = true;
			anim.SetTrigger("alert");
			atkDir = (model.localScale.x > 0) ? 1 : -1;
		} 
	}

	public void _LOSE_SIGHT()
	{
		if (sighted)
		{
			searchCounter = 0;
			attackingPlayer = false;
			idleCounter = 0;
			sighted = false;
			rb.velocity = new Vector2(0, rb.velocity.y);
			anim.Play("melon_entomb_idle_anim", -1, 0);
			
			// ChooseNextAction();
			FacePlayer();
		}
	}
}
