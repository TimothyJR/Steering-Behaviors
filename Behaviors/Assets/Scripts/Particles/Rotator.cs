using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	[SerializeField] private Vector3 rotateSpeed;

	// Update is called once per frame
	private void FixedUpdate ()
	{
		transform.Rotate(rotateSpeed);
	}
}
