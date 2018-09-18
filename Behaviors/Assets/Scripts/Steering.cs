using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
public class Steering {

	/// <summary>
	/// Creates a force that will move towards a target
	/// </summary>
	/// <param name="targetPosition"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="controller"></param>
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
	/// Creates a force that will move towards a target. Used for job system
	/// </summary>
	/// <param name="targetPosition"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="velocity"></param>
	/// <returns></returns>
	public static Vector3 SeekJob(Vector3 targetPosition, Vector3 currentPosition, float maxSpeed, Vector3 velocity)
	{
		Vector3 changeInVelocity = Vector3.zero;
		changeInVelocity = targetPosition - currentPosition;
		changeInVelocity = changeInVelocity.normalized * maxSpeed;
		changeInVelocity -= velocity;
		return changeInVelocity;
	}

	/// <summary>
	/// Creates a force that will push the gameobject in the direction the entire flock is going
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="controller"></param>
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
	/// Creates a force that will push the gameobject in the direction the entire flock is going. Used in job system
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="velocity"></param>
	/// <returns></returns>
	public static Vector3 AlignmentJob(Vector3 direction, Vector3 currentPosition, float maxSpeed, Vector3 velocity)
	{
		Vector3 changeInVelocity = Vector3.zero;
		changeInVelocity = direction - currentPosition;
		changeInVelocity = changeInVelocity.normalized * maxSpeed;
		changeInVelocity -= velocity;
		changeInVelocity.y = 0;
		return changeInVelocity;
	}

	/// <summary>
	/// Creates a force that will pull the flock closer together
	/// </summary>
	/// <param name="centroid"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="controller"></param>
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
	/// Creates a force that will pull the flock closer together. Used for job system
	/// </summary>
	/// <param name="centroid"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="veloicty"></param>
	/// <returns></returns>
	public static Vector3 CohesionJob(Vector3 centroid, Vector3 currentPosition, float maxSpeed, Vector3 velocity)
	{
		Vector3 changeInVelocity = Vector3.zero;
		changeInVelocity = centroid - currentPosition;
		changeInVelocity = changeInVelocity.normalized * maxSpeed;
		changeInVelocity -= velocity;
		return changeInVelocity;
	}

