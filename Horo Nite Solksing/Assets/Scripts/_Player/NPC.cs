using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
	[SerializeField] Animator textboxAnim;

	public void ToggleTextbox(bool active)
	{
		if (textboxAnim != null)
		{
			textboxAnim.SetTrigger(active ? "open" : "close");
		}
	}
}
