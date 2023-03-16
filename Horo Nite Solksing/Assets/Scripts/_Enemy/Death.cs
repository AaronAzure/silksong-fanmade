using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : Enemy
{
	[Space] [Header("Death")]
	[SerializeField] bool inAttackAnim;
	[SerializeField] bool tripleStrike;
	[SerializeField] float tripleStrikeForce=20;
	
	[Space] [SerializeField] FlameTrailProjectile flameAtk;
	[SerializeField] Transform flameAtkPos;
	private bool jumpedAgain=true;

	[Space] [SerializeField] EnemySickle sickleAtk;
	[Space] [SerializeField] Transform sickleAtkPos;


    public void ATTACK_PATTERN()
	{
		FacePlayer();
		if (!cannotAtk)
		{
			jumpedAgain = true;
			float distToTarget = Vector2.Distance(target.transform.position, self.position);
			Debug.Log(distToTarget);
			anim.SetBool("jumped", false);
			anim.SetTrigger("attack");
			int rng = (distToTarget < 6f) ? Random.Range(0,4) : Random.Range(0,3);
			if (rng == 0)
				anim.SetBool("jumped", true);
			else if (rng == 1)
				anim.SetBool("sickled", true);
			anim.SetFloat("atkPattern", rng);
		}
	}
	protected override void CallChildOnEarlyUpdate()
	{
		CheckIsGrounded();
		if (!isGrounded)
			anim.SetFloat("jumpVelocity", rb.velocity.y);
		if (!beenHurt)
		{
			if (tripleStrike)
			{
				rb.velocity = new Vector2(model.localScale.x * tripleStrikeForce, rb.velocity.y);
			}
			else if (!inAttackAnim)
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	public override void CallChildOnRoomEnter()
	{
		cannotAtk = false;
		ATTACK_PATTERN();
	}

	public void FACE_PLAYER()
	{
		FacePlayer();
	}

	public void JUMP_AT_PLAYER()
	{
		if (target == null) 
			return;
		rb.AddForce(
			new Vector2(
				(target.transform.position.x - self.position.x) * 2f, 
				jumpForce
			), 
			ForceMode2D.Impulse
		);
	}

	public void JUMP_AGAIN()
	{
		if (jumpedAgain && hp < maxHp / 2)
		{
			jumpedAgain = false;
			anim.SetBool("jumped", true);
			anim.SetFloat("atkPattern", 1);
		}
	}

	public void FLAME_ATTACK()
	{
		var obj = Instantiate(flameAtk, flameAtkPos.position, Quaternion.identity);
		obj.toRight = (model.localScale.x > 0) ? true : false;
		obj.transform.localScale = model.localScale;
	}

	public void THROW_SICKLE()
	{
		var obj = Instantiate(sickleAtk, sickleAtkPos.position, Quaternion.identity);
		obj.LaunchInDirection(
			(target.transform.position + new Vector3(0,0.5f) - sickleAtkPos.position).normalized
		);
		obj.returnPos = sickleAtkPos;
		obj.death = this;
	}

	public void RetrieveSickle()
	{
		anim.SetBool("sickled", false);
		anim.SetTrigger("retrieveSickle");
	}
}
