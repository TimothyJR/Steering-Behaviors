using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class SteeringVehicle : MonoBehaviour {

    private GameObject target = null;

    private Vector3 acceleration;
    private Vector3 velocity;

    private CharacterController controller;
    private FlockManager manager;

    public FlockManager Manager { set { manager = value; } }

    private SteeringAttributes attributes;

    public GameObject Target { set { target = value; } }
    public Vector3 Velocity { get { return velocity; } }


	// Use this for initialization
	void Start () {
        acceleration = Vector3.zero;
        velocity = transform.forward;
        attributes = manager.FlockAttributes;
        controller = GetComponent<CharacterController>();
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
        force += attributes.SeparationWeight * Steering.Separation(manager.Flockers, transform.position, attributes.MaxSpeed, controller);
        force = Vector3.ClampMagnitude(force, attributes.MaxForce);
        ApplyForce(force);

    }

    private void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / attributes.Mass;
    }
}
