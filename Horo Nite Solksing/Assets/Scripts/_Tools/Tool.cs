using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
	[HideInInspector] public int usesLeft=0;
	public int level;
	[HideInInspector] public bool isMaster;
	[HideInInspector] public bool inAir;
	[Space] [SerializeField] bool isVarTotaluses;
	[SerializeField] int totalusesDiff;
	[SerializeField] int capTotalDiff=4;
	public int totaluses=4;
	public int repairCost=3;
	public Sprite icon;
	[field: SerializeField] public bool quickCooldown {get; private set;}
	private int upgradeCost=50;

	[Space] public int dmg;
	public bool toRight=true;
	[SerializeField] protected Vector2 dir;
	[HideInInspector] public float velocityMultiplier=1;
	[SerializeField] protected float force;
	[SerializeField] protected Vector2 kbDir;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] public bool isMultiple;
	[SerializeField] public int nCopies=4;
	[Space] [SerializeField] protected GameObject strikePs;
	[SerializeField] protected float offset=15;

	[Space] [SerializeField] bool destroyOnWallHit;
	[SerializeField] bool destroyOnEnemyHit;
	[SerializeField] bool destroyOnSpecialHit;
	[SerializeField] bool canGoThruShield;
	[HideInInspector] public bool justThrown;
	[Space] [SerializeField] bool noStart;
	[SerializeField] protected float destroyAfter;
	[SerializeField] protected Coroutine destroyAfterCo;

	private void Start() 
	{
		if (!noStart)
		{
			if (dir != Vector2.zero)
			{
				LaunchDir();
			}
			CallChildOnStart();
		}
		CallChildOnStartAlways();
		if (destroyAfter > 0)
			destroyAfterCo = StartCoroutine( DestroyAfterCo() );
	}

	public int GetTotalUses()
	{
		return isVarTotaluses ? totaluses - Mathf.Min(totalusesDiff * level, capTotalDiff) : totaluses;
	}

	protected virtual void LaunchDir()
	{
		rb.velocity = new Vector2((toRight ? dir.x : -dir.x) * velocityMultiplier, dir.y);
		kbDir = new Vector2((toRight ? kbDir.x : -kbDir.x), kbDir.y);
		gameObject.transform.localScale = new Vector3(
			(toRight ? transform.localScale.x : -transform.localScale.x),
			transform.localScale.y
		);
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (destroyOnSpecialHit && other.CompareTag("ToolBreaker"))
		{
			CallChildOnSpecialHit();
		}
		else 
		{
			if (canGoThruShield && other.CompareTag("Shield"))
			{
				other.GetComponent<EnemyShield>().Attacked(false);
			}
			if (destroyOnWallHit && 
				(other.CompareTag("Ground") || other.CompareTag("Breakable") || other.CompareTag("Shield")))
				CallChildOnHit();
			if (other.CompareTag("Enemy"))
			{
				CallChildOnEnemyHit(other);
				if (destroyOnEnemyHit)
				{
					CallChildOnHit();
				}
			}
			if (other.CompareTag("Breakable"))
			{
				CallChildOnBreakableHit(other);
				if (destroyOnEnemyHit)
				{
					CallChildOnHit();
				}
			}
		}
	}

	protected virtual void CallChildOnEnemyHit(Collider2D other)
	{
		Enemy target = other.GetComponent<Enemy>();
		Vector2 temp = (other.transform.position - transform.position).normalized;
		float angleZ = 
			Mathf.Atan2(Mathf.Abs(temp.y), temp.x) * Mathf.Rad2Deg;

		if (target != null) 
		{
			if (!target.inParryState && strikePs != null)
			{
				Instantiate(
					strikePs, 
					other.transform.position, 
					Quaternion.Euler(0,0,angleZ + offset * temp.x)
				);
			}
			
			target.TakeDamage(
				dmg, 
				transform, kbDir, 
				force
			);
		}
	}
	protected virtual void CallChildOnBreakableHit(Collider2D other)
	{
		Breakable target = other.GetComponent<Breakable>();
		if (target != null) 
		{
			target.Damage(
				dmg
			);
		}
	}
	protected virtual void CallChildOnHit() 
	{
		Destroy(gameObject);
	}
	protected virtual void CallChildOnSpecialHit() 
	{
		Debug.Log("<color=magenta>TOOL BREAKER</color>");
		Destroy(gameObject);
	}
	protected virtual void CallChildOnStart() {}
	protected virtual void CallChildOnStartAlways() {}

	protected virtual IEnumerator DestroyAfterCo()
	{
		yield return new WaitForSeconds(destroyAfter);
		Destroy(gameObject);
	}

	public virtual void DESTROY() 
	{
		Destroy(gameObject);
	}

	public string GetToolLevel(int number)
	{
		if ((number < 0) || (number > 3999)) return "-";
		if (number < 1) return string.Empty;            
		if (number >= 1000) return "M" + GetToolLevel(number - 1000);
		if (number >= 900) return "CM" + GetToolLevel(number - 900); 
		if (number >= 500) return "D" + GetToolLevel(number - 500);
		if (number >= 400) return "CD" + GetToolLevel(number - 400);
		if (number >= 100) return "C" + GetToolLevel(number - 100);            
		if (number >= 90) return "XC" + GetToolLevel(number - 90);
		if (number >= 50) return "L" + GetToolLevel(number - 50);
		if (number >= 40) return "XL" + GetToolLevel(number - 40);
		if (number >= 10) return "X" + GetToolLevel(number - 10);
		if (number >= 9) return "IX" + GetToolLevel(number - 9);
		if (number >= 5) return "V" + GetToolLevel(number - 5);
		if (number >= 4) return "IV" + GetToolLevel(number - 4);
		if (number >= 1) return "I" + GetToolLevel(number - 1);
		return "-";
	}
}
