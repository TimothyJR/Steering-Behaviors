using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    [SerializeField] private float radius = -1.0f;

    public float Radius
    {
        get { return radius; }
    }

    // Use this for initialization
    void Start () {
	    if(radius < 0)
        {
            float s = transform.localScale.x / 2.0f;
            radius = Mathf.Sqrt(s * s + s * s);
        }
	}
	
}
