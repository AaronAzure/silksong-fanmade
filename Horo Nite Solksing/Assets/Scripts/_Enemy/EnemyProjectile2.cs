using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile2 : MonoBehaviour
{
	[SerializeField] bool rotateInDir;
	public Rigidbody2D rb;
	[SerializeField] GameObject breakVfx;
	private bool inDestroyCo;
	[SerializeField] bool dontDestroyOnHit;


	private void FixedUpdate() 
	{
		if (rotateInDir)
		{
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!dontDestroyOnHit && !inDestroyCo && 
			(other.CompareTag("Ground") || other.CompareTag("Player")))
		{
			inDestroyCo = true;
			StartCoroutine( DestroyCo() );
		}
	}

	IEnumerator DestroyCo()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		if (breakVfx != null)
			breakVfx.transform.parent = null;
		Destroy(gameObject);
	}
}
