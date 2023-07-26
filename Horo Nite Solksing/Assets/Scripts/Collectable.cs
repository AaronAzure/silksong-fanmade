using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Interactable
{
	public override void Interact()
	{
		textboxAnim.transform.parent = transform.parent;
		PlayerControls.Instance.GainCollectable();
		Destroy(gameObject);
	}
}
