using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShockWave : MonoBehaviour
{
    public float dir=1;
    [SerializeField] Rigidbody2D rb;
	[SerializeField] float speed=5;
	[SerializeField] float maxSpeed=10;
	[SerializeField] Animator anim;
	[SerializeField] [Range(0f,1f)] float startProgressTimeAnim;

	
	[Space] [SerializeField] protected Transform groundDetect;
	[SerializeField] protected Transform wallDetect;
	[SerializeField] protected float groundDistDetect=1f;
	[SerializeField] protected float wallDistDetect=0.5f;
	[SerializeField] protected  LayerMask whatIsGround;
	private bool end;


	public void Flip()
	{
		dir = -1;
		transform.localScale = new Vector3(
			-transform.localScale.x,
			transform.localScale.y,
			transform.localScale.z
		);
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!end && rb != null)
		{
			rb.AddForce(new Vector2(speed * dir * 5 * Time.fixedDeltaTime, 0), ForceMode2D.Force);
			rb.velocity = new Vector2(
				Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),
				0
			);
		}
		if (!end && groundDetect != null)
		{
			RaycastHit2D groundInfo = Physics2D.Linecast(
				groundDetect.position, 
				groundDetect.position + new Vector3(0, -groundDistDetect), 
				whatIsGround
			);
			RaycastHit2D wallInfo = Physics2D.Linecast(
				wallDetect.position, 
				wallDetect.position + new Vector3(dir * wallDistDetect, 0), 
				whatIsGround
			);
			if (groundInfo.collider == null || wallInfo.collider != null)
			{
				end = true;
				// anim.SetTrigger("end");
				anim.Play("shockwave_end_anim", -1, startProgressTimeAnim);
				rb.velocity = Vector2.zero;
			}
		}
    }

	public void DESTROY()
	{
		Destroy(gameObject);
	}
}
