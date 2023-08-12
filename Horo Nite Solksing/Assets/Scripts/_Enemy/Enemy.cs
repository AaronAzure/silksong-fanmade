using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] protected int hp=10;
	[SerializeField] protected int easyHp=5;
	[Space] [SerializeField] protected int phase2;
	[SerializeField] protected int easyPhase2;
	protected bool atPhase2;
	[SerializeField] protected int phase3;
	[SerializeField] protected int easyPhase3;
	protected bool atPhase3;


	[Space] [SerializeField] protected DmgPopup dmgPopup;
	[SerializeField] protected OnTriggerTest inArea;


	[Space] [SerializeField] protected int staggerCount=150;
	[SerializeField] protected int hitCount;
	[SerializeField] protected float recoverTime=1f;
	[SerializeField] protected float recoverTimer;

	[Space] public Transform self;
	[SerializeField] protected Transform model;
	[SerializeField] protected Transform eyes;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] protected Animator anim;
	[SerializeField] protected bool hasJumpVelocityAnim=true;
	[SerializeField] protected bool hasIsGroundedAnim=true;
	[SerializeField] protected bool hasIsMovingAnim=true;
	[SerializeField] protected bool hasMoveSpeedAnim=true;
	[SerializeField] protected bool hasIsDeadAnim=true;
	[SerializeField] protected bool hasSpawnAnim=true;
	[SerializeField] protected bool hasSpawningInAnim=true;
	
	[Space] [SerializeField] protected Rigidbody2D rb;
	[SerializeField] [Tooltip("Model")] protected Collider2D col;
	[SerializeField] protected Collider2D col2;
	

	[Space][SerializeField] protected GameObject closeRangeFinder;
	[SerializeField] protected GameObject distRangeFinder;
	[SerializeField] protected GameObject superCloseRangeFinder;


	[Space] [SerializeField] protected Material dmgMat;
	[SerializeField] protected Material defaultMat;
	[SerializeField] SortingGroup sortGroup;

	
	[Space] [SerializeField] Loot loot;
	[SerializeField] int nLoot=1;
	[SerializeField] int nShells=1;

	
	[Space] [Header("Platformer Related")]
	[SerializeField] protected bool isSmart; // if attacked face direction;
	[SerializeField] protected bool isStupid; // if attacked face direction;
	[SerializeField] protected bool noWalkingAround;
	[SerializeField] protected bool facePlayerOnSpawn;

	[field: SerializeField] public bool isFlying {get; private set;}
	[Space] [SerializeField] protected bool controlledByAnim;
	[SerializeField] protected bool cannotTakeKb;
	[SerializeField] protected bool cannotTakeDmg;
	[SerializeField] protected float moveSpeed=2.5f;
	[SerializeField] protected float chaseSpeed=7.5f;
	[SerializeField] protected float jumpForce=10f;
	[SerializeField] protected float maxSpeed=5;
	[field: SerializeField] protected Transform groundDetect;
	[field: SerializeField] protected Transform wallDetect;
	[field: SerializeField] protected float groundDistDetect=1f;
	[field: SerializeField] protected float wallDistDetect=0.5f;
	[field: SerializeField] protected  LayerMask whatIsPlayer;
	[field: SerializeField] protected  LayerMask whatIsGround;
	[field: SerializeField] protected  LayerMask finalMask;
	[SerializeField] protected bool isGrounded;
	[SerializeField] bool cannotTakeYKb;
	public bool idleActionOnly;
	protected bool beenHurt;
	protected bool receivingKb;
	[field: SerializeField] protected Transform groundCheck;
	[field: SerializeField] protected Vector2 groundCheckSize;

	
	[Space] [SerializeField] protected CurrentAction currentAction=0;
	protected float idleCounter=0;
	[SerializeField] float idleTotalCounter=5;
	[SerializeField] float stationaryThres=2.5f;
	[SerializeField] bool immediateFlip;
	[SerializeField] [Range(-1,1)] int initDir=-1;
	[SerializeField] protected bool stillAttacking;
	protected Coroutine jumpCo;
	protected Coroutine hurtCo;
	protected bool died;


	[Space] [Header("Target Related")]
	public PlayerControls target;
	protected Transform targetDest;
	public bool alwaysInRange;
	public bool neverLoseSight;
	public bool inRange; // player in area
	public bool inSight; // player in line of sight within area
	public bool isClose; // player in close area
	public bool toolSuperClose; // player in close area
	public bool isSuperClose; // player in close area
	protected bool cannotRotate;
	protected float moveDir;
	protected bool attackingPlayer;
	protected float searchCounter;
	[SerializeField] protected float maxSearchTime=2;
	[SerializeField] protected bool spawningIn; // set by animation
	private bool summoned;
	public bool cannotAtk;
	public bool activated;
	public bool inParryState;
	public bool isShielding;



	[Space] [Header("Particle Effect Objs")]
	[SerializeField] GameObject bloodEffectObj;
	[SerializeField] GameObject silkEffectObj;
	[SerializeField] GameObject stringEffectObj;
	[field: SerializeField] public Transform mainPos {get; private set;}


	[Space] [Header("Room Related")]
	public Room room;
	private bool roomEntered;
	public bool isWaiting;


	[Space] [Header("Debug")]
	[SerializeField] GameObject alert;
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
		if (groundCheck != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
		}
		CallChildOnGizmosSelected();
	}

	protected virtual void CallChildOnGizmosSelected() { }
	protected virtual void CallChildOnStart() { }
	protected virtual void CallChildOnSpawnIn() { }
	public virtual void CallChildOnIsSpecial() { }
	protected virtual void CallChildOnEarlyUpdate() { }
	protected virtual void CallChildOnFixedUpdate() { }
	protected virtual void CallChildOnIdleStop() { }
	protected virtual void CallChildOnPhase2() { }
	protected virtual void CallChildOnPhase3() { }
	protected virtual void CallChildOnInSight() { }
	protected virtual void CallChildOnLostSight() { }
	protected virtual void CallChildOnHurt(int dmg, Vector2 forceDir) { }
	protected virtual void CallChildOnHurtAfter() { }
	protected virtual void CallChildOnDeath() { }
	protected virtual void CallChildOnParry() { }
	protected virtual void CallChildOnShielded() { }
	public virtual void CallChildOnSuperClose() { }

	protected virtual bool CallChildOnIsPlayerInSight() 
	{ 
		if (target == null || (!inRange && !alwaysInRange)) return false;
		
		RaycastHit2D playerInfo = Physics2D.Linecast(
			(eyes != null) ? eyes.position : self.position, 
			target.self.position, 
			finalMask
		);
		bool canSeePlayer = (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"));
		if (canSeePlayer)
		{
			CallChildOnInSight();
			attackingPlayer = true;
			searchCounter = 0;
		} 
		return canSeePlayer;
	}
	protected virtual void CallChildOnInAreaSwap() 
	{ 
		inArea.SwapParent();
		gameObject.SetActive(false);
	}


	public void RoomEnter()
	{
		if (!roomEntered)
		{
			roomEntered = true;
			isWaiting = false;
			CallChildOnRoomEnter();
		}
	}
	public virtual void CallChildOnRoomEnter() { cannotAtk = false;}

    // Start is called before the first frame update
    public virtual void Start() 
    {
		if (!summoned && GameManager.Instance.CheckDivineHashmapIfNameIsRegistered(gameObject.name))
		{
			Destroy(gameObject);
		}

		if (GameManager.Instance.easyMode)
		{
			hp = easyHp;
			phase2 = easyPhase2;
			phase3 = easyPhase3;
		}

		if (PlayerControls.Instance != null)
		{
			target = PlayerControls.Instance;
			if (facePlayerOnSpawn)
				FacePlayer();
		}

		
		initDir = (model.localScale.x > 0) ? 1 : -1;
		currentAction = (initDir == 1) ? CurrentAction.right : CurrentAction.left;
		

		CallChildOnStart();

		if (room == null && inArea != null)
		{
			CallChildOnInAreaSwap();
		}
    }

	public virtual void FixedUpdate()
    {
		if (cannotAtk || !activated || (target == null))
			return;
		CallChildOnEarlyUpdate();
		if (spawningIn || controlledByAnim) return;

		if ((idleActionOnly || !attackingPlayer) && !alwaysInRange && !stillAttacking)
			IdleAction();
		else
			AttackingAction();

		if (!isStupid)
		{
			inSight = CallChildOnIsPlayerInSight();
			if (!neverLoseSight)
				KeepLookingForPlayer();
		}

		if (!isFlying)
		{
			CheckIsGrounded();

			if (hasJumpVelocityAnim && !isGrounded && anim != null)
				anim.SetFloat("jumpVelocity", rb.velocity.y);
		}

		CallChildOnFixedUpdate();
    }

	protected bool FacingPlayer()
	{
		return (
			(model.localScale.x > 0 && target.transform.position.x - self.position.x > 0) 
			|| (model.localScale.x < 0 && target.transform.position.x - self.position.x < 0)
		);
	}

	protected float DistanceToPlayer(bool includeYDist=false)
	{
		return includeYDist ? 
			Vector2.Distance(target.transform.position, self.position) : 
			Mathf.Abs(target.transform.position.x - self.position.x);
	}
	protected float DirectionToPlayer()
	{
		return target.transform.position.x - self.position.x;
	}
	protected bool PlayerIsToTheRight()
	{
		return (target.transform.position.x - self.position.x) > 0;
	}

	void KeepLookingForPlayer()
	{
		if (!inSight && attackingPlayer && !alwaysInRange)
		{
			searchCounter += Time.fixedDeltaTime;
			if (searchCounter > maxSearchTime)
			{
				searchCounter = 0;
				attackingPlayer = false;
				CallChildOnLostSight();
			}
		}
	}

	/// <summary>
	/// Return true if not safe (at wall or at cliff), otherwise Return false
	/// </summary>
	protected virtual bool CheckSurrounding()
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

	/// <summary>
	/// Return true if there is a wall, otherwise Return false
	/// </summary>
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

	protected virtual void CheckIsGrounded()
	{
		isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);
		if (hasIsGroundedAnim && anim != null) anim.SetBool("isGrounded", isGrounded);
	}

	protected virtual void IdleAction() {  }

	protected virtual void AttackingAction() { }

	private GameObject prevAtk;
	public void TakeDamage(int dmg, Transform opponent, Vector2 forceDir, 
		float force, bool canShake=true, bool canParry=true, bool canBlock=true, GameObject currAtk=null)
	{
		if (currAtk != null)
		{
			if (prevAtk != null && prevAtk == currAtk)
				return;
			prevAtk = currAtk;
		}
		// block attack when facing player
		if (inParryState && canParry && FacingPlayer())
		{
			CallChildOnParry();
		}
		else if (isShielding && canBlock && FacingPlayer())
		{
			CallChildOnShielded();
		}
		else if (!isWaiting && !died)
		{
			if (hp > 0 && hurtCo != null)
				StopCoroutine(hurtCo);
			hurtCo = StartCoroutine( TakeDamageCo(dmg, opponent, forceDir, force * 1.2f, canShake) );
		}
	}

	public void CallMasterOnShield(bool canBlock)
	{
		if (isShielding && canBlock && FacingPlayer())
		{
			CallChildOnShielded();
		}
	}

	protected virtual void CallChildOnLoseHp(int dmg)
	{
		if (!cannotTakeDmg)
			hp -= dmg;
		if (GameManager.Instance.showDmg && dmgPopup != null)
		{
			// var obj = Instantiate(dmgPopup, transform.position + Vector3.up, Quaternion.identity);
			if (dmgPopup.gameObject.activeSelf)
			{
				dmgPopup.txt.text = $"{dmg + int.Parse(dmgPopup.txt.text)}";
				dmgPopup.anim.SetTrigger("reset");

			}
			else
				dmgPopup.txt.text = $"{dmg}";
			dmgPopup.gameObject.SetActive(true);
		}
	}

	IEnumerator TakeDamageCo(int dmg, Transform opponent, Vector2 forceDir, float force, bool canShake)
	{
		CallChildOnLoseHp(dmg);
		beenHurt = true;
		if (opponent != null)
			forceDir = (self.position - opponent.position).normalized;
		float forceY = (isFlying || !isGrounded) ? forceDir.y : Mathf.Abs(forceDir.y);
		if (cannotTakeYKb)
			forceDir = new Vector2(forceDir.x, 0);
		float angleZ = 
			Mathf.Atan2(forceY, forceDir.x) * Mathf.Rad2Deg;
		if (silkEffectObj != null)
			Instantiate(silkEffectObj, mainPos != null ? mainPos.position : transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
		if (!cannotTakeDmg && bloodEffectObj != null)
			Instantiate(bloodEffectObj, mainPos != null ? mainPos.position : transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
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
		if (sprites != null)
			foreach (SpriteRenderer sprite in sprites)
				sprite.material = dmgMat;
		if (!died && hp <= 0)
		{
			died = true;
			rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
			if (room == null)
				GameManager.Instance.RegisterNameToEnemiesDefeated(gameObject.name);
			Died(canShake);
			yield break;
		}

		if (isSmart && !attackingPlayer)
		{
			Debug.Log("Face Player (" + name + ")");
			FacePlayer();
		}

		CallChildOnHurt(dmg, forceDir);
		yield return new WaitForSeconds(0.2f);
		if (sprites != null)
			foreach (SpriteRenderer sprite in sprites)
				sprite.material = defaultMat;

		beenHurt = false;
		receivingKb = false;
		if (!cannotTakeKb && force != 0)
			rb.velocity = new Vector2(0, isFlying ? 0 : rb.velocity.y);
		CallChildOnHurtAfter();
	}

	public IEnumerator _APPEAR_IN_CO()
	{
		Color newColor = new Color(0,0,0,1);
		if (sprites != null)
			foreach (SpriteRenderer sprite in sprites)
				sprite.color = newColor;
				
		yield return new WaitForSeconds(0.3f);
		for (float i=0.1f ; i<=1 ; i += 0.05f)
		{
			newColor = new Color(i, i, i, 1);
			if (sprites != null)
				foreach (SpriteRenderer sprite in sprites)
					sprite.color = newColor;
			yield return new WaitForSeconds(0.05f);
		}
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
		StopAllCoroutines();
		if (loot != null)
		{
			loot.SpawnLoot( 
				Mathf.RoundToInt(nLoot * target.lootMultiplier),
				Mathf.RoundToInt(nShells * target.lootMultiplier)	
			);
		}
		if (closeRangeFinder != null) Destroy(closeRangeFinder);
		if (distRangeFinder != null) Destroy(distRangeFinder);
		if (superCloseRangeFinder != null) Destroy(superCloseRangeFinder);
		if (inArea != null) 
		{
			inArea.OnMasterDeath();
		}
		if (shake) CinemachineShake.Instance.ShakeCam(2.5f, 0.5f);
		CallChildOnDeath();
		StartCoroutine( DiedCo() );
	}
	
	IEnumerator DiedCo()
	{
		if (room != null)
			room.Defeated();
		if (stringEffectObj != null)
			stringEffectObj.transform.parent = null;
		if (col != null) col.enabled = false;
		if (col2 != null) col2.enabled = false;
		this.gameObject.layer = 5;
		rb.velocity = Vector2.zero;
		rb.gravityScale = 1;
		this.enabled = false;
		// if (alert != null) alert.SetActive( false );
		if (hasIsDeadAnim && anim != null)
			anim.SetBool("isDead", true);
		sortGroup.sortingOrder = -1000 + PlayerControls.Instance.IncreaseKills();

		yield return new WaitForSeconds(0.2f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		foreach (SpriteRenderer sprite in sprites)
			sprite.color = new Color(0.4f,0.4f,0.4f,1);
		Destroy(this);
		// this.enabled = false;
	}

	public void SpawnIn()
	{
		summoned = true;
		if (hasSpawnAnim && anim != null) 
			anim.SetTrigger("spawn");
		if (PlayerControls.Instance != null)
			target = PlayerControls.Instance;
		FacePlayer();
		CallChildOnSpawnIn();
		spawningIn = true;
		if (hasSpawningInAnim && anim != null) 
			anim.SetBool("spawningIn", true);
		if (col != null) col.enabled = false;
		if (col2 != null) col2.enabled = false;
	}

	public void SHOW_MODEL()
	{
		model.gameObject.SetActive(true);
	}

	public void ACTIVATE_HITBOX()
	{
		if (!died && col != null)
			col.enabled = true;
		if (!died && col2 != null)
			col2.enabled = true;
		spawningIn = false;
		if (hasSpawningInAnim && anim != null)
			anim.SetBool("spawningIn", false);
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!activated && other.CompareTag("MainCamera"))	
		{
			this.enabled = activated = true;
		}
		if (other.CompareTag("QuickDeath"))	
		{
			TakeDamage(
				hp, other.transform, (Vector2) self.position - other.ClosestPoint(self.position), 
				5, true, false, false
			);
		}
	}
	
	private void OnTriggerExit2D(Collider2D other) 
	{
		if (activated && other.CompareTag("MainCamera2"))	
		{
			this.enabled = activated = false;
		}
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

	protected void ChooseNextAction()
	{
		if (currentAction == CurrentAction.right)
		{
			currentAction = immediateFlip ? CurrentAction.left : CurrentAction.none;
		}
		else if (currentAction == CurrentAction.left)
		{
			currentAction = immediateFlip ? CurrentAction.right : CurrentAction.none;
		}
		// stationary
		else
		{
			// looking right
			if (model.localScale.x > 0)
			{
				currentAction = CurrentAction.left;
				model.localScale = new Vector3(-1,1,1);
			}
			// looking left
			else
			{
				currentAction = CurrentAction.right;
				model.localScale = new Vector3(1,1,1);
			}
		}
	}

	protected void WalkAround()
	{
		if (noWalkingAround) return;

		if (idleTotalCounter > 0)
		{
			idleCounter += Time.fixedDeltaTime;

			if (idleCounter > idleTotalCounter)
			{
				idleCounter = 0;
				ChooseNextAction();
			}
		}
		else if (!immediateFlip && stationaryThres > 0 && 
			currentAction == CurrentAction.none && CheckSurrounding())
		{
			idleCounter += Time.fixedDeltaTime;

			if (idleCounter > stationaryThres)
			{
				idleCounter = 0;
				ChooseNextAction();
			}
		}

		// stop moving if about to walk into wall or off cliff
		if (isGrounded && !inSight && currentAction != CurrentAction.none && CheckSurrounding())
		{
			idleCounter = 0;
			CallChildOnIdleStop();
			if (immediateFlip)
			{
				ChooseNextAction();
			}
			else
			{
				currentAction = CurrentAction.none;
			}
		}

		if (receivingKb)
		{
			if (hasIsMovingAnim && anim != null) anim.SetBool("isMoving", false);
		}
		else if (currentAction == CurrentAction.left)
		{
			rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
			if (hasIsMovingAnim && anim != null) anim.SetBool("isMoving", true);
			model.localScale = new Vector3(-1,1,1);

		}
		else if (currentAction == CurrentAction.none)
		{
			rb.velocity = new Vector2(0, rb.velocity.y);
			if (hasIsMovingAnim && anim != null) anim.SetBool("isMoving", false);

		}
		else if (currentAction == CurrentAction.right)
		{
			rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
			if (hasIsMovingAnim && anim != null) anim.SetBool("isMoving", true);
			model.localScale = new Vector3(1,1,1);

		}
		if (hasMoveSpeedAnim && !isFlying && anim != null)
			anim.SetFloat("moveSpeed", Mathf.Abs(rb.velocity.x));
	}

	protected void FlyAround()
	{
		if (idleTotalCounter > 0)
		{
			idleCounter += Time.fixedDeltaTime;

			if (idleCounter > idleTotalCounter)
			{
				idleCounter = 0;
				ChooseNextAction();
			}
			// stop moving if about to walk into wall or off cliff
			else if (!inSight && currentAction != 0 && CheckWall())
			{
				idleCounter = 0;
				currentAction = CurrentAction.none;
			}
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

	protected virtual void ChasePlayer()
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
			if (hasMoveSpeedAnim && !isFlying)
				anim.SetFloat("moveSpeed", Mathf.Abs(rb.velocity.x));
			if (hasIsMovingAnim)
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

	public void ToggleSuperClose(bool active)
	{
		StartCoroutine( ToggleSuperCloseCo(active) );
	}

	IEnumerator ToggleSuperCloseCo(bool active)
	{
		yield return new WaitForSeconds(0.25f);
		isSuperClose = active;
	}

	protected IEnumerator JumpCo(float delay=0.2f)
	{
		rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		moveDir = moveSpeed * model.localScale.x;
		
		yield return new WaitForSeconds(delay);
		jumpCo = null;
	}

	public IEnumerator JUMP()
	{
		rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		moveDir = moveSpeed * model.localScale.x;
		
		yield return new WaitForSeconds(2f);
		jumpCo = null;
	}
}
