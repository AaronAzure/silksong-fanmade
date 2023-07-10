using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightPin : Tool
{
	[Space] [SerializeField] StraightPin pin;
	[HideInInspector] public float angle;
	[SerializeField] float heightOffset=1.5f;
	
	protected override void CallChildOnStart() 
	{
		switch (level)
		{
			case 1 :
				dmg = 11;
				if (isMaster) DuplicateUse(inAir ? 0.5f : 1);
				// 1 = 11
				// 2 = 22
				break;
			case 2 :
				dmg = 10;
				if (isMaster) DuplicateUse(1);
				if (isMaster) DuplicateUse(inAir ? -1 : 2);
				// 1 = 10
				// 2 = 20
				// 3 = 30
				break;
			case 3 :
				dmg = 9;
				if (isMaster) DuplicateUse(inAir ? 0.5f : 1);
				if (isMaster) DuplicateUse(inAir ? -1.5f : 2);
				if (isMaster) DuplicateUse(inAir ? 1.5f : 3);
				// 1 = 9
				// 2 = 18
				// 3 = 27
				// 4 = 36
				break;
			default :
				// 1 = 12
				break;
		}
	}

	protected override void LaunchDir()
	{
		if (isMaster && inAir && level > 0 && level % 2 == 1)
			rb.velocity = new Vector2((toRight ? dir.x : -dir.x) * velocityMultiplier, dir.y - (heightOffset/2));
		else
			rb.velocity = new Vector2((toRight ? dir.x : -dir.x) * velocityMultiplier, dir.y + (heightOffset * angle));
		kbDir = new Vector2((toRight ? kbDir.x : -kbDir.x), kbDir.y);
		gameObject.transform.rotation = Quaternion.Euler(0,0, 
			Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg
		);
	}

	private void DuplicateUse(float a)
	{
		var tool = Instantiate(pin, transform.position, Quaternion.identity);
		tool.isMaster = false;
		tool.toRight = this.toRight;
		tool.angle = a;
	}
}
