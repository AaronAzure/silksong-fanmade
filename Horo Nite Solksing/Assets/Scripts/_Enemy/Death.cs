using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : Enemy
{
	[Space] [Header("Death")]
	[SerializeField] bool inAttackAnim;
	[SerializeField] bool tripleStrike;
	[SerializeField] float tripleStrikeForce=20;
	// [SerializeField] float jumpForce=15;


    public void ATTACK_PATTERN()
	{
		FacePlayer();
		if (!cannotAtk)
		{
			Debug.Log(Vector2.Distance(target.transform.position, self.position));
			anim.SetBool("jumped", false);
			anim.SetTrigger("attack");
			int rng = Random.Range(0,3);
			if (rng == 1)
				anim.SetBool("jumped", true);
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
				rb.AddForce(new Vector2(model.localScale.x * tripleStrikeForce * 5, 0), ForceMode2D.Force);
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x, -tripleStrikeForce, tripleStrikeForce),
					rb.velocity.y
				);
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
				(target.transform.position.x - self.position.x) * 1.2f, 
				jumpForce
			), 
			ForceMode2D.Impulse
		);
	}
}
