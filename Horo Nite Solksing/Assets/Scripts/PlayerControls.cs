using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;
using ED.SC;
using TMPro;
// using SmartConsole;

public class PlayerControls : MonoBehaviour
{
	private int playerId;
	private Rewired.Player player;
	public Transform self {get; private set;}
	[SerializeField] Animator anim;
	public static PlayerControls Instance;


	[Space] [Header("Status")]
	[SerializeField] int maxHp=7;
	[SerializeField] int hp;
	public int[] atkDmg={10,8,15,10};
	public int gossamerDmg=5;
	public int stabDmg=30;
	public int rushDmg=20;
	[SerializeField] int silkMeter;
	[SerializeField] Animator[] silks;
	[SerializeField] GameObject[] hpMasks;
	[SerializeField] GameObject silkGlowNorm;
	[SerializeField] GameObject silkGlowHarp;
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
	[SerializeField] float risingForce=10;
	[SerializeField] Vector2 wallJumpForce;
	private float moveX;
	private float dashDir;
	private float moveDir;
	private float atkDir;
	private float skillDir; // 0 = stab, 1 = rush, else = storm
	private bool isTool1=true;
	// private float moveY;
	private bool jumped;
	private bool jumpDashed;
	private bool isDashing;
	private bool isJumping;
	private bool isWallJumping;
	[SerializeField] bool isGrounded;
	private bool isPogoing;
	private bool isPaused;
	private bool isWallSliding;
	private float jumpTimer;
	private bool canLedgeGrab;
	private bool ledgeGrab;
	private bool noControl;
	public bool isResting {get; private set;}
	[SerializeField] GameObject needToRestObj;
	private bool inStunLock;
	public bool justParried {get; private set;}
	[SerializeField] bool isInvincible;
	[SerializeField] bool hasLedge;
	[SerializeField] bool hasWall;
	[SerializeField] bool receivingKb;

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

	[Space] [SerializeField] Transform ceilingCheck;
	[SerializeField] Vector2 ceilingCheckSize;

	[Space] [SerializeField] float activeMoveSpeed;
	[SerializeField] float dashSpeed=10;
	[SerializeField] float dashBurstSpeed=15;
	[SerializeField] float dashDuration=0.25f;
	[SerializeField] float dashCooldownDuration=0.25f;
	[SerializeField] float dashCounter;
	[SerializeField] float dashCooldownCounter;
	private bool airDashed;
	[SerializeField] float shawForce=7.5f;
	[SerializeField] float nShaw;
	[SerializeField] float shawLimit=3;
	[SerializeField] float recoilForce=7.5f;
	[SerializeField] float risingRecoilForce=5f;
	[SerializeField] float quickAtkCooldownDuration=0.1f;
	[SerializeField] float atkCooldownDuration=0.2f;
	[SerializeField] float toolCooldownDuration=0.2f;
	[SerializeField] float rushSpeed=12.5f;

	[Space] [SerializeField] GameObject slashObj;
	private bool atk1;
	private Coroutine atkCo;
	private Coroutine toolCo;
	private Coroutine hurtCo;
	private Coroutine stunLockCo;
	private Coroutine bindCo;
	private Coroutine parryCo;
	private Coroutine pauseCo;
	[SerializeField] int bindCost=9;
	[SerializeField] int harpBindCost=6;
	[SerializeField] int skillStabCost=2;
	[SerializeField] int skillGossamerCost=2;
	private bool usingSkill;
	[SerializeField] Image spoolImg;
	[SerializeField] Sprite fullSpoolSpr;
	[SerializeField] Sprite emptySpoolSpr;
	[SerializeField] GameObject cacoonObj;
	[SerializeField] GameObject deathAnimObj;


	[Space] [Header("Sound effects")]
	[SerializeField] AudioSource shawSound;
	[SerializeField] AudioSource agaleSound;
	[SerializeField] AudioSource adimaSound;
	[SerializeField] AudioSource gitGudSound;


	[Space] [Header("Particle effects")]
	[SerializeField] GameObject dashEffectL;
	[SerializeField] GameObject dashEffectR;
	[SerializeField] GameObject healingPs;
	[SerializeField] GameObject bloodBurstPs;
	[SerializeField] GameObject bindPs;
	[SerializeField] ParticleSystem soulLeakPs;
	[SerializeField] ParticleSystem soulLeakShortPs;
	[SerializeField] Animator animeLinesAnim;
	[SerializeField] Transform dashSpawnPos;
	[SerializeField] Animator flashAnim;


	[Space] [Header("Animator Controlled")]
	[SerializeField] bool isLedgeGrabbing; // controlled by animator
	[SerializeField] bool inAnimation;
	[SerializeField] bool inRushSkill;
	[SerializeField] bool inInvincibleAnim;
	[SerializeField] bool performingGossamerStorm;
	[SerializeField] bool cantRotate;
	private bool cantRotate2;
	[SerializeField] bool inAirDash;
	[SerializeField] bool inAtkState;
	[SerializeField] bool inShawAtk;
	[SerializeField] bool isBinding;
	[SerializeField] bool downwardStrike;
	[SerializeField] bool dashStrike;
	[SerializeField] bool beenHurt;
	[SerializeField] bool risingAtk;
	private bool holdingRiseButton;
	private bool isDead=false;
	public bool isFinished=false;
	private float finTime;
	private bool beaten;


	[Space] [Header("In-Game Related")]
	[SerializeField] Bench bench;


	[Space] [Header("Tools")]
	[SerializeField] Transform toolSummonPos;
	[SerializeField] Tool[] tools;
	[SerializeField] Tool tool1;
	[SerializeField] Tool tool2;
	private int nToolUses1;
	private int nToolUses2;
	private float nToolSlowUses1;
	private float nToolSlowUses2;
	[Space] [SerializeField] Image toolUses1; // progress bar
	[SerializeField] Image toolUses2; // progress bar
	

	[Space] [SerializeField] bool hasShield;
	[SerializeField] GameObject shieldObj;
	[SerializeField] Image shieldImg;
	[SerializeField] Sprite[] shieldSprs;
	private int shieldHp;
	[SerializeField] bool hasExtraSpool;
	[SerializeField] GameObject normSpoolObj;
	[SerializeField] GameObject extraSpoolObj;
	[SerializeField] GameObject normHarpistSpoolObj;
	[SerializeField] GameObject extraHarpistSpoolObj;


	[Space] [Header("Ui")]
	[SerializeField] Animator transitionAnim;

	[Space] [SerializeField] GameObject pauseMenu;
	[SerializeField] Animator pauseAnim;
	[SerializeField] CanvasGroup pauseMenuUi;

	[Space] [SerializeField] GameObject pause2Menu;
	[SerializeField] Animator pause2Anim;
	[SerializeField] CanvasGroup pause2MenuUi;

	[Space] [SerializeField] Image[] toolIcons;
	[SerializeField] Image[] toolsEquipped;
	[SerializeField] Sprite emptySpr;
	private int nEquipped;



