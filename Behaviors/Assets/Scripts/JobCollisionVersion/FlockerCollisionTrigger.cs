using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockerCollisionTrigger : MonoBehaviour {

	private static FlockManagerCollisionJob manager;
	private int index;

	public static FlockManagerCollisionJob Manager
	{ set { manager = value; } }

	public int Index
	{ get { return index; } set { index = value; } }

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Butterfly"))
		{
			manager.AddToSeparationIndex(index, other.gameObject.GetComponent<FlockerCollisionTrigger>().Index);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Butterfly"))
		{
			manager.RemoveFromSeparationIndex(index, other.gameObject.GetComponent<FlockerCollisionTrigger>().Index);
		}
	}
}
