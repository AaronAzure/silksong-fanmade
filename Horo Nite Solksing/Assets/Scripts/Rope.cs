using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
		private LineRenderer lineRenderer;
		private List<RopeSegment> ropeSegments = new List<RopeSegment>();
		[SerializeField] [Range(0f,1f)] float ropeSegLen = 0.25f;
		[SerializeField] [Range(0,100)] int segmentLength = 35;
		[SerializeField] float lineWidth = 0.1f;
		private float counter;
		public Transform tailPos;
		public Vector2 startPos;
		public Vector2 endPos;
		[SerializeField] bool isSimulating=true;

		
		[Space] [Header("Funny")] 
		[SerializeField] bool isFunny;
		[SerializeField] float counterLimit=0.05f;
		[SerializeField] Vector2 bottomLeftBound;
		[SerializeField] Vector2 topRightBound;


		private void Awake() 
		{
			this.lineRenderer = this.GetComponent<LineRenderer>();
		}

		void OnEnable()
		{
			// Vector2 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 ropeStartPoint = (transform.position);
			float xDif = ((endPos.x - (transform.position.x)) / segmentLength);
			float yDif = ((endPos.y - (transform.position.y)) / segmentLength);

			if (this.ropeSegments != null)
				this.ropeSegments.Clear();

			for (int i = 0; i < segmentLength; i++)
			{
				this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
				if (isFunny)
				{
					ropeStartPoint.x = Random.Range(bottomLeftBound.x, topRightBound.x);
					ropeStartPoint.y = Random.Range(bottomLeftBound.y, topRightBound.y);
				}
				else
				{
					ropeStartPoint.x += xDif;
					ropeStartPoint.y += yDif;
				}
			}
		}

		private void OnDrawGizmosSelected() 
		{
			Gizmos.DrawWireCube(
				Vector3.Lerp(bottomLeftBound, topRightBound, 0.5f), 
				new Vector3(topRightBound.x - bottomLeftBound.x, topRightBound.y - bottomLeftBound.y)
			);
			Gizmos.color = Color.magenta;
			if (ropeSegments != null && ropeSegments.Count > 0)
				Gizmos.DrawLine(ropeSegments[0].posNow, endPos);
		}


		// Update is called once per frame
		void Update()
		{
			if (isFunny)
			{
				counter += Time.deltaTime;
				if (counter > counterLimit)
				{
					counter = 0;
					this.DrawRandomRope();
				}
			}
			else
				this.DrawRope();
		}

		private void FixedUpdate()
		{
			if (isSimulating)
				this.Simulate();
		}

		private void Simulate()
		{
			// SIMULATION
			Vector2 forceGravity = new Vector2(0f, -1.5f);

			for (int i = 1; i < this.segmentLength - 1; i++)
			{
				if (i < ropeSegments.Count)
				{
					RopeSegment firstSegment = this.ropeSegments[i];
					Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
					firstSegment.posOld = firstSegment.posNow;
					firstSegment.posNow += velocity;
					firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
					this.ropeSegments[i] = firstSegment;
				}
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
			firstSegment.posNow = (tailPos != null ? (Vector2) tailPos.position : startPos);
			this.ropeSegments[0] = firstSegment;

			for (int i = 0; i < this.segmentLength - 1; i++)
			{
				if (i < ropeSegments.Count)
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
		}

		private void DrawRope()
		{
			float lineWidth = this.lineWidth;
			lineRenderer.startWidth = lineWidth;
			lineRenderer.endWidth = lineWidth;

			Vector3[] ropePositions = new Vector3[this.segmentLength];
			for (int i = 0; i < this.segmentLength; i++)
			{
				if (i < ropeSegments.Count)
					ropePositions[i] = this.ropeSegments[i].posNow;
			}

			lineRenderer.positionCount = ropePositions.Length;
			lineRenderer.SetPositions(ropePositions);
		}

		private void DrawRandomRope()
		{
			Vector3[] ropePositions = new Vector3[this.segmentLength];
			ropePositions[0] = this.ropeSegments[0].posNow;

			for (int i = 1; i < this.segmentLength; i++)
			{
				if (i < ropeSegments.Count)
				{
					float newX = Random.Range(bottomLeftBound.x, topRightBound.x);
					float newY = Random.Range(bottomLeftBound.y, topRightBound.y);
					
					ropePositions[i] = new Vector3(newX, newY);
				}
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
