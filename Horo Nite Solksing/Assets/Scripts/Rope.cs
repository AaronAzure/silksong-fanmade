using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
	private LineRenderer lineRenderer;
	private List<RopeSegment> ropeSegments = new List<RopeSegment>();
	[SerializeField] float ropeSegLen = 0.25f;
	[SerializeField] int segmentLength = 35;
	[SerializeField] float lineWidth = 0.1f;
	private float counter;
	[SerializeField] float counterLimit=0.1f;
	public Vector2 startPos;
	public Vector2 endPos;

	
	[Space] [Header("Funny")] [SerializeField] Vector2 bottomLeftBound;
	[SerializeField] Vector2 topRightBound;

	// Use this for initialization
	void Start()
	{
		this.lineRenderer = this.GetComponent<LineRenderer>();
		// Vector2 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 ropeStartPoint = startPos;
		int xDif = (int) ((endPos.x - startPos.x) / ropeSegLen);
		int yDif = (int) ((endPos.y - startPos.y) / ropeSegLen);

		for (int i = 0; i < segmentLength; i++)
		{
			this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
			ropeStartPoint.x = Random.Range(bottomLeftBound.x, topRightBound.x);
			ropeStartPoint.y = Random.Range(bottomLeftBound.y, topRightBound.y);
			// ropeStartPoint.x += xDif;
			// ropeStartPoint.y += yDif;
		}
	}

	private void OnDrawGizmosSelected() 
	{
		Gizmos.DrawWireCube(
			Vector3.Lerp(bottomLeftBound, topRightBound, 0.5f), 
			new Vector3(topRightBound.x - bottomLeftBound.x, topRightBound.y - bottomLeftBound.y)
		);
	}


	// Update is called once per frame
	void Update()
	{
		counter += Time.deltaTime;
		if (counter > counterLimit)
		{
			counter = 0;
			// this.DrawRope();
			this.DrawRandomRope();
		}
	}

	private void FixedUpdate()
	{
		// this.Simulate();
	}

	private void Simulate()
	{
		// SIMULATION
		Vector2 forceGravity = new Vector2(0f, -1.5f);

		for (int i = 1; i < this.segmentLength - 1; i++)
		{
			RopeSegment firstSegment = this.ropeSegments[i];
			Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
			firstSegment.posOld = firstSegment.posNow;
			firstSegment.posNow += velocity;
			firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
			this.ropeSegments[i] = firstSegment;
		}

		//CONSTRAINTS
		for (int i = 0; i < 50; i++)
		{
			this.ApplyConstraint();
		}
	}

	private void ApplyConstraint()
	{
		//Constrant to Mouse
		RopeSegment firstSegment = this.ropeSegments[0];
		// firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		firstSegment.posNow = startPos;
		this.ropeSegments[0] = firstSegment;

		for (int i = 0; i < this.segmentLength - 1; i++)
		{
			RopeSegment firstSeg = this.ropeSegments[i];
			RopeSegment secondSeg = this.ropeSegments[i + 1];

			float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
			float error = Mathf.Abs(dist - this.ropeSegLen);
			Vector2 changeDir = Vector2.zero;

			if (dist > ropeSegLen)
			{
				changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
			} 
			else if (dist < ropeSegLen)
			{
				changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
			}

			Vector2 changeAmount = changeDir * error;
			if (i != 0)
			{
				firstSeg.posNow -= changeAmount * 0.5f;
				this.ropeSegments[i] = firstSeg;
				secondSeg.posNow += changeAmount * 0.5f;
				this.ropeSegments[i + 1] = secondSeg;
			}
			else
			{
				secondSeg.posNow += changeAmount;
				this.ropeSegments[i + 1] = secondSeg;
			}
		}
	}

	private void DrawRope()
	{
		float lineWidth = this.lineWidth;
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;

		Vector3[] ropePositions = new Vector3[this.segmentLength];
		for (int i = 0; i < this.segmentLength; i++)
		{
			ropePositions[i] = this.ropeSegments[i].posNow;
		}

		lineRenderer.positionCount = ropePositions.Length;
		lineRenderer.SetPositions(ropePositions);
	}

	private void DrawRandomRope()
	{
		Vector3[] ropePositions = new Vector3[this.segmentLength];
		for (int i = 0; i < this.segmentLength; i++)
		{
			float newX = Random.Range(bottomLeftBound.x, topRightBound.x);
			float newY = Random.Range(bottomLeftBound.y, topRightBound.y);
			
			ropePositions[i] = new Vector3(newX, newY);
		}

		lineRenderer.positionCount = ropePositions.Length;
		lineRenderer.SetPositions(ropePositions);
	}

	public struct RopeSegment
	{
		public Vector2 posNow;
		public Vector2 posOld;

		public RopeSegment(Vector2 pos)
		{
			this.posNow = pos;
			this.posOld = pos;
		}
	}
}
