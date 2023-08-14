using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] Vector2 forceDir;
	[SerializeField] float force=5;
	[SerializeField] bool canGoThruShield;

	[Space] [SerializeField] bool hasRecoil;
	bool melonSwordHit;
	[SerializeField] bool isMelonSword;
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
	[SerializeField] Collider2D col;
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
		if (!isMelonSword && col != null)
			col.enabled = true;
		melonSwordHit = false;
	}

	private void OnDisable() 
	{
		if (isMelonSword)	
		{
			if (melonSwordHit)
				p.IncreaseMelonSwordDmg();
			else
				p.ResetMelonSwordDmg();
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		bool hitSomething = false;
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
		if (other.CompareTag("Bouncy"))
		{
			if (ensureSingleHit)
			{
				if (alreadyHit.Contains(other.gameObject))
					return;
				else
					alreadyHit.Add(other.gameObject);
			}
			if (!isMelonSword && col != null)
				col.enabled = false;

			float bounceMultiplier = 1.3f;

			if (isShawAttack)
				p.ShawRetreat(isDashAttack, bounceMultiplier);
			else if (isRisingAttack)
				p.RisingAtkRetreat(bounceMultiplier);
			else if (hasRecoil && !p.justParried)
				p.Recoil(bounceMultiplier);
		}
		else if (other.CompareTag("Enemy"))
		{
			if (ensureSingleHit)
			{
				if (alreadyHit.Contains(other.gameObject))
					return;
				else
					alreadyHit.Add(other.gameObject);
			}
			Enemy target = other.GetComponent<Enemy>();

			if (target != null) 
			{
				hitSomething = true;

				// receive normal attack
				if (!target.inParryState && !target.isShielding)
				{
					Vector2 temp = (
						(target.mainPos != null ? target.mainPos.position : other.transform.position)
						 - transform.position).normalized;
					float angleZ = 
						Mathf.Atan2(Mathf.Abs(temp.y), temp.x) * Mathf.Rad2Deg;
					Instantiate(
						strikePs, 
						(target.mainPos != null ? target.mainPos.position : other.transform.position), 
						Quaternion.Euler(0,0,angleZ + offset * temp.x)
					);
				}

				int dmg = !isStabAttack ? (!isGossamerStorm ? p.atkDmg[p.crestNum] : p.gossamerDmg) : p.stabDmg;
				if (isMelonSword)
				{
					dmg = p.GetMelonSwordDmg();
					melonSwordHit = true;
				}
				if (isRushAttack)
					dmg = p.rushDmg;
				if (isStabAttack || isGossamerStorm || isRushAttack)
				{
					switch (p.crestNum)
					{
						// stronger special
						case 1:
							dmg = Mathf.RoundToInt(dmg * 1.3f);
							break;
						// weaker special
						case 2:
							dmg = Mathf.RoundToInt(dmg * 0.6f);
							break;
						case 3:
							dmg = Mathf.RoundToInt(dmg * 0.8f);
							break;
					}
				}

				target.TakeDamage(
					dmg, 
					isGossamerStorm ? transform : null,
					new Vector2(p.model.localScale.x * forceDir.x, forceDir.y),
					force, true, true, !canGoThruShield
				);


				// simple attack
				if (!isStabAttack && !isRushAttack && !isGossamerStorm && !isMelonSword &&
					!target.inParryState && (canGoThruShield || !target.isShielding))
					p.SetSilk(1);

				// recoil
				if (isShawAttack)
					p.ShawRetreat(isDashAttack);
				else if (isRisingAttack)
					p.RisingAtkRetreat();
				else if (hasRecoil && !p.justParried)
					p.Recoil();
			}
		}
		
		if (other.CompareTag("Shield"))
		{
			other.GetComponent<EnemyShield>().Attacked(!canGoThruShield);
		}
		

		if (other.CompareTag("SpecialEnemy"))
		{
			if (ensureSingleHit)
			{
				if (alreadyHit.Contains(other.gameObject))
					return;
				else
					alreadyHit.Add(other.gameObject);
			}

			Enemy target = other.GetComponent<Enemy>();
			if (target != null) 
			{
				hitSomething = true;
				if (!target.inParryState)
				{
					Vector2 temp = (
						(target.mainPos != null ? target.mainPos.position : other.transform.position)
						 - transform.position).normalized;
					float angleZ = 
						Mathf.Atan2(Mathf.Abs(temp.y), temp.x) * Mathf.Rad2Deg;
					Instantiate(
						strikePs, 
						(target.mainPos != null ? target.mainPos.position : other.transform.position), 
						Quaternion.Euler(0,0,angleZ + offset * temp.x)
					);
				}
				
				int dmg = !isStabAttack ? (!isGossamerStorm ? p.atkDmg[p.crestNum] : p.gossamerDmg) : p.stabDmg;
				if (isMelonSword)
				{
					dmg = p.GetMelonSwordDmg();
					melonSwordHit = true;
				}
				if (isRushAttack)
					dmg = p.rushDmg;

				if (isStabAttack || isGossamerStorm || isRushAttack)
				{
					switch (p.crestNum)
					{
						case 1:
							dmg = Mathf.RoundToInt(dmg * 1.3f);
							break;
						case 2:
							dmg = Mathf.RoundToInt(dmg * 0.6f);
							break;
						case 3:
							dmg = Mathf.RoundToInt(dmg * 0.8f);
							break;
					}
				}

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
				// else if (hasRecoil && !p.justParried)
				// 	p.Recoil();
			}
		}
		if (other.CompareTag("Breakable"))
		{
			Breakable target = other.GetComponent<Breakable>();
			if (target != null)
			{
				hitSomething = true;
				int dmg = !isStabAttack ? (!isGossamerStorm ? p.atkDmg[p.crestNum] : p.gossamerDmg) : p.stabDmg;
				if (isRushAttack)
					dmg = p.rushDmg;

				if (isStabAttack || isGossamerStorm || isRushAttack)
				{
					switch (p.crestNum)
					{
						// stronger special
						case 1:
							dmg = Mathf.RoundToInt(dmg * 1.3f);
							break;
						// weaker special
						case 2:
							dmg = Mathf.RoundToInt(dmg * 0.6f);
							break;
						case 3:
							dmg = Mathf.RoundToInt(dmg * 0.8f);
							break;
					}
				}

				target.Damage(dmg);
				
				if (target.hasShawRecoil && isShawAttack && !isDashAttack)
					p.ShawRetreat(isDashAttack);
				else if (target.hasDashRecoil && isShawAttack && isDashAttack)
					p.ShawRetreat(isDashAttack);
				else if (target.hasShawRecoil && isRisingAttack)
					p.RisingAtkRetreat();
				else if (target.hasRecoil && hasRecoil && !p.justParried)
					p.Recoil();
				else if (target.hasRecoil && !hitSomething && hasRecoil && !p.justParried)
					p.Recoil();
			}
		}
		if (!hitSomething && other.CompareTag("Ground") && hasRecoil && !p.justParried)
			p.Recoil();
	}
}
