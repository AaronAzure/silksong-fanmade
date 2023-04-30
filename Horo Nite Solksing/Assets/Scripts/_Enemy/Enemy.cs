using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] protected int hp=10;
	[SerializeField] protected int phase2;
	protected bool atPhase2;
	[SerializeField] protected int phase3;
	protected bool atPhase3;
	[Space] [SerializeField] DmgPopup dmgPopup;
	[Space] [SerializeField] protected int staggerCount=150;
	[SerializeField] protected int hitCount;
	[SerializeField] protected float recoverTime=1f;
	[SerializeField] protected float recoverTimer;

	[Space] public Transform self;
	[SerializeField] protected Transform model;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] protected Animator anim;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] Collider2D col;


	[SerializeField] protected Material defaultMat;
	[SerializeField] protected Material dmgMat;
	[SerializeField] SortingGroup sortGroup;

	
	[Space] [Header("Platformer Related")]
	[SerializeField] protected bool isSmart; // if attacked face direction;
	[SerializeField] protected bool controlledByAnim;
	[SerializeField] protected bool cannotTakeKb;
	[SerializeField] protected bool cannotTakeDmg;
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
	protected bool receivingKb;
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
	public bool inParryState;



	[Space] [Header("Particle Effect Objs")]
	[SerializeField] GameObject silkEffectObj;
	[SerializeField] GameObject bloodEffectObj;
	[SerializeField] GameObject stringEffectObj;
	[SerializeField] GameObject alert;


	[Space] [Header("Room Related")]
	public Room room;
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
	protected virtual void CallChildOnPhase2() { }
	protected virtual void CallChildOnPhase3() { }
	protected virtual void CallChildOnHurt(int dmg, Vector2 forceDir) { }
	protected virtual void CallChildOnHurtAfter() { }
	protected virtual void CallChildOnDeath() { }
	protected virtual void CallChildOnParry() { }


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

			if (!isGrounded && anim != null)
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

	protected bool FacingPlayer()
	{
		return (
			(model.localScale.x > 0 && target.transform.position.x - self.position.x > 0) 
			|| (model.localScale.x < 0 && target.transform.position.x - self.position.x < 0)
		);
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
	protected bool PlayerInFarFront()
	{
		RaycastHit2D playerInfo = Physics2D.Linecast(
			wallDetect.position, 
			wallDetect.position + new Vector3(model.localScale.x * wallDistDetect * 4, 0), 
			whatIsPlayer
		);
		return (playerInfo.collider != null);
	}

	protected void CheckIsGrounded()
	{
		isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);
		if (anim != null) anim.SetBool("isGrounded", isGrounded);
	}

	protected virtual void IdleAction() {  }

	protected virtual void AttackingAction() { }

	private GameObject prevAtk;
	public void TakeDamage(int dmg, Transform opponent, Vector2 forceDir, 
		float force, bool canShake=true, bool canParry=true, GameObject currAtk=null)
	{
		if (currAtk != null)
		{
			if (prevAtk != null && prevAtk == currAtk)
				return;
			prevAtk = currAtk;
		}
		if (inParryState && canParry && FacingPlayer())
		{
			CallChildOnParry();
		}
		else
		{
			if (hp > 0 && hurtCo != null)
				StopCoroutine(hurtCo);
			hurtCo = StartCoroutine( TakeDamageCo(dmg, opponent, forceDir, force, canShake) );
		}
	}

	IEnumerator TakeDamageCo(int dmg, Transform opponent, Vector2 forceDir, float force, bool canShake)
	{
		if (!cannotTakeDmg)
			hp -= dmg;
		if (GameManager.Instance.showDmg && dmgPopup != null)
		{
			var obj = Instantiate(dmgPopup, transform.position + Vector3.up, Quaternion.identity);
			obj.txt.text = $"{dmg}";
		}
		beenHurt = true;
		if (opponent != null)
			forceDir = (self.position - opponent.position).normalized;
		float angleZ = 
			Mathf.Atan2(Mathf.Abs(forceDir.y), forceDir.x) * Mathf.Rad2Deg;
		Instantiate(silkEffectObj, transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
		if (!cannotTakeDmg)
			Instantiate(bloodEffectObj, transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
		if (!cannotTakeKb && force != 0)
		{
			receivingKb = true;
			rb.velocity = Vector2.zero;
			rb.velocity = forceDir * force;
		}
		if (!atPhase3 && hp <= phase3)
		{
			atPhase3 = true;
			atPhase2 = true;
			CallChildOnPhase3();
		}
		if (!atPhase2 && hp <= phase2)
		{
			atPhase2 = true;
			CallChildOnPhase2();
		}
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = dmgMat;
		if (hp <= 0)
		{
			rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
			Died(canShake);
			yield break;
		}

		if (isSmart && !attackingPlayer)
			FacePlayer();

		CallChildOnHurt(dmg, forceDir);
		yield return new WaitForSeconds(0.2f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;

		beenHurt = false;
		receivingKb = false;
		if (!cannotTakeKb && force != 0)
			rb.velocity = new Vector2(0, rb.velocity.y);
		CallChildOnHurtAfter();
	}

	public IEnumerator FlashCo()
	{
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = dmgMat;

		yield return new WaitForSeconds(0.1f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
	}

	void Died(bool shake)
	{
		if (shake) CinemachineShake.Instance.ShakeCam(2.5f, 0.5f);
		StopAllCoroutines();
		StartCoroutine( DiedCo() );
		CallChildOnDeath();
	}
	
	IEnumerator DiedCo()
	{
		if (room != null)
			room.Defeated();
		if (stringEffectObj != null)
			stringEffectObj.transform.parent = null;
		col.enabled = false;
		this.gameObject.layer = 5;
		rb.velocity = Vector2.zero;
		rb.gravityScale = 1;
		this.enabled = false;
		// if (alert != null) alert.SetActive( false );
		if (anim != null)
			anim.SetBool("isDead", true);
		sortGroup.sortingOrder = -1000 + PlayerControls.Instance.IncreaseKills();

		yield return new WaitForSeconds(0.2f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		foreach (SpriteRenderer sprite in sprites)
			sprite.color = new Color(0.4f,0.4f,0.4f,1);
	}

	public void SpawnIn()
	{
		if (anim != null) anim.SetTrigger("spawn");
		spawningIn = true;
		col.enabled = false;
	}

	public void SHOW_MODEL()
	{
		model.gameObject.SetActive(true);
	}

	public void ACTIVATE_HITBOX()
	{
		col.enabled = true;
		spawningIn = false;
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

		if (receivingKb)
		{
			if (anim != null) anim.SetBool("isMoving", false);
		}
		else if (currentAction == CurrentAction.left)
		{
			rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
			if (anim != null) anim.SetBool("isMoving", true);
			model.localScale = new Vector3(-1,1,1);

		}
		else if (currentAction == CurrentAction.none)
		{
			rb.velocity = new Vector2(0, rb.velocity.y);
			if (anim != null) anim.SetBool("isMoving", false);

		}
		else if (currentAction == CurrentAction.right)
		{
			rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
			if (anim != null) anim.SetBool("isMoving", true);
			model.localScale = new Vector3(1,1,1);

		}
		if (!isFlying && anim != null)
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

		if (receivingKb)
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
		if (!receivingKb)
		{
			rb.AddForce(new Vector2(moveSpeed * playerDir * 5, 0), ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), 
				rb.velocity.y 
			);
		}
		
		if (anim != null) 
		{
			if (!isFlying)
				anim.SetFloat("moveSpeed", rb.velocity.x);
			anim.SetBool("isMoving", true);
		}
	}

	protected void MoveInPrevDirection()
	{
		if (!receivingKb)
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
