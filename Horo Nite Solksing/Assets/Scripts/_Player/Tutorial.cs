using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
	public Animator anim;
	[SerializeField] string[] actionNames;
	[SerializeField] TextMeshProUGUI[] actionTxts;
	public bool seenTutorial;
	[SerializeField] float autoClose=-1;

	[Space] [SerializeField] bool isRepairInstruction;
	[SerializeField] TextMeshProUGUI costTxt;
	// bool done;

	private void OnEnable() 
	{
		SetRepairCost();

		if (seenTutorial) return;

		for (int i=0 ; i<actionTxts.Length ; i++)	
		{
			if (actionTxts[i] != null && actionNames.Length > i && actionNames[i] != null)
			{
				int startInd = actionTxts[i].text.IndexOf('[') + 1;
				string elemName = PlayerControls.Instance.GetActionElementIdentifierName(actionNames[i]);
				Debug.Log(elemName);
				
				actionTxts[i].text = actionTxts[i].text.Insert(startInd, elemName);
			}
		}
		seenTutorial = true;

		if (autoClose > 0)
			StartCoroutine( CloseCo() );
	}

	public void SetRepairCost()
	{
		if (isRepairInstruction && costTxt != null)
			costTxt.text = $"-{PlayerControls.Instance.GetRepairCost()} ({PlayerControls.Instance.GetNumberOfToolsFixed()})";
	}

	IEnumerator CloseCo()
	{
		yield return new WaitForSeconds(autoClose);
		if (anim != null)
			anim.SetTrigger("close");
	}
}
