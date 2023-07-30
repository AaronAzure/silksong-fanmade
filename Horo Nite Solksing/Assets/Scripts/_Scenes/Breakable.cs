using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Breakable : MonoBehaviour
{
	[SerializeField] bool canBreak=true;


	[Space] [SerializeField] protected Animator anim;
	[field: SerializeField] public bool hasRecoil {get; private set;}=true;
	[field: SerializeField] public bool hasShawRecoil {get; private set;}=true;
	[field: SerializeField] public bool hasDashRecoil {get; private set;}=true;
	[field: SerializeField] public bool canBeHit {get; private set;}=true;
	[SerializeField] protected int hp;
	[SerializeField] protected ParticleSystem dmgFx;
	[SerializeField] protected int minEmit=25;
	[SerializeField] protected int maxEmit=35;
	[SerializeField] protected GameObject destroyedObj;

	[Space] [SerializeField] protected Animator[] revealAnims;
	[SerializeField] protected Breakable[] breakables;
	[SerializeField] protected GameObject[] extraTiles;
	[SerializeField] protected Collider2D col;
	[SerializeField] protected GameManager gm;
	[SerializeField] protected bool isSecret;

	[Space] [SerializeField] protected string exactName;


    // Start is called before the first frame update
    void Start()
    {
        CallChildOnStart();
    }

    protected virtual void CallChildOnStart() { }

	public void Damage(int dmg)
	{
		if (canBeHit && canBreak)
			CallChildOnDamage(dmg);
	}

    protected virtual void CallChildOnDamage(int dmg)
	{
		if (hp > 0)
		{
			CinemachineShake.Instance.ShakeCam(0.75f, 0.25f, 0.5f);
			hp -= dmg;
			if (dmgFx != null)
			{
				dmgFx.Emit(Random.Range(minEmit, maxEmit+1));
			}
			if (hp > 0 && anim != null)
			{
				anim.SetTrigger("damage");
			}
			if (hp <= 0)
			{
				// Destroyed VFX
				if (destroyedObj != null)
				{
					destroyedObj.SetActive(true);
				}
				// Don't respawn
				if (gm != null)
				{
					if (exactName == "")
						gm.RegisterDestroyedList(name, isSecret);
					else
					{
						gm.RegisterDestroyedList(exactName, isSecret, true);
						PlayerControls.Instance.SecretPathFoundMap(exactName);
					}
				}
				// Reveal any secrets
				if (revealAnims != null)
				{
					foreach (Animator revealAnim in revealAnims)
						revealAnim.SetTrigger("reveal");
				}
				// Enable secret chest can be hit
				if (breakables != null)
				{
					foreach (Breakable breakable in breakables)
						if (breakable != null)
							breakable.EnableCanBeHit();
				}
				// Hide Extra Tiles 
				if (extraTiles != null)
				{
					foreach (GameObject extraTile in extraTiles)
							extraTile.SetActive(false);
				}
				// Disable colision
				if (col != null)
				{
					col.enabled = false;
				}
				// Else, destroy itself
				else
				{
					Destroy(gameObject);
				}
			}
		}
	}

	public void EnableCanBeHit()
	{
		StartCoroutine( EnableCanBeHitCo() );
	}
	private IEnumerator EnableCanBeHitCo()
	{
		yield return new WaitForSeconds(0.5f);
		canBeHit = true;
	}
}
