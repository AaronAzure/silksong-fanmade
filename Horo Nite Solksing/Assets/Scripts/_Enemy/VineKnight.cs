using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineKnight : Enemy
{
	private GameManager gm;
	[SerializeField] Transform vineTrapPos;
	[SerializeField] GameObject vineTrapObj;
	[SerializeField] float trapOffset=1.5f;

	
	[SerializeField] bool inAlertAnim;
	[SerializeField] float atkTimer;
	[SerializeField] float atkTimerLimit=2f;

	[Space] [SerializeField] bool isBackDashing;
	private bool justBackDashed;
	[SerializeField] float backDashSpeed=3f;
	[field: SerializeField] protected Transform groundBehindDetect;

		
	protected override void CallChildOnStart()
	{
		gm = GameManager.Instance;
		if (gm != null && gm.easyMode)
			trapOffset = 2;
	}

	// return true if there is ground
	protected virtual bool CheckBehindForGround()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundBehindDetect.position, 
			groundBehindDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		return (groundInfo.collider != null);
	}
	// return true if there is wall behind
	protected virtual bool CheckBehindForWall()
	{
		RaycastHit2D wallInfo = Physics2D.Linecast(
			eyes.position, 
			groundBehindDetect.position, 
			whatIsGround
		);
		return (wallInfo.collider != null);
	}


	private bool sighted;
	protected override void CallChildOnInSight()
	{
		if (!sighted)
		{
			sighted = true;
			atkTimer = 0;
			rb.velocity = new Vector2(0, rb.velocity.y);
			currentAction = CurrentAction.none;
			anim.SetFloat("moveSpeed",2);
			anim.SetBool("isMoving", false);
			anim.SetTrigger("alert");
			anim.SetBool("isChasing", true);
		}
	}

	protected override void CallChildOnLostSight()
	{
		if (sighted)
		{
			anim.SetFloat("moveSpeed",1);
			sighted = false;
			idleCounter = 0;
			anim.SetBool("isChasing", false);
		}
	}

	protected override void IdleAction()
	{
		WalkAround();
	}

	protected override void AttackingAction()
	{
		if (!isBackDashing && !stillAttacking && !inAlertAnim)
		{
			if (!isSuperClose && !receivingKb)
			{
				if (isGrounded)
					ChasePlayer();
				else
					MoveInPrevDirection();
			}
			else if (!receivingKb)
			{
				rb.velocity = new Vector2(0, rb.velocity.y);
				anim.SetBool("isMoving", false);
			}

			FacePlayer();
			
			if (!inAlertAnim)
				atkTimer += Time.fixedDeltaTime;
			if (isSuperClose && atkTimer < 0.5f && CheckBehindForGround() && !CheckBehindForWall())
			{
				atkTimer = 0.5f;
				anim.SetTrigger("backDash");
			}
			else if (atkTimer > atkTimerLimit)
			{
				atkTimer = 0;
				anim.SetBool("isMoving", false);
				anim.SetBool("isClose", isSuperClose);
				anim.SetTrigger("attack");
			}
		}
		else if (isBackDashing && !receivingKb)
		{
			justBackDashed = true;
			if (CheckBehindForGround())
				rb.velocity = new Vector2(model.localScale.x * -backDashSpeed, rb.velocity.y);
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
		}
		else if (justBackDashed)
		{
			justBackDashed = false;
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
		else if (!receivingKb)
		{
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	private void OnDrawGizmosSelected() 
	{
		int n = 5;

		if (target != null)
		{
			for (int i=0 ; i<n ; i++)
			{
				Gizmos.color = Color.red;
				RaycastHit2D hitInfo = Physics2D.Raycast(
					target.self.position + (Vector3) GetTrapPosOffset(i) + new Vector3(0,0.1f), 
					Vector2.down, 
					4, 
					whatIsGround
				);
				
				if (hitInfo.collider != null)
					Gizmos.color = Color.green;

				Gizmos.DrawRay(target.self.position + (Vector3) GetTrapPosOffset(i) + new Vector3(0,0.1f), Vector2.down * 4);
			}
		}
	}

    public void _SET_TRAPS()
	{
		if (vineTrapObj != null)
		{
			int n = (gm != null && gm.easyMode) ? 3 : 5;
			RaycastHit2D hitInfo = Physics2D.Raycast(
				target.self.position + new Vector3(0,0.1f), 
				Vector2.down, 
				4, 
				whatIsGround
			);

			if (hitInfo.collider != null)
			{
				for (int i=0 ; i<n ; i++)
				{
					// side traps away from player must not be in a wall
					if (i != 0)
					{
						RaycastHit2D wallHitCheck = Physics2D.Raycast(
							hitInfo.point + new Vector2(0,0.1f), 
							i % 2 == 0 ? Vector2.right : Vector2.left, 
							i % 2 == 0 ? i/2 * trapOffset : (i+1)/2 * trapOffset,
							whatIsGround
						);

						// stop at wall
						if (wallHitCheck.collider != null)
							continue;
					}
					
					RaycastHit2D hitInfo2 = Physics2D.Raycast(
						hitInfo.point + GetTrapPosOffset(i) + new Vector2(0,0.1f), 
						Vector2.down, 
						4, 
						whatIsGround
					);

					// ground exists	
					if (hitInfo2.collider != null)
					{
						var obj = Instantiate(
							vineTrapObj, 
							hitInfo2.point, 
							Quaternion.identity
						);
					}
				}
			}
		}
	}

	private Vector2 GetTrapPosOffset(int x)
	{
		return (x % 2 == 0 ? new Vector2(x/2 * trapOffset, 0) : new Vector2(-(x+1)/2 * trapOffset, 0));
	}
}
