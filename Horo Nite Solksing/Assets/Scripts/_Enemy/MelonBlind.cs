using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonBlind : Enemy
{
	[SerializeField] Vector2 flyDir;
	private Coroutine changeFlyCo;
	private bool isStuck;
	[SerializeField] Collider2D origCol;

	protected override void CallChildOnStart()
	{
		flyDir = new Vector2(
			Random.Range(0.1f, 1f) * (Random.Range(0,2) == 0 ? 1 : -1), 
			Random.Range(0.1f, 1f) * (Random.Range(0,2) == 0 ? 1 : -1) 
		).normalized;
		
		if (model.localScale.x != 0)
			model.localScale = new Vector3(flyDir.x > 0 ? 1 : -1, 1, 1);
	}

	protected override void IdleAction()
	{
		if (!receivingKb)
		{
			if (isStuck)
				rb.velocity = Vector2.zero;
			else
				rb.velocity = flyDir * moveSpeed;
		}
	}

	protected override void AttackingAction()
	{
		if (!receivingKb)
		{
			if (isStuck)
				rb.velocity = Vector2.zero;
			else
				rb.velocity = flyDir * moveSpeed;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("NewArea"))	
		{
			if (changeFlyCo == null && gameObject.activeSelf)
				changeFlyCo = StartCoroutine( ChangeFlyDirection(other) );
		}
	}

	protected override void CallChildOnHurt(int dmg, Vector2 forceDir)
	{
		if (changeFlyCo == null && gameObject.activeSelf)
		{
			changeFlyCo = StartCoroutine( ChangeFlyDirection(null, target.self) );
		}
	}

	IEnumerator ChangeFlyDirection(Collision2D other, Transform pos=null)
	{
		isStuck = true;
		if (other != null)
			yield return new WaitForSeconds(0.5f);
		else
			yield return new WaitForEndOfFrame();

		isStuck = false;
		Vector2 point = 
			(other != null ? other.collider.ClosestPoint(self.position) : (Vector2) pos.position) 
			- (Vector2) self.position;
		point = point.normalized;

		// hit the ceiling
		if 		(point.y > 0.5f && point.x < 0.5f && point.x > -0.5f)
			point = new Vector2(point.x + Random.Range(-1f, 1f), point.y + 0.2f);
		// hit the floor
		else if (point.y < -0.5f && point.x < 0.5f && point.x > -0.5f)
			point = new Vector2(point.x + Random.Range(-1f, 1f), point.y - 0.2f);
		// hit the right wall
		else if (point.x > 0.5f && point.y < 0.5f && point.y > -0.5f)
			point = new Vector2(point.x, point.y+ Random.Range(-1f, 1f));
		// hit the left wall
		else if (point.x < -0.5f && point.y < 0.5f && point.y > -0.5f)
			point = new Vector2(point.x, point.y+ Random.Range(-1f, 1f));

		flyDir = -point.normalized;
		if (model.localScale.x != 0)
			model.localScale = new Vector3(flyDir.x > 0 ? 1 : -1, 1, 1);

		yield return new WaitForSeconds(0.1f);
		changeFlyCo = null;
		origCol.enabled = false;
		origCol.enabled = true;
	}
}
