using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] Enemy master;

	public void Attacked(bool canBlock)
	{
		if (master != null)
		{
			master.CallMasterOnShield(canBlock);
		}
	}
}
