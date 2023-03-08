using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControls : MonoBehaviour
{
	private int playerId;
	private Rewired.Player player;
	[SerializeField] Animator anim;

	[Header("Platformer")]
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Transform model;
	[SerializeField] float moveSpeed=5;
	[SerializeField] float jumpDashForce=10;
	[SerializeField] float jumpForce=10;
	[SerializeField] Vector2 wallJumpForce;
	private float moveX;
	private float moveDir;
	// private float moveY;
	private bool jumped;
	private bool jumpDashed;
	private bool isDashing;
	private bool isJumping;
	private bool isWallJumping;
	private bool isGrounded;
	private bool isWallSliding;
	private float jumpTimer;
	private bool hasLedge;
	private bool hasWall;
	private bool canLedgeGrab;
	[SerializeField] bool isLedgeGrabbing; // controlled by animator
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

	[Space] [Header("Particle effects")]
	[SerializeField] GameObject dashEffectL;
	[SerializeField] GameObject dashEffectR;
	[SerializeField] Transform dashSpawnPos;
	

	
	// Start is called before the first frame update
	void Start()
	{
		player = ReInput.players.GetPlayer(playerId);
		activeMoveSpeed = moveSpeed;
	}

	private void OnDrawGizmosSelected() 
	{
		// if (groundCheck != null)
		// 	Gizmos.DrawCube(groundCheck.position, groundCheckSize);
		// if (wallCheck != null)
		// 	Gizmos.DrawCube(wallCheck.position, wallCheckSize);
		if (ledgeCheckPos != null)
			Gizmos.DrawLine(ledgeCheckPos.position, ledgeCheckPos.position + new Vector3(model.localScale.x * ledgeGrabDist, 0));
		if (wallCheckPos != null)
			Gizmos.DrawLine(wallCheckPos.position, wallCheckPos.position + new Vector3(model.localScale.x * ledgeGrabDist, 0));
	}

	// Update is called once per frame
	void Update()
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

		else if (isWallSliding && player.GetButtonDown("B"))
		{
			WallJump();
		}

		// dash
		if (player.GetButtonDown("ZR") && dashCounter <= 0 && 
			dashCooldownCounter <= 0)
		{
			isDashing = true; // keep dashing if on ground
				
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

		CalcMove();
	}

	void FixedUpdate()
	{
		if (canLedgeGrab && !isLedgeGrabbing)
			LedgeGrab();

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
		CheckCanLedgeGrab();
		if (jumpDashed && jumped && (isGrounded || isWallSliding))
			jumpDashed = jumped = false;
	}


	void CalcMove()
	{
		moveX = player.GetAxis("Move Horizontal");
	}

	public void CheckIsGrounded()
	{
		isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);
		anim.SetBool("isGrounded", isGrounded);
	}
	public void CheckIsWalled()
	{
		isWallSliding = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, whatIsGround) && !isGrounded && moveX != 0;
		anim.SetBool("isWallSliding", isWallSliding);

		if (isWallSliding) 
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
	}
	void CheckCanLedgeGrab()
	{
		hasLedge = Physics2D.Raycast(ledgeCheckPos.position, new Vector2(moveDir, 0), ledgeGrabDist, whatIsGround);
		hasWall = Physics2D.Raycast(wallCheckPos.position, new Vector2(moveDir, 0), ledgeGrabDist, whatIsGround);
		canLedgeGrab = (!hasLedge && hasWall && !isGrounded);
	}


	void Jump()
	{
		// jumped while dashing
		if (isDashing)
		{
			moveDir = model.localScale.x; // -1 or 1
			jumpDashed = true;
			Invoke("JumpDashed", 0.25f);
			anim.SetTrigger("jump");
		}
		else
			isDashing = false;

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
		anim.SetTrigger("ledgeGrab");
	}
	public void SET_LEDGE_GRAB(bool on)
	{
		isLedgeGrabbing = on;
	}

	void Move()
	{
		if (jumpDashed)
			rb.velocity = new Vector2(moveDir * jumpDashForce, rb.velocity.y);
		if (isWallJumping || jumpDashed) return;
		anim.SetBool("isWalking", moveX != 0);
		anim.SetBool("isDashing", isDashing);

		// right
		if (moveX > 0) 
			model.localScale = new Vector3(1, 1, 1);
		// left
		else if (moveX < 0) 
			model.localScale = new Vector3(-1, 1, 1);

			
		// Controlled movement
		if (!isDashing)
		{
			if (moveX != 0)
				anim.speed = Mathf.Abs(moveX * activeMoveSpeed);
			else 
				anim.speed = 1;
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
			// Debug.Log($"speed={rb.velocity.x}");
		}
	}

}
