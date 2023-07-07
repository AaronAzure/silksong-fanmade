using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Rope : MonoBehaviour
{
	private LineRenderer lineRenderer;
	// [SerializeField] MeshCollider meshCol;
	// [SerializeField] bool generateMeshOnce=true;
	// [SerializeField] EdgeCollider2D col;

	
	private List<RopeSegment> ropeSegments = new List<RopeSegment>();
	private float ropeSegLen = 0.25f;
	private int origSegmentLength;
	[SerializeField] [Range(0,100)] int segmentLength = 35;
	[SerializeField] float nConstraint=50;
	[SerializeField] float lineWidth = 0.1f;
	private float counter;
	public Transform tailPos;
	public Transform target;
	public Vector2 startPos;
	public Vector2 endPos;
	public float dir;
	private Vector2 launchDir;
	[SerializeField] bool isSimulating=true;
	public bool isRetracting;
	private float retractTimer;
	private float retractSpeed=0.1f;
	[SerializeField] float retractDuration=0.45f;
	[HideInInspector] public bool skipStuckState;
	private float doneTimer;
	[SerializeField] float doneThreshold=0.5f;
	[SerializeField] Transform endVisualObj;

	
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
		origSegmentLength = segmentLength;
		// Vector2 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 ropeStartPoint = Vector2.zero;
		float xDif = ((endPos.x - (transform.position.x)) / (segmentLength)) * dir;
		float yDif = ((endPos.y - (transform.position.y)) / (segmentLength));
		transform.position = tailPos.position;

		ropeSegLen = (Vector2.Distance(endPos, transform.position) / segmentLength);
		launchDir = (endPos - (Vector2) transform.position).normalized;

		retractSpeed = retractDuration / segmentLength;
		doneTimer = 0;

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

		// endVisualObj.localPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
		endVisualObj.localPosition = Vector3.zero;
		endVisualObj.gameObject.SetActive(true);
	}

	private void OnDisable() 
	{
		this.ropeSegments.Clear();
		segmentLength = origSegmentLength;
		isRetracting = false;
		endVisualObj.gameObject.SetActive(false);
		skipStuckState = false;
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
		transform.position = tailPos.position;

		if (endVisualObj != null && lineRenderer.positionCount > 0)
			endVisualObj.localPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

		if (!skipStuckState && doneTimer < doneThreshold)
		{
			doneTimer += Time.fixedDeltaTime;
		}
		else if (isSimulating)
		{
			this.Simulate();
		}

		if (skipStuckState || isRetracting)
		{

			retractTimer += Time.fixedDeltaTime;
			if (retractTimer > retractSpeed)
			{
				retractTimer = 0;
				if (segmentLength > 0)
					segmentLength--;
				else
					gameObject.SetActive(false);
			}
		}
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
		for (int i = 0; i < nConstraint; i++)
		{
			this.ApplyConstraint();
		}
	}

	private void ApplyConstraint()
	{
		RopeSegment firstSegment = this.ropeSegments[0];
		firstSegment.posNow = Vector2.zero;
		this.ropeSegments[0] = firstSegment;

		for (int i = 0; i < this.segmentLength - 2; i++)
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
		
		lineRenderer.SetPositions( ropePositions );
		// col.points = ConvertArray(ropePositions);
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

	// private void GenerateMesh()
	// {
	// 	Mesh newMesh = new Mesh();
	// 	lineRenderer.BakeMesh(newMesh);
	// 	meshCol.sharedMesh = newMesh;
	// }

	private Vector2[] ConvertArray(Vector3[] v3)
	{
		Vector2[] v2 = new Vector2[v3.Length];

		for(int i=0 ; i<v3.Length ; i++)
		{
			v2[i] = (Vector2) v3[i];
		}
		return v2;
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
