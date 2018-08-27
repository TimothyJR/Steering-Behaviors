using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public class FlockManagerCollisionJob : MonoBehaviour
{


	[SerializeField] private int numberOfFlockers = 135;
	[SerializeField] private GameObject flockerPrefab;
	[SerializeField] private GameObject flockTargetPrefab;
	[SerializeField] private SteeringAttributes attributes;

	[Header("Obstacles")]
	[SerializeField] private GameObject[] obstaclePrefabs;
	[SerializeField] private float obstacleScaleMax = 8.0f;
	[SerializeField] private int numberOfObstacles = 0;


	private GameObject centroidObject;
	private Vector3 centroid;
	private Vector3 flockDirection;
	private GameObject flockTarget;

	private GameObject[] flockers;
	private NativeArray<Vector3> flockerPositions;
	private NativeArray<Vector3> flockerVelocity;
	private NativeMultiHashMap<int, int> separationRangeIndex;
	private NativeArray<Quaternion> flockerRotations;
	private GameObject[] obstacles;
	private NativeArray<Vector3> obstaclePositions;
	private NativeArray<float> obstacleRadius;

	private FlockJob flockJob;
	JobHandle flockJobHandle;

	private void Start()
	{
		Vector3 position = new Vector3(Random.Range(-80, 80), Random.Range(2, 60), Random.Range(-80, 80));
		flockTarget = (GameObject)Instantiate(flockTargetPrefab, position, Quaternion.identity);
		centroidObject = new GameObject();


		flockers = new GameObject[numberOfFlockers];
		flockerPositions = new NativeArray<Vector3>(numberOfFlockers, Allocator.Persistent);
		flockerVelocity = new NativeArray<Vector3>(numberOfFlockers, Allocator.Persistent);
		flockerRotations = new NativeArray<Quaternion>(numberOfFlockers, Allocator.Persistent);
		separationRangeIndex = new NativeMultiHashMap<int, int>(numberOfFlockers, Allocator.Persistent);
		AddFlockers();

		obstacles = new GameObject[numberOfObstacles];
		obstacleRadius = new NativeArray<float>(numberOfObstacles, Allocator.Persistent);
		obstaclePositions = new NativeArray<Vector3>(numberOfObstacles, Allocator.Persistent);

		Camera.main.GetComponent<CameraSmoothFollow>().Target = centroidObject.transform;
	}


	private void FixedUpdate()
	{
		flockJob = new FlockJob
		{
			time = Time.fixedDeltaTime,
			positions = flockerPositions,
			velocities = flockerVelocity,
			rotations = this.flockerRotations,
			separationLists = separationRangeIndex,
			obstaclePositions = this.obstaclePositions,
			obstacleRadius = this.obstacleRadius,
			attributes = this.attributes,
			targetPosition = flockTarget.transform.position,
			centroid = this.centroid,
			direction = flockDirection
		};

		flockJobHandle = flockJob.Schedule(numberOfFlockers, 32);

		flockJobHandle.Complete();

		Vector3 positionSum = Vector3.zero;
		
		for (int i = 0; i < numberOfFlockers; i++)
		{
		
			if (flockerVelocity[i] != Vector3.zero)
			{
				flockers[i].transform.forward = flockerVelocity[i].normalized;
				flockerRotations[i] = flockers[i].transform.rotation;
			}
			flockers[i].transform.position = flockerPositions[i] + (flockerVelocity[i] * Time.fixedDeltaTime);
			flockerPositions[i] = flockers[i].transform.position;
			positionSum += flockerPositions[i];
			flockDirection += flockerVelocity[i];
		
		}
		
		centroid = positionSum / numberOfFlockers;
		centroid.y += 1;
		flockDirection.Normalize();
		
		if (Vector3.Distance(centroid, flockTarget.transform.position) < (numberOfFlockers / 20) + 1)
		{
			flockTarget.transform.position = new Vector3(Random.Range(-80, 80), Random.Range(2, 60), Random.Range(-80, 80));
			flockTarget.transform.rotation = Quaternion.LookRotation(centroid - flockTarget.transform.position);
		}
		
		centroidObject.transform.position = centroid;
		centroidObject.transform.rotation = Quaternion.LookRotation(flockDirection);
	}

	private void AddFlockers()
	{
		for (int i = 0; i < numberOfFlockers; i++)
		{
			float xValue = Random.Range(-10.0f, 10.0f);
			float zValue = Random.Range(-10.0f, 10.0f);

			flockerPositions[i] = new Vector3(xValue, 0.0f, zValue);
			flockerVelocity[i] = Vector3.forward;

			GameObject flockerSpawn = GameObject.Instantiate(flockerPrefab, new Vector3(xValue, 0.0f, zValue), Quaternion.identity);

			// Randomly sets the start frame of the animation
			flockerSpawn.GetComponent<Animator>().SetFloat("offset", Random.Range(0.0f, 1.0f));
			flockers[i] = flockerSpawn;



			// Set up collision trigger script
			flockers[i].GetComponent<FlockerCollisionTrigger>().Index = i;
			flockers[i].GetComponent<SphereCollider>().radius = attributes.SeparationDistance;
		}
		FlockerCollisionTrigger.Manager = this;
	}

	private void AddObstacles()
	{
		// Make the obstacles
		Vector3 position;
		for (int i = 0; i < numberOfObstacles; i++)
		{
			position = new Vector3(Random.Range(-40, 40), 15.0f, Random.Range(-40, 40));
			Quaternion rotation = Quaternion.Euler(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));
			GameObject obstacle = (GameObject)Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], position, rotation);

			float ObstacleScale = Random.Range(2f, obstacleScaleMax);
			obstacle.transform.localScale = new Vector3(ObstacleScale, ObstacleScale, ObstacleScale);
			obstacles[i] = obstacle;
			obstaclePositions[i] = obstacle.transform.position;
			obstacleRadius[i] = obstacle.GetComponent<Obstacle>().Radius;
		}
	}

	private void OnDestroy()
	{
		flockerPositions.Dispose();
		flockerVelocity.Dispose();
		flockerRotations.Dispose();
		obstaclePositions.Dispose();
		obstacleRadius.Dispose();
		separationRangeIndex.Dispose();

	}

	struct FlockJob : IJobParallelFor
	{
		public float time;
		[ReadOnly] public NativeArray<Vector3> positions;
		public NativeArray<Vector3> velocities;
		[ReadOnly] public NativeArray<Quaternion> rotations;
		[ReadOnly] public NativeMultiHashMap<int, int> separationLists;
		[ReadOnly] public NativeArray<Vector3> obstaclePositions;
		[ReadOnly] public NativeArray<float> obstacleRadius;
		[ReadOnly] public SteeringAttributes attributes;
		[ReadOnly] public Vector3 targetPosition;
		[ReadOnly] public Vector3 centroid;
		[ReadOnly] public Vector3 direction;

		public void Execute(int index)
		{
			Vector3 force = Vector3.zero;
			
			for(int i = 0; i < obstaclePositions.Length; i++)
			{
				force += attributes.AvoidWeight * Steering.AvoidObstacleJob(obstaclePositions[i], obstacleRadius[i], attributes.AvoidDistance, positions[i], rotations[i], attributes);
			}
			
			force += attributes.InBoundsWeight * Steering.StayInBoundsJob(200, Vector3.zero, positions[index], attributes.MaxSpeed, velocities[index]);
			
			force += attributes.SeekWeight * Steering.SeekJob(targetPosition, positions[index], attributes.MaxSpeed, velocities[index]);
			
			force += attributes.CentroidWeight * Steering.CohesionJob(centroid, positions[index], attributes.MaxSpeed, velocities[index]);
			
			force += attributes.DirectionWeight * Steering.AlignmentJob(direction, positions[index], attributes.MaxSpeed, velocities[index]);
			
			force += attributes.SeparationWeight * Steering.SeparationJobCollision(positions, separationLists, index, attributes.MaxSpeed, velocities[index]);
			
			force = Vector3.ClampMagnitude(force, attributes.MaxForce);
			
			velocities[index] += force / attributes.Mass * time;
			velocities[index] = Vector3.ClampMagnitude(velocities[index], attributes.MaxSpeed);

			
		}
	}

	public void AddToSeparationIndex(int index, int collisionIndex)
	{
		separationRangeIndex.Add(index, collisionIndex);
	}

	public void RemoveFromSeparationIndex(int index, int collisionIndex)
	{
		int separationIndex;
		NativeMultiHashMapIterator<int> iterator;
		if (separationRangeIndex.TryGetFirstValue(index, out separationIndex, out iterator))
		{
			do
			{
				if(collisionIndex == separationIndex)
				{
					separationRangeIndex.Remove(iterator);
					return;
				}
			} while (separationRangeIndex.TryGetNextValue(out separationIndex, ref iterator));
		}
	}
}
