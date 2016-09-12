using UnityEngine;
using System.Collections;

[System.Serializable]
public class SteeringAttributes {
 
    [SerializeField] private float maxSpeed = 12.0f;
    [SerializeField] private float maxForce = 12.0f;
    [SerializeField] private float mass = 1.0f;
    [SerializeField] private float radius = 1.0f;

    [SerializeField] private float seekWeight = 80.0f;
    [SerializeField] private float inBoundsWeight = 20.0f;
    [SerializeField] private float avoidWeight = 40.0f;
    [SerializeField] private float avoidDistance = 14.0f;

    [SerializeField] private float directionWeight = 10.0f;
    [SerializeField] private float separationWeight = 45.0f;
    [SerializeField] private float centroidWeight = 5.0f;

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
    public float CentroidWeight { get { return centroidWeight; } }


    #endregion
}
