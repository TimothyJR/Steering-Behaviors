using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObjectMover : MonoBehaviour {

	[SerializeField] private AnimationCurve xMovement;
	[SerializeField] private AnimationCurve yMovement;
	[SerializeField] private AnimationCurve zMovement;
	[SerializeField] private float timeToFinsihMovement;
	private float timePassed;
	private Vector3 initialPosition;

	private void Start()
	{
		initialPosition = transform.position;
	}

	private void FixedUpdate ()
	{
		timePassed += Time.deltaTime;
		float evaluationTime = timePassed / timeToFinsihMovement;
		transform.position = new Vector3(initialPosition.x + xMovement.Evaluate(evaluationTime), initialPosition.y + yMovement.Evaluate(evaluationTime), initialPosition.z + zMovement.Evaluate(evaluationTime));
		
	}
}
