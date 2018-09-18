using UnityEngine;
using System.Collections;

public class CameraSmoothFollow : MonoBehaviour {

    private Transform target;

    [SerializeField] private float distance = 6.0f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private float heightDamping = 4.0f;
    [SerializeField] private float positionDamping = 4.0f;
    [SerializeField] private float rotationDamping = 4.0f;
	[SerializeField] private float maxHeight = 15.0f;
    public Transform Target {  set { target = value; } }

	void FixedUpdate()
    {
        // Do nothing if there is no target
        if(!target)
        {
            return;
        }

        float wantedHeight = target.position.y + height;
		if(wantedHeight < 1.0f)
		{
			wantedHeight = 1.0f;
		}
		else if(wantedHeight > maxHeight)
		{
			wantedHeight = maxHeight;
		}
        float currentHeight = transform.position.y;

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        Vector3 wantedPosition = target.position - target.right * distance;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        transform.forward = Vector3.Lerp(transform.forward, target.position - transform.position, rotationDamping * Time.deltaTime);
    }
}
