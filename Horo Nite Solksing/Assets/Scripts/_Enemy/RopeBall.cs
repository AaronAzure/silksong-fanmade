using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBall : MonoBehaviour
{
	[SerializeField] Rope rope;
	[SerializeField] float reachDuration=0.25f;
	private bool done;
	private Coroutine co;


	private void OnEnable() 
	{
		done = false;
		co = StartCoroutine( EndOfRopeCo() );
	}

	private void Start() 
	{
		reachDuration *= GameManager.Instance.easyMode ? 0.5f : 1;
	}

	IEnumerator EndOfRopeCo(float duration=0f, bool missed=true)
	{
		yield return new WaitForSeconds(duration > 0 ? duration : reachDuration);
		if (!done)
		{
			done = true;
			rope.CollidedWithGround(missed);
		}
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!done && other.CompareTag("Ground"))	
		{
			// called once per active
			StopCoroutine(co);
			StartCoroutine( EndOfRopeCo(0.01f, false) );
			// rope.CollidedWithGround(false);
		}
	}
}
