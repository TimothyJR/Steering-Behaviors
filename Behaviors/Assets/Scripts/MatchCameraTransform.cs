using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCameraTransform : MonoBehaviour {
	[SerializeField] private Transform mainCamera;
	private void FixedUpdate()
	{
		transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, transform.position.z);
		transform.rotation = mainCamera.rotation;
	}
}
