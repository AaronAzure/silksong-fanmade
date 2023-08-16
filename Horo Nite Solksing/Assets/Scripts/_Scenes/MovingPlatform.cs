using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	[SerializeField] float moveSpeed=0.5f;
	[SerializeField] Vector2 moveDir;
	[SerializeField] float idleTimeThres=2;
	[SerializeField] float moveTimeThres=5;
	private bool isReverse;
	private bool isIdle;
	private float moveTimer;
	private float idleTimer;


	// Update is called once per frame
	void FixedUpdate()
	{
		if (isIdle)
		{
			if (idleTimer < idleTimeThres)
			{
				idleTimer += Time.fixedDeltaTime;
			}
			else
			{
				isReverse = !isReverse;
				isIdle = false;
				idleTimer = 0;
				moveTimer = 0;
			}
		}
		else
		{
			if (moveTimer < moveTimeThres)
			{
				moveTimer += Time.fixedDeltaTime;
				transform.position += (Vector3) moveDir * moveSpeed * (isReverse ? -1 : 1);
			}
			else
			{
				isIdle = true;
				idleTimer = 0;
				moveTimer = 0;
			}
		}
	}
}
