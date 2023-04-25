using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pimpillo : Tool
{
	[Space] [Header("Pimpillo")]
	[SerializeField] ParticleSystem trailPs;
	[SerializeField] GameObject explosion;


	protected override void CallChildOnEnemyHit(Collider2D other)
	{
	}
	protected override void CallChildOnHit()
	{
		CinemachineShake.Instance.ShakeCam(10, 0.75f, 2);
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
