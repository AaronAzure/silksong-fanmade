using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Animator textboxAnim;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player"))	
		{
			ToggleTextbox(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag("Player"))	
		{
			ToggleTextbox(false);
		}
	}

	public void ToggleTextbox(bool active)
	{
		if (textboxAnim != null)
		{
			textboxAnim.SetTrigger(active ? "open" : "close");
		}
	}
}
