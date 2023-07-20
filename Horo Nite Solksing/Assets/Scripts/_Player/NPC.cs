using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
	[System.Serializable] public class Dialogue
	{
		[TextArea(1,3)] public string[] lines;
	}

	
	[SerializeField] Animator textboxAnim;
	public Dialogue[] dialogue;
	

	public void ToggleTextbox(bool active)
	{
		if (textboxAnim != null)
		{
			textboxAnim.SetTrigger(active ? "open" : "close");
		}
	}
}

