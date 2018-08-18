using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steering {

    /// <summary>
    /// Creates a force that will move towards a target
    /// </summary>
    /// <param name="currentPosition">Position of the transform that is doing the seeking</param>
    /// <param name="targetPosition">Position to go towards</param>
    /// <param name="maxSpeed">Max speed</param>
    /// <param name="rigidbody"></param>
    /// <returns></returns>
    public static Vector3 Seek(Vector3 targetPosition, Vector3 currentPosition, float maxSpeed, CharacterController controller)
    {
        Vector3 changeInVelocity = Vector3.zero;
        changeInVelocity = targetPosition - currentPosition;
        changeInVelocity = changeInVelocity.normalized * maxSpeed;
        changeInVelocity -= controller.velocity;
        return changeInVelocity;
    }

    /// <summary>
    /// Creates a force that will push the gameobject in the direction the entire flock is going
    /// </summary>
    /// <param name="direction">Direction fo the flock</param>
    /// <param name="currentPosition"></param>
    /// <param name="maxSpeed"></param>
    /// <param name="rigidbody"></param>
    /// <returns></returns>
    public static Vector3 Alignment(Vector3 direction, Vector3 currentPosition, float maxSpeed, CharacterController controller)
    {
        Vector3 changeInVelocity = Vector3.zero;
        changeInVelocity = direction - currentPosition;
        changeInVelocity = changeInVelocity.normalized * maxSpeed;
        changeInVelocity -= controller.velocity;
        changeInVelocity.y = 0;
        return changeInVelocity;
    }

    /// <summary>
    /// Creates a force that will pull the flock closer together
    /// </summary>
    /// <param name="centroid"></param>
    /// <param name="currentPosition"></param>
    /// <param name="maxSpeed"></param>
    /// <param name="rigidbody"></param>
    /// <returns></returns>
    public static Vector3 Cohesion(Vector3 centroid, Vector3 currentPosition, float maxSpeed, CharacterController controller)
    {
        Vector3 changeInVelocity = Vector3.zero;
        changeInVelocity = centroid - currentPosition;
        changeInVelocity = changeInVelocity.normalized * maxSpeed;
        changeInVelocity -= controller.velocity;
        return changeInVelocity;
    }
   
    /// <summary>
    /// Creates a force that will push the flock further apart
    /// </summary>
    /// <param name="flockers">List of flockers from Manager</param>
    /// <param name="currentPosition"></param>
    /// <param name="maxSpeed"></param>
    /// <param name="rigidbody"></param>
    /// <returns></returns>
    public static Vector3 Separation(List<GameObject> flockers, Vector3 currentPosition, float maxSpeed, CharacterController controller)
    {
        Vector3 changeInVelocity = Vector3.zero;
        Vector3 distance = Vector3.zero;
        Vector3 overallForce = Vector3.zero;

        for(int i = 0; i < flockers.Count; i++)
        {
            distance = currentPosition - flockers[i].transform.position;
            if(distance.magnitude < 5)
            {
                changeInVelocity = currentPosition - flockers[i].transform.position;
                changeInVelocity = changeInVelocity.normalized;
                if (distance.magnitude > 0)
                {
                    changeInVelocity = changeInVelocity / distance.magnitude;
                }
                overallForce += changeInVelocity;

            }
        }

        overallForce = overallForce.normalized * maxSpeed;
        overallForce -= controller.velocity;

        return overallForce;

    } 

    /// <summary>
    /// Creates a force to prevent wandering out of the area
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="center"></param>
    /// <param name="currentPosition"></param>
    /// <param name="maxSpeed"></param>
    /// <param name="rigidbody"></param>
    /// <returns></returns>
    public static Vector3 StayInBounds(float radius, Vector3 center, Vector3 currentPosition, float maxSpeed, CharacterController controller)
    {
        if(Vector3.Distance(currentPosition, center) > radius)
        {
            return Seek(center, currentPosition, maxSpeed, controller);
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Creates a force to avoid obstacles in the path
    /// </summary>
    /// <param name="obstacle"></param>
    /// <param name="safeDistance"></param>
    /// <param name="flockerTransform"></param>
    /// <param name="steeringAttributes"></param>
    /// <returns></returns>
    public static Vector3 AvoidObstacle(GameObject obstacle, float safeDistance, Transform flockerTransform, SteeringAttributes steeringAttributes)
    {
        Vector3 changeInVelocity = Vector3.zero;
        float obstacleRadius = obstacle.GetComponent<Obstacle>().Radius;

        // Ignore if above or below the obstacle
        if (flockerTransform.position.y > obstacle.transform.position.y + obstacleRadius + 3 || flockerTransform.position.y < obstacle.transform.position.y - obstacleRadius - 3)
        {
            return Vector3.zero;
        }

        Vector3 vectorToCenter = obstacle.transform.position - flockerTransform.position;
        vectorToCenter.y = 0;
        float distance = vectorToCenter.magnitude;

        // Ignore if too far away
        if(distance > safeDistance + obstacleRadius + steeringAttributes.Radius)
        {
            return Vector3.zero;
        }
        float dot = Vector3.Dot(flockerTransform.forward, vectorToCenter);
        // Ignore if obstacle is behind
        if (dot < 0)
        {
            return Vector3.zero;
        }

        float obstacleDistanceToSide = Vector3.Dot(flockerTransform.right, vectorToCenter);

        // Ignore if the obstacle is very far to the side
        if(Mathf.Abs(obstacleDistanceToSide) > steeringAttributes.Radius + obstacleRadius)
        {
            return Vector3.zero;
        }

        // Obstacle is in the way
        // Determine whether to go to the right or left
        if(obstacleDistanceToSide > 0)
        {
            changeInVelocity += flockerTransform.right * -steeringAttributes.MaxSpeed * safeDistance / distance;
        }
        else
        {
            changeInVelocity += flockerTransform.right * steeringAttributes.MaxSpeed * safeDistance / distance;
        }

        return changeInVelocity;
    }
}
