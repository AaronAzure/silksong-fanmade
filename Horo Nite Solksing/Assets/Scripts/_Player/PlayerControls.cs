using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	[SerializeField] Rewired.InputManager rm;


	[Space] [Header("STATUS")]
	private int maxHp=6;
	[SerializeField] int hp;
	public int nBonusHp;
	[SerializeField] int nTempHp;

	private int maxSilk=9;
	[SerializeField] int nSilk;
	public int nBonusSilk;
	
	
	[Space] [SerializeField] int nRosaries;
	[SerializeField] int nRosaryStrings;
	[SerializeField] int nShellShards;
	private int nGoldenWatermelons;
	private int nGoldenWatermelonsTraded;
	
	[Space] [SerializeField] int maxShellShards=400;
	[SerializeField] ParticleSystem rosaryCollectPs;
	private int oldRosaries=-1;
	private int oldShellShards=-1;

	
	[Space] [SerializeField] TextMeshProUGUI rosariesTxt;
	[SerializeField] TextMeshProUGUI rosaryStringsTxt;
	[SerializeField] TextMeshProUGUI shellShardsTxt;
	[SerializeField] TextMeshProUGUI goldenWatermelonTxt;


	[Space] [SerializeField] int rosariesReqForConversion=60;
	[SerializeField] int rosaryStringConverted=50;
	
	
	[Space] [Header("ATTACK")]
	public int[] atkDmg={10,8,15,10};
	public int gossamerDmg=5;
	public int stabDmg=30;
	public int rushDmg=20;
	[SerializeField] Animator[] silks;
	[SerializeField] GameObject[] hpMasks;
	[SerializeField] GameObject[] tempHps;
	[SerializeField] GameObject silkGlowNorm;
	[SerializeField] GameObject silkGlowHarp;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] Material defaultMat;
	[SerializeField] Material dmgMat;
	[SerializeField] Material greenMat;
	[SerializeField] Material flashMat;


	[Space] [Header("PLATFORMER")]
	[SerializeField] Rigidbody2D rb;
	public Transform model;
	public Transform camTarget;

	[Space] [SerializeField] ParticleSystem leafTrailPs;
	[SerializeField] ParticleSystem leafWallSlideTrailPsRight;
	[SerializeField] ParticleSystem leafWallSlideTrailPsLeft;

	[Space] [SerializeField] ParticleSystem dustTrailPs;
	[SerializeField] ParticleSystem dustWallSlideTrailPsRight;
	[SerializeField] ParticleSystem dustWallSlideTrailPsLeft;

	[HideInInspector] public bool inTemple;
	private bool isDustTrailPlaying=true;
	private bool isWallSlideTrailPlaying=true;
	[SerializeField] float moveSpeed=5;


	[Space] [Header("JUMP RELATED")]
	[SerializeField] float jumpDashForce=10;
	[SerializeField] float jumpForce=10;
	[SerializeField] float jumpingOutOfSceneForce=10;
	[SerializeField] [Range(0f,1f)] float jumpCutoffForce=0.75f;
	[SerializeField] [Range(0f,1f)] float maxJumpCutoffForce=0.6f;
	[SerializeField] float fallSpeed=3;
	[SerializeField] float fallClampSpeed=-10f;
	[SerializeField] float fallGrav=1.2f;
	[SerializeField] float jumpMaxTimer=0.5f;
	private float coyoteTimer;
	[SerializeField] float coyoteThreshold=0.1f;
	private float jumpBufferTimer;
	[SerializeField] float jumpBufferThreshold=0.2f;
	private float wallJumpTimer;
	[SerializeField] float wallJumpMin=0.125f; // can be released
	[SerializeField] float wallJumpControlThreshold=0.25f; // can control
	[SerializeField] float wallJumpThreshold=0.5f; // max height
	private float wallJumpDir;
	private float lookTimer;
	[SerializeField] float lookThreshold=0.5f;
	[SerializeField] float lookOffset=2.5f;
	[SerializeField] float lookOffsetMultiplier=2.5f;
	private Vector3 camOffset;
	private bool camOffsetReset;
	private float lookLerpTimer;
	private float lookLerpThres=1;

	[Space] [SerializeField] float risingForce=10;
	[SerializeField] Vector2 wallJumpForce;
	// [SerializeField] Vector2 wallJumpForceMultiplier=Vector2.one;
	private float origGrav;
	private float moveX;
	private float dashDir;
	private float moveDir;
	private float atkDir;
	private float skillDir; // 0 = stab, 1 = rush, else = storm
	private bool isTool1=true;
	private bool jumped;
	private bool jumpDashed;
	private bool isDashing;
	[Space][SerializeField] bool isJumping;
	private bool isUsingMap;
	[SerializeField] bool isWallJumping;

	[SerializeField] bool isGrounded;
	private bool isPlatformed;
	[SerializeField] bool jumpRegistered;[Space]
	private bool inWater;
	private bool isPogoing;
	private bool isPaused;
	private bool isPauseMenu1;
	private bool isWallSliding;
	private float jumpTimer;
	private bool canLedgeGrab;
	private bool ledgeGrab;
	private bool noControl;
	private bool isRespawning;
	public bool isResting {get; private set;}
	public bool canUnrest {get; private set;}
	[SerializeField] GameObject needToRestObj;
	private bool inStunLock;
	[SerializeField] float stunLockSpeed=8;
	public bool justParried {get; private set;}
	[SerializeField] bool isInvincible;
	[SerializeField] bool hasLedge;
	[SerializeField] bool hasWall;
	[SerializeField] bool isRespawningA;

	[SerializeField] Transform ledgeCheckPos;
	[SerializeField] Transform wallCheckPos;
	[SerializeField] float ledgeGrabDist=0.3f;

	[Space] [SerializeField] Transform groundCheck;
	[SerializeField] Vector2 groundCheckSize;
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
	[SerializeField] float airDashMultiplier=1;
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
	[SerializeField] TextMeshProUGUI areaCanvasTxt;


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
	[SerializeField] GameObject greenGooPs;
	[SerializeField] ParticleSystem soulLeakPs;
	[SerializeField] ParticleSystem soulLeakShortPs;
	[SerializeField] Animator animeLinesAnim;
	[SerializeField] Transform dashSpawnPos;
	[SerializeField] Animator flashAnim;
	[SerializeField] GameObject melonSwordSparklePs;


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
	[SerializeField] StraightPin straightPin;
	[SerializeField] Pimpillo pimpillo;
	[SerializeField] Caltrops caltrops;
	[SerializeField] SawBlade sawBlade;
	[SerializeField] Syringe syringe;
	[SerializeField] MelonSword melonSword;

	[Space] [SerializeField] Animator melonSwordMagicAnim;
	[SerializeField] ParticleSystem melonSwordReadyPs;
	[SerializeField] float melonSwordComboTime=5;
	[SerializeField] float melonSwordRechargeTime=8;

	[Space] [SerializeField] Transform toolSummonPos;
	[SerializeField] GameObject toolGaugeUi;
	[SerializeField] Tool[] tools;
	private Tool tool1;
	private Tool tool2;
	[SerializeField] GameObject straightPinUi;
	[SerializeField] GameObject pimpilloToolUi;
	[SerializeField] GameObject caltropsToolUi;
	[SerializeField] GameObject shawbladesToolUi;
	[SerializeField] GameObject syringeToolUi;
	[SerializeField] GameObject melonSwordToolUi;
	[SerializeField] GameObject shieldToolUi;
	[SerializeField] GameObject spoolToolUi;
	[SerializeField] GameObject lootCharmToolUi;

	[Space] [SerializeField] GameObject lootCharmToolShopUi;
	[Space] [SerializeField] GameObject syringeToolShopUi;
	private bool refillUses;
	private float nToolSlowUses1;
	private float nToolSlowUses2;
	[Space] [SerializeField] Image toolUses1; // progress bar
	[SerializeField] Image toolUses2; // progress bar
	[SerializeField] TextMeshProUGUI tool1Version;
	

	[Space] [SerializeField] bool hasShield;
	public int nShieldBonus;
	[SerializeField] GameObject shieldObj;
	[SerializeField] Image shieldImg;
	[SerializeField] Sprite shieldUndmgSpr;
	[SerializeField] Sprite[] shieldSprs;
	private int shieldHp;

	[Space] [SerializeField] bool hasExtraSpool;
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

	[Space] [SerializeField] bool hasLootCharm;
	public int nLootCharmBonus;


	[Space] [Header("UI")]
	[SerializeField] Animator mainUI;
	[SerializeField] Animator transitionAnim;
	[SerializeField] CanvasGroup iconI;
	[SerializeField] GameObject icons;
	[SerializeField] GameObject highlightObj;
	[SerializeField] TextMeshProUGUI rosariesUiTxt;
	[SerializeField] TextMeshProUGUI rosaryStringsUiTxt;
	[SerializeField] TextMeshProUGUI shellShardsUiTxt;


	private bool inConversion;
	[Space] [SerializeField] GameObject conversionUi;
	[SerializeField] CanvasGroup inventoryI;
	[SerializeField] Button conversionBtn;
	[SerializeField] Button conversionConfirmBtn;
	[SerializeField] GameObject collectedUi;

	[Space] [SerializeField] GameObject pauseMenu;
	[SerializeField] Animator pauseAnim;
	

	[Space] [SerializeField] GameObject pause2Menu;
	[SerializeField] bool canExitPause2Menu=true;
	[SerializeField] Animator pause2Anim;

	[Space] [SerializeField] Animator[] iconAnims;
	[SerializeField] GameObject[] otherUis;
	[SerializeField] RectTransform mapUi;
	[SerializeField] Animator mapUiAnim;
	[SerializeField] float mapScrollSpeed=5;
	private bool inMap;
	private int nIcon=1;

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
	private bool movingVertically;	// jumping or falling through new scene 
	private bool movingVerticallyJumping;
	private int jumpExitDir;
	private bool canMove;
	private bool canMoveBegin;
	private bool canMoveHorz;


	[Space] [Header("DOOR")]
	[SerializeField] bool isDoorExit;
	[SerializeField] bool movingToNextScene;
	[SerializeField] bool moveIntoDoor;
	[SerializeField] bool moveOutOfDoor;
	[SerializeField] Coroutine moveThruCo;


	[Space] [Header("SHOP")]
	[SerializeField] bool isNearShop;
	[SerializeField] bool isAtShop;
	private NPC npc; // current npc nearby
	private int nAaronTalked;
	private Interactable interactable;
	[SerializeField] GameObject shopCanvas;
	[SerializeField] GameObject shopCam;
	[SerializeField] Animator shopAnim;
	[SerializeField] UiDialogue uiDialogue;
	[SerializeField] UiShopHighlight uiShopHighlight;


	[Space] [Header("MAP")]
	[SerializeField] bool isUsingMapAnim;
	[SerializeField] PlayerMap playerMap;
	[SerializeField] PlayerMap playerWorldMap;
	[SerializeField] Animator mapAnim;

	
	[Space] [SerializeField] Transform safeZonePos;
	private Vector2 roomStartPos;

	[Space] [Header("TUTORIALS")]
	[SerializeField] Tutorial activeTutorial;
	[SerializeField] Tutorial[] tutorials;
	[Space] [SerializeField] Tutorial repairTutorial;
	
	bool gainSilkFirstTime;
	bool canUseSkillFirstTime;
	bool canUseBindFirstTime;
	bool useSkillFirstTime;
	bool useBindFirstTime;
	bool madeFirstPurchase;
	bool seenInventoryTutorial;
	bool openInventoryFirst;
	bool firstTimeInTemple;



	[Space] [Header("DEBUG")]
	[SerializeField] [Range(1,10)] int silkMultiplier=1;
	[Range(1,10)] public float lootMultiplier=1;
	[SerializeField] bool invincible;
	[SerializeField] bool infiniteHp;
	[SerializeField] bool infiniteSilk;
	[SerializeField] bool infiniteToolUses;
	
	[Space] [SerializeField] bool canParry=true;
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
		if (greenGooPs != null) Destroy(greenGooPs);
		if (healingPs != null) Destroy(healingPs);
		if (bloodBurstPs != null) Destroy(bloodBurstPs);
		Destroy(gameObject);
	}


	// Start is called before the first frame update
	void Start()
	{
		self = transform;
		tools = new Tool[1];
		origGrav = rb.gravityScale;

		savedScene = SceneManager.GetActiveScene().name;
		savedPos = self.position;

		player = ReInput.players.GetPlayer(playerId);

		activeMoveSpeed = moveSpeed;
		bindPs.transform.parent = null;
		if (greenGooPs != null) greenGooPs.transform.parent = null;
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
		if (iconAnims != null && nIcon < iconAnims.Length && iconAnims[nIcon] != null)
			iconAnims[nIcon].SetBool("isSelected", true);

		gm = GameManager.Instance;
		if (gm != null)
			rm = gm.rm;

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
			hp = maxHp + nBonusHp;
			SetUiHp();
		}

		if (startWalkingIn)
			StartCoroutine( MoveOutOfStartCo() );
		else
			StartCoroutine( StartCo() );

		PlayBackgroundMusic();
		if (playerWorldMap != null)
			playerWorldMap.Setup();
		NewLevelFound(SceneManager.GetActiveScene().name);
	}

	bool CanControl()
	{
		return (!isLedgeGrabbing && !ledgeGrab && !beenHurt && !noControl && !isBinding &&
			!inAnimation && !isDead && !inStunLock && !isResting && !isPaused && !inRushSkill 
			&& !isFinished && !isAtShop && !isRespawning
		);
	}
	bool CanControlForJump()
	{
		return (!isLedgeGrabbing && !ledgeGrab && !beenHurt && !isBinding &&
			!inAnimation && !isDead && !inStunLock && !isResting && !isPaused && !inRushSkill 
			&& !isFinished && !isAtShop && !isRespawning
		);
	}

	public void HideShopCam()
	{
		shopCam.SetActive(false);
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
		if (isAtShop && npc != null)
			npc.ToggleTextbox(true);
		isPaused = false;
		pauseCo = null;
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
				if (rosariesUiTxt != null)
					rosariesUiTxt.text = nRosaries.ToString();
			}
		}

		if (nShellShards != oldShellShards)
		{
			oldShellShards = nShellShards;
			shellShardsTxt.text = nShellShards.ToString();
			shellShardsUiTxt.text = $"{nShellShards}/{maxShellShards}";
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

		// dialogue
		if (uiDialogue.gameObject.activeSelf && 
			(player.GetButtonDown("Yes") || player.GetButtonDown("No")))
		{
			uiDialogue.NextLine();
		}
		// inventory open
		else if (!isPaused && !isAtShop && pauseAnim != null && player.GetButtonDown("Minus"))
		{
			isPaused = true;
			isPauseMenu1 = true;
			pauseMenu.SetActive(true);
			if (iconAnims != null && nIcon < iconAnims.Length && iconAnims[nIcon] != null)
				iconAnims[nIcon].SetBool("isSelected", true);
			pauseAnim.SetTrigger("open");
			if (isResting && madeFirstPurchase && !openInventoryFirst)
			{
				openInventoryFirst = true;
				DeactivateTutorial(8);
			}
		}
		// pause open
		else if (!isPaused && !isAtShop && pause2Anim != null && player.GetButtonDown("Start"))
		{
			isPaused = true;
			isPauseMenu1 = false;
			pause2Menu.SetActive(true);
			pause2Anim.SetTrigger("open");
		}
		// inventory close
		else if (isPaused)
		{
			if (inConversion)
			{
				// exit zoom map
				if (player.GetButtonDown("No"))
				{
					ToggleConversionUi(false);
				}
				// exit everything
				else if (player.GetButtonDown("Minus"))
				{
					ToggleConversionUi(false);
					if (isPauseMenu1)
					{
						pauseAnim.SetTrigger("close");
						toolGaugeUi.SetActive(tool1 != null);
					}
					if (!isPauseMenu1 && canExitPause2Menu)
						pause2Anim.SetTrigger("close");
				}
			}
			// In Map UI
			else if (inMap)
			{
				float y = player.GetAxis("Move Vertical");
				float x = player.GetAxis("Move Horizontal");
				mapUi.localPosition -= (new Vector3(x, y) * mapScrollSpeed);
				// exit zoom map
				if (player.GetButtonDown("No"))
				{
					ZoomOutMap();
				}
				// exit everything
				else if (player.GetButtonDown("Minus"))
				{
					ZoomOutMap();
					if (isPauseMenu1)
					{
						pauseAnim.SetTrigger("close");
						toolGaugeUi.SetActive(tool1 != null);
					}
					if (!isPauseMenu1 && canExitPause2Menu)
						pause2Anim.SetTrigger("close");
				}
			}
			else
			{
				// close inventory
				if (isPauseMenu1 && (player.GetButtonDown("No") || player.GetButtonDown("Minus")))
				{
					ZoomOutMap();
					pauseAnim.SetTrigger("close");
					toolGaugeUi.SetActive(tool1 != null);
				}
				// close Pause
				if (!isPauseMenu1 && canExitPause2Menu && (player.GetButtonDown("No") || player.GetButtonDown("Start")))
					pause2Anim.SetTrigger("close");
			}
		}
		// map
		else if (isUsingMapAnim && isUsingMap)
		{
			if (!player.GetButton("Map") || !isGrounded)
			{
				isUsingMap = false;
				anim.SetBool("isUsingMap", false);
				if (mapAnim != null) mapAnim.SetFloat("speed", -2);
			}
			if (CanControl() && !inShawAtk)
				CalcMove();
		}
		// basic movement
		else if (CanControl() && !inShawAtk)
		{
			if (!inAirDash)
			{
				if (player.GetButtonDown("Attack") && atkCo == null && !anim.GetBool("isAttacking"))
					Attack();
				else if (player.GetButtonDown("Skill"))
					SkillAttack();

				// bind (heal)
				else if (player.GetButtonDown("Bind") && (infiniteSilk || nSilk >= GetBindCost()) && bindCo == null)
					bindCo = StartCoroutine( BindCo() );

				// tools
				else if (tool1 != null && player.GetButtonDown("Tool") && toolCo == null && !anim.GetBool("isAttacking"))
				{
					int tool = 0;
					if (infiniteToolUses || (tool == 0 && tool1.usesLeft > 0))
						toolCo = StartCoroutine( UseToolCo(0) );
				}

				// rest on bench
				else if (!isResting && bench != null && isGrounded && player.GetAxis("Move Vertical") > 0.85f)
				{
					DeactivateTutorial(0);
					isResting = true;
					needToRestObj.SetActive(false);
					isDashing = jumpDashed = false;
					rb.gravityScale = 0;
					rb.velocity = Vector2.zero;
					anim.SetBool("isResting", true);
					if (bench != null) bench.ToggleTextbox(false);
					startPosition = transform.position;

					if (repairTutorial != null && tool1 != null)
					{
						repairTutorial.gameObject.SetActive(false);
						repairTutorial.gameObject.SetActive(true);
					}
				}

				// Open Shop
				else if (isNearShop && isGrounded && player.GetAxis("Move Vertical") > 0.85f)
				{
					if (npc != null)
					{
						// Introduction chat
						if (nAaronTalked == 0)
							uiDialogue.SetLines(npc.dialogue[nAaronTalked].lines);

						if (!uiDialogue.IsDefaultText())
							nAaronTalked++;

						// sold out
						if (uiShopHighlight != null && !uiShopHighlight.HasStuffToPurchase())
						{
							if (nGoldenWatermelons > 0)
							{
								uiDialogue.SetLines(npc.goldenMelonDialogue[0].lines);
								uiDialogue.isGoldenWatermelonAfter = true;
							}
							else
							{
								uiDialogue.SetLines(npc.soldDialogue[0].lines);
								uiDialogue.isShopAfter = false;
							}
						}
						else
							uiDialogue.isShopAfter = true;
						uiDialogue.gameObject.SetActive(true);
						SetMainUI(false);
						npc.ToggleTextbox(false);
					}
					isAtShop = true;
					// shopCanvas.SetActive(true);
					// shopCam.SetActive(true);
					rb.velocity = new Vector2(0, rb.velocity.y);
					anim.SetBool("isWalking", false);
				}

				// Interactable
				else if (interactable != null && isGrounded && !movingToNextScene && 
					player.GetAxis("Move Vertical") > 0.85f)
				{
					interactable.ToggleTextbox(false);
					interactable.Interact();
					rb.velocity = new Vector2(0, rb.velocity.y);
					anim.SetBool("isWalking", false);
				}
			}

			// look at map
			if (player.GetButtonDown("Map") && isGrounded)
			{
				if (activeTutorial == tutorials[9])
					DeactivateTutorial(9);
				isUsingMap = true;
				anim.SetBool("isUsingMap", true);
				if (mapAnim != null) mapAnim.SetFloat("speed", 1);
			}

			// jump
			// JumpMechanic();

			// dash
			DashMechanic();

			CalcMove();
			if (atkCo == null)
				CheckCanLedgeGrab();
			// Ledge Grab
			if (canLedgeGrab && !isWallJumping && !isLedgeGrabbing && !ledgeGrab)
				LedgeGrab();
		}
		// manual repair tools
		else if (isResting && canUnrest && player.GetButtonDown("Attack"))
		{
			RepairTools();
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
			if (bench != null)
				bench.ToggleTextbox(true);
			if (repairTutorial != null && repairTutorial.gameObject.activeSelf)
			{
				repairTutorial.anim.SetTrigger("close");
			}
		}
		// leave shop
		else if (isAtShop && shopCanvas.activeSelf && player.GetButtonDown("No"))
		{
			if (madeFirstPurchase && !seenInventoryTutorial)
			{
				seenInventoryTutorial = true;
				ActivateTutorial(8);
			}
			if (shopAnim != null)
				shopAnim.SetTrigger("close");
			else
			{
				shopCanvas.SetActive(false);
				shopCam.SetActive(false);
			}
			if (npc != null)
			{
				SetMainUI(false);
				// has golden watermelons
				if (nGoldenWatermelons > 0)
				{
					uiDialogue.SetLines(npc.goldenMelonDialogue[0].lines);
					uiDialogue.isGoldenWatermelonAfter = true;
				}
				// goodbye text
				else
				{
					uiDialogue.SetLines(npc.endDialogue[0].lines);
					uiDialogue.isGoldenWatermelonAfter = false;
				}
				uiDialogue.isShopAfter = false;
				uiDialogue.gameObject.SetActive(true);
				npc.ToggleTextbox(false);
			}
		}
	
		if (CanControlForJump() && !inShawAtk)
		{
			JumpMechanic();
		}
	}

	void FixedUpdate()
	{
		if (isDead)
			rb.velocity = Vector2.zero;
		if (stuckToNewScene)
		{
			transform.position = this.newScenePos;
		}
		if (CanControlForJump() && !inShawAtk)
		{
			JumpCalculation();
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
				// CheckIsInWater();
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

				CheckIsGrounded();
				CoyoteTimeMechanic();
				// CheckIsInWater();
				CheckIsWalled();

				// Dash
				CalcDash();
				LookAround();

				// Normal movement
				if (!inAirDash)
					Move();
				// Air dashed
				else if (!isWallJumping)
					rb.velocity = new Vector2(model.localScale.x * dashSpeed * airDashMultiplier, rb.velocity.y / 2);

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
		}
		else if (!isDead && bench != null && isResting && t < 1)
		{
			anim.SetFloat("restTime", t);
			t += Time.fixedDeltaTime/stunLockTime;
			transform.position = Vector3.Lerp(startPosition, bench.restPos.position, t);
		}
		if (toolUses1 != null && tool1 != null)
		{
			if (nToolSlowUses1 != tool1.usesLeft)
			{
				nToolSlowUses1 = Mathf.Lerp(nToolSlowUses1, tool1.usesLeft, t1);
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
			if (nToolSlowUses2 != tool2.usesLeft)
			{
				nToolSlowUses2 = Mathf.Lerp(nToolSlowUses2, tool2.usesLeft, t2);
				t2 += 0.5f * Time.fixedDeltaTime;
				toolUses2.fillAmount = nToolSlowUses2/tool2.GetTotalUses();
			}
			else
			{
				t2 = 0;
			}
		}
	}

	void SetDustTrail(bool isLeafTrail)
	{
		if (leafTrailPs != null)
			leafTrailPs.gameObject.SetActive(isLeafTrail);
		if (leafWallSlideTrailPsLeft != null)
			leafWallSlideTrailPsLeft.gameObject.SetActive(isLeafTrail);
		if (leafWallSlideTrailPsRight != null)
			leafWallSlideTrailPsRight.gameObject.SetActive(isLeafTrail);
		if (dustTrailPs != null)
			dustTrailPs.gameObject.SetActive(!isLeafTrail);
		if (dustWallSlideTrailPsLeft != null)
			dustWallSlideTrailPsLeft.gameObject.SetActive(!isLeafTrail);
		if (dustWallSlideTrailPsRight != null)
			dustWallSlideTrailPsRight.gameObject.SetActive(!isLeafTrail);
	}

	void PlayFloorDustTrail()
	{
		if (!inTemple)
		{
			if (leafTrailPs != null)
			{
				if (isDustTrailPlaying && !isGrounded)
				{
					isDustTrailPlaying = false;
					leafTrailPs.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				}
				else if (!isDustTrailPlaying && isGrounded && !isPlatformed)
				{
					isDustTrailPlaying = true;
					leafTrailPs.Play();
					leafTrailPs.Emit(10);
				}
			}
		}
		else
		{
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
		}
	}


	void PlayWallDustTrail()
	{
		if (!inTemple)
		{
			if (leafWallSlideTrailPsRight != null && leafWallSlideTrailPsLeft != null)
			{
				if (isWallSlideTrailPlaying && !isWallSliding)
				{
					isWallSlideTrailPlaying = false;
					leafWallSlideTrailPsRight.Stop(false, ParticleSystemStopBehavior.StopEmitting);
					leafWallSlideTrailPsLeft.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				}
				else if (!isWallSlideTrailPlaying && isWallSliding)
				{
					isWallSlideTrailPlaying = true;
					if (IsFacingRight())
					{
						leafWallSlideTrailPsLeft.Emit(10);
						leafWallSlideTrailPsLeft.Play();
					}
					else
					{
						leafWallSlideTrailPsRight.Emit(10);
						leafWallSlideTrailPsRight.Play();
					}
				}
			}
		}
		else
		{
			if (dustWallSlideTrailPsRight != null && dustWallSlideTrailPsLeft != null)
			{
				if (isWallSlideTrailPlaying && !isWallSliding)
				{
					isWallSlideTrailPlaying = false;
					dustWallSlideTrailPsRight.Stop(false, ParticleSystemStopBehavior.StopEmitting);
					dustWallSlideTrailPsLeft.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				}
				else if (!isWallSlideTrailPlaying && isWallSliding)
				{
					isWallSlideTrailPlaying = true;
					if (IsFacingRight())
					{
						dustWallSlideTrailPsLeft.Emit(10);
						dustWallSlideTrailPsLeft.Play();
					}
					else
					{
						dustWallSlideTrailPsRight.Emit(10);
						dustWallSlideTrailPsRight.Play();
					}
				}
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
			rb.gravityScale = origGrav;

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
		if (isDashing && !player.GetButton("Dash") && dashCounter <= 0)
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

	private bool holdingJumpButton;
	void JumpMechanic()
	{
		// Wall jump
		if (isWallJumping)
		{
			// wallJumpTimer += Time.deltaTime;

			// reach jump hold threshold
			if (wallJumpTimer >= wallJumpThreshold)
			{
				isWallJumping = false;
				wallJumpTimer = 0;
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * maxJumpCutoffForce);
			}
			// release jump button
			else if (!player.GetButton("Jump") && wallJumpTimer >= wallJumpMin)
			{
				isWallJumping = false;
				wallJumpTimer = 0;
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutoffForce);
			}
		}
		// Regular jump
		else
		{
			// First Frame of Jump
			if (player.GetButtonDown("Jump"))
			{
				jumpBufferTimer = 0;
				jumpRegistered = true;
			}
			// if (jumpRegistered && jumpBufferTimer < jumpBufferThreshold)
			// {
			// 	jumpBufferTimer += Time.fixedDeltaTime;
			// }
			// First Frame of Jump
			if (!isJumping && jumpBufferTimer < jumpBufferThreshold && coyoteTimer < coyoteThreshold)
			{
				jumpRegistered = false;
				jumpBufferTimer = jumpBufferThreshold;
				Jump();
			}
			// Released jump button
			else if (player.GetButtonUp("Jump") || CheckIsCeiling() || isLedgeGrabbing)
			{
				if (isJumping)
					rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutoffForce);
				jumpRegistered = isJumping = false;
				coyoteTimer = coyoteThreshold;
			}
			// Holding jump button
			else if (isJumping && player.GetButton("Jump"))
			{
				// if (!usingSkill && jumpTimer < jumpMaxTimer)
				// {
				// 	rb.velocity = new Vector2(rb.velocity.x, jumpForce);
				// 	jumpTimer += Time.deltaTime;
				// }
				// // jump over
				// else
				// {
				// 	if (isJumping)
				// 		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * maxJumpCutoffForce);
				// 	isJumping = false;
				// 	jumpTimer = 0;
				// }
			}
			// wall sliding
			else if (isWallSliding && player.GetButtonDown("Jump"))
			{
				WallJump();
			}
		}
	}

	void JumpCalculation()
	{
		if (isWallJumping)
		{
			wallJumpTimer += Time.fixedDeltaTime;
		}
		else
		{
			if (jumpRegistered && jumpBufferTimer < jumpBufferThreshold)
			{
				jumpBufferTimer += Time.fixedDeltaTime;
			}
			if (isJumping && player.GetButton("Jump"))
			{
				if (!usingSkill && jumpTimer < jumpMaxTimer)
				{
					rb.velocity = new Vector2(rb.velocity.x, jumpForce);
					jumpTimer += Time.deltaTime;
				}
				// jump over
				else
				{
					if (isJumping)
						rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * maxJumpCutoffForce);
					isJumping = false;
					jumpTimer = 0;
				}
			}
		}
	}

	void ResetAllBools()
	{
		npc = null;
		interactable = null;
		isAtShop = false;
		isUsingMap = false;
		anim.SetBool("isUsingMap", false);
		if (mapAnim != null) mapAnim.SetFloat("speed", -2);
		shopCanvas.SetActive(false);
		shopCam.SetActive(false);
		stuckToNewScene = false;
		isJumping = jumpDashed = jumped = false;
		airDashed = isDashing = false;
		canLedgeGrab = ledgeGrab = false;
		jumpTimer = 0;
		rb.gravityScale = origGrav;
		isWallSliding = false;
		isWallJumping = false;
		wallJumpTimer = 0;
		rb.gravityScale = 1;
		activeMoveSpeed = moveSpeed;
		bindCo = null;
		noControl = false;
		anim.SetBool("isAirDash", false);
	}

	void LookAround()
	{
		float temp = player.GetAxis("Move Vertical");
		
		if (!isGrounded || (lookLerpTimer <= 0 && player.GetButtonDown("Attack")))
		{
			lookTimer = 0;
		}

		if (isGrounded && (temp >= 0.85f || temp <= -0.85f))
		{
			if (!camOffsetReset)
			{
				camOffsetReset = true;
				camOffset = new Vector3(0,temp > 0 ? lookOffset : -lookOffset);
			}
			if (lookTimer < lookThreshold)
			{
				lookTimer += Time.deltaTime;
			}
			else if (lookLerpTimer < lookLerpThres)
			{
				lookLerpTimer = Mathf.Min(1, lookLerpTimer + Time.deltaTime * lookOffsetMultiplier);
				CinemachineMaster.Instance.SetCamOffset(camOffset, lookLerpTimer);
			}
		}
		else if (lookTimer > 0 || lookLerpTimer > 0)
		{
			if (camOffsetReset)
			{
				camOffsetReset = false;
			}
			lookTimer = 0;
			lookLerpTimer = Mathf.Max(0, lookLerpTimer - Time.deltaTime * lookOffsetMultiplier * 2);
			CinemachineMaster.Instance.SetCamOffset(camOffset, lookLerpTimer);
		}
	}

	void CalcMove()
	{
		float temp = player.GetAxis("Move Horizontal");
		moveX = (temp < 0.4f && temp > -0.4f) ? 0 : temp;
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
				return nextSceneSpeed * activeMoveSpeed * 1.1f;
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
						movingVerticallyJumping ? jumpingOutOfSceneForce : rb.velocity.y
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
				if (canMove && !moveOutOfDoor)
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

		// quicker fall speed
		if (!jumpDashed && !isGrounded && !inShawAtk && !isJumping && !isWallSliding && rb.velocity.y < fallSpeed)
			rb.gravityScale = fallGrav;
		else
			rb.gravityScale = origGrav;

		if (isWallJumping)
		{
			rb.velocity = new Vector2(
				(model.localScale.x > 0 ? -1 : 1) * wallJumpForce.x, 
				wallJumpForce.y
			);

			if (wallJumpTimer < wallJumpControlThreshold)
			{
				rb.velocity = new Vector2(
					wallJumpDir * wallJumpForce.x, 
					wallJumpForce.y
				);
			}
			else
			{
				rb.velocity = new Vector2(
					moveX * activeMoveSpeed, 
					wallJumpForce.y
				);
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
			}
		}
			
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
			rb.velocity = new Vector2(x * activeMoveSpeed * (isUsingMap ? 0.25f : 1), rb.velocity.y);
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

		PlayFloorDustTrail();
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
		PlayWallDustTrail();
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
		coyoteTimer = coyoteThreshold;

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

			// reaper and beast crest dash attack
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
				else
				{
					jumpRegistered = isJumping = false;
					coyoteTimer = coyoteThreshold;
				}
			}
			// normal slash
			else
			{
				MusicManager.Instance.PlayHornetAtkSfx(atk1);
				atk1 = !atk1;
			}

			// yield return new WaitForSeconds(0.25f);
			// if (crestNum == 1)
			// 	yield return new WaitForSeconds(0.25f);
			// else
				yield return new WaitForSeconds(atkDir != 2 ? 0.25f : 0.4f);
			anim.SetBool("isAttacking", false);
			anim.SetFloat("crestNum", crestNum);
		}

		// atk cooldown (rising atk)
		yield return new WaitForSeconds(
			(crestNum > 1 && atkDir == 1) ? quickAtkCooldownDuration : atkCooldownDuration
		);
		atkCo = null;
	}
	
	void SkillAttack()
	{
		if ((infiniteSilk || nSilk >= skillStabCost))
			atkCo = StartCoroutine( SkillAttackCo() );
	}

	IEnumerator SkillAttackCo()
	{
		anim.SetBool("isAttacking", false);
		usingSkill = true;

		CancelDash();
		isWallJumping = jumpDashed = false;
		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;

		if (!useSkillFirstTime)
		{
			useSkillFirstTime = true;
			DeactivateTutorial(6);
		}

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

	public void ResetMelonSwordDmg()
	{
		if (melonSword != null)
			melonSword.level = 0;
		tool1Version.text = melonSword.GetToolLevel(melonSword.level + 1);
		if (melonSwordCo != null)
			StopCoroutine(melonSwordCo);
		melonSwordCo = StartCoroutine( RechargeMelonSwordUseCo() );
	}
	public void IncreaseMelonSwordDmg()
	{
		if (melonSword != null)
			melonSword.level += 1;
		tool1Version.text = melonSword.GetToolLevel(melonSword.level + 1);
		melonSword.usesLeft = 1;
		refillUses = true;
		if (melonSwordCo != null)
			StopCoroutine(melonSwordCo);
		melonSwordCo = StartCoroutine( DepleteMelonSwordUseCo() );
	}
	public int GetMelonSwordDmg()
	{
		if (melonSword != null)
			return 10 + (10 * melonSword.level);
		return 0;
	}

	IEnumerator UseToolCo(int tool=0)
	{
		if (toolCo != null) yield break;
		isTool1 = (tool == 0);
		bool isSyringe = ((isTool1 || tool2 == null ? tool1 : tool2) == syringe);
		bool isMelonSword = ((isTool1 || tool2 == null ? tool1 : tool2) == melonSword);
		
		if (isWallSliding)
			Flip();

		CancelDash();
		
		// melon sword tool
		if (isMelonSword)
		{
			rb.gravityScale = 0;
			rb.velocity = Vector2.zero;
			isJumping = false;
			CancelDash();
			atkDir = 6;
		}
		// syringe tool
		else if (isSyringe)
		{
			rb.gravityScale = 0;
			rb.velocity = Vector2.zero;
			isJumping = false;
			CancelDash();
			atkDir = 5;
		}
		// ordinary tool
		else
			atkDir = 4;

		anim.SetFloat("atkDir", atkDir);
		anim.SetTrigger("attack");
		anim.SetBool("isAttacking", true);
		MusicManager.Instance.PlayHornetAtkSfx(atk1);

		if (isMelonSword)
			yield return new WaitForSeconds(0.75f);
		else
			yield return new WaitForSeconds(isSyringe ? 0.5f : 0.25f);
		anim.SetBool("isAttacking", false);
		if (isSyringe || isMelonSword)
			rb.gravityScale = fallGrav;

		// atk cooldown
		yield return new WaitForSeconds(
			toolCooldownDuration * (tool1 != null && tool1.quickCooldown ? 0.5f : 1f)
		);
		toolCo = null;
	}

	public void USE_TOOL()
	{
		// melon sword tool
		if ((isTool1 || tool2 == null ? tool1 : tool2) == melonSword)
		{
			if (!infiniteToolUses)
			{
				if (isTool1) tool1.usesLeft--;
				else tool2.usesLeft--;
			}
		}
		// syringe tool
		else if ((isTool1 || tool2 == null ? tool1 : tool2) == syringe)
		{
			if (!infiniteToolUses)
			{
				if (isTool1) tool1.usesLeft--;
				else tool2.usesLeft--;
			}
		}
		// ordinary tools
		else
		{
			var tool = Instantiate( 
				isTool1 || tool2 == null ? tool1 : tool2, 
				toolSummonPos.position, 
				Quaternion.identity
			);
			tool.toRight = IsFacingRight();
			tool.inAir = !isGrounded;
			tool.isMaster = true;
			if (!infiniteToolUses)
			{
				if (isTool1) tool1.usesLeft--;
				else tool2.usesLeft--;
			}

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
	}

	Coroutine melonSwordCo;
	// slowly go back to level 1
	public IEnumerator DepleteMelonSwordUseCo()
	{
		melonSwordMagicAnim.SetTrigger("reset");
		yield return new WaitForSeconds(melonSwordRechargeTime);
		if (tool1 == melonSword)
		{
			if (melonSword != null)
				melonSword.level = 0;
			tool1Version.text = melonSword.GetToolLevel(melonSword.level + 1);
		}
		melonSwordCo = null;
	}
	// missed completely
	public IEnumerator RechargeMelonSwordUseCo()
	{
		melonSwordMagicAnim.SetTrigger("deplete");
		melonSwordReadyPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);

		yield return new WaitForSeconds(melonSwordComboTime);
		melonSwordReadyPs.Play();
		melonSwordMagicAnim.SetTrigger("recharge");
		if (tool1 == melonSword)
		{
			melonSword.usesLeft = 1;
			refillUses = true;
		}
		if (melonSwordSparklePs != null)
		{
			melonSwordSparklePs.SetActive(false);		
			melonSwordSparklePs.SetActive(true);
		}
		melonSwordCo = null;
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

			// is melon sword tool
			if ((isTool1 || tool2 == null ? tool1 : tool2) == melonSword)
			{
				melonSwordMagicAnim.gameObject.SetActive(true);
				melonSwordMagicAnim.SetTrigger("recharge");
				if (melonSwordCo != null)
					StopCoroutine(melonSwordCo);
				tool1.usesLeft = 1;
			}
			else
			{
				melonSwordMagicAnim.gameObject.SetActive(false);
			}
			if (repairTutorial != null && tool1 != null)
			{
				repairTutorial.gameObject.SetActive(false);
				repairTutorial.gameObject.SetActive(true);
			}
			tool1Version.text = tool.GetToolLevel(tool.level + 1);
			return true;
		}
		currToolUi = null;
		currTool = null;
		toolsEquipped[0].sprite = emptySpr;
		toolIcons[0].sprite = emptySpr;
		tools[0] = null;
		tool1 = null;
		nEquipped--;
		melonSwordMagicAnim.gameObject.SetActive(false);
		FullRestore();
		if (repairTutorial != null && repairTutorial.gameObject.activeSelf)
		{
			repairTutorial.anim.SetTrigger("close");
		}
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
					int totalSilk1 = 
						(!hasExtraSpool ? maxSilk + nBonusSilk + 3 + nExtraSpoolBonus : maxSilk + nBonusSilk);
					if (nSilk > totalSilk1)
						SetSilk(totalSilk1 - nSilk);
				}
				else
				{
					shieldHp = 2 + nShieldBonus;
					shieldImg.gameObject.SetActive(false);
				}
				ChangeSpoolNotch();
				SetUiSilk();
				return hasShield;
			// extended spool
			case 1:
				int totalSilk = 
					(!hasExtraSpool ? maxSilk + nBonusSilk + 3 + nExtraSpoolBonus : maxSilk + nBonusSilk);
				if (nSilk > totalSilk)
					SetSilk(totalSilk - nSilk);
				hasExtraSpool = !hasExtraSpool;
				hasShield = false;
				shieldImg.gameObject.SetActive(false);
				ChangeSpoolNotch();
				SetUiSilk();
				return hasExtraSpool;
			// loot multiplier
			case 2:
				hasExtraSpool = false;
				hasShield = false;

				// equip/unequip
				hasLootCharm = !hasLootCharm;
				if (hasLootCharm)
				{
					lootMultiplier = 1.5f + (nLootCharmBonus * 0.5f);
				}
				else
				{
					lootMultiplier = 1f;
				}
				shieldImg.gameObject.SetActive(false);
				ChangeSpoolNotch();
				SetUiSilk();
				return hasLootCharm;
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
			anim.SetBool("isAirDash", false);
			CancelDash();
			airDashed = false;
			isWallSliding = false;
			
			wallJumpTimer = 0;
			isWallJumping = true;
			wallJumpDir = (model.localScale.x > 0 ? -1 : 1);
			model.localScale = new Vector3(wallJumpDir, 1, 1);
			// Invoke("ResetWallJump", 0.25f);
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
		isWallJumping = jumpRegistered = isJumping = jumped = false;
		wallJumpTimer = 0;
		coyoteTimer = coyoteThreshold;
		jumpBufferTimer  = jumpBufferThreshold;
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
		isJumping = isWallSliding = canLedgeGrab = ledgeGrab = false;
		dashCooldownCounter = dashCounter = 0;
		rb.gravityScale = fallGrav;
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

	public void FullRestore(bool clearShadowRealmList=false)
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
	}

	public void RepairTools()
	{
		if (tool1 != null && toolUses1 != null)
		{
			int maxReplenished = Mathf.Max(0, tool1.GetTotalUses() - tool1.usesLeft); 
			if (maxReplenished > 0)
			{
				int nReplenished = (tool1.repairCost == 0 ? 0 :
					Mathf.Clamp(nShellShards / tool1.repairCost, 0, maxReplenished)
				);
				nShellShards -= (nReplenished * tool1.repairCost);
				tool1.usesLeft += nReplenished;
				refillUses = true;
			}
		}
		if (tool2 != null && toolUses2 != null)
		{
			tool2.usesLeft = tool2.GetTotalUses();
		}
		if (repairTutorial != null && repairTutorial.gameObject.activeSelf)
		{
			repairTutorial.SetRepairCost();
		}
	}
	public int GetRepairCost()
	{
		if (tool1 != null && toolUses1 != null)
		{
			int maxReplenished = Mathf.Max(0, tool1.GetTotalUses() - tool1.usesLeft); 
			if (maxReplenished > 0 && tool1.repairCost != 0)
			{
				int nReplenished = Mathf.Clamp(nShellShards / tool1.repairCost, 0, maxReplenished);
				return nReplenished * tool1.repairCost;
			}
		}
		return 0;
	}
	public string GetNumberOfToolsFixed()
	{
		if (tool1 != null && tool1.repairCost != 0)
		{
			int maxReplenished = Mathf.Max(0, tool1.GetTotalUses() - tool1.usesLeft); 
			int nReplenished = Mathf.Clamp(nShellShards / tool1.repairCost, 0, maxReplenished);
			return $"Craft {tool1.usesLeft + nReplenished}/{tool1.GetTotalUses()} Uses";
		}
		return "";
	}

	public void ShawRetreat(bool dashStrike=false, float multiplier=1)
	{
		isJumping = false;
		jumpTimer = 0;
		jumpDashed = false;
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
					rb.velocity = new Vector2(rb.velocity.x , 1);
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
		rb.velocity = new Vector2(0, rb.velocity.y);
		isDashing = false;
		jumpDashed = false;

		rb.AddForce( new Vector2(-moveDir * recoilForce * multiplier, 0), ForceMode2D.Impulse);
		StartCoroutine( RegainControlCo(0.1f) );
	}
	public void RisingAtkRetreat(float multiplier=1)
	{
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

	public void CallOnTakeDamage(Transform other)
	{
		if (CanBeHurt() && hurtCo == null)
			hurtCo = StartCoroutine( TakeDamageCo(other) );
	}

	private bool CanBeHurt()
	{
		return (!isDead && !invulnerable && !invincible && !justParried && !inInvincibleAnim && !movingToNextScene);
	}
	private bool CanBeHurtNotInvincible()
	{
		return (!isDead && !justParried && !inInvincibleAnim && !movingToNextScene && !isRespawning);
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		if (CanBeHurtNotInvincible() && other.CompareTag("QuickDeath"))
		{
			isRespawning = true;
			if (hurtCo != null)
			{
				StopCoroutine(hurtCo);
				foreach (SpriteRenderer sprite in sprites)
					sprite.material = defaultMat;
				hurtCo = null;
			}
			hurtCo = StartCoroutine( TakeDamageCo(other.transform) );
		}
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
		if (!isDead && other.CompareTag("SafeZone"))
		{
			safeZonePos = other.transform;
		}
		if (!isDead && other.CompareTag("NPC") && hurtCo == null)
		{
			isNearShop = true;
			npc = other.GetComponent<NPC>();
			if (npc != null)
				npc.ToggleTextbox(true);
		}
		if (!isDead && other.CompareTag("Interactable") && hurtCo == null)
		{
			interactable = other.GetComponent<Interactable>();
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
		if (!isDead && !moveOutOfDoor && !movingToNextScene && moveThruCo == null && other.CompareTag("NewArea"))
		{
			NewScene n = other.GetComponent<NewScene>();

			// scene exists
			// if (UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(n.newSceneName) >= 0)
			// {
			if (!n.isDoorEntrance)
			{
				isDoorExit = n.isDoorExit;
				movingVertically = n.isVertical;
				if (movingVertically)
				{
					// jumping up to new scene
					jumpExitDir = n.GetDirection();
					movingVerticallyJumping = n.transform.position.y - self.position.y > 0;
				}
				exitPointInd = n.exitIndex;
				isUsingMap = false;
				anim.SetBool("isUsingMap", false);
				if (mapAnim != null) mapAnim.SetFloat("speed", -2);
				StartCoroutine( MoveToNextSceneCo(n.newSceneName) );
			}
			// }
		}
		if (!isFinished && other.CompareTag("Goal"))
		{
			beaten = isFinished = true;
			transitionAnim.SetTrigger("toBlack");
			Debug.Log("<color=green>Thanks for Playing</color>");
			CANCEL_DASH();

			// gm.transitionAnim.SetTrigger("toBlack");
			isCountingTime = false;
			isUsingMap = false;
			anim.SetBool("isUsingMap", false);
			if (mapAnim != null) mapAnim.SetFloat("speed", -2);
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
		if (other.CompareTag("Interactable"))
		{
			interactable = null;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag("MovingTutorial"))
		{
			transform.parent = other.transform;
		}
	}
	private void OnCollisionExit2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag("MovingTutorial"))
		{
			transform.parent = null;
		}
	}

	public void _GAIN_TEMP_HP(int tempHpToGain)
	{
		for (int i=0 ; i<tempHpToGain ; i++)
		{
			if (tempHps.Length > nTempHp && tempHps[nTempHp] != null)
			{
				tempHps[nTempHp].SetActive(true);
			}
			nTempHp++;
		}
	}
	public void LoseAllTempHp()
	{
		int n = nTempHp;
		for (int i=0 ; i<n ; i++)
		{
			nTempHp = Mathf.Max(0, nTempHp - 1);
			if (tempHps.Length > nTempHp && tempHps[nTempHp] != null)
			{
				tempHps[nTempHp].SetActive(false);
			}
		}
	}

	void LoseHp(int dmg=1)
	{
		if (!infiniteHp)
		{
			for (int i=0 ; i<dmg ; i++)
			{
				// Take temp damage
				if (nTempHp > 0)
				{
					nTempHp = Mathf.Max(0, nTempHp - 1);
					if (tempHps.Length > nTempHp && tempHps[nTempHp] != null)
					{
						tempHps[nTempHp].SetActive(false);
					}
				}
				// Take shield damage
				else if (hasShield && hp == 1 && shieldHp > 0)
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
		// Cancel Coroutine if Invincible
		if (isInvincible)
		{
			hurtCo = null;
			yield break;
		}

		// Default damage sound effect
		if (dmg == 1)
			MusicManager.Instance.PlayHurtSFX();
		// Double damage sound effect
		else
			MusicManager.Instance.PlayHurt2SFX();
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
		}
		anim.SetBool("isSkillAttacking", false);
		anim.SetBool("isGossamerStorm", false);
		anim.SetBool("isBinding", false);
		usingSkill = false;

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

			
		if (!isRespawning)
		{
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
		}
		else
		{
			rb.velocity = new Vector2(0, 5);
		}

		// respawning fade to black
		if (isRespawning)
		{
			yield return new WaitForSecondsRealtime(0.125f);
			gm.transitionAnim.SetFloat("speed", 1);
			gm.transitionAnim.SetTrigger("toBlack");
			yield return new WaitForSecondsRealtime(0.125f);
			if (!isPaused)
				Time.timeScale = 1;

			yield return new WaitForSecondsRealtime(0.251f);
			anim.SetBool("isHurt", false);
			anim.SetBool("isRespawning", true);
			gm.transitionAnim.SetTrigger("reset");
			if (safeZonePos != null)
			{
				RaycastHit2D hit = Physics2D.Raycast
				(
					safeZonePos.position, 
					Vector2.down,
					10,
					whatIsGround
				);
				self.position = (hit.collider != null ? (Vector3) hit.point : safeZonePos.position);
			}
			else
				self.position = roomStartPos;

			// Can control again
			yield return new WaitForSeconds(1f);
			anim.SetBool("isRespawning", false);
			isRespawning = beenHurt = false;

			// invincibility over
			yield return new WaitForSeconds(gm.invincibilityDuration);
			foreach (SpriteRenderer sprite in sprites)
				sprite.material = defaultMat;
			hurtCo = null;
		}
		// resume 
		else
		{
			// Can control again
			yield return new WaitForSeconds(0.25f);
			beenHurt = false;
			anim.SetBool("isHurt", false);

			// invincibility over
			yield return new WaitForSeconds(gm.invincibilityDuration);
			foreach (SpriteRenderer sprite in sprites)
				sprite.material = defaultMat;
			hurtCo = null;
		}
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
		SetMainUI(false);
		anim.SetBool("isRespawning", false);
		isRespawning = false;

		if (saveDeath)
		{
			collectedCacoon = false;
			deathScene = SceneManager.GetActiveScene().name;
			deathPos = transform.position;
			nRosaries = 0;
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
		
		// from black
		yield return new WaitForSeconds(0.05f);
		SetMainUI(true);
		if (deathAnimObj != null)
			deathAnimObj.SetActive(false);
		gm.transitionAnim.SetFloat("speed", 1);
		loadExitPoint = true;
		NewLevelFound(savedScene);

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
		isWallSliding = isWallJumping = false;
		wallJumpTimer = 0;
		anim.SetBool("isWallSliding", isWallSliding);
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

	public void NewLevelFound(string exactName)
	{
		if (playerMap != null)
		{
			playerMap.CheckForSceneInMap(exactName);
			playerMap.PlaceMarker(exactName);
		}
		if (playerWorldMap != null)
		{
			playerWorldMap.CheckForSceneInMap(exactName);
			playerWorldMap.PlaceMarker(exactName);
		}
	}
	public void SecretPathFoundMap(string exactName)
	{
		playerMap.CheckForSceneInMap(exactName);
		playerWorldMap.CheckForSceneInMap(exactName);
	}

	public void MoveThruDoor(NewScene newScene)
	{
		if (!movingToNextScene && moveThruCo == null)
		{
			canMoveBegin = movingToNextScene = invulnerable = true;
			isDoorExit = newScene.isDoorExit;
			movingVertically = false;
			exitPointInd = newScene.exitIndex;
			isUsingMap = false;
			anim.SetBool("isUsingMap", false);
			if (mapAnim != null) 
				mapAnim.SetFloat("speed", -2);
			nextSceneSpeed = newScene.GetDirection() == 1 ? 1 : -1;
			moveThruCo = StartCoroutine( MoveThruDoorCo(newScene.newSceneName) );
			
			anim.SetBool("isEntering", true);
			interactable = null;
			moveIntoDoor = true;
		}
	}

	IEnumerator MoveThruDoorCo(string newSceneName)
	{
		isWallJumping = isCountingTime = false;
		wallJumpTimer = 0;
		yield return new WaitForSeconds(0.1f);
		gm.transitionAnim.SetTrigger("toBlack");

		yield return new WaitForSeconds(0.25f);
		anim.SetBool("isWalking", true);
		// right
		if (nextSceneSpeed > 0) 
			model.localScale = new Vector3(1, 1, 1);
		// left
		else 
			model.localScale = new Vector3(-1, 1, 1);
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(newSceneName);
		rb.velocity = Vector2.zero;
		NewLevelFound(newSceneName);
	}

	IEnumerator MoveToNextSceneCo(string newSceneName)
	{
		canMoveBegin = canMove = movingToNextScene = invulnerable = true;
		if (movingVerticallyJumping)
			nextSceneSpeed = (IsFacingRight()) ? 1 : -1;
		else
			nextSceneSpeed = (rb.velocity.x > 0) ? 1 : -1;
		isWallJumping = isCountingTime = false;
		wallJumpTimer = 0;
		yield return new WaitForSeconds(0.1f);
		gm.transitionAnim.SetTrigger("toBlack");

		yield return new WaitForSeconds(0.25f);
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(newSceneName);
		rb.velocity = Vector2.zero;
		NewLevelFound(newSceneName);
	}

	public void MoveOutOfNewScene(Vector2 newScenePos)
	{
		if (movingVertically && movingVerticallyJumping)
		{
			if (jumpExitDir != 0)
				model.localScale = new Vector3(jumpExitDir == 1 ? 1 : -1, 1, 1);
			nextSceneSpeed = (IsFacingRight()) ? 1 : -1;
		} 

		StartCoroutine( MoveOutOfNewSceneCo(newScenePos) );
	}

	IEnumerator MoveOutOfNewSceneCo(Vector2 newScenePos)
	{
		isCountingTime = true;
		isWallJumping = false;
		wallJumpTimer = 0;
		isWallSliding = false;
		canMoveBegin = canMoveHorz = canMove = false;
		this.newScenePos = transform.position = newScenePos;
		if (movingVertically) stuckToNewScene = true;
		gm.transitionAnim.SetTrigger("reset");
		moveOutOfDoor = isDoorExit;
		if (moveOutOfDoor)
		{
			anim.SetBool("isWalking", false);
			anim.SetBool("isDashing", false);
			anim.SetFloat("moveSpeed", 1);
			PlayBackgroundMusic();
		}
		if (moveIntoDoor)
		{
			anim.SetBool("isEntering", false);
			PlayBackgroundMusic();
		}

		CheckForCacoon();

		// clear dust trail PS
		if (leafWallSlideTrailPsRight != null)
			leafWallSlideTrailPsRight.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (leafWallSlideTrailPsLeft != null)
			leafWallSlideTrailPsLeft.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (leafTrailPs != null)
			leafTrailPs.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (dustTrailPs != null)
			dustTrailPs.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (dustWallSlideTrailPsRight != null)
			dustWallSlideTrailPsRight.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (dustWallSlideTrailPsLeft != null)
			dustWallSlideTrailPsLeft.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

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

		// black screen over
		yield return new WaitForSeconds((movingVertically || moveOutOfDoor) ? 0.125f: 0.5f);
		if (movingVertically)
		{
			stuckToNewScene = false;
		} 
		canMove = true;
		moveThruCo = null;

		if (movingVertically && movingVerticallyJumping)
		{
			yield return new WaitForSeconds(0.1f);
			canMoveHorz = true;
		}
		
		yield return new WaitForSeconds(moveOutOfDoor ? 0.125f : 0.5f);
		isDoorExit = moveIntoDoor = moveOutOfDoor = canMoveHorz = canMove = 
			movingToNextScene = invulnerable = false;
		roomStartPos = self.position;
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
		roomStartPos = self.position;
		areaCanvas.SetActive(true);
	}

	IEnumerator StartCo()
	{
		yield return new WaitForSeconds(0.5f);
		started = canMove = movingToNextScene = invulnerable = false;
		roomStartPos = self.position;
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
		if (!useBindFirstTime)
		{
			useBindFirstTime = true;
			DeactivateTutorial(7);
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
			if ((isTool1 || tool2 == null ? tool1 : tool2) == melonSword)
				tool1.usesLeft = 1;
			LoseAllTempHp();
			FullRestore(true); // rest
			canUnrest = true;
			SetUiHp();
			SetUiSilk();
		}

		yield return new WaitForSeconds(0.1f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		anim.SetBool("isBinding", false);
	}
	public IEnumerator GreenGlowCo()
	{
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = greenMat;

		SpawnExistingObjAtSelf(greenGooPs);

		yield return new WaitForSeconds(0.1f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;
		// anim.SetBool("isBinding", false);
	}

	void SetMainUI(bool active)
	{
		if (mainUI != null)
			mainUI.SetTrigger(active ? "open" : "close");
	}

	public void _SET_UI(int n)
	{
		// already on Map
		if (n == 2 && nIcon == 2)
			ZoomInMap();

		if (n != nIcon && otherUis != null && n < otherUis.Length && otherUis[n] != null)
		{
			// disable previous ui
			if (iconAnims != null && nIcon < iconAnims.Length && iconAnims[nIcon] != null)
				iconAnims[nIcon].SetBool("isSelected", false);
			otherUis[nIcon].SetActive(false);

			// register new 
			nIcon = n;
			
			// enable new ui
			if (iconAnims != null && nIcon < iconAnims.Length && iconAnims[nIcon] != null)
				iconAnims[nIcon].SetBool("isSelected", true);
			otherUis[nIcon].SetActive(true);
		}
	}
	public void _CONVERT_TO_STRING()
	{
		if (nRosaries >= rosariesReqForConversion)
		{
			nRosaries -= rosariesReqForConversion;
			nRosaryStrings += rosaryStringConverted;
			SetRosaryStringText();
		}
	}
	public void _TOGGLE_CONVERSION_UI(int n=0)
	{
		if (isResting)
			ToggleConversionUi(n == 0);
	}
	public void ZoomInMap()
	{
		mapUiAnim.SetBool("zoomIn", true);
		iconI.interactable = false;
		icons.SetActive(false);
		highlightObj.SetActive(false);
		inMap = true;
	}

	void ZoomOutMap()
	{
		mapUiAnim.SetBool("zoomIn", false);
		iconI.interactable = true;
		icons.SetActive(true);
		highlightObj.SetActive(true);
		inMap = false;
		iconAnims[nIcon].SetBool("isSelected", true);
		iconAnims[nIcon].Play("ui_icon_select_anim", -1, 1f);
	}

	// ui related
	void ToggleConversionUi(bool active)
	{
		if (active && !isResting)
			return;
		conversionUi.SetActive(active);
		inConversion = active;

		inventoryI.interactable = !active;
		iconI.interactable = !active;
		highlightObj.SetActive(!active);
		icons.SetActive(!active);

		// back to normal ui
		if (!active)
		{
			iconAnims[nIcon].SetBool("isSelected", true);
			iconAnims[nIcon].Play("ui_icon_select_anim", -1, 1f);
			conversionBtn.Select();
		}
		else
			conversionConfirmBtn.Select();
	}
	void SetRosaryStringText()
	{
		rosaryStringsTxt.text = nRosaryStrings.ToString();
		rosaryStringsUiTxt.text = nRosaryStrings.ToString();
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

		// tutorial on silk usage
		if (!gainSilkFirstTime && nSilk > 0)
		{
			gainSilkFirstTime = true;
			ActivateTutorial(5);
		}

		// tutorial on skill attack
		if (!canUseSkillFirstTime && nSilk >= 3)
		{
			canUseSkillFirstTime = true;
			ActivateTutorial(6);
		}

		// tutorial on bind (heal)
		if (!canUseBindFirstTime && nSilk >= GetBindCost() && hp < (maxHp + nBonusHp))
		{
			canUseBindFirstTime = true;
			ActivateTutorial(7);
		}

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

	public void ToggleShop(bool active)
	{
		shopCanvas.SetActive(active);
		shopCam.SetActive(active);
		if (active)
			SetMainUI(true);
	}

	public void TradeGoldenWatermelon(bool isTrading)
	{
		if (isTrading)
		{
			nGoldenWatermelons = Mathf.Max(0, nGoldenWatermelons - 1);
			nGoldenWatermelonsTraded++;
			goldenWatermelonTxt.text = nGoldenWatermelons.ToString();
			switch (nGoldenWatermelonsTraded)
			{
				case 1:  lootCharmToolUi.SetActive(true); lootCharmToolShopUi.SetActive(true); break;
				case 2:  syringeToolUi.SetActive(true); syringeToolShopUi.SetActive(true); break;
				case 3:  melonSwordToolUi.SetActive(true); break;
			}
		}
		isAtShop = false;
		SetMainUI(true);
	}

  	public void ToggleMainUi(bool active)
	{
		isAtShop = false;
		SetMainUI(active);
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
		{
			m.PlayMusic(m.melonBgMusic, m.melonBgMusicVol);
			inTemple = false;
			SetDustTrail(true);
		}
		else if (SceneManager.GetActiveScene().name.StartsWith("Temple"))
		{
			m.PlayMusic(m.melonBgMusic2, m.melonBgMusicVol2);
			inTemple = true;
			SetDustTrail(false);
			if (!firstTimeInTemple)
			{
				firstTimeInTemple = true;
				areaCanvas.SetActive(false);
				areaCanvas.SetActive(true);
				areaCanvasTxt.text = "Watermelon Temple";
			}
		}
		else
		{
			m.PlayMusic(m.bgMusic, m.bgMusicVol);
		}
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
	public void GainShellShard(int x)
	{
		if (x > 0 && rosaryCollectPs != null)
		{
			rosaryCollectPs.Emit(2);
			var main = rosaryCollectPs.shape;
			main.rotation = new Vector3(0,0,UnityEngine.Random.Range(30,90));
		}
		nShellShards = Mathf.Clamp(nShellShards + x, 0, maxShellShards);
	}
	public void GainCollectable()
	{
		nGoldenWatermelons++;
		goldenWatermelonTxt.text = nGoldenWatermelons.ToString();
		collectedUi.SetActive(false);
		collectedUi.SetActive(true);
		StartCoroutine( FlashCo() );
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
		NewLevelFound(savedScene);
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
			case UiShopButton.Upgrade.syringe:
				return (50 * (int) Mathf.Pow(3, syringe.level));
			case UiShopButton.Upgrade.shield:
				return (50 * (int) Mathf.Pow(3, nShieldBonus));
			case UiShopButton.Upgrade.extraSpool:
				return (50 * (int) Mathf.Pow(3, nExtraSpoolBonus));
			case UiShopButton.Upgrade.lootCharm:
				return (50 * (int) Mathf.Pow(3, nLootCharmBonus));
			case UiShopButton.Upgrade.health:
				return (100 * (int) Mathf.Pow(3, nBonusHp));
			case UiShopButton.Upgrade.spool:
				return (100 * (int) Mathf.Pow(3, nBonusSilk));
		}
		return -1;
	}
	public bool CanAffordPurchase(UiShopButton.Upgrade u)
	{
		return (nRosaries + nRosaryStrings) >= GetCost(u);
	}
	public bool CanAffordDirectPurchase(int cost)
	{
		return (nRosaries + nRosaryStrings) >= cost;
	}

	public void MakePurchase(UiShopButton.Upgrade u)
	{
		// pay with just rosaries
		if (nRosaries >= GetCost(u))
			nRosaries -= GetCost(u);
		// pay with rosaries and rosary strings
		else
		{
			int remainingCost = GetCost(u) - nRosaries;
			nRosaries = 0;
			nRosaryStrings -= remainingCost;
			SetRosaryStringText();
		}
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
			case UiShopButton.Upgrade.syringe:
				syringe.level++;
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
			case UiShopButton.Upgrade.lootCharm:
				nLootCharmBonus++;
				if (hasLootCharm)
				{
					lootMultiplier = 1.5f + (nLootCharmBonus * 0.5f);
				}
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

	public void UnlockPurchase(UiShopButton.Upgrade u, int cost)
	{
		// pay with just rosaries
		if (nRosaries >= cost)
			nRosaries -= cost;
		// pay with rosaries and rosary strings
		else
		{
			int remainingCost = cost - nRosaries;
			nRosaries = 0;
			nRosaryStrings -= remainingCost;
			SetRosaryStringText();
		}

		if (!madeFirstPurchase)
		{
			madeFirstPurchase = true;
		}

		switch (u)
		{
			case UiShopButton.Upgrade.pin:
				straightPinUi.SetActive(true);
				break;
			case UiShopButton.Upgrade.pimpillo:
				pimpilloToolUi.SetActive(true);
				break;
			case UiShopButton.Upgrade.caltrop:
				caltropsToolUi.SetActive(true);
				break;
			case UiShopButton.Upgrade.sawblade:
				shawbladesToolUi.SetActive(true);
				break;
			case UiShopButton.Upgrade.syringe:
				syringeToolUi.SetActive(true);
				break;
			case UiShopButton.Upgrade.shield:
				shieldToolUi.SetActive(true);
				break;
			case UiShopButton.Upgrade.extraSpool:
				spoolToolUi.SetActive(true);
				SetUiSilk();
				break;
		}
	}

	public void _CHEAT_MONEY()
	{
		nRosaries = 100000;
	}



	// todo ------------------------------------------------------------------
	// todo ------- TUTORIAL -------------------------------------------------

	public void ActivateTutorial(int index)
	{
		if (tutorials.Length > index)
		{
			// hide prev tutorial
			if (activeTutorial != null)
			{
				activeTutorial.anim.SetTrigger("close");
				activeTutorial = null;
			}
			// show new tutorial
			activeTutorial = tutorials[index];
			if (!activeTutorial.seenTutorial)
			{
				activeTutorial.gameObject.SetActive(true);
			}
		}
	}

	public void DeactivateTutorial(int index)
	{
		if (tutorials.Length > index)
		{
			activeTutorial = tutorials[index];
			activeTutorial.anim.SetTrigger("close");
			activeTutorial = null;
		}
	}

	public string GetActionElementIdentifierName(string actionName)
	{
		if (gm == null)
			gm = GameManager.Instance;
		
		if (player == null)
			player = ReInput.players.GetPlayer(playerId);

		Debug.Log($"<color=cyan>{player.controllers.Joysticks.Count}</color>");

		ActionElementMap aem = player.controllers.maps.GetFirstButtonMapWithAction(actionName, true);
		if (aem.elementType == ControllerElementType.Axis)
		{
			return aem.elementIdentifierName;
		}
		if (aem == null)
			return "unassigned";

		return aem.elementIdentifierName;
	}

	
	// todo ------- TUTORIAL -------------------------------------------------
	// todo ------------------------------------------------------------------
}
