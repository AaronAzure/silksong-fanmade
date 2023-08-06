using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
	[SerializeField] NewScene newScene;


    public override void Interact() 
	{ 
		PlayerControls.Instance.MoveThruDoor(newScene);
	}
}
