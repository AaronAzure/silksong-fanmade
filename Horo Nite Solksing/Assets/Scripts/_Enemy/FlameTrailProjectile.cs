using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTrailProjectile : MonoBehaviour
{
	[SerializeField] LayerMask whatIsGround;
	public bool toRight;
	[SerializeField] Transform self;
	[SerializeField] Transform offset;
	[SerializeField] ParticleSystem ps;
	[SerializeField] float speed=0.01f;
	[SerializeField] float distCheck=0.1f;
	private bool stop;


    void FixedUpdate()
	{
		if (!stop)
		{
			self.position += new Vector3(toRight ? speed : -speed , 0);
			RaycastHit2D wallInfo = Physics2D.Linecast(
				offset.position, 
				offset.position + new Vector3(toRight ? distCheck : -distCheck, 0),
				whatIsGround
			);
			if (wallInfo.collider != null && ps != null)
			{
				stop = true;
				ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	private void OnParticleCollision(GameObject other) 
	{
		if (other != null)
			Debug.Log($"{other.tag} | {other.name}");	
	}

	private void OnParticleTrigger() {
		Debug.Log("laksdnv");
	}
}
