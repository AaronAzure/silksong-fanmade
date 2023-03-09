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
	[SerializeField] int silkMeter;
	[SerializeField] Animator[] silks;
	[SerializeField] GameObject[] hpMasks;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] Material defaultMat;
	[SerializeField] Material dmgMat;


	[Space] [Header("Platformer")]
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Transform model;
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
	[SerializeField] Image spoolImg;
	[SerializeField] Sprite fullSpoolSpr;
	[SerializeField] Sprite emptySpoolSpr;


	[Space] [Header("Particle effects")]
	[SerializeField] AudioSource shawSound;


	[Space] [Header("Particle effects")]
	[SerializeField] GameObject dashEffectL;
	[SerializeField] GameObject dashEffectR;
	[SerializeField] Transform dashSpawnPos;


	[Space] [Header("Animator Controlled")]
	[SerializeField] bool isLedgeGrabbing; // controlled by animator
	[SerializeField] bool cantRotate;
	[SerializeField] bool inAtkState;
	[SerializeField] bool inShawAtk;
	[SerializeField] bool beenHurt;

	

	void Awake()
	{
		self = transform;
	}

	// Start is called before the first frame update
	void Start()
	{
		player = ReInput.players.GetPlayer(playerId);
		activeMoveSpeed = moveSpeed;
		FullRestore();
	}

	private void OnDrawGizmosSelected() 
	{
		// if (groundCheck != null)
		// 	Gizmos.DrawCube(groundCheck.position, groundCheckSize);
		// if (wallCheck != null)
		// 	Gizmos.DrawCube(wallCheck.position, wallCheckSize);
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
		if (!isLedgeGrabbing && !inShawAtk && !beenHurt)
		{
			if (player.GetButtonDown("Y") && atkCo == null)
				Attack();

			// bind (heal)
			if (player.GetButtonDown("A") && bindCo == null)
				bindCo = StartCoroutine( BindCo() );

			// jump
			JumpMechanic();

			// dash
			DashMechanic();

			CalcMove();
			CheckCanLedgeGrab();
		}
	}

	void FixedUpdate()
	{
		if (!isLedgeGrabbing && !beenHurt)
		{
			if (inShawAtk)
			{
				rb.velocity = new Vector2(moveDir * shawForce, -shawForce);

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

				// Ledge Grab
				if (canLedgeGrab && !isLedgeGrabbing && !ledgeGrab)
					LedgeGrab();
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
			isDashing = false;
			activeMoveSpeed = moveSpeed;
		}
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
				rb.velocity = Vector2.up * jumpForce;
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
		isWallSliding = false;
		isWallJumping = false;
		canLedgeGrab = ledgeGrab = false;
		anim.speed = 1;
		activeMoveSpeed = moveSpeed;
		bindCo = null;
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
		// attack up
		if (player.GetAxis("Move Vertical") > 0.9f)
			atkCo = StartCoroutine( AttackCo(1) );
		// shaw attack
		else if (player.GetAxis("Move Vertical") < -0.9f)
		{
			atkCo = StartCoroutine( AttackCo(2) );
			shawSound.Play();
		}
		// attack front
		else
			atkCo = StartCoroutine( AttackCo() );
	}

	IEnumerator AttackCo(int atkDir=0)
	{
		if (atkCo != null) yield break;
		this.atkDir = atkDir;

		if (slashObj != null)
		{
			anim.speed = 1;
			anim.SetFloat("atkDir", atkDir);
			anim.SetTrigger("attack");
			anim.SetBool("isAttacking", true);

			yield return new WaitForSeconds(atkDir != 2 ? 0.25f : 0.5f);
			anim.SetBool("isAttacking", false);
		}

		// atk cooldown
		yield return new WaitForSeconds(atkCooldownDuration);
		atkCo = null;
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
		isDashing = false;
		jumpDashed = false;

		moveDir = model.localScale.x;
		activeMoveSpeed = moveSpeed;
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
		anim.SetBool("isWalking", moveX != 0);
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
				if (moveX != 0)
					anim.speed = Mathf.Abs(moveX * activeMoveSpeed);
				else 
					anim.speed = 1;
			}
			rb.velocity = new Vector2(moveX * activeMoveSpeed, rb.velocity.y);
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

	void FullRestore()
	{
		hp = maxHp;
		SetHp();
	}


	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Enemy") && hurtCo == null)	
			hurtCo = StartCoroutine( TakeDamageCo(other.transform) );
	}

	IEnumerator TakeDamageCo(Transform opponent)
	{
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = dmgMat;

		hp = Mathf.Max(0, hp - 1);
		anim.SetBool("isHurt", true);
		ResetAllBools();
		beenHurt = true;
		SetHp();
		rb.velocity = Vector2.zero;

		Vector2 direction = (opponent.position - transform.position).normalized;
        rb.velocity = new Vector2(-direction.x * 10, 5);
		CinemachineShake.Instance.ShakeCam(25, 0.25f);

		// stop healing
		if (bindCo != null) 
			StopCoroutine(bindCo);
		bindCo = null;

		// yield return new WaitForSeconds(0.1f);
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
		SetSilk(-bindCost);

		yield return new WaitForSeconds(0.25f);
		hp = Mathf.Min(hp+3, hpMasks.Length);
		SetHp();
		bindCo = null;
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
		silkMeter = Mathf.Min(silkMeter + addToSilk, silks.Length);

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
