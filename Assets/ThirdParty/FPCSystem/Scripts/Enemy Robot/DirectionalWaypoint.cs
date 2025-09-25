using UnityEngine;
using System.Collections;

public class DirectionalWaypoint : MonoBehaviour
{
    // The start waypoint, this is initialized in Awake.
    // This variable is static thus all instances of the waypoint script share it.
    // public  DirectionalWaypoint start;

    // The next waypoint, this variable needs to be assigned in the inspector.
    // You can select all waypoints to see the full waypoint path.
    public DirectionalWaypoint next;

    // This is used to determine where the start waypoint is.
    public bool isStart;

	private Transform myTransform;

	// This initializes the start and goal static variables.
	// We have to do this inside Awake because the waypoints need 
	// to be initialized before the AI scripts use it.
	// All Awake function are always called before all Start functions.
	void Awake()
	{
		if (!this.next)
			Debug.Log("This waypoint is not connected, you need to set the next waypoint!", this);
	}

	void Start()
	{
		myTransform = this.transform;
	}

    // Returns where the AI should drive towards position is the current position of the car.
    public DirectionalWaypoint CalculateTargetPosition(Vector3 position, float nDistance)
    {
        // If we are getting close to the waypoint, we return the next waypoint.
        // This gives us better car behaviour when cars don't exactly hit the waypoint
		if (CalculateDistance(position) < nDistance)
            return next;
        else
			return this;	// We are still far away from the next waypoint, just return the waypoints position
    }

    public float CalculateDistance(Vector3 position)
    {
        // If we are getting close to the waypoint, we return the next waypoint.
        // This gives us better car behaviour when cars don't exactly hit the waypoint
		return Vector3.Distance(myTransform.position, position);
    }

    

    /*function OnDrawGizmos () {
		Gizmos.DrawIcon (transform.position, "waypoint.png");
		if (next) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine (transform.position, next.transform.position);
		}
	}*/

	// Draw the waypoint lines only when you select one of the waypoints
	/*function OnDrawGizmosSelected () {
		Gizmos.DrawIcon (transform.position, "waypoint.png");
		if (next) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine (transform.position, next.transform.position);
		}
	}*/
}