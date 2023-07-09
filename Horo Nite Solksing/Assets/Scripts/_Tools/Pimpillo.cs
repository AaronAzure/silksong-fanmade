using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pimpillo : Tool
{
	[Space] [Header("Pimpillo")]
	[SerializeField] ParticleSystem trailPs;
	[SerializeField] GameObject explosionObj;
	[SerializeField] Tool explosion;
	private bool createdExplosion;

	protected override void CallChildOnStart()
	{
		switch (level)
		{
			case 1 :
				explosion.dmg = 40;
				break;
			case 2 :
				explosion.dmg = 50;
				break;
			case 3 :
				explosion.dmg = 60;
				break;
			default :
				break;
		}
	}

	protected override void CallChildOnEnemyHit(Collider2D other)
	{
	}

	protected override void CallChildOnSpecialHit()
	{
		// create single explosion
		if (createdExplosion) return;
		createdExplosion = true;

		if (trailPs != null)
		{
			trailPs.transform.parent = null;
			trailPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (explosionObj != null)
		{
			Instantiate(explosionObj, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
	protected override void CallChildOnHit()
	{
		// create single explosion
		if (createdExplosion) return;
		createdExplosion = true;
		
		if (trailPs != null)
		{
			trailPs.transform.parent = null;
			trailPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (explosionObj != null)
		{
			Instantiate(explosionObj, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}

	protected override void CallChildOnBreakableHit(Collider2D other)
	{
		// create single explosion
		if (createdExplosion) return;
		createdExplosion = true;
		
		if (trailPs != null)
		{
			trailPs.transform.parent = null;
			trailPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (explosion != null)
		{
			Instantiate(explosion, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
}
