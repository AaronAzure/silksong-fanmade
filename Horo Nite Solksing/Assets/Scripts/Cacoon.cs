using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cacoon : MonoBehaviour
{
	[SerializeField] PlayerControls p;
	[SerializeField] GameObject silkPs;
    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Finish"))
		{
			p.SetSilk(9);
			Instantiate(silkPs, this.transform.position, Quaternion.identity);
			this.gameObject.SetActive(false);
		}
	}
}
