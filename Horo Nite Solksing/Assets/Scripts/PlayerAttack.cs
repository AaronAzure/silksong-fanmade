using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] Vector2 forceDir;
	[SerializeField] float force=5;
	[SerializeField] bool hasRecoil;
	[SerializeField] bool isShawAttack;
	[SerializeField] bool isDashAttack;
	[SerializeField] bool isRisingAttack;
	[SerializeField] bool isStabAttack;
	[SerializeField] bool isRushAttack;
	[SerializeField] bool isGossamerStorm;
	[SerializeField] GameObject strikePs;
	[SerializeField] float offset=15;
	[SerializeField] GameObject parryEffect;


	[Space] [SerializeField] bool ensureSingleHit;
	private HashSet<GameObject> alreadyHit;

	private void Awake() 
	{
		if (ensureSingleHit)
		{
			alreadyHit = new HashSet<GameObject>();
		}	
	}

	private void OnEnable() 
	{
		if (ensureSingleHit)
		{
			alreadyHit.Clear();
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (gameObject.CompareTag("Finish") && other.CompareTag("EnemyAttack"))
		{
			if (parryEffect != null)
			{
				Vector2 orig = Vector2.Lerp(other.transform.position, transform.position, 0.5f);
				Instantiate(parryEffect, orig, Quaternion.identity);
				p.Parry();
			}
			
			if (isShawAttack)
				p.ShawRetreat(isDashAttack);
			else if (isRisingAttack)
				p.RisingAtkRetreat();
		}
		if (other.CompareTag("Enemy"))
		{
			if (ensureSingleHit)
			{
				if (alreadyHit.Contains(other.gameObject))
					return;
				else
					alreadyHit.Add(other.gameObject);
			}
			Enemy target = other.GetComponent<Enemy>();
			Vector2 temp = (other.transform.position - transform.position).normalized;
			float angleZ = 
				Mathf.Atan2(Mathf.Abs(temp.y), temp.x) * Mathf.Rad2Deg;

			if (target != null) 
			{
				if (!target.inParryState)
				{
					Instantiate(
						strikePs, 
						other.transform.position, 
						Quaternion.Euler(0,0,angleZ + offset * temp.x)
					);
				}
				
				int dmg = !isStabAttack ? (!isGossamerStorm ? p.atkDmg[p.crestNum] : p.gossamerDmg) : p.stabDmg;
				if (isRushAttack)
					dmg = p.rushDmg;

				// stronger special
				if ((isStabAttack || isGossamerStorm || isRushAttack) && p.crestNum == 1)
					dmg = Mathf.RoundToInt(dmg * 1.25f);
				// weaker special
				else if ((isStabAttack || isGossamerStorm || isRushAttack) && p.crestNum == 2)
					dmg = (int) (dmg * 0.75f);
				else if ((isStabAttack || isGossamerStorm || isRushAttack) && p.crestNum == 3)
					dmg = (int) (dmg * 0.85f);
				target.TakeDamage(
					dmg, 
					isGossamerStorm ? transform : null,
					new Vector2(p.model.localScale.x * forceDir.x, forceDir.y),
					force
				);
				if (isShawAttack)
					p.ShawRetreat(isDashAttack);
				else if (isRisingAttack)
					p.RisingAtkRetreat();
				else if (hasRecoil && !p.justParried)
					p.Recoil();

				// simple attack
				if (!isStabAttack && !isGossamerStorm && !isRushAttack)
					p.SetSilk(1);
			}
		}
		else if (other.CompareTag("Ground") && hasRecoil && !p.justParried)
			p.Recoil();
	}
}
