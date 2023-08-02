using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
	[SerializeField] bool showTutorial=true;
	[SerializeField] int index;

	
    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player"))	
		{
			if (showTutorial)
				PlayerControls.Instance.ActivateTutorial(index);
			else
				PlayerControls.Instance.DeactivateTutorial(index);
			gameObject.SetActive(false);
		}
	}
}
