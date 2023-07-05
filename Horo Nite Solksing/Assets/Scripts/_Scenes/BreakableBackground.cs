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

	protected override void CallChildOnStart() { }

	protected override void CallChildOnDamage(int dmg)
	{
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
}