	[System.Serializable] public class Crest 
	{
		public string title;
		public GameObject[] objs;

		public void ToggleCrest(bool value)
		{
			foreach (GameObject obj in objs) obj.SetActive(value);
		}
	}

	[Space] [Header("Crests")]
	[SerializeField] Crest[] crests;
	[SerializeField] Image[] crestIcons;
	public int crestNum;
	[SerializeField] GameObject shawEffectObj;
	[SerializeField] GameObject hunterSpinObj;


	private bool invulnerable;
	private float nextSceneSpeed;
	private bool movingToNextScene;
	private bool canMove;
	private bool movingRight;


	[Space] [Header("Debug")]
	[SerializeField] [Range(1,10)] int silkMultiplier=1;
	[SerializeField] bool invincible;
	[SerializeField] bool infiniteSilk;
	[SerializeField] bool canParry=true;
	[SerializeField] [Range(-1,0)] float shawDir=-0.7f;
	[SerializeField] string savedScene="Scene1";
	[SerializeField] Vector2 savedPos;
	[SerializeField] string deathScene;
	[SerializeField] Vector2 deathPos;
	private bool timeStarted;
	public bool isCountingTime;
	// [SerializeField] TimeSpan timeSpan;
	[SerializeField] float timePlayed;
	[SerializeField] TextMeshProUGUI timePlayedTxt;
	[SerializeField] TextMeshProUGUI finalTimePlayedTxt;
	[SerializeField] GameObject replayObj;
	[SerializeField] Rewired.Integration.UnityUI.RewiredStandaloneInputModule rinput;
	// public static PlayerControls Instance;
	private int nKilled=0;

