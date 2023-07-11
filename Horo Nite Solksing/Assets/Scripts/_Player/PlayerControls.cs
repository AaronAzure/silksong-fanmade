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
	private GameManager gm;


	[Space] [Header("STATUS")]
	private int maxHp=6;
	[SerializeField] int hp;
	public int nBonusHp;

	private int maxSilk=9;
	[SerializeField] int nSilk;
	public int nBonusSilk;
	
	
	[Space] [SerializeField] int nRosaries;
	[SerializeField] ParticleSystem rosaryCollectPs;
	private int oldRosaries;
	[SerializeField] TextMeshProUGUI rosariesTxt;
	public int[] atkDmg={10,8,15,10};
	public int gossamerDmg=5;
	public int stabDmg=30;
	public int rushDmg=20;
	[SerializeField] Animator[] silks;
	[SerializeField] GameObject[] hpMasks;
	[SerializeField] GameObject silkGlowNorm;
	[SerializeField] GameObject silkGlowHarp;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] Material defaultMat;
	[SerializeField] Material dmgMat;
	[SerializeField] Material flashMat;


	[Space] [Header("PLATFORMER")]
	[SerializeField] Rigidbody2D rb;
	public Transform model;
	public Transform camTarget;

	[Space] [SerializeField] ParticleSystem dustTrailPs;
	[SerializeField] ParticleSystem wallSlideTrailPsRight;
	[SerializeField] ParticleSystem wallSlideTrailPsLeft;
	private bool isDustTrailPlaying=true;
	private bool isWallSlideTrailPlaying=true;
	[SerializeField] float moveSpeed=5;
	[SerializeField] float jumpDashForce=10;
	[SerializeField] float jumpForce=10;
	[SerializeField] float fallSpeed=3;
	[SerializeField] float fallClampSpeed=-10f;
	[SerializeField] float fallGrav=1.2f;
	// private bool isFalling;
	[SerializeField] float risingForce=10;
	[SerializeField] Vector2 wallJumpForce;
	private float origGrav;
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

	private bool isGrounded;
	private bool isPlatformed;
	// private bool isCloseToGround;
	private bool jumpRegistered;
	private bool inWater;
	private bool isPogoing;
	private bool isPaused;
	private bool isPauseMenu1;
	private bool isWallSliding;
	private float jumpTimer;
	private bool canLedgeGrab;
	private bool ledgeGrab;
	private bool noControl;
	public bool isResting {get; private set;}
	public bool canUnrest {get; private set;}
	[SerializeField] GameObject needToRestObj;
	private bool inStunLock;
	[SerializeField] float stunLockSpeed=8;
	public bool justParried {get; private set;}
	[SerializeField] bool isInvincible;
	[SerializeField] bool hasLedge;
	[SerializeField] bool hasWall;
	[SerializeField] bool receivingKb;

	[SerializeField] Transform ledgeCheckPos;
	[SerializeField] Transform wallCheckPos;
	[SerializeField] float ledgeGrabDist=0.3f;

	[Space] [SerializeField] float jumpMaxTimer=0.5f;
	private float coyoteTimer;
	[SerializeField] float coyoteThreshold=0.1f;
	private float jumpBufferTimer;
	[SerializeField] float jumpBufferThreshold=0.2f;
	[SerializeField] Transform groundCheck;
	// [SerializeField] Transform closeToGroundCheck;
	[SerializeField] Vector2 groundCheckSize;
	// [SerializeField] Vector2 closeToGroundCheckSize;
	[SerializeField] Vector2 waterCheckSize;
	[SerializeField] LayerMask whatIsPlayer;
	private int whatIsPlayerValue;
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] LayerMask whatIsPlatform;
	private int whatIsPlatformValue;
	[SerializeField] LayerMask whatIsWater;
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
	[SerializeField] GameObject areaCanvas;


	[Space] [Header("SOUND EFFECTS")]
	[SerializeField] AudioSource shawSound;
	[SerializeField] AudioSource agaleSound;
	[SerializeField] AudioSource adimaSound;
	[SerializeField] AudioSource gitGudSound;


	[Space] [Header("PARTICLE EFFECTS")]
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


	[Space] [Header("ANIMATOR CONTROLLED")]
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


	[Space] [Header("IN-GAME RELATED")]
	[SerializeField] Bench bench;
	[SerializeField] bool startWalkingIn;


	[Space] [Header("TOOLS")]
	// [SerializeField] Tool equippedTool;
	[SerializeField] StraightPin straightPin;
	[SerializeField] Pimpillo pimpillo;
	[SerializeField] Caltrops caltrops;
	[SerializeField] SawBlade sawBlade;

	[Space] [SerializeField] Transform toolSummonPos;
	[SerializeField] GameObject toolGaugeUi;
	[SerializeField] Tool[] tools;
	[SerializeField] Tool tool1;
	[SerializeField] Tool tool2;
	private bool refillUses;
	private int nToolUses1;
	private int nToolUses2;
	private float nToolSlowUses1;
	private float nToolSlowUses2;
	[Space] [SerializeField] Image toolUses1; // progress bar
	[SerializeField] Image toolUses2; // progress bar
	

	[Space] [SerializeField] bool hasShield;
	public int nShieldBonus;
	[SerializeField] GameObject shieldObj;
	[SerializeField] Image shieldImg;
	[SerializeField] Sprite shieldUndmgSpr;
	[SerializeField] Sprite[] shieldSprs;
	private int shieldHp;
	[SerializeField] bool hasExtraSpool;
	public int nExtraSpoolBonus;
	[SerializeField] GameObject[] spoolObj;
	[SerializeField] GameObject[] extraSpoolObj;
	[SerializeField] Image spoolBindMarkerImg6;
	[SerializeField] Image spoolBindMarkerImg9;
	[SerializeField] Image extraMidMarkerImg;
	[SerializeField] Sprite spoolNormSpr;
	[SerializeField] Sprite spoolBindMarkerSpr;
	[SerializeField] Sprite extraMidMarkerSpr;
	[SerializeField] Sprite extraBindMarkerSpr;
	[SerializeField] GameObject spoolEndObj;
	// [SerializeField] GameObject normSpoolObj;
	// [SerializeField] GameObject extraSpoolObj;
	// [SerializeField] GameObject normHarpistSpoolObj;
	// [SerializeField] GameObject extraHarpistSpoolObj;


	[Space] [Header("UI")]
	[SerializeField] Animator transitionAnim;

	[Space] [SerializeField] GameObject pauseMenu;
	[SerializeField] Animator pauseAnim;
	// [SerializeField] CanvasGroup pauseMenuUi;

	[Space] [SerializeField] GameObject pause2Menu;
	[SerializeField] bool canExitPause2Menu=true;
	// [SerializeField] GameObject pause2Buttons;
	[SerializeField] Animator pause2Anim;
	// [SerializeField] CanvasGroup pause2MenuUi;

	[Space] [SerializeField] Image[] toolIcons;
	[SerializeField] Image[] toolsEquipped;
	[SerializeField] Sprite emptySpr;
	[SerializeField] TextMeshProUGUI showDmgTxt;
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

	[Space] [Header("CRESTS")]
	[SerializeField] Crest[] crests;
	[SerializeField] Image[] crestIcons;
	public int crestNum;
	[SerializeField] GameObject shawEffectObj;
	[SerializeField] GameObject hunterSpinObj;


	private bool invulnerable;
	private float nextSceneSpeed;
	[HideInInspector] public bool started=true;
	[HideInInspector] public bool loadExitPoint=true;
	private bool movingToNextScene;
	private bool movingVertically;	// jumping or falling through new scene 
	private bool movingVerticallyJumping;
	private bool canMove;
	private bool canMoveBegin;
	private bool canMoveHorz;
	private bool movingRight;


	[Space] [Header("SHOP")]
	[SerializeField] bool isNearShop;
	[SerializeField] bool isAtShop;
	private NPC npc; // current npc nearby
	[SerializeField] GameObject shopCanvas;
	[SerializeField] GameObject shopCam;
	[SerializeField] Animator shopAnim;


	[Space] [Header("DEBUG")]
	[SerializeField] [Range(1,10)] int silkMultiplier=1;
	[SerializeField] bool invincible;
	[SerializeField] bool infiniteHp;
	[SerializeField] bool infiniteSilk;
	[SerializeField] bool canParry=true;
	[SerializeField] bool calcHeight;
	private float peakHeight=-9999;
	private float shawDir=-0.85f;
	[SerializeField] string savedScene="Scene1";
	[SerializeField] Vector2 savedPos;
	[SerializeField] string deathScene;
	[SerializeField] Vector2 deathPos;
	private bool collectedCacoon=true;
	private bool timeStarted;
	public bool isCountingTime;
	// [SerializeField] TimeSpan timeSpan;
	[SerializeField] float timePlayed;
	[SerializeField] TextMeshProUGUI timePlayedTxt;
	[SerializeField] TextMeshProUGUI finalTimePlayedTxt;
	[SerializeField] GameObject difficultyObj;
	[SerializeField] TextMeshProUGUI iframeTxt;
	[SerializeField] GameObject replayObj;
	[SerializeField] Rewired.Integration.UnityUI.RewiredStandaloneInputModule rinput;
	// public static PlayerControls Instance;
	private int nKilled=0;

	float t;
	float t1;
	float t2;
    Vector3 startPosition;
    Vector3 newScenePos;
    public int exitPointInd {get; private set;}=-1;
	private bool stuckToNewScene;
	private Transform stunLockPos;
    float stunLockTime=0.5f;


	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			if (GameManager.Instance != null)
				GameManager.Instance.SetFirstSceneName();
		}
		else
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
	}

	public void DestroyItself()
	{
		if (cacoonObj != null) Destroy(cacoonObj);
		if (bindPs != null) Destroy(bindPs);
		if (healingPs != null) Destroy(healingPs);
		if (bloodBurstPs != null) Destroy(bloodBurstPs);
		Destroy(gameObject);
	}


	// Start is called before the first frame update
	void Start()
	{
		self = transform;
		tools = new Tool[1];

		savedScene = SceneManager.GetActiveScene().name;
		savedPos = self.position;

		player = ReInput.players.GetPlayer(playerId);

		activeMoveSpeed = moveSpeed;
		bindPs.transform.parent = null;
		healingPs.transform.parent = null;
		bloodBurstPs.transform.parent = null;
		cacoonObj.transform.parent = null;

		whatIsPlayerValue = LayerMask.NameToLayer("Player");
		whatIsPlatformValue = LayerMask.NameToLayer("Platform");

		DontDestroyOnLoad(cacoonObj);
		FullRestore(); // starting
		// Screen.SetResolution((int) (16f/9f * Screen.height), Screen.height, true);

		if (pauseMenu != null) pauseMenu.SetActive(false);
		if (pause2Menu != null) pause2Menu.SetActive(false);

		gm = GameManager.Instance;
		origGrav = rb.gravityScale;

		if (gm.showDmg)
		{
			showDmgTxt.text = "Show Dmg: On";
		}
		else
		{
			showDmgTxt.text = "Show Dmg: Off";
		}

		// More Hp and More invincibilty on Easy Mode
		if (gm.easyMode)
		{
			if (difficultyObj != null)
				difficultyObj.SetActive(true);
			maxHp = 8;
			SetUiHp();
		}

		if (startWalkingIn)
			StartCoroutine( MoveOutOfStartCo() );
		else
			StartCoroutine( StartCo() );

		PlayBackgroundMusic();
	}

	bool CanControl()
	{
		return (!isLedgeGrabbing && !ledgeGrab && !beenHurt && !noControl && !isBinding &&
			!inAnimation && !isDead && !inStunLock && !isResting && !isPaused && !inRushSkill 
			&& !isFinished && !isAtShop
		);
	}

	public void Unpause()
	{
		if (pauseCo != null)
			StopCoroutine(pauseCo);
		pauseCo = StartCoroutine( UnpauseCo() );
		pauseMenu.SetActive(false);
		pause2Menu.SetActive(false);
		shopCanvas.SetActive(false);
		shopCam.SetActive(false);
	}

	IEnumerator UnpauseCo()
	{
		yield return null;
		yield return null;
		isAtShop = isPaused = false;
		pauseCo = null;
		if (npc != null)
			npc.ToggleTextbox(true);
	}

	private string ConvertToTime(TimeSpan time)
	{
		return time.Hours > 0 ? time.ToString(@"hh\:mm\:ss\.ff") : time.ToString(@"mm\:ss\.ff");
	}

	private bool replaying;

	// Update is called once per frame
	void Update()
	{
		if (rosariesTxt != null)
		{
			if (nRosaries != oldRosaries)
			{
				oldRosaries = nRosaries;
				rosariesTxt.text = nRosaries.ToString();
			}
		}

		if (calcHeight)
		{
			float temp = transform.position.y;
			if (temp > peakHeight)
			{
				peakHeight = temp;
				Debug.Log($"height = <color=cyan>{peakHeight}</color>");
			}
		}


		if (isFinished && !replaying)
		{
			if (finTime < 3.25f)
				finTime += Time.unscaledDeltaTime;
			else if (player.GetAnyButton())
			{
				finalTimePlayedTxt.gameObject.SetActive(false);
				replaying = true;
				finTime = 0;
				gm.ClearBossClearedList();
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
			timePlayedTxt.text = ConvertToTime(time);
		}

		// inventory open
		if (!isPaused && pauseAnim != null && player.GetButtonDown("Minus"))
		{
			isPaused = true;
			isPauseMenu1 = true;
			pauseMenu.SetActive(true);
			pauseAnim.SetTrigger("open");
		}
		// pause open
		else if (!isPaused && pause2Anim != null && player.GetButtonDown("Start"))
		{
			isPaused = true;
			isPauseMenu1 = false;
			pause2Menu.SetActive(true);
			pause2Anim.SetTrigger("open");
		}
		// inventory close
		else if (isPaused)
		{
			// close inventory
			if (isPauseMenu1 && (player.GetButtonDown("No") || player.GetButtonDown("Minus")))
			{
				pauseAnim.SetTrigger("close");
				toolGaugeUi.SetActive(tool1 != null);
			}
			// close Pause
			if (!isPauseMenu1 && canExitPause2Menu && (player.GetButtonDown("No") || player.GetButtonDown("Start")))
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
				else if (player.GetButtonDown("Bind") && (infiniteSilk || nSilk >= GetBindCost()) && bindCo == null)
					bindCo = StartCoroutine( BindCo() );

				// tools
				else if (player.GetButtonDown("Tool") && toolCo == null)
				{
					int tool = 0;
					if (tool == 0 && nToolUses1 > 0)
						toolCo = StartCoroutine( UseTool(0) );
				}

				// rest on bench
				else if (!isResting && bench != null && isGrounded && player.GetAxis("Move Vertical") > 0.85f)
				{
					isResting = true;
					needToRestObj.SetActive(false);
					isDashing = jumpDashed = false;
					rb.gravityScale = 0;
					rb.velocity = Vector2.zero;
					anim.SetBool("isResting", true);
					startPosition = transform.position;
				}

				// Open Shop
				else if (isNearShop && isGrounded && player.GetAxis("Move Vertical") > 0.85f)
				{
					if (npc != null)
						npc.ToggleTextbox(false);
					isAtShop = true;
					shopCanvas.SetActive(true);
					shopCam.SetActive(true);
					rb.velocity = new Vector2(0, rb.velocity.y);
					anim.SetBool("isWalking", false);
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
		// Leave Bench
		else if (isResting && canUnrest &&
			(player.GetButtonDown("No") || player.GetAxis("Move Vertical") < -0.85f
			|| player.GetAxis("Move Horizontal") < -0.7f || player.GetAxis("Move Horizontal") > 0.85f)
		)
		{
			t = 0;
			canUnrest = isResting = false;
			needToRestObj.SetActive(true);
			activeMoveSpeed = moveSpeed;
			rb.gravityScale = 1;
			rb.velocity = Vector2.zero;
			anim.SetBool("isResting", false);
		}
		// leave shop
		else if (isAtShop && player.GetButtonDown("No"))
		{
			if (shopAnim != null)
				shopAnim.SetTrigger("close");
			else
			{
				shopCanvas.SetActive(false);
				shopCam.SetActive(false);
				isAtShop = false;
			}
		}
	}

	void FixedUpdate()
	{
		if (stuckToNewScene)
		{
			transform.position = this.newScenePos;
		}
		if (CanControl())
		{
			if (inShawAtk)
			{
				if (downwardStrike)
					rb.velocity = new Vector2(moveDir * shawForce, -shawForce);
				else if (dashStrike)
					rb.velocity = new Vector2(moveDir * dashSpeed, rb.velocity.y);

				CheckIsGrounded();
				CoyoteTimeMechanic();
				CheckIsInWater();
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

				// CheckIsCloseToGround();
				CheckIsGrounded();
				CoyoteTimeMechanic();
				CheckIsInWater();
				CheckIsWalled();

				// Dash
				CalcDash();

				// Normal movement
				if (!inAirDash)
					Move();
				// Air dashed
				else
					rb.velocity = new Vector2(model.localScale.x * dashSpeed * 0.9f, rb.velocity.y);

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
			// t += Time.fixedDeltaTime/stunLockTime;
			Vector2 dir = (stunLockPos.position - transform.position);
			rb.velocity = dir.normalized 
				* stunLockSpeed * dir.magnitude;
			// transform.position = Vector3.Lerp(startPosition, stunLockPos.position, t);
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
				t1 += 0.5f * Time.fixedDeltaTime * (refillUses ? 5 : 1);
				toolUses1.fillAmount = nToolSlowUses1/tool1.GetTotalUses();
			}
			else
			{
				t1 = 0;
				refillUses = false;
			}
		}
		if (toolUses2 != null && tool2 != null)
		{
			if (nToolSlowUses2 != nToolUses2)
			{
				nToolSlowUses2 = Mathf.Lerp(nToolSlowUses2, nToolUses2, t2);
				t2 += 0.5f * Time.fixedDeltaTime;
				toolUses2.fillAmount = nToolSlowUses2/tool2.GetTotalUses();
			}
			else
			{
				t2 = 0;
			}
		}
	}

	void DashMechanic(bool alreadyRegistered=false)
	{
		// First frame of pressing dash button
		if (alreadyRegistered || (!isLedgeGrabbing && player.GetButtonDown("Dash") 
			&& dashCounter <= 0 && dashCooldownCounter <= 0))
		{
			isDashing = true; // keep dashing if on ground
			isJumping = jumpDashed = jumped = false;
			jumpTimer = jumpMaxTimer;
			jumpTimer = 0;

			// air dash
			if (!isGrounded)
			{
				if (!isWallSliding && moveX != 0)
				{
					if (moveX > 0) 
						model.localScale = new Vector3(1, 1, 1);
					// left
					else if (moveX < 0) 
						model.localScale = new Vector3(-1, 1, 1);
					moveDir = model.localScale.x;
				}
				else if (isWallSliding)
					Flip();
				anim.SetBool("isAirDash", true);
				anim.SetFloat("moveSpeed", 1);
				airDashed = true;
			}
				
			dashCounter = dashDuration;
			activeMoveSpeed = dashBurstSpeed;
			anim.SetFloat("moveSpeed", dashBurstSpeed);

			if (IsFacingRight())
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
		if (player.GetButtonDown("Jump"))
		{
			jumpBufferTimer = 0;
			jumpRegistered = true;
		}
		if (jumpRegistered && jumpBufferTimer < jumpBufferThreshold)
		{
			jumpBufferTimer += Time.fixedDeltaTime;
		}
		// First Frame of Jump
		if (!isJumping && jumpBufferTimer < jumpBufferThreshold && coyoteTimer < coyoteThreshold)
		{
			jumpRegistered = false;
			jumpBufferTimer = jumpBufferThreshold;
			Jump();
		}
		// Released jump button
		else if (player.GetButtonUp("Jump") || CheckIsCeiling())
		{
			if (isJumping)
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.75f);
			jumpRegistered = isJumping = false;
			coyoteTimer = coyoteThreshold;
		}
		// Holding jump button
		else if (isJumping && player.GetButton("Jump"))
		{
			if (!usingSkill && jumpTimer < jumpMaxTimer)
			{
				rb.velocity = new Vector2(rb.velocity.x, jumpForce);
				jumpTimer += Time.deltaTime;
			}
			// jump over
			else
			{
				isJumping = false;
				jumpTimer = 0;
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
		isAtShop = false;
		shopCanvas.SetActive(false);
		shopCam.SetActive(false);
		isAtShop = false;
		stuckToNewScene = false;
		isJumping = jumpDashed = jumped = false;
		airDashed = isDashing = false;
		canLedgeGrab = ledgeGrab = false;
		jumpTimer = 0;
		rb.gravityScale = origGrav;
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

	float NewSceneJumpMovement()
	{
		if (movingVerticallyJumping)
		{
			// carries player's momentum
			if (canMoveBegin)
				return rb.velocity.x;
			// preset velocity
			if (canMoveHorz)
				return nextSceneSpeed * activeMoveSpeed;
		}
		else
		{
			// carries player's momentum
			if (canMoveBegin)
				return rb.velocity.x;
		}
		// preset velocity
		return 0;
	}

	void Move()
	{
		// clamp fall speed
		if (rb.velocity.y < fallClampSpeed)
			rb.velocity = new Vector2(rb.velocity.x, fallClampSpeed);
			
		// cutscene
		if (movingToNextScene)
		{
			// jumping up or falling down
			if (movingVertically)
			{
				if (canMove)
				{
					rb.velocity = new Vector2(
						NewSceneJumpMovement(), 
						movingVerticallyJumping ? jumpForce : rb.velocity.y
					);
				}
				else
				{
					rb.velocity = new Vector2(0, 0);
				}
			}
			// moving left or right
			else
			{
				if (canMove)
				{
					rb.velocity = new Vector2(
						nextSceneSpeed * activeMoveSpeed, 
						rb.velocity.y
					);
				}
				else
				{
					rb.velocity = new Vector2(0, rb.velocity.y);
				}
			}
			return;
		}

		if (jumpDashed)
			rb.velocity = new Vector2(dashDir * jumpDashForce, rb.velocity.y);
			
		if (isWallJumping || jumpDashed || isLedgeGrabbing || inShawAtk) return;

		float x = moveX;
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
				anim.SetFloat("moveSpeed", x != 0 ? Mathf.Abs(x * activeMoveSpeed) : 1);
			}
			if (!isGrounded && !inShawAtk && !isJumping && !isWallJumping && rb.velocity.y < fallSpeed)
				rb.gravityScale = fallGrav;
			else
				rb.gravityScale = origGrav;
			rb.velocity = new Vector2(x * activeMoveSpeed, rb.velocity.y);
		}
		// dashing
		else
		{
			bool facingRight = IsFacingRight();
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

	void CoyoteTimeMechanic()
	{
		if (isGrounded)
			coyoteTimer = 0;
		else
			coyoteTimer += Time.fixedDeltaTime;
	}

	void CheckIsInWater()
	{
		inWater = Physics2D.OverlapBox(model.position, waterCheckSize, 0, whatIsWater);
	}
	void CheckIsGrounded()
	{
		isPlatformed = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsPlatform);
		if (isPlatformed)
			isGrounded = true;
		else
			isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround);

		if (dustTrailPs != null)
		{
			if (isDustTrailPlaying && !isGrounded)
			{
				isDustTrailPlaying = false;
				dustTrailPs.Stop(false, ParticleSystemStopBehavior.StopEmitting);
			}
			else if (!isDustTrailPlaying && isGrounded && !isPlatformed)
			{
				isDustTrailPlaying = true;
				dustTrailPs.Play();
				dustTrailPs.Emit(10);
			}
		}
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
	// void CheckIsCloseToGround()
	// {
	// 	isCloseToGround = Physics2D.OverlapBox(
	// 		closeToGroundCheck.position, closeToGroundCheckSize, 0, whatIsGround | whatIsPlatform
	// 	);
	// }
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
		if (wallSlideTrailPsRight != null && wallSlideTrailPsLeft != null)
		{
			if (isWallSlideTrailPlaying && !isWallSliding)
			{
				isWallSlideTrailPlaying = false;
				wallSlideTrailPsRight.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				wallSlideTrailPsLeft.Stop(false, ParticleSystemStopBehavior.StopEmitting);
			}
			else if (!isWallSlideTrailPlaying && isWallSliding)
			{
				isWallSlideTrailPlaying = true;
				if (IsFacingRight())
				{
					wallSlideTrailPsLeft.Emit(10);
					wallSlideTrailPsLeft.Play();
				}
				else
				{
					wallSlideTrailPsRight.Emit(10);
					wallSlideTrailPsRight.Play();
				}
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

	bool IsFacingRight()
	{
		return model.localScale.x > 0;
	}

	void Attack()
	{
		// dash atk
		if (isDashing && crestNum <= 1)
			atkCo = StartCoroutine( AttackCo(3) );
		// attack up
		else if (player.GetAxis("Move Vertical") > 0.85f)
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
		// Stabby stabby strike
		if ((infiniteSilk || nSilk >= skillStabCost))
			atkCo = StartCoroutine( SkillAttackCo() );
	}

	IEnumerator SkillAttackCo()
	{
		anim.SetBool("isAttacking", false);
		usingSkill = true;

		CancelDash();
		jumpDashed = false;
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;

		// stabby stabby strike
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
			Physics2D.IgnoreLayerCollision(whatIsPlayerValue, whatIsPlatformValue, true);
			shawSound.Play();

			yield return new WaitForSeconds(0.1666f);
			SkillAttackEffect();

			yield return new WaitForSeconds(0.25f);
			anim.SetBool("isSkillAttacking", false);
			usingSkill = false;
			Physics2D.IgnoreLayerCollision(whatIsPlayerValue, whatIsPlatformValue, false);
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
		if (!player.GetButton("Skill") || (!infiniteSilk && nSilk <= 0))
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
		yield return new WaitForSeconds(
			toolCooldownDuration * (tool1 != null && tool1.quickCooldown ? 0.5f : 1f)
		);
		toolCo = null;
	}

	public void USE_TOOL()
	{
		var tool = Instantiate( 
			isTool1 || tool2 == null ? tool1 : tool2, 
			toolSummonPos.position, 
			Quaternion.identity
		);
		tool.toRight = IsFacingRight();
		tool.inAir = !isGrounded;
		tool.isMaster = true;

		// caltrops only
		if (tool.isMultiple)
		{
			for (int i=1; i<tool.nCopies+tool.level ; i++)
			{
				var toolCopy = Instantiate( 
					isTool1 || tool2 == null ? tool1 : tool2, 
					toolSummonPos.position, 
					Quaternion.identity
				);
				toolCopy.velocityMultiplier = UnityEngine.Random.Range(0.7f,1.3f);
				toolCopy.toRight = IsFacingRight();
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
					shieldHp = 2 + nShieldBonus;
					// shieldImg.sprite = shieldUndmgSpr;
					shieldImg.gameObject.SetActive(true);
					if (shieldImg != null && shieldHp < shieldSprs.Length) 
						shieldImg.sprite = shieldSprs[(shieldHp == 2 + nShieldBonus) ? (shieldSprs.Length - 1) : shieldHp];
				}
				else
				{
					shieldHp = 2 + nShieldBonus;
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
				SetUiSilk();
				return hasExtraSpool;
			default:
				return false;
		}
	}
	void ChangeSpoolNotch()
	{
		// other crests
		if (crestNum != 1 && silkGlowNorm != null)
		{
			silkGlowHarp.SetActive(false);
			silkGlowNorm.SetActive(nSilk >= 9);
		}
		// harpist crest
		else if (crestNum == 1 && silkGlowHarp != null)
		{
			silkGlowNorm.SetActive(false);
			silkGlowHarp.SetActive(nSilk >= 6);
		}

		spoolImg.sprite = (nSilk >= GetBindCost()) ? 
			fullSpoolSpr : emptySpoolSpr;

		SetUiSilk();
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
		if (!movingToNextScene)
		{
			isWallSliding = false;
			isWallJumping = true;
			rb.velocity = model.localScale.x > 0 ? new Vector2(-wallJumpForce.x, wallJumpForce.y) : wallJumpForce;
			model.localScale = rb.velocity.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
			Invoke("ResetWallJump", 0.25f);
		}
	}

	void ResetWallJump()
	{
		if (!movingToNextScene)
		{
			isWallJumping = false;
		}
	}

	public void FinishAirDash()
	{
		anim.SetBool("isAirDash", false);
	}

	void LedgeGrab()
	{
		isJumping = jumped = false;
		jumpTimer = 0;
		anim.SetBool("isAirDash", false);
		CancelDash();
		jumpDashed = false;

		moveDir = model.localScale.x;
		rb.gravityScale = 0;
		dashCounter = 0;
		rb.velocity = Vector2.zero;
		anim.SetFloat("moveDir", moveDir);
		anim.SetBool("isLedgeGrabbing", true);
		ledgeGrab = true;
	}
	public void GRAB_LEDGE()
	{
		transform.position += new Vector3(moveDir * 0.5f, 0.8f);
		isWallSliding = canLedgeGrab = ledgeGrab = false;
		dashCooldownCounter = dashCounter = 0;
		rb.gravityScale = 1;
		rb.velocity = Vector2.zero;
		anim.SetBool("isLedgeGrabbing", false);
		if (player.GetButton("Dash"))
		{
			Invoke("LedgeGrabDash", 0.01f);
		}
	}

	void LedgeGrabDash()
	{
		DashMechanic(true);
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
		hp = maxHp + nBonusHp;
		SetHp(true);

		if (clearShadowRealmList)
		{
			gm.ClearEnemiesDefeated();
			savedScene = SceneManager.GetActiveScene().name;
			savedPos = self.position;
		}

		if (hasShield)
			shieldHp = 2 + nShieldBonus;
		if (shieldImg != null && (shieldHp) < shieldSprs.Length) 
			shieldImg.sprite = shieldSprs[(shieldHp == 2 + nShieldBonus) ? (shieldSprs.Length - 1) : shieldHp];

		if (tool1 != null && toolUses1 != null)
		{
			nToolUses1 = tool1.GetTotalUses();
			refillUses = true;
		}
		if (tool2 != null && toolUses2 != null)
		{
			nToolUses2 = tool2.GetTotalUses();
		}
	}

	public void ShawRetreat(bool dashStrike=false, float multiplier=1)
	{
		isJumping = false;
		jumpTimer = 0;
		switch (crestNum)
		{
			case 0:
				anim.SetBool("isAttacking", false);
				rb.velocity = Vector2.zero;
				rb.AddForce( new Vector2(-moveDir * shawForce * multiplier, shawForce * multiplier), ForceMode2D.Impulse);
				StartCoroutine( RegainControlCo(0.1f) );
				break;
			case 1:
				if (dashStrike)
				{
					anim.SetBool("isAttacking", false);
					rb.velocity = Vector2.zero;
					rb.AddForce( new Vector2(-moveDir * shawForce * multiplier, shawForce * multiplier), ForceMode2D.Impulse);
					StartCoroutine( RegainControlCo(0.1f) );
				}
				else
				{
					rb.velocity = new Vector2(rb.velocity.x , 0);
					rb.AddForce( new Vector2(0, shawForce * multiplier), ForceMode2D.Impulse);
				}
				break;
			case 2:
				if (!hunterSpinObj.activeSelf)
					StartCoroutine( HUNTER_SPIN_ON() );
				anim.SetBool("isAttacking", false);
				rb.velocity = Vector2.zero;
				anim.SetFloat("pogoing", 1);
				isPogoing = true;
				rb.AddForce( new Vector2(0, shawForce * multiplier), ForceMode2D.Impulse);
				// StartCoroutine( RegainControlCo(0.1f, false, true) );
				StartCoroutine( SwoopCo(0.2f) );
				// StartCoroutine( PogoCo(0.25f) );
				break;
			case 3:
				anim.SetBool("isAttacking", false);
				rb.velocity = Vector2.zero;
				anim.SetFloat("pogoing", 1);
				isPogoing = true;
				rb.AddForce( new Vector2(0, shawForce * multiplier), ForceMode2D.Impulse);
				// StartCoroutine( RegainControlCo(0.1f, false, true) );
				StartCoroutine( SwoopCo(0.2f) );
				// StartCoroutine( PogoCo(0.25f) );
				shawEffectObj.SetActive(true);
				break;
		}
	}
	public void Recoil(float multiplier=1)
	{
		rb.velocity = Vector2.zero;
		isDashing = false;
		jumpDashed = false;

		rb.AddForce( new Vector2(-moveDir * recoilForce * multiplier, 0), ForceMode2D.Impulse);
		StartCoroutine( RegainControlCo(0.1f) );
	}
	public void RisingAtkRetreat(float multiplier=1)
	{
		// anim.SetBool("isAttacking", false);
		rb.velocity = new Vector2(0, rb.velocity.y);

		rb.AddForce( new Vector2(-moveDir * risingRecoilForce * multiplier, 0), ForceMode2D.Impulse);
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
	IEnumerator PogoCo(float duration, float multiplier=1)
	{
		isInvincible = true;
		noControl = true;
		
		yield return new WaitForSeconds(0.05f);
		rb.velocity = new Vector2(rb.velocity.x, 0);

		yield return new WaitForSeconds(0.05f);
		rb.velocity = new Vector2(rb.velocity.x, 0);
		rb.AddForce( new Vector2(0, shawForce * multiplier), ForceMode2D.Impulse);
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
		return (!isDead && !invulnerable && !invincible && !justParried && !inInvincibleAnim && !movingToNextScene);
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
		if (!isDead && other.CompareTag("NPC") && hurtCo == null)
		{
			isNearShop = true;
			npc = other.GetComponent<NPC>();
			if (npc != null)
				npc.ToggleTextbox(true);
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
			movingVertically = n.isVertical;
			if (movingVertically)
			{
				// jumping up to new scene
				movingVerticallyJumping = n.transform.position.y - self.position.y > 0;
			}
			exitPointInd = n.exitIndex;
			StartCoroutine( MoveToNextSceneCo(n.newSceneName) );
		}
		if (!isFinished && other.CompareTag("Goal"))
		{
			beaten = isFinished = true;
			transitionAnim.SetTrigger("toBlack");
			Debug.Log("<color=green>Thanks for Playing</color>");
			CANCEL_DASH();

			// gm.transitionAnim.SetTrigger("toBlack");
			isCountingTime = false;
			TimeSpan time = TimeSpan.FromSeconds(timePlayed);
			Debug.Log($"<color=green>Thanks for Playing: {ConvertToTime(time)}</color>");
			if (gm.invincibilityDuration > 0.5f)
			{
				iframeTxt.gameObject.SetActive(true);
				iframeTxt.text = "Now try with shorter iframes xD";
			}
			finalTimePlayedTxt.text = ConvertToTime(time);
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
		if (other.CompareTag("NPC"))
		{
			isNearShop = false;
			if (npc != null)
				npc.ToggleTextbox(false);
			npc = null;
		}
	}

	void LoseHp(int dmg=1)
	{
		if (!infiniteHp)
		{
			for (int i=0 ; i<dmg ; i++)
			{
				// Take shield damage
				if (hasShield && hp == 1 && shieldHp > 0)
				{
					shieldHp = Mathf.Max(0, shieldHp - 1);
					if (shieldImg != null && (shieldHp) < shieldSprs.Length) 
						shieldImg.sprite = shieldSprs[(shieldHp == 2 + nShieldBonus) ? (shieldSprs.Length - 1) : shieldHp];
				}
				// Take damage
				else
				{
					hp = Mathf.Max(0, hp - 1);
				}
			}
			SetHp();
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
		gm.Vignette();
		if (hunterSpinObj != null) hunterSpinObj.SetActive(false);

		anim.SetBool("isHurt", true);
		ResetAllBools();
		atkCo = toolCo = null;
		beenHurt = true;

		LoseHp(dmg);
		rb.velocity = Vector2.zero;
		if (hp != 0)
		{
			CinemachineShake.Instance.ShakeCam(15, 0.25f);
			// if (hp > 1 || (hasShield && hp == 1 && shieldHp > 0))
			// 	animeLinesAnim.SetTrigger("dmg");
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
		float dirX = model.localScale.x;
		if ((opponent.position.x - transform.position.x) > 0)
			dirX = 1;
		else if ((opponent.position.x - transform.position.x) < 0)
			dirX = -1;
        rb.velocity = new Vector2(-dirX * 8, 5);

		// Freeze frame over
		yield return new WaitForSecondsRealtime(0.25f);
		anim.SetBool("isHurt", false);
		if (!isPaused)
			Time.timeScale = 1;

		// Can control again
		yield return new WaitForSeconds(0.25f);
		beenHurt = false;

		// invincibility over
		yield return new WaitForSeconds(gm.invincibilityDuration);
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
		
		LoseHp(1);
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
		loadExitPoint = false;
		nKilled = 0;
		rb.velocity = Vector2.zero;
		rb.gravityScale = 0;
		anim.SetBool("isDead", true);
		rb.gravityScale = origGrav;

		if (saveDeath)
		{
			collectedCacoon = false;
			deathScene = SceneManager.GetActiveScene().name;
			deathPos = transform.position;
		}

		yield return new WaitForSeconds(2);
		gm.ClearEnemiesDefeated();
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
		gm.transitionAnim.SetFloat("speed", 0);
		gm.transitionAnim.SetTrigger("reset");
		yield return new WaitForSeconds(0.05f);
		if (deathAnimObj != null)
			deathAnimObj.SetActive(false);
		gm.transitionAnim.SetFloat("speed", 1);
		loadExitPoint = true;

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
		Physics2D.IgnoreLayerCollision(whatIsPlayerValue, whatIsPlatformValue, false);
		beenHurt = false;
		if (soulLeakPs != null) soulLeakPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		PlayBackgroundMusic();
		FullRestore();	// respawn
		SetSilk(-nSilk);
		hurtCo = null;
	}

	void CheckForCacoon()
	{
		if (!collectedCacoon && cacoonObj != null && deathScene == SceneManager.GetActiveScene().name)
		{
			cacoonObj.SetActive(true);
			cacoonObj.transform.position = deathPos;
		}
		else
		{
			cacoonObj.SetActive(false);
		}
	}


	IEnumerator MoveToNextSceneCo(string newSceneName)
	{
		canMoveBegin = canMove = movingToNextScene = invulnerable = true;
		if (movingVerticallyJumping)
			nextSceneSpeed = (IsFacingRight()) ? 1 : -1;
		else
			nextSceneSpeed = (rb.velocity.x > 0) ? 1 : -1;
		isWallJumping = isCountingTime = false;
		yield return new WaitForSeconds(0.1f);
		gm.transitionAnim.SetTrigger("toBlack");

		yield return new WaitForSeconds(0.25f);
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(newSceneName);
		// float loadTime = 0;
		rb.velocity = Vector2.zero;

		// wait for scene to load
		// while (!loadingOperation.isDone && loadTime < 5)
		// {
		// 	loadTime += Time.deltaTime;
		// 	yield return null;
		// }
	}

	public void MoveOutOfNewScene(Vector2 newScenePos)
	{
		StartCoroutine( MoveOutOfNewSceneCo(newScenePos) );
	}

	IEnumerator MoveOutOfNewSceneCo(Vector2 newScenePos)
	{
		isCountingTime = true;
		isWallJumping = false;
		isWallSliding = false;
		canMoveBegin = canMoveHorz = canMove = false;
		this.newScenePos = transform.position = newScenePos;
		if (movingVertically) stuckToNewScene = true;
		gm.transitionAnim.SetTrigger("reset");

		CheckForCacoon();
		if (wallSlideTrailPsRight != null)
			wallSlideTrailPsRight.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (wallSlideTrailPsLeft != null)
			wallSlideTrailPsLeft.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (dustTrailPs != null)
			dustTrailPs.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (hp > 1 && soulLeakShortPs != null)
		{
			soulLeakShortPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		if ((!hasShield && hp == 1) || (hasShield && shieldHp == 0 && hp == 1))
		{
			animeLinesAnim.SetBool("show",true);
			soulLeakPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			soulLeakPs.Play();
		}

		yield return new WaitForSeconds(movingVertically ? 0.125f: 0.5f);
		if (movingVertically) stuckToNewScene = false;
		canMove = true;

		if (movingVertically && movingVerticallyJumping)
		{
			yield return new WaitForSeconds(0.1f);
			canMoveHorz = true;
		}
		
		yield return new WaitForSeconds(0.5f);
		canMoveHorz = canMove = movingToNextScene = invulnerable = false;
	}

	IEnumerator MoveOutOfStartCo()
	{
		started = canMove = movingToNextScene = invulnerable = true;
		nextSceneSpeed = (model.localScale.x > 0) ? 1 : -1;
		canMove = isCountingTime = false;

		yield return new WaitForSeconds(0.5f);
		canMove = true;
		moveX = moveSpeed * nextSceneSpeed;
		anim.SetBool("isWalking", true);
		anim.SetFloat("moveSpeed", moveSpeed * nextSceneSpeed);

		yield return new WaitForSeconds(0.75f);
		started = canMove = movingToNextScene = invulnerable = false;
		areaCanvas.SetActive(true);
	}

	IEnumerator StartCo()
	{
		yield return new WaitForSeconds(0.5f);
		started = canMove = movingToNextScene = invulnerable = false;
		areaCanvas.SetActive(true);
	}


	private int GetBindCost()
	{
		return crestNum == 1 ? harpBindCost : bindCost;
	}

	IEnumerator BindCo()
	{
		if (nSilk < GetBindCost())
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
		jumpTimer = 0;
		isDashing = false;
		canLedgeGrab = ledgeGrab = false;
		SpawnExistingObjAtSelf(healingPs);
		gitGudSound.Play();

		yield return new WaitForSeconds(0.333f / anim.GetFloat("bindSpeed"));
		if (bindCo == null)
			yield break;
		rb.gravityScale = 1;
		// anim.SetBool("isBinding", false);
		hp = Mathf.Min(hp+(crestNum == 1 ? 2 : 3), maxHp+nBonusHp);
		SetHp(true);
		bindCo = null;
	}

	public IEnumerator FlashCo()
	{
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = flashMat;

		SpawnExistingObjAtSelf(bindPs);
		if (isResting)
		{
			FullRestore(true); // rest
			canUnrest = true;
			SetUiHp();
			SetUiSilk();
		}

		// Instantiate(bindPs, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(0.1f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		anim.SetBool("isBinding", false);
	}

	void SetUiHp()
	{
		for (int i=0 ; i<maxHp+nBonusHp ; i++)
			if (hpMasks.Length > i)
				hpMasks[i].transform.parent.gameObject.SetActive(true);
	}

	void SetUiSilk()
	{
		// Extra Spool
		for (int i=0 ; i<4+nExtraSpoolBonus ; i++)
			if (extraSpoolObj.Length > i)
				extraSpoolObj[i].SetActive(hasExtraSpool);
		// foreach (GameObject o in extraSpoolObj)
		// 	o.SetActive(hasExtraSpool);
		spoolEndObj.SetActive(!hasExtraSpool);
		
		// harpist
		if (crestNum == 1)
		{
			spoolBindMarkerImg9.sprite = spoolNormSpr;
			spoolBindMarkerImg6.sprite = spoolBindMarkerSpr;
			extraMidMarkerImg.sprite = extraMidMarkerSpr;
		}
		// everything other crests
		else
		{
			spoolBindMarkerImg6.sprite = spoolNormSpr;
			if (maxSilk+nBonusSilk > 9)
			{
				spoolBindMarkerImg9.sprite = spoolBindMarkerSpr;
				extraMidMarkerImg.sprite = extraMidMarkerSpr;
			}
			else
			{
				spoolBindMarkerImg9.sprite = spoolNormSpr;
				extraMidMarkerImg.sprite = extraBindMarkerSpr;
			}
		}

		for (int i=0 ; i<maxSilk+nBonusSilk-2 ; i++)
			if (spoolObj.Length > i)
				spoolObj[i].SetActive(true);
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
		int prevSilk = nSilk;
		int totalSilk = (hasExtraSpool ? maxSilk + nBonusSilk + 3 + nExtraSpoolBonus : maxSilk + nBonusSilk);
		nSilk = Mathf.Clamp(
			nSilk + addToSilk * (addToSilk > 0 ? silkMultiplier : 1), 
			0,
			totalSilk
		);

		// cancel if no changes
		if (prevSilk == nSilk) return;

		if (spoolImg != null)
		{
			spoolImg.sprite = (nSilk >= GetBindCost()) ? 
				fullSpoolSpr : emptySpoolSpr;
		}

		for (int i=0 ; (i<silks.Length && i<totalSilk) ; i++)
		{
			// visible
			if (i < nSilk && i >= prevSilk)
				silks[i].SetTrigger("latch");
			// invisble
			else if (i >= nSilk && i < prevSilk)
				silks[i].SetTrigger("unlatch");
		}

		if (nSilk != prevSilk)
		{
			if (crestNum == 1 && silkGlowHarp != null)
				silkGlowHarp.SetActive(nSilk >= 6);
			else if (crestNum != 1 && silkGlowNorm != null)
				silkGlowNorm.SetActive(nSilk >= 9);
		}
	}

	public void CollectCacoon()
	{
		collectedCacoon = true;
	}

	public void Parry()
	{
		if (!canParry) return;
		if (parryCo != null) StopCoroutine( ParryCo() );
		Time.timeScale = 0;
		parryCo = StartCoroutine( ParryCo() );
	}

	public void PlayBackgroundMusic()
	{
		MusicManager m = MusicManager.Instance;
		if (SceneManager.GetActiveScene().name.StartsWith("Melon"))
			m.PlayMusic(m.melonBgMusic, m.melonBgMusicVol);
		else
			m.PlayMusic(m.bgMusic, m.bgMusicVol);
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

	public void GainCurrency(int x)
	{
		nRosaries += x;
		if (x > 0 && rosaryCollectPs != null)
		{
			rosaryCollectPs.Emit(2);
			var main = rosaryCollectPs.shape;
			main.rotation = new Vector3(0,0,UnityEngine.Random.Range(30,90));
		}
		nRosaries = Mathf.Max(0, nRosaries);
	}


	[Command("toggle_invincibility", "toggle_invincibility", MonoTargetType.All)] public void toggle_invincibility()
	{
		invincible = !invincible;
	}

	[Command("toggle_infinite_silk", "toggle_infinite_silk", MonoTargetType.All)] public void toggle_infinite_silk()
	{
		infiniteSilk = !infiniteSilk;
	}

	[Command("debug_finished", "debug_finished", MonoTargetType.All)] public void debug_finished()
	{
		Debug.Log(isFinished);
	}

	[Command("restart", "restart", MonoTargetType.All)] public void restart()
	{
		transform.position = savedPos;
		SceneManager.LoadScene(savedScene);
		gm.transitionAnim.SetTrigger("reset");
		cacoonObj.SetActive(false);
		isFinished = inStunLock = isDead = false;
		rb.gravityScale = 1;
		anim.SetBool("isDead", false);
		anim.SetBool("isHurt", false);
		beenHurt = false;
		if (soulLeakPs != null) soulLeakPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		FullRestore();
		SetSilk(-nSilk);
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
		collectedCacoon = true;
		gm.Restart();
		if (UiMouseSupport.Instance != null)
			UiMouseSupport.Instance.RevertToOriginalSortingOrder();
	}
	public void REMAP_CONTROLS()
	{
		pause2Menu.SetActive(false);
		gm.OpenRemapControls();
	}
	public void SHOW_DMG()
	{
		if (gm.ToggleDmgIndicator())
		{
			showDmgTxt.text = "Show Dmg: On";
		}
		else
		{
			showDmgTxt.text = "Show Dmg: Off";
		}
	}
	public void DoneRemapping()
	{
		canExitPause2Menu = false;
		pause2Menu.SetActive(true);
		StartCoroutine( CanExitPause2MenuCo() );
	}

	IEnumerator CanExitPause2MenuCo()
	{
		yield return null;
		canExitPause2Menu = true;
	}

	public void ExitGame()
	{
		pause2Menu.SetActive(false);
		gm.ExitGame();
	}


	public int GetCost(UiShopButton.Upgrade u)
	{
		switch (u)
		{
			case UiShopButton.Upgrade.pin:
				return (50 * (int) Mathf.Pow(3, straightPin.level));
			case UiShopButton.Upgrade.pimpillo:
				return (50 * (int) Mathf.Pow(3, pimpillo.level));
			case UiShopButton.Upgrade.caltrop:
				return (50 * (int) Mathf.Pow(3, caltrops.level));
			case UiShopButton.Upgrade.sawblade:
				return (50 * (int) Mathf.Pow(3, sawBlade.level));
			case UiShopButton.Upgrade.shield:
				return (50 * (int) Mathf.Pow(3, nShieldBonus));
			case UiShopButton.Upgrade.extraSpool:
				return (50 * (int) Mathf.Pow(3, nExtraSpoolBonus));
			case UiShopButton.Upgrade.health:
				return (50 * (int) Mathf.Pow(3, nBonusHp));
			case UiShopButton.Upgrade.spool:
				return (50 * (int) Mathf.Pow(3, nBonusSilk));
		}
		return -1;
	}
	public bool CanAffordPurchase(UiShopButton.Upgrade u)
	{
		return nRosaries >= GetCost(u);
	}

	public void MakePurchase(UiShopButton.Upgrade u)
	{
		nRosaries -= GetCost(u);
		switch (u)
		{
			case UiShopButton.Upgrade.pin:
				straightPin.level++;
				break;
			case UiShopButton.Upgrade.pimpillo:
				pimpillo.level++;
				break;
			case UiShopButton.Upgrade.caltrop:
				caltrops.level++;
				break;
			case UiShopButton.Upgrade.sawblade:
				sawBlade.level++;
				break;
			case UiShopButton.Upgrade.shield:
				nShieldBonus++;
				if (hasShield)
				{
					shieldHp = 2 + nShieldBonus;
					// shieldImg.sprite = shieldUndmgSpr;
					shieldImg.gameObject.SetActive(true);
					if (shieldImg != null && shieldHp < shieldSprs.Length) 
						shieldImg.sprite = shieldSprs[(shieldHp == 2 + nShieldBonus) ? (shieldSprs.Length - 1) : shieldHp];
				}
				break;
			case UiShopButton.Upgrade.extraSpool:
				nExtraSpoolBonus++;
				SetUiSilk();
				break;
			case UiShopButton.Upgrade.health:
				nBonusHp++;
				SetUiHp();
				FullRestore();
				break;
			case UiShopButton.Upgrade.spool:
				nBonusSilk++;
				SetUiSilk();
				break;
		}
	}
}
