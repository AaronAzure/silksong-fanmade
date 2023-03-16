using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySickle : MonoBehaviour
{
	[SerializeField] Collider2D hurtBox;
	[SerializeField] Rigidbody2D rb;
	public Transform returnPos;
	[SerializeField] float speed=15;
	private bool hitWall;
	private bool playerHit;
	public Death death;


    void FixedUpdate() 
	{
		if (hitWall)
		{
			LaunchInDirection((returnPos.position - transform.position).normalized);
			if (Vector2.Distance(transform.position, returnPos.position) < 0.25f)
			{
				death.RetrieveSickle();
				Destroy(this.gameObject);
			}
		}	
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!playerHit && other.CompareTag("Finish"))
		{
			playerHit = true;
			LaunchInDirection((transform.position - other.transform.position).normalized);
			hurtBox.enabled = false;
		}
		else if (!hitWall && other.CompareTag("Ground"))
		{
			hitWall = true;
			LaunchInDirection((returnPos.position - transform.position).normalized);
		}
	}


	public void LaunchInDirection(Vector2 dir)
	{
		rb.velocity = dir * speed;
	}
}