	float t;
	float t1;
	float t2;
    Vector3 startPosition;
	private Transform stunLockPos;
    float stunLockTime=0.5f;


	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		self = transform;
		tools = new Tool[1];
	}


	// Start is called before the first frame update
	void Start()
	{
		transitionAnim.SetTrigger("fromBlack");
		savedScene = SceneManager.GetActiveScene().name;
		savedPos = self.position;

		player = ReInput.players.GetPlayer(playerId);

		activeMoveSpeed = moveSpeed;
		bindPs.transform.parent = null;
		healingPs.transform.parent = null;
		bloodBurstPs.transform.parent = null;
		cacoonObj.transform.parent = null;

		DontDestroyOnLoad(cacoonObj);
		FullRestore(); // starting
		// Screen.SetResolution((int) (16f/9f * Screen.height), Screen.height, true);

		if (pauseMenu != null) pauseMenu.SetActive(false);
		if (pause2Menu != null) pause2Menu.SetActive(false);
		
		// var controller = player.controllers.GetFirstControllerWithTemplate<IGamepadTemplate>();
		// IGamepadTemplate gamepad = controller.GetTemplate<IGamepadTemplate>();

		// // Get the first Controller assigned to the Player that implements Gamepad Template
		// var controller = player.controllers.GetFirstControllerWithTemplate<IGamepadTemplate>();

		// // Get the Gamepad Template from the Controller
		// var gamepad = controller.GetTemplate<IGamepadTemplate>();

		// // Get a list of all Controller Templates of a particular type in all Controllers found on the system
		// var gamepads = ReInput.controllers.GetControllerTemplates<IGamepadTemplate>();

		// // Iterate through all Controller Templates implemented by a Controller
		// for(int i = 0; i < controller.templateCount; i++) {
		// 	Debug.Log(controller.name + " implements the " + controller.Templates[i].name + " Template.");
		// }    
	}

	bool CanControl()
	{
		return (!isLedgeGrabbing && !ledgeGrab && !beenHurt && !noControl && !isBinding &&
			!inAnimation && !isDead && !inStunLock && !isResting && !isPaused && !inRushSkill && !isFinished);
	}

	public void Unpause()
	{
		if (pauseCo != null)
			StopCoroutine(pauseCo);
		pauseCo = StartCoroutine( UnpauseCo() );
	}

	IEnumerator UnpauseCo()
	{
		yield return null;
		yield return null;
		isPaused = false;
		pauseCo = null;
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

	private bool replaying;

	// Update is called once per frame
	void Update()
	{
		if (isFinished && !replaying)
		{
			if (finTime < 3.25f)
				finTime += Time.unscaledDeltaTime;
			else if (player.GetAnyButton())
			{
				finalTimePlayedTxt.gameObject.SetActive(false);
				replaying = true;
				finTime = 0;
				StartCoroutine( DiedCo(false) );
			}
		}

		// starting timer
		if (!timeStarted && player.GetAnyButton())
		{
			timeStarted = true;
			isCountingTime = true;
		}
		// timer going
		if (!beaten && isCountingTime)
		{
			timePlayed += Time.unscaledDeltaTime;
			TimeSpan time = TimeSpan.FromSeconds(timePlayed);
			timePlayedTxt.text = time.ToString(@"mm\:ss\.ff");
			// float now = Time.realtimeSinceStartup;
			// int mins = (int) (timePlayed / 60f);
			// float secs = timePlayed % 60;
			// string secTxt = secs < 10 ? "0" + secs.ToString("F2") : secs.ToString("F2");
			// timePlayedTxt.text = $"{mins}:{secTxt}";
		}

		// inventory open
		if (!isPaused && pauseAnim != null && player.GetButtonDown("Minus"))
		{
			isPaused = true;
			pauseMenu.SetActive(true);
			pauseAnim.SetTrigger("open");
		}
		// pause open
		else if (!isPaused && pause2Anim != null && player.GetButtonDown("Start"))
		{
			isPaused = true;
			pause2Menu.SetActive(true);
			pause2Anim.SetTrigger("open");
		}
		// inventory close
		else if (isPaused)
		{
			// close inventory
			if (pauseMenuUi.gameObject.activeInHierarchy && pauseMenuUi.interactable 
				&& (player.GetButtonDown("No") || player.GetButtonDown("Minus")))
				pauseAnim.SetTrigger("close");
			// close Pause
			if (pause2MenuUi.gameObject.activeInHierarchy && pause2MenuUi.interactable 
				&& (player.GetButtonDown("No") || player.GetButtonDown("Start")))
				pause2Anim.SetTrigger("close");
		}
		// basic movement
		else if (CanControl() && !inShawAtk)
		{
			if (!inAirDash)
			{
				if (player.GetButtonDown("Attack") && atkCo == null)
					Attack();
				else if (player.GetButtonDown("Skill"))
					SkillAttack();

				// bind (heal)
				else if (player.GetButtonDown("Bind") && (infiniteSilk || silkMeter >= GetBindCost()) && bindCo == null)
					bindCo = StartCoroutine( BindCo() );

				// tools
				else if (player.GetButtonDown("Tool") && toolCo == null)
				{
					// int tool = (player.GetAxis("Move Vertical") < -0.7f ? 1 : 0);
					int tool = 0;
					if (tool == 0 && nToolUses1 > 0)
						toolCo = StartCoroutine( UseTool(0) );
					// else if (tool == 1 && nToolUses2 > 0)
					// 	toolCo = StartCoroutine( UseTool(1) );
				}

				// rest on bench
				else if (bench != null && isGrounded && player.GetAxis("Move Vertical") > 0.7f)
				{
					isResting = true;
					needToRestObj.SetActive(false);
					isDashing = jumpDashed = false;
					rb.gravityScale = 0;
					rb.velocity = Vector2.zero;
					anim.SetBool("isResting", true);
					startPosition = transform.position;
					// FullRestore(true); // rest
				}
			}

			// jump
			JumpMechanic();

			// dash
			DashMechanic();

			CalcMove();
			if (atkCo == null)
				CheckCanLedgeGrab();
			// Ledge Grab
			if (canLedgeGrab && !isWallJumping && !isLedgeGrabbing && !ledgeGrab)
				LedgeGrab();
		}
		else if (isResting && 
			(player.GetButtonDown("No") || player.GetAxis("Move Vertical") < -0.7f
			|| player.GetAxis("Move Horizontal") < -0.7f || player.GetAxis("Move Horizontal") > 0.7f)
		)
		{
			t = 0;
			isResting = false;
			needToRestObj.SetActive(true);
			activeMoveSpeed = moveSpeed;
			rb.gravityScale = 1;
			rb.velocity = Vector2.zero;
			anim.SetBool("isResting", false);
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
					hunterSpinObj.SetActive(false);
				}
			}
			else
			{
				if (!isGrounded)
					anim.SetFloat("jumpVelocity", rb.velocity.y);

				// Dash
				CalcDash();

				if (!inAirDash)
					Move();
				else
					rb.velocity = new Vector2(model.localScale.x * dashSpeed * 0.9f, rb.velocity.y);
				CheckIsGrounded();
				CheckIsWalled();
				if (jumpDashed && jumped && (isGrounded || isWallSliding || canLedgeGrab))
					jumpDashed = jumped = false;
			}
		}
		else if (inRushSkill)
		{
			rb.velocity = new Vector2(model.localScale.x * rushSpeed, 0);
		}
		else if (!isDead && inStunLock)
		{
			t += Time.fixedDeltaTime/stunLockTime;
			transform.position = Vector3.Lerp(startPosition, stunLockPos.position, t);
		}
		else if (!isDead && bench != null && isResting && t < 1)
		{
			anim.SetFloat("restTime", t);
			t += Time.fixedDeltaTime/stunLockTime;
			transform.position = Vector3.Lerp(startPosition, bench.restPos.position, t);
		}
		if (toolUses1 != null && tool1 != null)
		{
			if (nToolSlowUses1 != nToolUses1)
			{
				nToolSlowUses1 = Mathf.Lerp(nToolSlowUses1, nToolUses1, t1);
				t1 += 0.5f * Time.fixedDeltaTime;
				toolUses1.fillAmount = nToolSlowUses1/tool1.totaluses;
			}
			else
			{
				t1 = 0;
			}
		}
		if (toolUses2 != null && tool2 != null)
		{
			if (nToolSlowUses2 != nToolUses2)
			{
				nToolSlowUses2 = Mathf.Lerp(nToolSlowUses2, nToolUses2, t2);
				t2 += 0.5f * Time.fixedDeltaTime;
				toolUses2.fillAmount = nToolSlowUses2/tool2.totaluses;
			}
			else
			{
				t2 = 0;
			}
		}
	}

	void DashMechanic()
	{
		// First frame of pressing dash button
		if (player.GetButtonDown("Dash") && dashCounter <= 0 && 
			dashCooldownCounter <= 0)
		{
			isDashing = true; // keep dashing if on ground
			jumpDashed = jumped = false;
			jumpTimer = jumpMaxTimer;

			if (!isGrounded)
			{
				anim.SetBool("isAirDash", true);
				airDashed = true;
			}
				
			dashCounter = dashDuration;
			activeMoveSpeed = dashBurstSpeed;
			anim.SetFloat("moveSpeed", dashBurstSpeed);

			if (model.transform.localScale.x > 0)
				Instantiate(dashEffectL, dashSpawnPos.position, dashEffectL.transform.rotation, null);
			else
				Instantiate(dashEffectR, dashSpawnPos.position, dashEffectR.transform.rotation, null);
		}
		// First frame of finishing dash
		if (isDashing && player.GetButtonUp("Dash") && dashCounter <= 0)
		{
			CancelDash();
		}
	}

	void CalcDash()
	{
		if (dashCounter > 0)
		{
			dashCounter -= Time.fixedDeltaTime;
			if (!isGrounded) 
				rb.velocity = new Vector2(rb.velocity.x, -0.5f);

			if (dashCounter <= 0)
			{
				// if (!isGrounded || !player.GetButton("Dash") || risingAtk) 
				if (!player.GetButton("Dash") || risingAtk) 
					isDashing = false;
				activeMoveSpeed = (isDashing) ? dashSpeed : moveSpeed;
				anim.SetFloat("moveSpeed", activeMoveSpeed);
				dashCooldownCounter = dashCooldownDuration;
			}
		}
		if (!airDashed && !isDashing && dashCooldownCounter > 0)
		{
			dashCooldownCounter -= Time.fixedDeltaTime;
		}
	}

	public void CANCEL_DASH()
	{
		anim.SetBool("isDashing", false);
		jumpDashed = false;
		CancelDash();
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
		if (isGrounded && !isJumping && player.GetButtonDown("Jump"))
		{
			Jump();
		}
		// Released jump button
		else if (player.GetButtonUp("Jump") || CheckIsCeiling())
		{
			isJumping = false;
		}
		// Holding jump button
		else if (isJumping && player.GetButton("Jump"))
		{
			if (!usingSkill && jumpTimer < jumpMaxTimer)
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
		else if (isWallSliding && player.GetButtonDown("Jump"))
		{
			WallJump();
		}
	}

	void ResetAllBools()
	{
		isJumping = jumpDashed = jumped = false;
		airDashed = isDashing = false;
		canLedgeGrab = ledgeGrab = false;
		isWallSliding = false;
		isWallJumping = false;
		rb.gravityScale = 1;
		activeMoveSpeed = moveSpeed;
		bindCo = null;
		noControl = false;
		anim.SetBool("isAirDash", false);
	}

	void CalcMove()
	{
		float temp = player.GetAxis("Move Horizontal");
		moveX = (temp < 0.3f && temp > -0.3f) ? 0 : temp;
	}

	void Move()
	{
		if (movingToNextScene)
		{
			if (canMove)
			{
				rb.velocity = new Vector2(
					nextSceneSpeed * activeMoveSpeed, 
					rb.velocity.y
				);
			}
			else
				rb.velocity = new Vector2(0, rb.velocity.y);
			return;
		}

		if (jumpDashed)
			rb.velocity = new Vector2(dashDir * jumpDashForce, rb.velocity.y);
			
		if (isWallJumping || jumpDashed || isLedgeGrabbing || inShawAtk) return;

		float moveY = player.GetAxis("Move Vertical");
		float x = (isGrounded && moveY > 0.8f) ? 0 : moveX;
		anim.SetBool("isWalking", x != 0);
		if (!risingAtk)
			anim.SetBool("isDashing", isDashing);
		else
			anim.SetBool("isDashing", false);

		if (!cantRotate && !cantRotate2)
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
				anim.SetFloat("moveSpeed", x != 0 ? x * activeMoveSpeed : 1);
			}
			rb.velocity = new Vector2(x * activeMoveSpeed, rb.velocity.y);
		}
		// dashing
		else
		{
			bool facingRight = (model.localScale.x > 0);
			rb.AddForce(
				new Vector2((facingRight ? 1 : -1) * activeMoveSpeed * 5 * (risingAtk ? 0.5f : 1), 0), 
				ForceMode2D.Force
			);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -dashSpeed * (risingAtk ? 0.5f : 1), dashSpeed * (risingAtk ? 0.5f : 1)), 
				rb.velocity.y
			);
		}
	}

	void Flip()
	{
		// right
		if (moveX < 0) 
			model.localScale = new Vector3(1, 1, 1);
		// left
		else if (moveX > 0) 
			model.localScale = new Vector3(-1, 1, 1);
	}

	void CheckIsGrounded()
	{
		isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);
		if (isGrounded && nShaw != 0)
			nShaw = 0;
		if (isGrounded && isPogoing)
		{
			isPogoing = false;
			anim.SetFloat("pogoing", 0);
		}
		if (isGrounded && airDashed)
		{
			airDashed = false;
			dashCooldownCounter = 0;
		}
		if (!inAtkState) 
			anim.SetBool("isGrounded", isGrounded);
		if (isGrounded && anim.GetBool("isAirDash")) 
			anim.SetBool("isAirDash", false);
	}
	void CheckIsWalled()
	{
		isWallSliding = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, whatIsGround) && !isGrounded && moveX != 0;
		anim.SetBool("isWallSliding", isWallSliding);

		if (isWallSliding) 
		{
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
			if (isPogoing && nShaw < shawLimit)
			{
				isPogoing = false;
				anim.SetFloat("pogoing", 0);
			}
			if (airDashed)
			{
				airDashed = false;
				dashCooldownCounter = 0;
			}
		}
	}
	bool CheckIsCeiling()
	{
		return (!isGrounded && Physics2D.OverlapBox(ceilingCheck.position, ceilingCheckSize, 0, whatIsGround));
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
		// dash atk
		if (isDashing && crestNum <= 1)
			atkCo = StartCoroutine( AttackCo(3) );
		// attack up
		else if (player.GetAxis("Move Vertical") > 0.7f)
			atkCo = StartCoroutine( AttackCo(1) );
		// shaw attack
		else if (player.GetAxis("Move Vertical") < shawDir && nShaw < shawLimit)
			atkCo = StartCoroutine( AttackCo(2) );
		// attack front
		else
			atkCo = StartCoroutine( AttackCo(0) );
	}

	IEnumerator AttackCo(int atkDir=0)
	{
		if (atkCo != null) yield break;

		// cannot shaw on the ground
		if (atkDir == 2 && isGrounded)
			atkDir = 0;
		this.atkDir = atkDir;

		// not rising attack
		if (crestNum <= 1 && atkDir != 1)
			CancelDash();
		// rising attack (aerial)
		else 
		{
			if (crestNum > 1 && atkDir == 1 && !isGrounded)
				anim.SetFloat("crestNum",0);
		}
		
		if (isDashing && crestNum > 1 && isGrounded)
			atkDir = 1;

		if (slashObj != null)
		{
			if (isWallSliding)
				Flip();
			anim.SetFloat("atkDir", atkDir);
			anim.SetTrigger("attack");
			anim.SetBool("isAttacking", true);

			if (crestNum > 1 && atkDir == 1)
			{
				yield return new WaitForSeconds(0.167f);
				
			}
			// shaw attack
			if (atkDir == 2)
			{
				shawSound.Play();
				if (crestNum != 1)
				{
					nShaw++;
					if (nShaw >= shawLimit)
					{
						anim.SetFloat("pogoing", 1);
						isPogoing = true;
					}
					jumpDashed = false;
					rb.velocity = Vector2.zero;
					rb.gravityScale = 0;
					yield return new WaitForSeconds(0.1f);
					rb.gravityScale = 1;
				}
			}
			// normal slash
			else
			{
				MusicManager.Instance.PlayHornetAtkSfx(atk1);
				atk1 = !atk1;
			}

			if (crestNum == 1)
				yield return new WaitForSeconds(0.25f);
			else
				yield return new WaitForSeconds(atkDir != 2 ? 0.25f : 0.4f);
			anim.SetBool("isAttacking", false);
			anim.SetFloat("crestNum", crestNum);
		}

		// atk cooldown
		yield return new WaitForSeconds(
			(crestNum > 1 && atkDir == 1) ? quickAtkCooldownDuration : atkCooldownDuration
		);
		atkCo = null;
	}
	
	void SkillAttack()
	{
		// Gossamer Storm
		// if (player.GetAxis("Move Vertical") > 0.7f && (infiniteSilk || silkMeter >= skillStabCost))
		// 	atkCo = StartCoroutine( SkillAttackCo(1) );
		// Stabby stabby strike
		if ((infiniteSilk || silkMeter >= skillStabCost))
			atkCo = StartCoroutine( SkillAttackCo() );
	}

	IEnumerator SkillAttackCo()
	{
		anim.SetBool("isAttacking", false);
		usingSkill = true;

		// this.atkDir = atkDir;
		// this.skillDir = atkDir;
		CancelDash();
		jumpDashed = false;
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;

		if (skillDir == 0)
		{
			if (!infiniteSilk) SetSilk(-skillStabCost);
			anim.SetBool("isSkillAttacking", true);
			anim.SetFloat("skillDir", skillDir);
			adimaSound.Play();

			yield return new WaitForSeconds(0.25f);
			SkillAttackEffect();

			yield return new WaitForSeconds(0.25f);
			anim.SetBool("isSkillAttacking", false);
			usingSkill = false;
			rb.gravityScale = 1;

			atkCo = null;
		}
		// Rush
		else if (skillDir == 1)
		{
			if (!infiniteSilk) SetSilk(-skillGossamerCost);
			anim.SetBool("isSkillAttacking", true);
			anim.SetFloat("skillDir", skillDir);
			// adimaSound.Play();

			yield return new WaitForSeconds(0.1666f);
			SkillAttackEffect();

			yield return new WaitForSeconds(0.25f);
			anim.SetBool("isSkillAttacking", false);
			usingSkill = false;
			rb.gravityScale = 1;

			atkCo = null;
		}
		// Gossamer Storm
		else
		{
			if (!infiniteSilk) SetSilk(-skillGossamerCost);
			anim.SetBool("isGossamerStorm", true);

			yield return new WaitForSeconds(0.25f);
			agaleSound.Play();
			SkillAttackEffect();
		}
	}

	void SkillAttackEffect()
	{
		if (!beenHurt)
		{
			CinemachineShake.Instance.ShakeCam(5, 0.5f);
			flashAnim.SetTrigger("flash");
		}
	}

	public void CancelGossamerStorm()
	{
		// done
		if (!player.GetButton("Skill") || (!infiniteSilk && silkMeter <= 0))
		{
			anim.SetBool("isGossamerStorm", false);
			rb.gravityScale = 1;
			atkCo = null;
			usingSkill = false;
		}
		// Continue Performing Gossamer Storm
		else if (!infiniteSilk) 
		{
			SetSilk(-1);
		}
	}

	IEnumerator UseTool(int tool=0)
	{
		if (toolCo != null) yield break;
		isTool1 = (tool == 0);
		if (isTool1) nToolUses1--;
		else nToolUses2--;

		CancelDash();
		atkDir = 4;

		anim.SetFloat("atkDir", atkDir);
		anim.SetTrigger("attack");
		anim.SetBool("isAttacking", true);
		MusicManager.Instance.PlayHornetAtkSfx(atk1);

		yield return new WaitForSeconds(0.25f);
		anim.SetBool("isAttacking", false);

		// atk cooldown
		yield return new WaitForSeconds(toolCooldownDuration);
		toolCo = null;
	}

	public void USE_TOOL()
	{
		var tool = Instantiate( 
			isTool1 || tool2 == null ? tool1 : tool2, 
			toolSummonPos.position, 
			Quaternion.identity
		);
		tool.toRight = model.localScale.x > 0 ? true : false;

		// caltrops
		if (tool.isMultiple)
		{
			for (int i=1; i<tool.nCopies ; i++)
			{
				var toolCopy = Instantiate( 
					isTool1 || tool2 == null ? tool1 : tool2, 
					toolSummonPos.position, 
					Quaternion.identity
				);
				toolCopy.velocityMultiplier = UnityEngine.Random.Range(0.7f,1.3f);
				toolCopy.toRight = model.localScale.x > 0 ? true : false;
			}
		}
	}

	public void EquipSkill(int n)
	{
		// no change
		if (skillDir == n) return;

		skillDir = n;
		anim.SetFloat("skillDir", skillDir);
	}
	private Tool currTool;
	private UiSelectable currToolUi;
	public bool EquipTool(Tool tool, UiSelectable toolUi)
	{
		// equip new tool
		if (currTool != tool)
		{
			// unequip prev tool
			if (currTool != null)
			{
				toolsEquipped[0].sprite = emptySpr;
				toolIcons[0].sprite = emptySpr;
				tools[0] = null;
				tool1 = null;
				nEquipped--;
				FullRestore();
			}
			if (currToolUi != null)
				currToolUi.UNEQUIP_SKILL();
			currToolUi = toolUi;
			currTool = tool;
			toolsEquipped[0].sprite = tool.icon;
			toolIcons[0].sprite = tool.icon;
			tools[0] = tool;
			currTool = tool;
			tool1 = tool;
			nEquipped++;
			FullRestore();	// equipped
			return true;
		}
		currToolUi = null;
		currTool = null;
		toolsEquipped[0].sprite = emptySpr;
		toolIcons[0].sprite = emptySpr;
		tools[0] = null;
		tool1 = null;
		nEquipped--;
		FullRestore();
		return false;
	}
	public void EquipCrest(int n)
	{
		// no change
		if (crestNum == n) return;

		// new crest
		if (crestNum < crests.Length)
		{
			crests[crestNum].ToggleCrest(false);
			if (crestNum < crestIcons.Length) 
				crestIcons[crestNum].color = new Color(1,1,1,0.4f);

			crestNum = n;
			ChangeSpoolNotch();
			anim.SetFloat("bindSpeed", n > 1 ? 0.5f : 1);
			crests[crestNum].ToggleCrest(true);
			crestIcons[crestNum].color = new Color(1,1,1,1);
		}
		anim.SetFloat("crestNum", crestNum);
	}
	
	public bool EquipPassive(int n)
	{
		switch (n)
		{
			// shield
			case 0:
				hasShield = !hasShield;
				hasExtraSpool = false;
				if (hasShield)
				{
					shieldHp = 3;
					shieldImg.gameObject.SetActive(true);
					if (shieldImg != null && shieldHp < shieldSprs.Length) 
						shieldImg.sprite = shieldSprs[shieldHp];
				}
				else
				{
					shieldHp = 3;
					shieldImg.gameObject.SetActive(false);
				}
				ChangeSpoolNotch();
				return hasShield;
			// extended spool
			case 1:
				hasExtraSpool = !hasExtraSpool;
				hasShield = false;
				shieldImg.gameObject.SetActive(false);
				ChangeSpoolNotch();
				return hasExtraSpool;
			default:
				return false;
		}
	}
	void ChangeSpoolNotch()
	{
		extraHarpistSpoolObj.SetActive(false);
		extraSpoolObj.SetActive(false);
		normHarpistSpoolObj.SetActive(false);
		normSpoolObj.SetActive(false);

		if (crestNum != 1 && silkGlowNorm != null)
		{
			silkGlowHarp.SetActive(false);
			silkGlowNorm.SetActive(silkMeter >= 9);
		}
		else if (crestNum == 1 && silkGlowHarp != null)
		{
			silkGlowNorm.SetActive(false);
			silkGlowHarp.SetActive(silkMeter >= 6);
		}

		spoolImg.sprite = (silkMeter >= GetBindCost()) ? 
			fullSpoolSpr : emptySpoolSpr;

		if (hasExtraSpool)
		{
			// harpist
			if (crestNum == 1)
				extraHarpistSpoolObj.SetActive(true);
			// else
			else
				extraSpoolObj.SetActive(true);
		}
		else
		{
			// harpist
			if (crestNum == 1)
				normHarpistSpoolObj.SetActive(true);
			// else
			else
				normSpoolObj.SetActive(true);
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

	public void PROPEL_UP()
	{
		if (isGrounded)
		{
			rb.velocity = new Vector2(rb.velocity.x, risingForce);
			jumpDashed = false;
		}
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

	public void FinishAirDash()
	{
		anim.SetBool("isAirDash", false);
	}

	void LedgeGrab()
	{
		isJumping = jumped = false;
		anim.SetBool("isAirDash", false);
		CancelDash();
		jumpDashed = false;

		moveDir = model.localScale.x;
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;
		anim.SetFloat("moveDir", moveDir);
		anim.SetBool("isLedgeGrabbing", true);
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


	// todo --------------------------------------------------------------------

	void SpawnExistingObjAtSelf(GameObject obj)
	{
		obj.SetActive(false);
		obj.SetActive(true);
		obj.transform.position = self.position;
	}

	void FullRestore(bool clearShadowRealmList=false)
	{
		hp = maxHp;
		SetHp(true);

		if (clearShadowRealmList)
		{
			GameManager.Instance.ClearEnemiesDefeated();
			savedScene = SceneManager.GetActiveScene().name;
			savedPos = self.position;
		}

		if (hasShield)
			shieldHp = 3;
		if (shieldImg != null && shieldHp < shieldSprs.Length) 
			shieldImg.sprite = shieldSprs[shieldHp];

		if (tool1 != null && toolUses1 != null)
		{
			nToolUses1 = tool1.totaluses;
		}
		if (tool2 != null && toolUses2 != null)
		{
			nToolUses2 = tool2.totaluses;
		}
	}

	public void ShawRetreat(bool dashStrike=false)
	{
		switch (crestNum)
		{
			case 0:
				anim.SetBool("isAttacking", false);
				rb.velocity = Vector2.zero;
				rb.AddForce( new Vector2(-moveDir * shawForce, shawForce), ForceMode2D.Impulse);
				StartCoroutine( RegainControlCo(0.1f) );
				break;
			case 1:
				if (dashStrike)
				{
					anim.SetBool("isAttacking", false);
					rb.velocity = Vector2.zero;
					rb.AddForce( new Vector2(-moveDir * shawForce, shawForce), ForceMode2D.Impulse);
					StartCoroutine( RegainControlCo(0.1f) );
				}
				else
				{
					rb.velocity = new Vector2(rb.velocity.x , 0);
					rb.AddForce( new Vector2(0, shawForce), ForceMode2D.Impulse);
				}
				break;
			case 2:
				if (!hunterSpinObj.activeSelf)
					StartCoroutine( HUNTER_SPIN_ON() );
				anim.SetBool("isAttacking", false);
				rb.velocity = Vector2.zero;
				anim.SetFloat("pogoing", 1);
				isPogoing = true;
				rb.AddForce( new Vector2(0, shawForce), ForceMode2D.Impulse);
				// StartCoroutine( RegainControlCo(0.1f, false, true) );
				StartCoroutine( SwoopCo(0.2f) );
				// StartCoroutine( PogoCo(0.25f) );
				break;
			case 3:
				anim.SetBool("isAttacking", false);
				rb.velocity = Vector2.zero;
				anim.SetFloat("pogoing", 1);
				isPogoing = true;
				rb.AddForce( new Vector2(0, shawForce), ForceMode2D.Impulse);
				// StartCoroutine( RegainControlCo(0.1f, false, true) );
				StartCoroutine( SwoopCo(0.2f) );
				// StartCoroutine( PogoCo(0.25f) );
				shawEffectObj.SetActive(true);
				break;
		}
	}
	public void Recoil()
	{
		rb.velocity = Vector2.zero;
		isDashing = false;
		jumpDashed = false;

		rb.AddForce( new Vector2(-moveDir * recoilForce, 0), ForceMode2D.Impulse);
		StartCoroutine( RegainControlCo(0.1f) );
	}
	public void RisingAtkRetreat()
	{
		// anim.SetBool("isAttacking", false);
		rb.velocity = new Vector2(0, rb.velocity.y);

		rb.AddForce( new Vector2(-moveDir * risingRecoilForce, 0), ForceMode2D.Impulse);
		StartCoroutine( RegainControlCo(0.1f) );
	}

	IEnumerator RegainControlCo(float duration, bool noControl=true, bool invincibility=false)
	{
		if (invincibility) 
			isInvincible = true;
		if (noControl)
			this.noControl = true;

		yield return new WaitForSeconds(duration);
		if (invincibility) 
			isInvincible = false;
		if (noControl)
			this.noControl = false;
	}
	IEnumerator SwoopCo(float duration)
	{
		isInvincible = true;
		cantRotate2 = true;

		yield return new WaitForSeconds(duration);
		isInvincible = false;
		cantRotate2 = false;
		shawEffectObj.SetActive(false);
	}
	IEnumerator PogoCo(float duration)
	{
		isInvincible = true;
		noControl = true;
		
		yield return new WaitForSeconds(0.05f);
		rb.velocity = new Vector2(rb.velocity.x, 0);

		yield return new WaitForSeconds(0.05f);
		rb.velocity = new Vector2(rb.velocity.x, 0);
		rb.AddForce( new Vector2(0, shawForce), ForceMode2D.Impulse);
		noControl = false;

		yield return new WaitForSeconds(duration);
		isInvincible = false;
	}

	public IEnumerator HUNTER_SPIN_ON()
	{
		if (hunterSpinObj != null) hunterSpinObj.SetActive(true);
		yield return new WaitForSeconds(0.3f);
		if (hunterSpinObj != null) hunterSpinObj.SetActive(false);
	}

	private bool CanBeHurt()
	{
		return (!isDead && !invulnerable && !invincible && !justParried && !inInvincibleAnim);
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		if (CanBeHurt() && (other.CompareTag("Enemy") || other.CompareTag("EnemyAttack")) && hurtCo == null)
			hurtCo = StartCoroutine( TakeDamageCo(other.transform) );
		if (CanBeHurt() && other.CompareTag("EnemyStrongAttack") && hurtCo == null)
			hurtCo = StartCoroutine( TakeDamageCo(other.transform, 2) );
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (CanBeHurt() && other.CompareTag("EnemyStun"))
		{
			if (stunLockCo != null)
				StopCoroutine( stunLockCo );
			stunLockCo = StartCoroutine( StunLockCo(other.transform) );
		}	
		if (!isDead && other.CompareTag("Death") && hurtCo == null)
		{
			hurtCo = StartCoroutine( InstantDeathCo() );
		}
		if (other.CompareTag("Bench"))
		{
			bench = other.GetComponent<Bench>();
			t = 0;
		}
		if (!isDead && !movingToNextScene && other.CompareTag("NewArea"))
		{
			movingRight = (other.transform.position.x - self.position.x > 0);
			NewScene n = other.GetComponent<NewScene>();
			StartCoroutine( MoveToNextScene(n.newSceneName, n.newScenePos) );
		}
		if (!isFinished && other.CompareTag("EditorOnly"))
		{
			beaten = isFinished = true;
			transitionAnim.SetTrigger("toBlack");
			// GameManager.Instance.transitionAnim.SetTrigger("toBlack");
			isCountingTime = false;
			TimeSpan time = TimeSpan.FromSeconds(timePlayed);
			finalTimePlayedTxt.text = time.ToString(@"mm\:ss\.ff");
			timePlayedTxt.gameObject.SetActive(false);
			finalTimePlayedTxt.gameObject.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag("Bench"))
		{
			bench = null;
		}
	}

	IEnumerator TakeDamageCo(Transform opponent, int dmg=1)
	{
		if (isInvincible)
		{
			hurtCo = null;
			yield break;
		}
		MusicManager.Instance.PlayHurtSFX();
		MusicManager.Instance.SoftenBgMusic();
		if (hunterSpinObj != null) hunterSpinObj.SetActive(false);

		anim.SetBool("isHurt", true);
		ResetAllBools();
		atkCo = toolCo = null;
		beenHurt = true;

		for (int i=0 ; i<dmg ; i++)
		{
			// Take shield damage
			if (hasShield && hp == 1 && shieldHp > 0)
			{
				shieldHp = Mathf.Max(0, shieldHp - 1);
				if (shieldImg != null && shieldHp < shieldSprs.Length) 
					shieldImg.sprite = shieldSprs[shieldHp];
			}
			// Take damage
			else
			{
				hp = Mathf.Max(0, hp - 1);
			}
		}
		SetHp();
		rb.velocity = Vector2.zero;
		if (hp != 0)
		{
			CinemachineShake.Instance.ShakeCam(15, 0.25f);
			if (hp > 1 || (hasShield && hp == 1 && shieldHp > 0))
				animeLinesAnim.SetTrigger("dmg");
		}
		anim.SetBool("isSkillAttacking", false);
		anim.SetBool("isGossamerStorm", false);
		anim.SetBool("isBinding", false);
		usingSkill = false;
		// rb.gravityScale = 1;

		// stop healing
		if (bindCo != null) 
		{
			StopCoroutine(bindCo);
			bindCo = null;
			rb.gravityScale = 1;
		}
		// stop stun lock
		if (stunLockCo != null) 
		{
			StopCoroutine(stunLockCo);
			anim.SetBool("isStunLock", false);
			inStunLock = false;
			stunLockCo = null;
			rb.gravityScale = 1;
		}
		// stop stun lock
		if (isResting) 
		{
			t = 0;
			isResting = false;
			needToRestObj.SetActive(true);
			rb.gravityScale = 1;
			rb.velocity = Vector2.zero;
			anim.SetBool("isResting", false);
		}
		// stop parry effect
		if (parryCo != null)
			StopCoroutine(parryCo);

		// Died
		if (hp <= 0)
		{
			yield return DiedCo();
			yield break;
		}

		foreach (SpriteRenderer sprite in sprites)
			sprite.material = dmgMat;
		
		SpawnExistingObjAtSelf(bloodBurstPs);

		// Dramatic slow down
		Time.timeScale = 0;

		// teleport to ledge if in animation
		if (anim.GetBool("isLedgeGrabbing"))
		{
			GRAB_LEDGE();
		}

		// Knockback
		Vector2 direction = (opponent.position - transform.position).normalized;
        rb.velocity = new Vector2(-direction.x * 10, 5);

		yield return new WaitForSecondsRealtime(0.25f);
		anim.SetBool("isHurt", false);
		if (!isPaused)
			Time.timeScale = 1;

		yield return new WaitForSeconds(0.25f);
		beenHurt = false;

		yield return new WaitForSeconds(0.5f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		hurtCo = null;
	}

	IEnumerator StunLockCo(Transform stunLockPos)
	{
		if (isInvincible)
		{
			stunLockCo = null;
			yield break;
		}
		if (hunterSpinObj != null) hunterSpinObj.SetActive(false);
		anim.SetBool("isStunLock", true);
		inStunLock = true;
		ResetAllBools();
		atkCo = toolCo = null;
		// beenHurt = true;
		hp = Mathf.Max(0, hp - 1);
		SetHp();
		anim.SetBool("isSkillAttacking", false);
		anim.SetBool("isGossamerStorm", false);
		anim.SetBool("isBinding", false);
		usingSkill = false;
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;

		// stop healing
		if (bindCo != null) 
		{
			StopCoroutine(bindCo);
			bindCo = null;
			rb.gravityScale = 1;
		}
		// stop parry effect
		if (parryCo != null)
			StopCoroutine(parryCo);

		// teleport to ledge if in animation
		if (anim.GetBool("isLedgeGrabbing"))
		{
			GRAB_LEDGE();
		}

		// transform.position = stunLockPos.position;
		SetStunLockDest(stunLockPos, 0.2f);

		yield return new WaitForSeconds(0.1f);
		rb.velocity = Vector2.zero;

		yield return new WaitForSeconds(0.5f);
		// Died
		if (hp <= 0)
		{
			yield return DiedCo();
			yield break;
		}
		else
		{
			rb.gravityScale = 1;
			inStunLock = false;

			anim.SetBool("isStunLock", false);
			stunLockCo = null;
		}
	}

	void SetStunLockDest(Transform dest, float time)
	{
		t = 0;
		startPosition = transform.position;
		stunLockTime = time;
		stunLockPos = dest; 
	}

	IEnumerator InstantDeathCo()
	{
		
		MusicManager.Instance.PlayHurtSFX();

		anim.SetBool("isHurt", true);
		ResetAllBools();
		atkCo = toolCo = null;
		beenHurt = true;
		hp = 0;
		SetHp();
		rb.velocity = Vector2.zero;
		CinemachineShake.Instance.ShakeCam(15, 0.25f);
		anim.SetBool("isSkillAttacking", false);
		anim.SetBool("isGossamerStorm", false);
		anim.SetBool("isBinding", false);
		usingSkill = false;
		// rb.gravityScale = 1;

		// stop healing
		if (bindCo != null) 
		{
			StopCoroutine(bindCo);
			bindCo = null;
			rb.gravityScale = 1;
		}
		// stop stun lock
		if (stunLockCo != null) 
		{
			StopCoroutine(stunLockCo);
			anim.SetBool("isStunLock", false);
			inStunLock = false;
			stunLockCo = null;
			rb.gravityScale = 1;
		}
		// stop parry effect
		if (parryCo != null)
			StopCoroutine(parryCo);

		// Died
		if (hp <= 0)
		{
			yield return DiedCo();
			yield break;
		}
	}

	public void DEATH_ANIM_ON()
	{
		if (deathAnimObj != null)
			deathAnimObj.SetActive(true);
	}
	IEnumerator DiedCo(bool saveDeath=true)
	{
		CinemachineShake.Instance.ShakeCam(8f, 5f, 0.8f, true);
		MusicManager.Instance.PlayMusic(null);
		isDead = true;
		nKilled = 0;
		rb.velocity = Vector2.zero;
		rb.gravityScale = 0;
		anim.SetBool("isDead", true);

		if (saveDeath)
		{
			deathScene = SceneManager.GetActiveScene().name;
			deathPos = transform.position;
		}

		yield return new WaitForSeconds(2);
		GameManager.Instance.ClearEnemiesDefeated();
		transform.position = savedPos;
		isCountingTime = false;
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(savedScene);
		float loadTime = 0;
		// wait for scene to load
		while (!loadingOperation.isDone && loadTime < 5)
		{
			loadTime += Time.deltaTime;
			yield return null;
		}
		isCountingTime = true;
		GameManager.Instance.transitionAnim.SetFloat("speed", 0);
		GameManager.Instance.transitionAnim.SetTrigger("reset");
		yield return new WaitForSeconds(0.05f);
		if (deathAnimObj != null)
			deathAnimObj.SetActive(false);
		GameManager.Instance.transitionAnim.SetFloat("speed", 1);

		if (!saveDeath)
		{
			timePlayedTxt.gameObject.SetActive(true);
			transitionAnim.SetTrigger("reset");
		}
		else
		{
			CheckForCacoon();
		}

		replaying = isFinished = inStunLock = isDead = false;
		justParried = false;
		rb.gravityScale = 1;
		anim.SetBool("isDead", false);
		anim.SetBool("isHurt", false);
		anim.SetBool("isStunLock", false);
		beenHurt = false;
		if (soulLeakPs != null) soulLeakPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		MusicManager.Instance.PlayMusic(MusicManager.Instance.bgMusic);
		FullRestore();	// respawn
		SetSilk(-silkMeter);
		hurtCo = null;
	}

	void CheckForCacoon()
	{
		if (cacoonObj != null && deathScene == SceneManager.GetActiveScene().name)
		{
			cacoonObj.SetActive(true);
			cacoonObj.transform.position = deathPos;
		}
		else
		{
			cacoonObj.SetActive(false);
			// cacoonObj.transform.position = deathPos;
		}
	}


	IEnumerator MoveToNextScene(string newSceneName, Vector2 newScenePos)
	{
		canMove = movingToNextScene = invulnerable = true;
		nextSceneSpeed = (rb.velocity.x > 0) ? 1 : -1;
		isCountingTime = false;
		yield return new WaitForSeconds(0.1f);
		GameManager.Instance.transitionAnim.SetTrigger("toBlack");

		yield return new WaitForSeconds(0.25f);
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(newSceneName);
		float loadTime = 0;
		// wait for scene to load
		while (!loadingOperation.isDone && loadTime < 5)
		{
			loadTime += Time.deltaTime;
			yield return null;
		}
		isCountingTime = true;
		canMove = false;
		transform.position = newScenePos;
		GameManager.Instance.transitionAnim.SetTrigger("reset");
		CheckForCacoon();
		if (hp > 1 && soulLeakShortPs != null)
		{
			soulLeakShortPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			soulLeakShortPs.Play();
		}
		if ((!hasShield && hp == 1) || (hasShield && shieldHp == 0 && hp == 1))
		{
			animeLinesAnim.SetBool("show",true);
			soulLeakPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			soulLeakPs.Play();
		}

		yield return new WaitForSeconds(0.5f);
		canMove = true;
		
		yield return new WaitForSeconds(0.5f);
		canMove = movingToNextScene = invulnerable = false;
	}


	private int GetBindCost()
	{
		return crestNum == 1 ? harpBindCost : bindCost;
	}

	IEnumerator BindCo()
	{
		if (silkMeter < GetBindCost())
		{
			bindCo = null;
			yield break;
		} 
		anim.SetBool("isBinding", true);
		if (!infiniteSilk) SetSilk(-GetBindCost());
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;
		activeMoveSpeed = moveSpeed;
		isJumping = jumpDashed = jumped = false;
		isDashing = false;
		canLedgeGrab = ledgeGrab = false;
		SpawnExistingObjAtSelf(healingPs);
		gitGudSound.Play();

		yield return new WaitForSeconds(0.333f / anim.GetFloat("bindSpeed"));
		if (bindCo == null)
			yield break;
		rb.gravityScale = 1;
		// anim.SetBool("isBinding", false);
		hp = Mathf.Min(hp+(crestNum == 1 ? 2 : 3), hpMasks.Length);
		SetHp(true);
		bindCo = null;
	}

	public IEnumerator FlashCo()
	{
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = flashMat;

		SpawnExistingObjAtSelf(bindPs);
		if (isResting)
			FullRestore(true); // rest

		// Instantiate(bindPs, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(0.1f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		anim.SetBool("isBinding", false);
	}

	void SetHp(bool healed=false)
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

		if (!healed && hp != 1 && soulLeakShortPs != null)
		{
			soulLeakShortPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			soulLeakShortPs.Play();
		}
		if ((!hasShield && hp == 1) || (hasShield && shieldHp == 0 && hp == 1))
		{
			animeLinesAnim.SetBool("show",true);
			if (soulLeakPs != null) soulLeakPs.Play();
		}
		else
		{
			animeLinesAnim.SetBool("show",false);
			if (soulLeakPs != null) soulLeakPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}
	
	public void SetSilk(int addToSilk=0)
	{
		int prevSilk = silkMeter;
		silkMeter = Mathf.Clamp(
			silkMeter + addToSilk * (addToSilk > 0 ? silkMultiplier : 1), 
			0,
			(hasExtraSpool ? silks.Length : silks.Length - 3)
		);

		// cancel if no changes
		if (prevSilk == silkMeter) return;

		if (spoolImg != null)
		{
			spoolImg.sprite = (silkMeter >= GetBindCost()) ? 
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

		if (silkMeter != prevSilk)
		{
			if (crestNum == 1 && silkGlowHarp != null)
				silkGlowHarp.SetActive(silkMeter >= 6);
			else if (crestNum != 1 && silkGlowNorm != null)
				silkGlowNorm.SetActive(silkMeter >= 9);
		}
	}

	public void Parry()
	{
		if (!canParry) return;
		if (parryCo != null) StopCoroutine( ParryCo() );
		Time.timeScale = 0;
		parryCo = StartCoroutine( ParryCo() );
	}

	public IEnumerator ParryCo()
	{
		// Time.timeScale = 0;
		MusicManager.Instance.PlayParrySFX();
		justParried = true;

		yield return new WaitForSecondsRealtime(0.25f);
		if (!isPaused)
			Time.timeScale = 1;
		parryCo = null;

		yield return new WaitForSecondsRealtime(0.25f);
		justParried = false;
	}


	public int IncreaseKills()
	{
		return ++nKilled;
	}


	[Command("toggle_invincibility", "toggle_invincibility", MonoTargetType.All)] public void toggle_invincibility()
	{
		invincible = !invincible;
	}

	[Command("toggle_infinite_silk", "toggle_infinite_silk", MonoTargetType.All)] public void toggle_infinite_silk()
	{
		infiniteSilk = !infiniteSilk;
	}

	[Command("restart", "restart", MonoTargetType.All)] public void restart()
	{
		transform.position = savedPos;
		SceneManager.LoadScene(savedScene);
		GameManager.Instance.transitionAnim.SetTrigger("reset");
		cacoonObj.SetActive(false);
		isFinished = inStunLock = isDead = false;
		rb.gravityScale = 1;
		anim.SetBool("isDead", false);
		anim.SetBool("isHurt", false);
		beenHurt = false;
		if (soulLeakPs != null) soulLeakPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		FullRestore();
		SetSilk(-silkMeter);
		hurtCo = null;
		cacoonObj.SetActive(false);
	}



	// todo --------------------------------------------------------------------

	public void UNPAUSE()
	{
		pause2Anim.SetTrigger("close");
		if (pauseCo != null)
			StopCoroutine(pauseCo);
		pauseCo = StartCoroutine( UnpauseCo() );
	}

	public void RESTART()
	{
		GameManager.Instance.Restart();
	}
	public void REMAP_CONTROLS()
	{
		pause2Menu.SetActive(false);
		GameManager.Instance.OpenRemapControls();
	}
	public void SHOW_DMG(TextMeshProUGUI uiTxt)
	{
		if (GameManager.Instance.ToggleDmgIndicator())
		{
			uiTxt.text = "Hide Dmg";
		}
		else
		{
			uiTxt.text = "Show Dmg";
		}
	}
	public void DoneRemapping()
	{
		pause2Menu.SetActive(true);
		// GameManager.Instance.OpenRemapControls();
	}
}
