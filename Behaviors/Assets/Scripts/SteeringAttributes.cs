using UnityEngine;
using System.Collections;

[System.Serializable]
public struct SteeringAttributes {
 
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private float mass;
    [SerializeField] private float radius;

    [SerializeField] private float seekWeight;
    [SerializeField] private float inBoundsWeight;
    [SerializeField] private float avoidWeight;
    [SerializeField] private float avoidDistance;

    [SerializeField] private float directionWeight;
    [SerializeField] private float separationWeight;
	[SerializeField] private float separationDistance;
    [SerializeField] private float centroidWeight;

    #region Properties

    public float MaxSpeed { get { return maxSpeed; } }
    public float MaxForce { get { return maxForce; } }
    public float Mass { get { return mass; } }
    public float Radius { get { return radius; } }

    public float SeekWeight { get { return seekWeight; } }
    public float InBoundsWeight { get { return inBoundsWeight; } }
    public float AvoidWeight { get { return avoidWeight; } }
    public float AvoidDistance { get { return avoidDistance; } }

    public float DirectionWeight { get { return directionWeight; } }
    public float SeparationWeight { get { return separationWeight; } }
	public float SeparationDistance { get { return separationDistance; } }
    public float CentroidWeight { get { return centroidWeight; } }

    #endregion
}
