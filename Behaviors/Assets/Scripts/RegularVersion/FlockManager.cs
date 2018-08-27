using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockManager : MonoBehaviour {

    private GameObject flockTarget;
    private Vector3 centroid;
    private Vector3 flockDirection;
    private List<GameObject> flockers;
    private List<GameObject> obstacles;

    [SerializeField] private GameObject flockTargetPrefab;
    [SerializeField] private GameObject flockerPrefab;
    [SerializeField] private GameObject centroidObject;
    [SerializeField] private List<GameObject> obstaclePrefabs;
    [SerializeField] private SteeringAttributes steeringAttributes;

    [SerializeField] private float obstacleScaleMax = 8.0f;
    [SerializeField] private int numberOfFlockers = 2;
    [SerializeField] private int numberOfObstacles = 0;

    #region Properties

    public Vector3 Centroid { get { return centroid; } }

    public Vector3 FlockDirection {  get { return flockDirection; } }

    public List<GameObject> Flockers {  get { return flockers; } }
    public List<GameObject> Obstacles { get { return obstacles; } }

    public SteeringAttributes FlockAttributes {  get { return steeringAttributes; } }
    #endregion


    // Use this for initialization
    void Start () {
        Vector3 position = new Vector3(Random.Range(-80, 80), Random.Range(2, 60), Random.Range(-80, 80));
        flockTarget = (GameObject)Instantiate(flockTargetPrefab, position, Quaternion.identity);

        centroidObject = new GameObject();


        // Make the flockers
        flockers = new List<GameObject>();
        for(int i = 0; i < numberOfFlockers; i++)
        {
            position = new Vector3(Random.Range(-20, 20), Random.Range(8, 15), Random.Range(-20, 20));
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 90), 0);
            flockers.Add((GameObject)Instantiate(flockerPrefab, position, rotation));
			SteeringVehicle steeringVehicle = flockers[i].GetComponent<SteeringVehicle>();
			steeringVehicle.Target = flockTarget;
			steeringVehicle.Manager = this;
			flockers[i].GetComponent<Animator>().SetFloat("offset", Random.Range(0.0f, 1.0f));

        }

        // Make the obstacles
        obstacles = new List<GameObject>();
        for(int i = 0; i < numberOfObstacles; i++)
        {
            position = new Vector3(Random.Range(-40, 40), 15.0f, Random.Range(-40, 40));
            Quaternion rotation = Quaternion.Euler(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));
            GameObject obstacle = (GameObject)Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], position, rotation);

            float ObstacleScale = Random.Range(2f, obstacleScaleMax);
            obstacle.transform.localScale = new Vector3(ObstacleScale, ObstacleScale, ObstacleScale);
            obstacles.Add(obstacle);
        }

        Camera.main.GetComponent<CameraSmoothFollow>().Target = centroidObject.transform;

	}
	
	// Update is called once per frame
	void Update () {
        UpdateFlockInformation();

        // Move the target if the flock gets too close
        if(Vector3.Distance(centroid, flockTarget.transform.position) < (numberOfFlockers / 20) + 1)
        {
            flockTarget.transform.position = new Vector3(Random.Range(-80, 80), Random.Range(2, 60), Random.Range(-80, 80));
			flockTarget.transform.rotation = Quaternion.LookRotation(centroid - flockTarget.transform.position);
        }

        centroidObject.transform.position = centroid;
        centroidObject.transform.rotation = Quaternion.LookRotation(flockDirection);
	}

    /// <summary>
    /// Updates the direction the flock is going and the centroids position
    /// </summary>
    private void UpdateFlockInformation()
    {
        Vector3 positionSum = Vector3.zero;

        for(int i = 0; i < numberOfFlockers; i++)
        {
            positionSum += flockers[i].transform.position;
            flockDirection += flockers[i].GetComponent<SteeringVehicle>().Velocity.normalized;
        }

        centroid = positionSum / numberOfFlockers;
        centroid.y += 1;
        flockDirection.Normalize();
    }
}
