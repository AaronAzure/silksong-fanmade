using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] int hp=10;
	public Transform self;
	[SerializeField] Transform model;
	[SerializeField] SpriteRenderer[] sprites;
	[SerializeField] Animator anim;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] Collider2D col;


	[SerializeField] Material defaultMat;
	[SerializeField] Material dmgMat;
	[SerializeField] SortingGroup sortGroup;

	
	[Space] [Header("Platformer Related")]
	[SerializeField] float moveSpeed=2.5f;
	[SerializeField] float runSpeed=5;
	[SerializeField] Transform groundDetect;
	[SerializeField] Transform wallDetect;
	[SerializeField] float groundDistDetect=0.5f;
	[SerializeField] float wallDistDetect=1;
	[SerializeField] LayerMask whatIsPlayer;
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] LayerMask finalMask;
	private bool beenHurt;
	[Space] [SerializeField] CurrentAction currentAction=0;
	[SerializeField] float idleCounter=0;
	[SerializeField] float idleTotalCounter=5;
	[SerializeField] [Range(-1,1)] int initDir=-1;


	[Space] [Header("Target Related")]
	public PlayerControls target;
	public bool alwaysInRange;
	public bool inRange; // player in area
	public bool inSight; // player in line of sight within area
	private bool attackingPlayer;
	private float searchCounter;
	private float searchTotalCounter=5;



	[Space] [Header("Particle Effect Objs")]
	[SerializeField] GameObject silkEffectObj;
	[SerializeField] GameObject bloodEffectObj;
	[SerializeField] GameObject alert;


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

    // Start is called before the first frame update
    public virtual void Start() 
    {
		CallChildOnStart();
    }

	protected virtual void CallChildOnStart() { }

	protected virtual bool PlayerInSight()
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
			if (searchCounter > searchTotalCounter)
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

	protected void WalkAround()
	{
		idleCounter += Time.fixedDeltaTime;
		if (idleCounter > idleTotalCounter)
		{
			idleCounter = 0;
			initDir = -initDir;
			currentAction = initDir == 1 ? CurrentAction.right : CurrentAction.left;
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
	}

	protected virtual void IdleAction() {  }

	protected virtual void AttackingAction() { }

    // // Update is called once per frame
    public virtual void FixedUpdate()
    {
		if (!inSight && currentAction != 0 && CheckSurrounding())
			currentAction = CurrentAction.none;

		if (!attackingPlayer)
			IdleAction();
		else
			AttackingAction();

		inSight = PlayerInSight();
		KeepLookingForPlayer();
		if (alert != null) alert.SetActive( attackingPlayer );
    }

	public void TakeDamage(int dmg, Vector2 forceDir, float force)
	{
		StartCoroutine( TakeDamageCo(dmg, forceDir, force) );
	}

	IEnumerator TakeDamageCo(int dmg, Vector2 forceDir, float force)
	{
		hp -= dmg;
		beenHurt = true;
		float angleZ = 
			Mathf.Atan2(Mathf.Abs(forceDir.y), forceDir.x) * Mathf.Rad2Deg;
		Instantiate(silkEffectObj, transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
		Instantiate(bloodEffectObj, transform.position, Quaternion.Euler(0,0,angleZ+offset*forceDir.x));
		if (hp <= 0)
		{
			Died();
			yield break;
		}
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = dmgMat;

		rb.velocity = Vector2.zero;
        rb.velocity = forceDir * force;

		yield return new WaitForSeconds(0.2f);
		foreach (SpriteRenderer sprite in sprites)
			sprite.material = defaultMat;

		// yield return new WaitForSeconds(0.1f);
		beenHurt = false;
		rb.velocity = new Vector2(0, rb.velocity.y);
	}
	
	void Died()
	{
		col.enabled = false;
		this.gameObject.layer = 5;
		foreach (SpriteRenderer sprite in sprites)
			sprite.color = new Color(0.2f,0.2f,0.2f,1);
		this.enabled = false;
		if (alert != null) alert.SetActive( false );
		if (anim != null)
			anim.SetBool("isDead", true);
		// transform.rotation = Quaternion.Euler(0,0,90);
		sortGroup.sortingOrder = -1;
	}
	
}
