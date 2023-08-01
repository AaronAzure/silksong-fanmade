using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rosary : MonoBehaviour
{
	[SerializeField] int value=1;
	public SpriteRenderer sr;
	public Rigidbody2D rb;
	
    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && PlayerControls.Instance != null)
		{
			PlayerControls.Instance.GainCurrency(value);
			MusicManager.Instance.PlayCurrencySFX();
			Destroy(gameObject);
		}
	}
}
