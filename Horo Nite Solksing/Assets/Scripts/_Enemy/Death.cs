using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : Enemy
{
	public static Death Instance;
	[Space] [Header("Death")]
	[SerializeField] bool inAttackAnim;
	[SerializeField] bool tripleStrike;
	[SerializeField] float tripleStrikeForce=20;
	[SerializeField] SpriteRenderer sickleL;
	[SerializeField] SpriteRenderer sickleR;
	[Space] [SerializeField] int setAtk=-1;
	
	[Space] [SerializeField] FlameTrailProjectile flameAtk;
	[SerializeField] Transform flameAtkPos;
	private bool jumpedAgain=true;
	private Coroutine parryCo;

	[Space] [SerializeField] EnemySickle sickleAtk;
	[SerializeField] Transform[] sickleAtkPos;
	[SerializeField] GameObject[] sickles;
	[SerializeField] GameObject silkExplosionVfx;
	private int nSickleOut;
	private int nSickleRetrieved;
	private Vector2 sickleDir;
	[SerializeField] float sickleUp=0.6f;
	private int[] closeAtks={0,1,3};
	// private int[] distAtks={0,1}; 
	[Space] [SerializeField] GameObject ultAtkObj;
	[Space] [SerializeField] GameObject ultAtkDmgObj;
	[SerializeField] Transform ultAtkEndPos;
	private int ultAtkCounter;
	private int ultAtkCount;
	private bool old;


	public void Awake()
	{
		Instance = this;
	}

	public void ToggleOldVer()
	{
		old = !old;
	}

    public void ATTACK_PATTERN()
	{
		FacePlayer();
		if (!cannotAtk)
		{
			jumpedAgain = true;
			float distToTarget = Vector2.Distance(target.transform.position, self.position);
			Debug.Log(distToTarget);
			anim.SetBool("jumped", false);
			anim.SetBool("sickled", false);
			anim.SetTrigger("attack");

			int rng=0;
			if (!old)
			{
				rng = (anim.GetBool("atPhase2")) ? 
					(distToTarget < 6f ? Random.Range(0,5) : Random.Range(0,3)) :
					(distToTarget < 6f ? closeAtks[Random.Range(0, closeAtks.Length)] : Random.Range(0,2));
				if (anim.GetBool("atPhase3"))
				{
					ultAtkCount++;
					if (ultAtkCount >= ultAtkCounter)
					{
						ultAtkCount = 0;
						ultAtkCounter = Random.Range(2,5);
						anim.SetFloat("atkPattern", 100);
						return;
					}
				}
			} 
			else
			{
				rng = (distToTarget < 6f ? closeAtks[Random.Range(0, closeAtks.Length)] : Random.Range(0,2));
			}
			if (setAtk != -1)
				rng = setAtk;
			if (rng == 0)
				anim.SetBool("jumped", true);
			else if (rng == 1)
			{
				nSickleRetrieved = 0;
				anim.SetBool("sickled", true);
			}
			anim.SetFloat("atkPattern", rng);
		}
	}
	protected override void CallChildOnEarlyUpdate()
	{
		CheckIsGrounded();
		if (!isGrounded)
			anim.SetFloat("jumpVelocity", rb.velocity.y);
		if (tripleStrike)
		{
			rb.velocity = new Vector2(model.localScale.x * tripleStrikeForce, rb.velocity.y);
		}
		else if (!receivingKb && !inAttackAnim)
			rb.velocity = new Vector2(0, rb.velocity.y);
		if (hitCount > 0 && recoverTimer < recoverTime)
		{
			recoverTimer += Time.fixedDeltaTime;
			if (recoverTimer > recoverTime)
			{
				hitCount = Mathf.Max(0, hitCount - 10);
				recoverTimer = 0;
			}
		}
	}

	protected override void CallChildOnPhase2()
	{
		anim.SetBool("atPhase2", true);
	}

	protected override void CallChildOnPhase3()
	{
		anim.SetBool("atPhase3", true);
	}

	protected override void CallChildOnParry()
	{
		anim.SetTrigger("parry");
		Parry();
	}

	public override void CallChildOnRoomEnter()
	{
		anim.SetTrigger("spawn");
		
		// cannotAtk = false;
		// ATTACK_PATTERN();
	}

	protected override void CallChildOnHurt(int dmg=0)
	{
		if (anim.GetBool("isStagger"))
			return;
		hitCount += dmg;
		recoverTimer = 0;
		if (hitCount >= staggerCount)
		{
			hitCount = 0;
			rb.AddForce(Vector2.up * 4, ForceMode2D.Impulse);
			anim.SetBool("jumped", false);
			anim.SetBool("sickled", false);
			StartCoroutine( StaggerCo() );
		}
	}

	protected override void CallChildOnDeath()
	{
		silkExplosionVfx.transform.parent = null;
		StartCoroutine(DramaticFinish());
	}

	IEnumerator StaggerCo()
	{
		anim.SetBool("isStagger", true);
		CinemachineShake.Instance.ShakeCam(2.5f, 0.25f);
		Time.timeScale = 0f;
		if (ultAtkObj != null) ultAtkObj.SetActive(false);

		yield return new WaitForSecondsRealtime(0.25f);
		Time.timeScale = 1f;

		yield return new WaitForSeconds(1.5f);
		anim.SetBool("isStagger", false);
	}

	IEnumerator DramaticFinish()
	{
		Time.timeScale = 0.25f;
		if (ultAtkObj != null) ultAtkObj.SetActive(false);

		yield return new WaitForSecondsRealtime(0.5f);
		Time.timeScale = 1;
	}

	public void START_FIGHTING()
	{
		cannotAtk = false;
	}

	public void HIDE_SICKLES()
	{
		sickleL.gameObject.SetActive(false);
		sickleR.gameObject.SetActive(false);
	}

	public void REVEAL_SICKLES()
	{
		sickleL.gameObject.SetActive(true);
		sickleR.gameObject.SetActive(true);
	}

	public void FACE_PLAYER()
	{
		FacePlayer();
	}

	public void JUMP_AT_PLAYER()
	{
		if (target == null) 
			return;
		rb.AddForce(
			new Vector2(
				(target.transform.position.x - self.position.x) * 2f, 
				jumpForce
			), 
			ForceMode2D.Impulse
		);
	}

	public void JUMP_AGAIN()
	{
		// if (jumpedAgain && hp < halfHp)
		// {
		// 	jumpedAgain = false;
		// 	anim.SetBool("jumped", true);
		// 	anim.SetFloat("atkPattern", 0);
		// }
	}

	public void FLAME_ATTACK()
	{
		var obj = Instantiate(flameAtk, flameAtkPos.position, Quaternion.identity);
		obj.toRight = (model.localScale.x > 0) ? true : false;
		obj.transform.localScale = model.localScale;
	}

	public void GET_SICKLE_DIR(int x)
	{
		sickleDir = (target.transform.position + new Vector3(0,sickleUp) - sickleAtkPos[x].position).normalized;
	}

	public void THROW_SICKLE(int x)
	{
		FacePlayer();
		var obj = Instantiate(sickleAtk, sickleAtkPos[x].position, Quaternion.identity);
		obj.LaunchInDirection(sickleDir);
		obj.returnPos = sickleAtkPos[x];
		obj.death = this;
		obj.nSickle = nSickleOut;
		sickles[nSickleOut++].SetActive(false);
	}

	public void RetrieveSickle(int x)
	{
		if (!anim.GetBool("isDead"))
		{
			sickles[x].SetActive(true);
			nSickleOut--;
			if (nSickleOut <= 0)
			{
				anim.SetBool("sickled", false);
				anim.SetTrigger("retrieveSickle");
			}
		}
	}

	public void Parry()
	{
		if (parryCo != null) StopCoroutine( ParryCo() );
		Time.timeScale = 0;
		parryCo = StartCoroutine( ParryCo() );
		MusicManager.Instance.PlayParrySFX();
	}

	public IEnumerator ParryCo()
	{
		// Time.timeScale = 0;
		sickleL.material = dmgMat;
		sickleR.material = dmgMat;

		yield return new WaitForSecondsRealtime(0.25f);
		sickleL.material = defaultMat;
		sickleR.material = defaultMat;
		Time.timeScale = 1;
		parryCo = null;
	}

	public void SHOW_ULT_ATK_AOE()
	{
		if (ultAtkObj != null)
		{
			ultAtkObj.SetActive(false);
			ultAtkObj.SetActive(true);
		}
	}
	public void SHOW_ULT_ATK_DMG()
	{
		if (ultAtkDmgObj != null)
		{
			ultAtkDmgObj.transform.localScale = model.localScale;
			ultAtkDmgObj.SetActive(false);
			ultAtkDmgObj.SetActive(true);
		}
	}

	public void START_ULT_ATK()
	{
		model.gameObject.SetActive(false);
	}
	public void END_ULT_ATK()
	{
		model.gameObject.SetActive(true);
		if (ultAtkEndPos != null)
		{
			transform.position = ultAtkEndPos.position;
		}
	}
}
