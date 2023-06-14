using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
	[SerializeField] NewScene[] exitPoints;

    // Start is called before the first frame update
    void Start()
    {
        PlayerControls p = PlayerControls.Instance;
		if (p != null && !p.started)
		{
			if (exitPoints == null)
				p.MoveOutOfNewScene(Vector2.zero);
			else
			{
				Vector2 exitPos = exitPoints[
					(p.exitPointInd >= 0 && p.exitPointInd < exitPoints.Length) ? p.exitPointInd : 0
				].transform.position;
				p.MoveOutOfNewScene(exitPos);
			}
		}
    }
}
