using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellShard : MonoBehaviour
{
    [SerializeField] int value=1;
	public SpriteRenderer sr;
	public Rigidbody2D rb;
	
    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player") && PlayerControls.Instance != null)
		{
			PlayerControls.Instance.GainShellShard(value);
			MusicManager.Instance.PlayCurrencySFX();
			Destroy(gameObject);
		}
	}
}
