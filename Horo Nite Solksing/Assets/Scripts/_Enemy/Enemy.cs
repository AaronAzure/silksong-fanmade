using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] protected int hp=10;
	[SerializeField] protected int halfHp;
	public Transform self;
	[SerializeField] protected Transform model;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] protected Animator anim;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] Collider2D col;


	[SerializeField] Material defaultMat;
	[SerializeField] Material dmgMat;
	[SerializeField] SortingGroup sortGroup;

	
	[Space] [Header("Platformer Related")]
	[SerializeField] protected bool isSmart; // if attacked face direction;
	[SerializeField] protected bool controlledByAnim;
	[SerializeField] protected bool cannotTakeKb;
	[SerializeField] float moveSpeed=2.5f;
	[SerializeField] protected float chaseSpeed=7.5f;
	[SerializeField] protected float jumpForce=10f;
	[SerializeField] float runSpeed=5;
	[SerializeField] Transform groundDetect;
	[SerializeField] Transform wallDetect;
	[SerializeField] float groundDistDetect=0.5f;
	[SerializeField] float wallDistDetect=1;
	[SerializeField] LayerMask whatIsPlayer;
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] LayerMask finalMask;
	protected bool isGrounded;
	[SerializeField] bool isFlying;
	[SerializeField] protected bool idleActionOnly;
	protected bool beenHurt;
	[SerializeField] Transform groundCheck;
	[SerializeField] Vector2 groundCheckSize;

	
	[Space] protected CurrentAction currentAction=0;
	private float idleCounter=0;
	[SerializeField] float idleTotalCounter=5;
	[SerializeField] [Range(-1,1)] int initDir=-1;
	protected int nextDir;
	protected Coroutine jumpCo;
	protected Coroutine hurtCo;
	// protected RaycastHit2D playerInfo;
	// protected RaycastHit2D groundInfo;
	// protected RaycastHit2D wallInfo;


	[Space] [Header("Target Related")]
	public PlayerControls target;
	public bool alwaysInRange;
	public bool inRange; // player in area
	public bool inSight; // player in line of sight within area
	public bool isClose; // player in close area
	protected bool cannotRotate;
	protected float moveDir;
	protected bool attackingPlayer;
	protected float searchCounter;
	protected float maxSearchTime=2;
	[SerializeField] protected bool spawningIn; // set by animation
	public bool cannotAtk;



	[Space] [Header("Particle Effect Objs")]
	[SerializeField] GameObject silkEffectObj;
	[SerializeField] GameObject bloodEffectObj;
	[SerializeField] GameObject alert;


	[Space] [Header("Room Related")]
	[SerializeField] Room room;
	private bool roomEntered;


	[Space] [Header("Debug")]
	[SerializeField] float offset=30;


	protected enum CurrentAction {left=-1, none=0, right=1}


	private void OnDrawGizmosSelected() 
	{
		if (groundDetect != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(groundDetect.position, groundDetect.position + new Vector3(0, -groundDistDetect));
		}
		if (wallDetect != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(wallDetect.position, wallDetect.position + new Vector3(model.localScale.x * wallDistDetect, 0));
		}
	}

	protected virtual void CallChildOnStart() { }
	protected virtual void CallChildOnEarlyUpdate() { }
	protected virtual void CallChildOnFixedUpdate() { }
	protected virtual void CallChildOnHalfHp() { }


	public void RoomEnter()
	{
		if (!roomEntered)
		{
			roomEntered = true;
			CallChildOnRoomEnter();
		}
	}
	public virtual void CallChildOnRoomEnter() { }

    // Start is called before the first frame update
    public virtual void Start() 
    {
		// isFlying = rb.gravityScale == 0 ? true : false;
		initDir = (int) model.localScale.x;
		nextDir = -initDir;
		currentAction = (initDir == 1) ? CurrentAction.right : CurrentAction.left;
		CallChildOnStart();
    }

	public virtual void FixedUpdate()
    {
		CallChildOnEarlyUpdate();
		if (spawningIn || controlledByAnim) return;

		if (idleActionOnly || !attackingPlayer)
			IdleAction();
		else
			AttackingAction();

		inSight = PlayerInSight();
		KeepLookingForPlayer();

		if (!isFlying)
		{
			CheckIsGrounded();

			if (!isGrounded)
				anim.SetFloat("jumpVelocity", rb.velocity.y);
		}

		// if (alert != null) alert.SetActive( attackingPlayer );
		CallChildOnFixedUpdate();
    }

	protected bool PlayerInSight()
	{
		if (target == null || (!inRange && !alwaysInRange)) return false;
		
		RaycastHit2D playerInfo = Physics2D.Linecast(self.position, target.self.position, finalMask);
		bool canSeePlayer = (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"));
		if (canSeePlayer)
		{
			attackingPlayer = true;
			searchCounter = 0;
		} 
		return canSeePlayer;
	}

	void KeepLookingForPlayer()
	{
		if (!inSight && attackingPlayer)
		{
			searchCounter += Time.fixedDeltaTime;
			if (searchCounter > maxSearchTime)
			{
				searchCounter = 0;
				attackingPlayer = false;
			}
		}
	}

	protected bool CheckSurrounding()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundDetect.position, 
			groundDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		RaycastHit2D wallInfo = Physics2D.Linecast(
			wallDetect.position, 
			wallDetect.position + new Vector3(model.localScale.x * wallDistDetect, 0), 
			whatIsGround
		);
		return (groundInfo.collider == null || wallInfo.collider != null);
	}
	protected bool CheckWall()
	{
		RaycastHit2D wallInfo = Physics2D.Linecast(
			wallDetect.position, 
			wallDetect.position + new Vector3(model.localScale.x * wallDistDetect, 0), 
			whatIsGround
		);
		return (wallInfo.collider != null);
	}

	protected bool CheckCliff()
	{
		RaycastHit2D groundInfo = Physics2D.Linecast(
			groundDetect.position, 
			groundDetect.position + new Vector3(0, -groundDistDetect), 
			whatIsGround
		);
		return (groundInfo.collider != null);
	}

	protected bool PlayerInFront()
	{
		RaycastHit2D playerInfo = Physics2D.Linecast(
			wallDetect.position, 
			wallDetect.position + new Vector3(model.localScale.x * wallDistDetect, 0), 
			whatIsPlayer
		);
		return (playerInfo.collider != null);
	}

	protected void CheckIsGrounded()
	{
		isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);
		anim.SetBool("isGrounded", isGrounded);
	}

	protected virtual void IdleAction() {  }

	protected virtual void AttackingAction() { }

	public void TakeDamage(int dmg, Transform opponent, Vector2 forceDir, float force)
	{
		if (hp > 0 && hurtCo != null)
			StopCoroutine(hurtCo);
		hurtCo = StartCoroutine( TakeDamageCo(dmg, opponent, forceDir, force) );
	}

	IEnumerator TakeDamageCo(int dmg, Transform opponent, Vector2 forceDir, float force)
	{
		hp -= dmg;
		beenHurt = true;
		if (opponent != null)
			forceDir = (self.position - opponent.position).normalized;
		float angleZ = 
			Mathf.Atan2(Mathf.Abs(forceDir.y), forceDir.x) * Mathf.Rad2Deg;
		Instantiate(silkEffectObj, transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
		Instantiate(bloodEffectObj, transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
		if (!cannotTakeKb && force != 0)
		{
			rb.velocity = Vector2.zero;
			rb.velocity = forceDir * force;
		}
		if (hp <= halfHp)
		{
			CallChildOnHalfHp();
		}
		if (hp <= 0)
		{
			Died();
			yield break;
		}
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = dmgMat;

		if (isSmart && !attackingPlayer)
			FacePlayer();

		yield return new WaitForSeconds(0.2f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;

		beenHurt = false;
		if (!cannotTakeKb && force != 0)
			rb.velocity = new Vector2(0, rb.velocity.y);
	}
	
	void Died()
	{
		col.enabled = false;
		this.gameObject.layer = 5;
		rb.velocity = Vector2.zero;
		rb.gravityScale = 1;
		this.enabled = false;
		StopAllCoroutines();
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		foreach (SpriteRenderer sprite in sprites)
			sprite.color = new Color(0.4f,0.4f,0.4f,1);
		if (alert != null) alert.SetActive( false );
		if (anim != null)
			anim.SetBool("isDead", true);
		// transform.rotation = Quaternion.Euler(0,0,90);
		sortGroup.sortingOrder = -1;
	}

	public void SpawnIn()
	{
		anim.SetTrigger("spawn");
		col.enabled = false;
	}

	public void ACTIVATE_HITBOX()
	{
		col.enabled = true;
	}
	

	// todo --------------------------------------------------------------------
	// todo ------------------ Attack Patterns ---------------------------------

	protected CurrentAction GetAction(int actionInd)
	{
		switch (actionInd)
		{
			case -1: return CurrentAction.left;
			case 0: return CurrentAction.none;
			case 1: return CurrentAction.right;
			default: return CurrentAction.none;
		}
	}

	protected void FacePlayer(int playerDir=0)
	{
		if (target == null)
		{
			model.localScale = new Vector3(-model.localScale.x,1,1); // look other way
			return;
		}
		if (playerDir == 0)
			playerDir = (target.self.position.x - self.position.x > 0) ? 1 : -1;
		model.localScale = new Vector3(playerDir,1,1);
	}

	protected void WalkAround()
	{
		idleCounter += Time.fixedDeltaTime;
		if (idleCounter > idleTotalCounter)
		{
			idleCounter = 0;
			currentAction = currentAction + nextDir;
			if (currentAction == CurrentAction.right)
				nextDir = -1;
			else if (currentAction == CurrentAction.left)
				nextDir = 1;
		}
		// stop moving if about to walk into wall or off cliff
		else if (!inSight && currentAction != 0 && CheckSurrounding())
		{
			idleCounter = 0;
			currentAction = CurrentAction.none;
		}

		if (beenHurt)
		{
			anim.SetBool("isMoving", false);
		}
		else if (currentAction == CurrentAction.left)
		{
			rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
			anim.SetBool("isMoving", true);
			model.localScale = new Vector3(-1,1,1);

		}
		else if (currentAction == CurrentAction.none)
		{
			rb.velocity = new Vector2(0, rb.velocity.y);
			anim.SetBool("isMoving", false);

		}
		else if (currentAction == CurrentAction.right)
		{
			rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
			anim.SetBool("isMoving", true);
			model.localScale = new Vector3(1,1,1);

		}
		if (!isFlying)
			anim.SetFloat("moveSpeed", rb.velocity.x);
	}

	protected void FlyAround()
	{
		idleCounter += Time.fixedDeltaTime;
		if (idleCounter > idleTotalCounter)
		{
			idleCounter = 0;
			currentAction = currentAction + nextDir;
			if (currentAction == CurrentAction.right)
				nextDir = -1;
			else if (currentAction == CurrentAction.left)
				nextDir = 1;
		}
		// stop moving if about to walk into wall or off cliff
		else if (!inSight && currentAction != 0 && CheckWall())
		{
			idleCounter = 0;
			currentAction = CurrentAction.none;
		}

		Debug.Log("flying around");

		if (beenHurt)
		{

		}
		else if (currentAction == CurrentAction.left)
		{
			rb.velocity = new Vector2(-moveSpeed, 0);
			model.localScale = new Vector3(-1,1,1);
		}
		else if (currentAction == CurrentAction.none)
		{
			rb.velocity = new Vector2(0, 0);
		}
		else if (currentAction == CurrentAction.right)
		{
			rb.velocity = new Vector2(moveSpeed, 0);
			model.localScale = new Vector3(1,1,1);
		}
	}

	protected void ChasePlayer()
	{
		int playerDir = (target.self.position.x - self.position.x > 0) ? 1 : -1;
		FacePlayer( playerDir );
		if (!beenHurt)
		{
			rb.AddForce(new Vector2(moveSpeed * playerDir * 5, 0), ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), 
				rb.velocity.y 
			);
		}
		if (!isFlying)
			anim.SetFloat("moveSpeed", rb.velocity.x);
		anim.SetBool("isMoving", true);
	}

	protected void MoveInPrevDirection()
	{
		if (!beenHurt)
		{
			rb.AddForce(new Vector2(moveSpeed * moveDir * 5, 0), ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), 
				rb.velocity.y 
			);
		}
	}

	protected IEnumerator JumpCo(float delay=0.2f)
	{
		rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		moveDir = moveSpeed * model.localScale.x;
		
		yield return new WaitForSeconds(delay);
		jumpCo = null;
	}
}
