using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] Animator anim;
	[SerializeField] int hp;
	[SerializeField] ParticleSystem dmgFx;
	[SerializeField] int minEmit=25;
	[SerializeField] int maxEmit=35;
	[SerializeField] GameObject destroyedObj;

	[Space] [SerializeField] Animator[] revealAnims;
	[SerializeField] Collider2D col;
	private GameManager gm;


	private void Start() 
	{
		gm = GameManager.Instance;
		if (gm != null && gm.CheckDestroyedList(gameObject.name))
			Destroy(gameObject);
		if (dmgFx != null)
			dmgFx.transform.parent = null;
		if (destroyedObj != null)
			destroyedObj.transform.parent = null;
		// if (revealAnim != null)
		// 	revealAnim.transform.parent = null;
	}

	public void Damage(int dmg)
	{
		if (hp > 0)
		{
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
					gm.RegisterDestroyedList(name);
				}
				// Reveal any secrets
				if (revealAnims != null)
				{
					foreach (Animator revealAnim in revealAnims)
						revealAnim.SetTrigger("reveal");
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
}
