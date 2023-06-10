using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineSnap : MonoBehaviour
{
	[SerializeField] CinemachineBrain brain;
	[SerializeField] CinemachineVirtualCamera[] vcams;
	private List<CinemachineTransposer> transposers;

    private void Start() 
	{
		StartCoroutine( ResetBlendTimeCo() );
	}

	IEnumerator ResetBlendTimeCo()
	{
		if (vcams != null)
		{
			transposers = new List<CinemachineTransposer>();
			foreach (CinemachineVirtualCamera vcam in vcams)
			{
				var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
				if (transposer != null)
				{
					transposer.m_XDamping = 0;
					transposer.m_YDamping = 0;
					transposer.m_ZDamping = 0;
					transposers.Add( transposer );
				}
			}
		}
		if (brain != null)
		{
			brain.m_DefaultBlend.m_Time = 0;
		}
		yield return new WaitForSeconds(0.1f);
		if (brain != null)
		{
			brain.m_DefaultBlend.m_Time = 0.75f;
		}
		if (transposers != null)
		{
			foreach (CinemachineTransposer transposer in transposers)
			{
				transposer.m_XDamping = 1;
				transposer.m_YDamping = 1;
				transposer.m_ZDamping = 1;
			}
		}
	}
}
