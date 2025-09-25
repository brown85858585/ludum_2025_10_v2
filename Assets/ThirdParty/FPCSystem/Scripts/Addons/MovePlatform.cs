using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{

    public Transform[] waypoints;
    public float speed = 1f;
    private float radius = 1f;

    [SerializeField]
    [Disable]
    private int currentWaypoint = 0;

    private Transform myTransform;

	void Start ()
    {
        myTransform = transform;
    }
	

	void Update ()
    {
		if(Vector3.Distance(waypoints[currentWaypoint].position, myTransform.position) < radius)
        {
            currentWaypoint++;
            if(currentWaypoint == waypoints.Length) { currentWaypoint = 0; }
        }

        myTransform.position = Vector3.MoveTowards(myTransform.position, waypoints[currentWaypoint].position, Time.deltaTime * speed);
    }
}
