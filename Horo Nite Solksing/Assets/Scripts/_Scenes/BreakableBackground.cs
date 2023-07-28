using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBackground : Breakable
{
	[Space] [Header("background")] 
	[SerializeField] GameObject visualObj;
	[SerializeField] GameObject hitboxObj;
	[SerializeField] GameObject dmgVfxObj;
	[SerializeField] bool destoryAfter;

	
	[Space] [Header("TRAP")] 
	[SerializeField] bool isTrap;
	[SerializeField] float despawnTime=10f;


	protected override void CallChildOnStart() 
	{ 
		if (isTrap)
			StartCoroutine( DespawnCo() );
	}

	protected override void CallChildOnDamage(int dmg)
	{
		CinemachineShake.Instance.ShakeCam(0.75f, 0.25f, 0.5f);

		if (visualObj != null)
		{
			visualObj.SetActive(false);
		}
		if (hitboxObj != null)
		{
			hitboxObj.SetActive(false);
		}
		if (col != null)
		{
			col.enabled = false;
		}
		if (dmgVfxObj != null)
		{
			dmgVfxObj.SetActive(true);
		}
		if (destoryAfter)
			StartCoroutine( DestroyCo() );
	}

	IEnumerator DestroyCo()
	{
		yield return new WaitForSeconds(3);
		Destroy(gameObject);
	}

	IEnumerator DespawnCo()
	{
		yield return new WaitForSeconds(despawnTime);
		CallChildOnDamage(10000);
	}
}
