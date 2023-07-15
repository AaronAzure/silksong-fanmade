using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoMelon : Enemy
{
	private bool inSightAnim;
	private float closeDistTimer;
	[SerializeField] float closeDistTotal=1f;
	[SerializeField] bool facingPlayerAnim;
	[SerializeField] bool inAttackAnim;
	[SerializeField] Transform shotPos;
	[SerializeField] float shotForce=5;
	[SerializeField] EnemyProjectile2 projectile;


	protected override void IdleAction()
	{
		if (inSightAnim)
		{
			inSightAnim = false;
			anim.SetBool("inSight", false);
		}
	}

	protected override void AttackingAction()
	{

		if (!inAttackAnim)
		{
			if (closeDistTimer > closeDistTotal)
			{
				if (!inSightAnim)
				{
					inSightAnim = true;
					anim.SetBool("inSight", true);
				}

				closeDistTimer = 0;
			}
			else if (isClose)
				closeDistTimer += Time.fixedDeltaTime;
			else if (closeDistTimer > 0)
				closeDistTimer -= (Time.fixedDeltaTime * 0.5f);
		}
		else if (facingPlayerAnim)
			FacePlayer();
	}

	public void SHOOT()
	{
		var atk = Instantiate(projectile, shotPos.position, Quaternion.identity);
		Vector2 dir = (target.transform.position + new Vector3(0,0.5f)) - shotPos.position;
		atk.rb.AddForce(dir.normalized * shotForce, ForceMode2D.Impulse);
		if (inSightAnim)
		{
			inSightAnim = false;
			anim.SetBool("inSight", false);
		}
	}

	public void ENABLE_HITBOX()
	{
		if (!died && col != null)
		{
			col.enabled = true;
		}
	}
	public void DISABLE_HITBOX()
	{
		if (col != null)
			col.enabled = false;
	}
}
