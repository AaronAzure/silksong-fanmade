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
	public int nSickle;


    void FixedUpdate() 
	{
		if (hitWall)
		{
			LaunchInDirection((returnPos.position - transform.position).normalized);
			if (Vector2.Distance(transform.position, returnPos.position) < 0.25f)
			{
				death.RetrieveSickle(nSickle);
				Destroy(this.gameObject);
			}
		}	
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!playerHit && other.CompareTag("Finish"))
		{
			hurtBox.enabled = false;
			playerHit = true;
			hitWall = false;
			LaunchInDirection((transform.position - other.transform.position).normalized);
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
