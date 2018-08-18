using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class SteeringVehicleCollision
	: MonoBehaviour {

    private GameObject target = null;

    private Vector3 acceleration;
    private Vector3 velocity;

    private CharacterController controller;
    private FlockManagerCollision manager;

    public FlockManagerCollision Manager { set { manager = value; } }

    private SteeringAttributes attributes;

	private List<GameObject> separationObjects;

    public GameObject Target { set { target = value; } }
    public Vector3 Velocity { get { return velocity; } }

	private 

	// Use this for initialization
	void Start () {
        acceleration = Vector3.zero;
        velocity = transform.forward;
        attributes = manager.FlockAttributes;
        controller = GetComponent<CharacterController>();
		separationObjects = new List<GameObject>();
	}
	

	void FixedUpdate () {
        CalcSteeringForce();

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, attributes.MaxSpeed);

        if(velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }

        controller.Move(velocity * Time.fixedDeltaTime);
        acceleration = Vector3.zero;

	}

    private void CalcSteeringForce()
    {
        Vector3 force = Vector3.zero;

        for(int i = 0; i < manager.Obstacles.Count; i++)
        {
            force += attributes.AvoidWeight * Steering.AvoidObstacle(manager.Obstacles[i], attributes.AvoidDistance, transform, attributes);
        }
        
        force += attributes.InBoundsWeight * Steering.StayInBounds(200, Vector3.zero, transform.position, attributes.MaxSpeed, controller);
        
        if(target != null)
        {
            force += attributes.SeekWeight * Steering.Seek(target.transform.position, transform.position, attributes.MaxSpeed, controller);
        }
        
        force += attributes.CentroidWeight * Steering.Cohesion(manager.Centroid, transform.position, attributes.MaxSpeed, controller);
        force += attributes.DirectionWeight * Steering.Alignment(manager.FlockDirection, transform.position, attributes.MaxSpeed, controller);
        force += attributes.SeparationWeight * Steering.Separation(separationObjects, transform.position, attributes.MaxSpeed, controller);
        force = Vector3.ClampMagnitude(force, attributes.MaxForce);
        ApplyForce(force);

    }

    private void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / attributes.Mass;
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Butterfly"))
		{
			separationObjects.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Butterfly"))
		{
			separationObjects.Remove(other.gameObject);
		}	
	}
}
