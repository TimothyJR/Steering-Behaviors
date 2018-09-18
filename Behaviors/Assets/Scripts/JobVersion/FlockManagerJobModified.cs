using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public class FlockManagerJobModified : MonoBehaviour
{


	[SerializeField] private int numberOfFlockers = 135;
	[SerializeField] private GameObject flockerPrefab;
	[SerializeField] private GameObject flockTargetPrefab;
	[SerializeField] private SteeringAttributes attributes;



	private GameObject centroidObject;
	private Vector3 centroid;
	private Vector3 flockDirection;
	private GameObject flockTarget;

	private GameObject[] flockers;
	private NativeArray<Vector3> flockerPositions;
	private NativeArray<Vector3> flockerVelocity;
	private NativeArray<Quaternion> flockerRotations;

	private FlockJob flockJob;
	JobHandle flockJobHandle;

	private NativeArray<float> timeDelay;
	[SerializeField] private float timeDelayModifier = 1.0f;

	private void Start()
	{
		flockTarget = (GameObject)Instantiate(flockTargetPrefab, transform.position, Quaternion.identity);
		centroidObject = new GameObject();


		flockers = new GameObject[numberOfFlockers];
		flockerPositions = new NativeArray<Vector3>(numberOfFlockers, Allocator.Persistent);
		flockerVelocity = new NativeArray<Vector3>(numberOfFlockers, Allocator.Persistent);
		flockerRotations = new NativeArray<Quaternion>(numberOfFlockers, Allocator.Persistent);
		timeDelay = new NativeArray<float>(numberOfFlockers, Allocator.Persistent);
		AddFlockers();

	}

	private void FixedUpdate()
	{
		float fixedTime = Time.fixedDeltaTime;

		flockJob = new FlockJob
		{
			time = fixedTime,
			positions = flockerPositions,
			velocities = flockerVelocity,
			rotations = this.flockerRotations,
			attributes = this.attributes,
			targetPosition = flockTarget.transform.position,
			centroid = this.centroid,
			direction = flockDirection,
			timeDelays = timeDelay
		};
		
		flockJobHandle = flockJob.Schedule(numberOfFlockers, 32);

		flockJobHandle.Complete();

		Vector3 positionSum = Vector3.zero;
		flockDirection = Vector3.zero;
		

		for (int i = 0; i < numberOfFlockers; i++)
		{
		
			if (flockerVelocity[i] != Vector3.zero)
			{
				flockers[i].transform.forward = Vector3.Lerp(flockers[i].transform.forward, flockerVelocity[i].normalized, fixedTime);
				flockerRotations[i] = flockers[i].transform.rotation;
			}
			flockers[i].transform.position += flockerVelocity[i] * fixedTime; 
			flockerPositions[i] = flockers[i].transform.position;
			positionSum += flockerPositions[i];
			flockDirection += flockerVelocity[i];
		
		}



		centroid = positionSum / numberOfFlockers;
		centroid.y += 1;
		flockDirection.Normalize();

		centroidObject.transform.position = centroid;
		centroidObject.transform.rotation = Quaternion.LookRotation(flockDirection);
	}

	private void AddFlockers()
	{
		for (int i = 0; i < numberOfFlockers; i++)
		{
			float xValue = Random.Range(-10.0f + transform.position.x, 10.0f + transform.position.x);
			float zValue = Random.Range(-10.0f + transform.position.z, 10.0f + transform.position.z);

			flockerPositions[i] = new Vector3(xValue, 0.0f, zValue);
			flockerVelocity[i] = Vector3.zero;
			flockerRotations[i] = Quaternion.LookRotation(flockTarget.transform.position - flockerPositions[i]);

			GameObject flockerSpawn = GameObject.Instantiate(flockerPrefab, flockerPositions[i], flockerRotations[i]);
			flockerSpawn.GetComponent<Animator>().SetFloat("offset", Random.Range(0.0f, 1.0f));

			flockers[i] = flockerSpawn;
			
		timeDelay[i] = -Mathf.Abs(Vector3.Magnitude(transform.position - flockerPositions[i]) * timeDelayModifier);
		}
	}


	private void OnDestroy()
	{
		flockerPositions.Dispose();
		flockerVelocity.Dispose();
		flockerRotations.Dispose();
		timeDelay.Dispose();
	}

	struct FlockJob : IJobParallelFor
	{
		[ReadOnly] public float time;
		[ReadOnly] public NativeArray<Vector3> positions;
		public NativeArray<Vector3> velocities;
		[ReadOnly] public NativeArray<Quaternion> rotations;
		[ReadOnly] public SteeringAttributes attributes;
		[ReadOnly] public Vector3 targetPosition;
		[ReadOnly] public Vector3 centroid;
		[ReadOnly] public Vector3 direction;
		public NativeArray<float> timeDelays;

		public void Execute(int index)
		{
			Vector3 force = Vector3.zero;
			if(timeDelays[index] > 0)
			{

				force += attributes.InBoundsWeight * Steering.StayInBoundsJob(200, Vector3.zero, positions[index], attributes.MaxSpeed, velocities[index]);

				force += attributes.SeekWeight * Steering.SeekJob(targetPosition, positions[index], attributes.MaxSpeed, velocities[index]);

				force += attributes.CentroidWeight * Steering.CohesionJob(centroid, positions[index], attributes.MaxSpeed, velocities[index]);

				force += attributes.DirectionWeight * Steering.AlignmentJob(direction, positions[index], attributes.MaxSpeed, velocities[index]);

				force += attributes.SeparationWeight * Steering.SeparationJob(positions, positions[index], attributes.MaxSpeed, velocities[index], attributes.SeparationDistance);

				force = Vector3.ClampMagnitude(force, attributes.MaxForce);

				

			}

			timeDelays[index] += time;

			velocities[index] += force / attributes.Mass * time;
			velocities[index] = Vector3.ClampMagnitude(velocities[index], attributes.MaxSpeed);


		}
	}

}