	/// <summary>
	/// Creates a force that will push the flock further apart
	/// </summary>
	/// <param name="flockers">List of flockers from Manager</param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="controller"></param>
	/// <returns></returns>
	public static Vector3 Separation(List<GameObject> flockers, Vector3 currentPosition, float maxSpeed, CharacterController controller)
    {
        Vector3 changeInVelocity = Vector3.zero;
        Vector3 distance = Vector3.zero;
        Vector3 overallForce = Vector3.zero;

        for(int i = 0; i < flockers.Count; i++)
        {
            distance = currentPosition - flockers[i].transform.position;
            if(distance.magnitude < 3.5)
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
	/// Creates a force that will push the flock further apart. Used for job system
	/// </summary>
	/// <param name="flockers">List of flockers from Manager</param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="velocity"></param>
	/// <returns></returns>
	public static Vector3 SeparationJob(NativeArray<Vector3> flockers, Vector3 currentPosition, float maxSpeed, Vector3 velocity, float avoidDistance)
	{
		Vector3 changeInVelocity = Vector3.zero;
		Vector3 distance = Vector3.zero;
		Vector3 overallForce = Vector3.zero;

		for (int i = 0; i < flockers.Length - 1; i++)
		{
			distance = currentPosition - flockers[i];
			if (distance.magnitude < avoidDistance)
			{
				changeInVelocity = currentPosition - flockers[i];
				changeInVelocity = changeInVelocity.normalized;
				if (distance.magnitude > 0)
				{
					changeInVelocity = changeInVelocity / distance.magnitude;
				}
				overallForce += changeInVelocity;

			}
		}

		overallForce = overallForce.normalized * maxSpeed;
		overallForce -= velocity;

		return overallForce;

	}

	/// <summary>
	/// Creates a force that will push the flock further apart. Used for job collision system
	/// </summary>
	/// <param name="flockers">List of flockers from Manager</param>
	/// <param name="indexToCheck"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="velocity"></param>
	/// <returns></returns>
	public static Vector3 SeparationJobCollision(NativeArray<Vector3> flockers, NativeMultiHashMap<int,int> collisionIndex, int index, float maxSpeed, Vector3 velocity)
	{
		Vector3 changeInVelocity = Vector3.zero;
		Vector3 overallForce = Vector3.zero;
		NativeMultiHashMapIterator<int> iterator;
		int separationIndex;
		if(collisionIndex.TryGetFirstValue(index, out separationIndex, out iterator))
		{
			do
			{
				changeInVelocity = flockers[index] - flockers[separationIndex];
				if (changeInVelocity.magnitude > 0)
				{
					Vector3 distance = changeInVelocity;
					changeInVelocity = changeInVelocity.normalized;
					changeInVelocity = changeInVelocity / distance.magnitude;
				}
				overallForce += changeInVelocity;

			} while (collisionIndex.TryGetNextValue(out separationIndex, ref iterator));
		}

		overallForce = overallForce.normalized * maxSpeed;
		overallForce -= velocity;

		return overallForce;

	}
	/// <summary>
	/// Creates a force to prevent wandering out of the area
	/// </summary>
	/// <param name="radius"></param>
	/// <param name="center"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="controller"></param>
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
	/// Creates a force to prevent wandering out of the area. Used for job sytem
	/// </summary>
	/// <param name="radius"></param>
	/// <param name="center"></param>
	/// <param name="currentPosition"></param>
	/// <param name="maxSpeed"></param>
	/// <param name="velocity"></param>
	/// <returns></returns>
	public static Vector3 StayInBoundsJob(float radius, Vector3 center, Vector3 currentPosition, float maxSpeed, Vector3 velocity)
	{
		if (Vector3.Distance(currentPosition, center) > radius)
		{
			return SeekJob(center, currentPosition, maxSpeed, velocity);
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

	/// <summary>
	/// Creates a force to avoid obstacles in the path. Used for the job system version
	/// </summary>
	/// <param name="obstaclePosition"></param>
	/// <param name="obstacleRadius"></param>
	/// <param name="safeDistance"></param>
	/// <param name="flockerPosition"></param>
	/// <param name="rotation"></param>
	/// <param name="steeringAttributes"></param>
	/// <returns></returns>
	public static Vector3 AvoidObstacleJob(Vector3 obstaclePosition, float obstacleRadius, float safeDistance, Vector3 flockerPosition, Quaternion rotation, SteeringAttributes steeringAttributes)
	{
		Vector3 changeInVelocity = Vector3.zero;
		Vector3 forward = GetForwardFromQuaternion(rotation);
		Vector3 right = GetRightFromQuaternion(rotation);

		// Ignore if above or below the obstacle
		if (flockerPosition.y > obstaclePosition.y + obstacleRadius + 3 || flockerPosition.y < obstaclePosition.y - obstacleRadius - 3)
		{
			return Vector3.zero;
		}

		Vector3 vectorToCenter = obstaclePosition - flockerPosition;
		vectorToCenter.y = 0;
		float distance = vectorToCenter.magnitude;

		// Ignore if too far away
		if (distance > safeDistance + obstacleRadius + steeringAttributes.Radius)
		{
			return Vector3.zero;
		}
		
		float dot = Vector3.Dot(forward, vectorToCenter);
		// Ignore if obstacle is behind
		if (dot < 0)
		{
			return Vector3.zero;
		}

		float obstacleDistanceToSide = Vector3.Dot(right, vectorToCenter);

		// Ignore if the obstacle is very far to the side
		if (Mathf.Abs(obstacleDistanceToSide) > steeringAttributes.Radius + obstacleRadius)
		{
			return Vector3.zero;
		}

		// Obstacle is in the way
		// Determine whether to go to the right or left
		if (obstacleDistanceToSide > 0)
		{
			changeInVelocity += right * -steeringAttributes.MaxSpeed * safeDistance / distance;
		}
		else
		{
			changeInVelocity += right * steeringAttributes.MaxSpeed * safeDistance / distance;
		}

		return changeInVelocity;
	}

	private static Vector3 GetForwardFromQuaternion(Quaternion rotation)
	{
		return rotation * new Vector3(0, 0, 1);
	}

	private static Vector3 GetRightFromQuaternion(Quaternion rotation)
	{
		return rotation * new Vector3(1, 0, 0);
	}
}
