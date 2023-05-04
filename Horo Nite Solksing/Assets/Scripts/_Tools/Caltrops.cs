using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caltrops : Tool
{
	[Space] [Header("Caltrops")]
	[SerializeField] Caltrops c;
	[SerializeField] float angularChangeInDegrees=30;
	[SerializeField] Animator anim;
	[SerializeField] int counter=5;
	private bool done;


	protected override void CallChildOnStartAlways()
	{
		if (rb != null)
		{
			float impulse = (angularChangeInDegrees * (toRight ? -1 : 1) * Random.Range(2f,5f) * Mathf.Deg2Rad) * rb.inertia;
			rb.AddTorque(impulse, ForceMode2D.Impulse);
		}
	}
	protected override void CallChildOnEnemyHit(Collider2D other)
	{
		if (counter > 0 && anim != null)
		{
			anim.SetBool("attack", true);
		}
		else if (!done)
		{
			done = true;
			if (destroyAfterCo != null)
			{
				StopCoroutine(destroyAfterCo);
			}
			if (anim != null)
				anim.SetTrigger("destroy");
		}
	}

	public void RESET_DESTROY_CO()
	{
		if (c != null)
		{
			c.ChildResetDestroyCo();
		}
	}
	public void ChildResetDestroyCo()
	{
		if (destroyAfterCo != null)
		{
			StopCoroutine(destroyAfterCo);
		}
		destroyAfterCo = StartCoroutine( DestroyAfterCo() );
	}

	public void COUNTDOWN()
	{
		if (c != null)
		{
			c.ChildCountdown();
		}
	}
	public void ChildCountdown()
	{
		if (counter > 0)
		{
			counter--;
			if (counter == 0 && anim != null)
			{
				anim.SetBool("done", true);
				anim.SetBool("attack", false);
				anim.SetTrigger("destroy");
			}
		}
	}


	protected override IEnumerator DestroyAfterCo()
	{
		yield return new WaitForSeconds(destroyAfter);
		if (anim != null)
			anim.SetTrigger("destroy");
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (anim != null && other.CompareTag("Enemy"))
		{
			anim.SetBool("attack", false);
		}
	}
}
