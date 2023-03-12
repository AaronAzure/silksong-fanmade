using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerControls : MonoBehaviour
{
	private int playerId;
	private Rewired.Player player;
	public Transform self {get; private set;}
	[SerializeField] Animator anim;


	[Space] [Header("Status")]
	[SerializeField] int maxHp=7;
	[SerializeField] int hp;
	public int atkDmg=10;
	public int gossamerDmg=5;
	public int stabDmg=20;
	[SerializeField] int silkMeter;
	[SerializeField] Animator[] silks;
	[SerializeField] GameObject[] hpMasks;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] Material defaultMat;
	[SerializeField] Material dmgMat;
	[SerializeField] Material flashMat;


	[Space] [Header("Platformer")]
	[SerializeField] Rigidbody2D rb;
	public Transform model;
	[SerializeField] float moveSpeed=5;
	[SerializeField] float jumpDashForce=10;
	[SerializeField] float jumpForce=10;
	[SerializeField] Vector2 wallJumpForce;
	private float moveX;
	private float dashDir;
	private float moveDir;
	private float atkDir;
	// private float moveY;
	private bool jumped;
	private bool jumpDashed;
	private bool isDashing;
	private bool isJumping;
	private bool isWallJumping;
	private bool isGrounded;
	private bool isWallSliding;
	private float jumpTimer;
	private bool canLedgeGrab;
	private bool ledgeGrab;
	private bool noControl;
	[SerializeField] bool isInvincible;
	[SerializeField] bool hasLedge;
	[SerializeField] bool hasWall;
	[SerializeField] Transform ledgeCheckPos;
	[SerializeField] Transform wallCheckPos;
	[SerializeField] float ledgeGrabDist=0.3f;

	[Space] [SerializeField] float jumpMaxTimer=0.5f;
	[SerializeField] Transform groundCheck;
	[SerializeField] Vector2 groundCheckSize;
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] Transform wallCheck;
	[SerializeField] Vector2 wallCheckSize;
	[SerializeField] float wallSlideSpeed=2;

	[Space] [SerializeField] float activeMoveSpeed;
	[SerializeField] float dashSpeed=10;
	[SerializeField] float dashBurstSpeed=15;
	[SerializeField] float dashDuration=0.25f;
	[SerializeField] float dashCooldownDuration=0.5f;
	[SerializeField] float dashCounter;
	[SerializeField] float dashCooldownCounter;
	[SerializeField] float shawForce=7.5f;
	[SerializeField] float atkCooldownDuration=0.4f;

	[Space] [SerializeField] GameObject slashObj;
	private Coroutine atkCo;
	private Coroutine hurtCo;
	private Coroutine bindCo;
	[SerializeField] int bindCost=9;
	[SerializeField] int skillStabCost=2;
	[SerializeField] Image spoolImg;
	[SerializeField] Sprite fullSpoolSpr;
	[SerializeField] Sprite emptySpoolSpr;


	[Space] [Header("Sound effects")]
	[SerializeField] AudioSource shawSound;
	[SerializeField] AudioSource agaleSound;


	[Space] [Header("Particle effects")]
	[SerializeField] GameObject dashEffectL;
	[SerializeField] GameObject dashEffectR;
	[SerializeField] GameObject healingPs;
	[SerializeField] GameObject bloodBurstPs;
	[SerializeField] GameObject bindPs;
	[SerializeField] Transform dashSpawnPos;
	[SerializeField] Animator flashAnim;


	[Space] [Header("Animator Controlled")]
	[SerializeField] bool isLedgeGrabbing; // controlled by animator
	[SerializeField] bool inAnimation;
	[SerializeField] bool performingGossamerStorm;
	[SerializeField] bool cantRotate;
	[SerializeField] bool inAtkState;
	[SerializeField] bool inShawAtk;
	[SerializeField] bool isBinding;
	[SerializeField] bool downwardStrike;
	[SerializeField] bool dashStrike;
	[SerializeField] bool beenHurt;


	[Space] [Header("Debug")]
	[SerializeField] [Range(1,10)] int silkMultiplier=1;
	

	void Awake()
	{
		self = transform;
	}

	// Start is called before the first frame update
	void Start()
	{
		player = ReInput.players.GetPlayer(playerId);
		activeMoveSpeed = moveSpeed;
		bindPs.transform.parent = null;
		healingPs.transform.parent = null;
		bloodBurstPs.transform.parent = null;
		FullRestore();
	}

	bool CanControl()
	{
		return (!isLedgeGrabbing && !beenHurt && !noControl && !isBinding &&
			!inAnimation);
	}

	private void OnDrawGizmosSelected() 
	{
		if (ledgeCheckPos != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(ledgeCheckPos.position, ledgeCheckPos.position + new Vector3(model.localScale.x * ledgeGrabDist, 0));
		}
		if (wallCheckPos != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(wallCheckPos.position, wallCheckPos.position + new Vector3(model.localScale.x * ledgeGrabDist, 0));
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (CanControl() && !inShawAtk )
		{
			if (player.GetButtonDown("Y") && atkCo == null)
				Attack();
			else if (player.GetButtonDown("X") && atkCo == null)
				SkillAttack();

			// bind (heal)
			if (player.GetButtonDown("A") && silkMeter >= bindCost && bindCo == null)
				bindCo = StartCoroutine( BindCo() );

			// jump
			JumpMechanic();

			// dash
			DashMechanic();

			CalcMove();
			CheckCanLedgeGrab();
			// Ledge Grab
			if (canLedgeGrab && !isWallJumping && !isLedgeGrabbing && !ledgeGrab)
				LedgeGrab();
		}
	}

	void FixedUpdate()
	{
		if (CanControl())
		{
			if (inShawAtk)
			{
				if (downwardStrike)
					rb.velocity = new Vector2(moveDir * shawForce, -shawForce);
				else if (dashStrike)
					rb.velocity = new Vector2(moveDir * dashSpeed, rb.velocity.y);

				CheckIsGrounded();
				CheckIsWalledWhistShaw();
				if (atkDir == 2 && (isGrounded || isWallSliding))
				{
					atkCo = null;
					anim.SetBool("isAttacking", false);
				}
			}
			else
			{
				if (!isGrounded)
					anim.SetFloat("jumpVelocity", rb.velocity.y);

				// Dash
				if (dashCounter > 0)
				{
					dashCounter -= Time.fixedDeltaTime;
					if (!isGrounded) 
						rb.velocity = new Vector2(rb.velocity.x, -0.5f);

					if (dashCounter <= 0)
					{
						if (!isGrounded || !player.GetButton("ZR")) isDashing = false;
						activeMoveSpeed = (isDashing) ? dashSpeed : moveSpeed;
						anim.speed = activeMoveSpeed;
						dashCooldownCounter = dashCooldownDuration;
					}
				}
				if (!isDashing && dashCooldownCounter > 0)
				{
					dashCooldownCounter -= Time.fixedDeltaTime;
				}
				
				Move();
				CheckIsGrounded();
				CheckIsWalled();
				if (jumpDashed && jumped && (isGrounded || isWallSliding || canLedgeGrab))
					jumpDashed = jumped = false;
			}
		}
	}

	void DashMechanic()
	{
		if (player.GetButtonDown("ZR") && dashCounter <= 0 && 
			dashCooldownCounter <= 0)
		{
			isDashing = true; // keep dashing if on ground
			jumpDashed = jumped = false;
			jumpTimer = jumpMaxTimer;
				
			dashCounter = dashDuration;
			activeMoveSpeed = dashBurstSpeed;
			anim.speed = dashBurstSpeed;
			if (model.transform.localScale.x > 0)
				Instantiate(dashEffectL, dashSpawnPos.position, dashEffectL.transform.rotation, null);
			else
				Instantiate(dashEffectR, dashSpawnPos.position, dashEffectR.transform.rotation, null);
		}
		if (isDashing && player.GetButtonUp("ZR") && dashCounter <= 0)
		{
			CancelDash();
		}
	}

	void CancelDash()
	{
		dashCounter = 0;
		isDashing = false;
		activeMoveSpeed = moveSpeed;
	}

	void JumpMechanic()
	{
		// First Frame of Jump
		if (isGrounded && !isJumping && player.GetButtonDown("B"))
		{
			Jump();
		}
		// Released jump button
		else if (player.GetButtonUp("B"))
		{
			isJumping = false;
		}
		// Holding jump button
		else if (isJumping && player.GetButton("B"))
		{
			if (jumpTimer < jumpMaxTimer)
			{
				rb.velocity = new Vector2(rb.velocity.x, jumpForce);
				jumpTimer += Time.deltaTime;
			}
			else
			{
				isJumping = false;
			}
		}
		// wall sliding
		else if (isWallSliding && player.GetButtonDown("B"))
		{
			WallJump();
		}
	}

	void ResetAllBools()
	{
		jumpDashed = jumped = false;
		isDashing = false;
		canLedgeGrab = ledgeGrab = false;
		isWallSliding = false;
		isWallJumping = false;
		anim.speed = 1;
		rb.gravityScale = 1;
		activeMoveSpeed = moveSpeed;
		bindCo = null;
		noControl = false;
	}

	void CalcMove()
	{
		moveX = player.GetAxis("Move Horizontal");
	}

	void CheckIsGrounded()
	{
		isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);
		if (!inAtkState) anim.SetBool("isGrounded", isGrounded);
	}
	void CheckIsWalled()
	{
		isWallSliding = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, whatIsGround) && !isGrounded && moveX != 0;
		anim.SetBool("isWallSliding", isWallSliding);

		if (isWallSliding) 
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
	}
	void CheckIsWalledWhistShaw()
	{
		isWallSliding = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, whatIsGround) && !isGrounded;
		anim.SetBool("isWallSliding", isWallSliding);
	}
	void CheckCanLedgeGrab()
	{
		hasLedge = Physics2D.Raycast(ledgeCheckPos.position, new Vector2(moveDir, 0), ledgeGrabDist, whatIsGround);
		hasWall = Physics2D.Raycast(wallCheckPos.position, new Vector2(moveDir, 0), ledgeGrabDist, whatIsGround);
		canLedgeGrab = (!hasLedge && hasWall && !isGrounded);
	}


	void Attack()
	{
		if (isDashing)
			atkCo = StartCoroutine( AttackCo(3) );
		// attack up
		else if (player.GetAxis("Move Vertical") > 0.9f)
			atkCo = StartCoroutine( AttackCo(1) );
		// shaw attack
		else if (player.GetAxis("Move Vertical") < -0.9f)
			atkCo = StartCoroutine( AttackCo(2) );
		// attack front
		else
			atkCo = StartCoroutine( AttackCo(0) );
	}

	IEnumerator AttackCo(int atkDir=0)
	{
		if (atkCo != null) yield break;
		if (atkDir == 2 && isGrounded)
			atkDir = 0;
		this.atkDir = atkDir;
		CancelDash();

		if (slashObj != null)
		{
			anim.speed = 1;
			anim.SetFloat("atkDir", atkDir);
			anim.SetTrigger("attack");
			anim.SetBool("isAttacking", true);
			if (atkDir == 2)
			{
				shawSound.Play();
				jumpDashed = false;
				rb.velocity = Vector2.zero;
				rb.gravityScale = 0;
				yield return new WaitForSeconds(0.1f);
				rb.gravityScale = 1;
			}

			yield return new WaitForSeconds(atkDir != 2 ? 0.25f : 0.4f);
			anim.SetBool("isAttacking", false);
		}

		// atk cooldown
		yield return new WaitForSeconds(atkCooldownDuration);
		atkCo = null;
	}
	
	void SkillAttack()
	{
		if (silkMeter >= skillStabCost && player.GetAxis("Move Vertical") > 0.9f)
			atkCo = StartCoroutine( SkillAttackCo(1) );
		else if (silkMeter >= skillStabCost)
			atkCo = StartCoroutine( SkillAttackCo() );
	}

	IEnumerator SkillAttackCo(int atkDir=0)
	{
		if (atkCo != null) yield break;
		SetSilk(-skillStabCost);
		// CinemachineShake.Instance.ShakeCam(20,0.5f);

		this.atkDir = atkDir;
		CancelDash();
		jumpDashed = false;
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;
		anim.speed = 1;

		// Gossamer Storm
		if (atkDir == 1)
		{
			anim.SetBool("isGossamerStorm", true);

			yield return new WaitForSeconds(0.25f);
			agaleSound.Play();
			SkillAttackEffect();
		}
		else
		{
			anim.SetBool("isSkillAttacking", true);

			yield return new WaitForSeconds(0.25f);
			SkillAttackEffect();

			yield return new WaitForSeconds(0.25f);
			anim.SetBool("isSkillAttacking", false);
			rb.gravityScale = 1;

			// atk cooldown
			yield return new WaitForSeconds(atkCooldownDuration);
			atkCo = null;
		}
	}

	void SkillAttackEffect()
	{
		CinemachineShake.Instance.ShakeCam(5, 0.5f);
		flashAnim.SetTrigger("flash");
	}

	public void CancelGossamerStorm()
	{
		// done
		if (!player.GetButton("X"))
		{
			anim.SetBool("isGossamerStorm", false);
			anim.ResetTrigger("cancelGossamerStorm");
			rb.gravityScale = 1;
			atkCo = null;
		}
		// Continue Performing Gossamer Storm
		else
		{
			if (silkMeter <= 0)
				anim.SetTrigger("cancelGossamerStorm");
			else
				SetSilk(-1);
		}
	}

	void Jump()
	{
		// jumped while dashing
		if (isDashing)
		{
			dashDir = model.localScale.x; // -1 or 1
			jumpDashed = true;
			Invoke("JumpDashed", 0.25f);
			if (!inAtkState) anim.SetTrigger("jump");
		}
		else
		{
			isDashing = false;
		}
		isJumping = true;
		jumpTimer = 0;
	}

	void JumpDashed()
	{
		jumped = true;
	}

	void WallJump()
	{
		isWallSliding = false;
		isWallJumping = true;
		rb.velocity = model.localScale.x > 0 ? new Vector2(-wallJumpForce.x, wallJumpForce.y) : wallJumpForce;
		model.localScale = rb.velocity.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
		Invoke("ResetWallJump", 0.25f);
	}

	void ResetWallJump()
	{
		isWallJumping = false;
	}

	void LedgeGrab()
	{
		isJumping = jumped = false;
		CancelDash();
		jumpDashed = false;

		moveDir = model.localScale.x;
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;
		anim.SetFloat("moveDir", moveDir);
		anim.SetBool("isLedgeGrabbing", true);
		anim.speed = 1;
		ledgeGrab = true;
	}
	public void GRAB_LEDGE()
	{
		transform.position += new Vector3(moveDir * 0.5f, 0.8f);
		canLedgeGrab = ledgeGrab = false;
		rb.gravityScale = 1;
		rb.velocity = Vector2.zero;
		anim.SetBool("isLedgeGrabbing", false);
	}

	void Move()
	{
		if (jumpDashed)
			rb.velocity = new Vector2(dashDir * jumpDashForce, rb.velocity.y);
			
		if (isWallJumping || jumpDashed || isLedgeGrabbing || inShawAtk) return;

		float moveY = player.GetAxis("Move Vertical");
		float x = (isGrounded && moveY > 0.8f) ? 0 : moveX;
		anim.SetBool("isWalking", x != 0);
		anim.SetBool("isDashing", isDashing);

		if (!cantRotate)
		{
			// right
			if (moveX > 0) 
				model.localScale = new Vector3(1, 1, 1);
			// left
			else if (moveX < 0) 
				model.localScale = new Vector3(-1, 1, 1);
			moveDir = model.localScale.x;
		}

			
		// Controlled movement
		if (!isDashing)
		{
			if (!inAtkState)
			{
				if (x != 0)
					anim.speed = Mathf.Abs(x * activeMoveSpeed);
				else 
					anim.speed = 1;
			}
			rb.velocity = new Vector2(x * activeMoveSpeed, rb.velocity.y);
		}
		else
		{
			bool facingRight = (model.localScale.x > 0);
			rb.AddForce(
				new Vector2((facingRight ? 1 : -1) * activeMoveSpeed * 5, 0), 
				ForceMode2D.Force
			);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -dashSpeed, dashSpeed), 
				rb.velocity.y
			);
		}
	}


	// todo --------------------------------------------------------------------

	void SpawnExistingObjAtSelf(GameObject obj)
	{
		obj.SetActive(false);
		obj.SetActive(true);
		obj.transform.position = self.position;
	}

	void FullRestore()
	{
		hp = maxHp;
		SetHp();
	}

	public void ShawRetreat()
	{
		anim.SetBool("isAttacking", false);
		rb.velocity = Vector2.zero;

		rb.AddForce( new Vector2(-moveDir * shawForce, atkDir == 2 ? shawForce : 0), ForceMode2D.Impulse);
		StartCoroutine( RegainControlCo(0.1f) );
	}

	IEnumerator RegainControlCo(float duration, bool invincibility=false)
	{
		if (invincibility) 
			isInvincible = true;
		noControl = true;

		yield return new WaitForSeconds(duration);
		if (invincibility) 
			isInvincible = false;
		noControl = false;
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy") && hurtCo == null)	
			hurtCo = StartCoroutine( TakeDamageCo(other.transform) );
	}

	IEnumerator TakeDamageCo(Transform opponent)
	{
		if (isInvincible)
		{
			hurtCo = null;
			yield break;
		}

		foreach (SpriteRenderer sprite in sprites)
			sprite.material = dmgMat;

		anim.SetBool("isHurt", true);
		hp = Mathf.Max(0, hp - 1);
		ResetAllBools();
		atkCo = null;
		beenHurt = true;
		SetHp();
		rb.velocity = Vector2.zero;
		SpawnExistingObjAtSelf(bloodBurstPs);

		Vector2 direction = (opponent.position - transform.position).normalized;
        rb.velocity = new Vector2(-direction.x * 10, 5);
		CinemachineShake.Instance.ShakeCam(15, 0.25f);
		anim.SetBool("isSkillAttacking", false);
		anim.SetBool("isGossamerStorm", false);
		// if (anim.GetBool("isGossamerStorm"))
		// 	anim.SetTrigger("cancelGossamerStorm");

		// stop healing
		if (bindCo != null) 
		{
			StopCoroutine(bindCo);
			anim.SetBool("isBinding", false);
			bindCo = null;
			rb.gravityScale = 1;
		}

		Time.timeScale = 0;

		yield return new WaitForSecondsRealtime(0.25f);
		anim.SetBool("isHurt", false);
		Time.timeScale = 1;

		yield return new WaitForSeconds(0.25f);
		beenHurt = false;

		yield return new WaitForSeconds(0.5f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		hurtCo = null;
	}

	IEnumerator BindCo()
	{
		if (silkMeter < bindCost)
		{
			bindCo = null;
			yield break;
		} 
		anim.SetBool("isBinding", true);
		anim.speed = 1;
		SetSilk(-bindCost);
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;
		activeMoveSpeed = moveSpeed;
		jumpDashed = jumped = false;
		isDashing = false;
		canLedgeGrab = ledgeGrab = false;
		SpawnExistingObjAtSelf(healingPs);

		yield return new WaitForSeconds(0.25f);
		rb.gravityScale = 1;
		// anim.SetBool("isBinding", false);
		hp = Mathf.Min(hp+3, hpMasks.Length);
		SetHp();
		bindCo = null;
	}

	public IEnumerator FlashCo()
	{
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = flashMat;

		SpawnExistingObjAtSelf(bindPs);
		// Instantiate(bindPs, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(0.1f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		anim.SetBool("isBinding", false);
	}

	void SetHp()
	{
		for (int i=0 ; i<hpMasks.Length ; i++)
		{
			// visible
			if (i < hp && !hpMasks[i].activeSelf)
				hpMasks[i].SetActive(true);
			// invisble
			else if (i >= hp && hpMasks[i].activeSelf)
				hpMasks[i].SetActive(false);
		}
	}
	
	public void SetSilk(int addToSilk=0)
	{
		int prevSilk = silkMeter;
		silkMeter = Mathf.Min(
			silkMeter + addToSilk * (addToSilk > 0 ? silkMultiplier : 1), 
			silks.Length
		);

		// cancel if no changes
		if (prevSilk == silkMeter) return;

		if (spoolImg != null)
		{
			spoolImg.sprite = (silkMeter >= bindCost) ? 
				fullSpoolSpr : emptySpoolSpr;
		}

		for (int i=0 ; i<silks.Length ; i++)
		{
			// visible
			if (i < silkMeter && i >= prevSilk)
				silks[i].SetTrigger("latch");
			// invisble
			else if (i >= silkMeter && i < prevSilk)
				silks[i].SetTrigger("unlatch");
		}
	}

}
