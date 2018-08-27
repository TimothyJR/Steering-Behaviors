using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public class JobTestManager : MonoBehaviour {

	[SerializeField] private int numberOfTestObjects = 100;
	[SerializeField] private GameObject testObject;
	private List<GameObject> testObjects;
	Transform[] testTransforms;
	TransformAccessArray testAccessArray;
	TestUpdateJob testJob;
	JobHandle testJobHandle;

	private void Start()
	{
		testObjects = new List<GameObject>();
		testTransforms = new Transform[numberOfTestObjects];
		AddTestObjects();
		testAccessArray = new TransformAccessArray(testTransforms);
	}

	private void Update()
	{
		testJob = new TestUpdateJob()
		{
			time = Time.deltaTime
		};

		testJobHandle = testJob.Schedule(testAccessArray);
	}

	public void LateUpdate()
	{
		testJobHandle.Complete();
	}

	public void OnDestroy()
	{
		testAccessArray.Dispose();
	}

	private void AddTestObjects()
	{

		for(int i = 0; i < numberOfTestObjects; i++)
		{
			float xValue = Random.Range(-10.0f, 10.0f);
			float zValue = Random.Range(-10.0f, 10.0f);
			GameObject testSpawn = GameObject.Instantiate(testObject, new Vector3(xValue, 0.0f, zValue), Quaternion.identity);
			testObjects.Add(testSpawn);
			testTransforms[i] = testSpawn.transform;
		}

	}

	struct TestUpdateJob : IJobParallelForTransform
	{
		public float time;

		public void Execute(int i, TransformAccess transform)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + 1 * time, transform.position.z);
		}
	}
}
