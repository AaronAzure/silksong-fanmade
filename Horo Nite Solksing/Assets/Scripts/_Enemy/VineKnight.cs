using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineKnight : Enemy
{
	private GameManager gm;
	[SerializeField] Transform vineTrapPos;
	[SerializeField] GameObject vineTrapObj;
	[SerializeField] float trapOffset=1.5f;

		
	protected override void CallChildOnStart()
	{
		gm = GameManager.Instance;
		if (gm != null && gm.easyMode)
			trapOffset = 2;
	}

	private void OnDrawGizmosSelected() 
	{
		int n = 5;

		if (target != null)
		{
			for (int i=0 ; i<n ; i++)
			{
				Gizmos.color = Color.red;
				RaycastHit2D hitInfo = Physics2D.Raycast(
					target.self.position + (Vector3) GetTrapPosOffset(i) + new Vector3(0,0.1f), 
					Vector2.down, 
					4, 
					whatIsGround
				);
				
				if (hitInfo.collider != null)
					Gizmos.color = Color.green;

				Gizmos.DrawRay(target.self.position + (Vector3) GetTrapPosOffset(i) + new Vector3(0,0.1f), Vector2.down * 4);
			}
		}
	}

    public void _SET_TRAPS()
	{
		if (vineTrapObj != null)
		{
			int n = (gm != null && gm.easyMode) ? 3 : 5;
			RaycastHit2D hitInfo = Physics2D.Raycast(
				target.self.position + new Vector3(0,0.1f), 
				Vector2.down, 
				4, 
				whatIsGround
			);

			if (hitInfo.collider != null)
			{
				for (int i=0 ; i<n ; i++)
				{
					// side traps away from player must not be in a wall
					if (i != 0)
					{
						RaycastHit2D wallHitCheck = Physics2D.Raycast(
							hitInfo.point + new Vector2(0,0.1f), 
							i % 2 == 0 ? Vector2.right : Vector2.left, 
							i % 2 == 0 ? i/2 * trapOffset : (i+1)/2 * trapOffset,
							whatIsGround
						);

						// stop at wall
						if (wallHitCheck.collider != null)
							continue;
					}
					
					RaycastHit2D hitInfo2 = Physics2D.Raycast(
						hitInfo.point + GetTrapPosOffset(i) + new Vector2(0,0.1f), 
						Vector2.down, 
						4, 
						whatIsGround
					);

					// ground exists	
					if (hitInfo2.collider != null)
					{
						var obj = Instantiate(
							vineTrapObj, 
							hitInfo2.point, 
							Quaternion.identity
						);
					}
				}
			}
		}
	}

	private Vector2 GetTrapPosOffset(int x)
	{
		return (x % 2 == 0 ? new Vector2(x/2 * trapOffset, 0) : new Vector2(-(x+1)/2 * trapOffset, 0));
	}
}
