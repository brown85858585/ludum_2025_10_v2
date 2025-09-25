// trajectory code that works very well. Doesn't detect collisions.
// https://stackoverflow.com/questions/37580258/how-to-draw-projectile-trajectory-with-unity3ds-built-in-physics-engine
//
//

using UnityEngine;

/// <summary>
/// Controls the Laser Sight for the player's aim
/// </summary>
public class TrajectorySimulation : MonoBehaviour
{
    // Reference to the LineRenderer we will use to display the simulated path
    public LineRenderer sightLine;

    // Reference to a Component that holds information about fire strength, location of cannon, etc.
    //public PlayerFire playerFire;

    //public float launchForce = 15;
    public Color lineColor = Color.blue;

    // Number of segments to calculate - more gives a smoother line
    public int segmentCount = 20;

    // Length scale for each segment
    public float segmentScale = 1;

    private CatchObjectOptions catchObjectOptionsSrc;
    private GameObject MyObj;
    private Status statusSrc;
    private CatchObject catchObjectSrc;


    void Start()
    {
        catchObjectOptionsSrc = GetComponent<CatchObjectOptions>();

         MyObj = GameObject.FindWithTag("Player"); //GameObject.Find("Player");
         if (MyObj == null)
         {
             Debug.LogError("Player NOT Found!");
         }
         else
         {
             statusSrc = MyObj.GetComponent<Status>();
         }

         catchObjectSrc = statusSrc.GetCatchObjectScr();

        // Set the colour of our path to the colour of the next ball
        Color startColor = lineColor;
        Color endColor = startColor;
        startColor.a = 1;
        endColor.a = 0;
        sightLine.startColor = startColor;
        sightLine.endColor = endColor;
        //sightLine.SetColors(startColor, endColor);


        Init(Physics.gravity, catchObjectOptionsSrc.launchForce, 1f, 0f);
    }


    // The way to calculate drag in to match the Unity engine is to compute and keep track of velocity in FixedUpdate and then update velocity for Drag.
    // The Unity formula for Drag was provided here: http://forum.unity3d.com/threads/drag-factor-what-is-it.85504/
    //
    // It is indeed simple; the key part is this:
    // velocity *= Mathf.Clamp01(1f - Drag* Time.fixedDeltaTime);
    //
    // The custom Physics class had to be reworked to require that the time increment be Unity's Time.fixedDeltaTime 
    // (the time between FixedUpdate calls)
    private Vector3 Gravity;
    private float Force, Mass, Drag;
    public void Init(Vector3 gravity, float force, float mass, float drag)
    {
        Gravity = gravity;
        Force = force;
        Mass = mass;
        Drag = drag;
    }

    /// <summary>
    /// Computes an array of Trajectory object positions by time.
    /// </summary>
    /// <returns>Number of positions filled into the buffer</returns>
    /// <param name="startPos">Starting Position</param>
    /// <param name="direction">Direction (and magnitude) vector</param>
    /// <param name="yFloor">Minimum height, below which is clipped</param>
    /// <param name="positions">Buffer to fill with positions</param>
    public int GetPositions(Vector3 startPos, Vector3 direction, float yFloor, Vector3[] positions)
    {
        float timeIncrement = Time.fixedDeltaTime;

        int maxItems = positions.Length;
        int i = 0;
        Vector3 velocity = direction * Force / Mass;
        Vector3 pos = startPos;
        for (; i < maxItems; i++)
        {
            velocity += Gravity * timeIncrement;
            velocity *= Mathf.Clamp01(1f - Drag * timeIncrement); // DRAG calculation!!!
            pos += velocity * timeIncrement;
            if (pos.y < yFloor)
                break;
            positions[i] = pos;
        }
        return i;
    }

    // No calculating the drag paramenter as it should.
    /*private Vector3 HalfGravity;
    private float ForceMultiplier;
    public void Init(Vector3 gravity, float slingForce, float mass, float drag)
    {
        HalfGravity = gravity / 2.0f;
        ForceMultiplier = slingForce / mass / (1 + drag);
    }

    public Vector3 GetPosition(Vector3 start, Vector3 direction, float timeDelta)
    {
        Vector3 pos = direction * (timeDelta * ForceMultiplier);
        pos.y += (HalfGravity.y * timeDelta * timeDelta);
        return start + pos;
    }

    /// <summary>
    /// Computes an array of Trajectory object positions by time.
    /// </summary>
    /// <returns>Number of positions filled into the buffer</returns>
    /// <param name="startPos">Starting Position</param>
    /// <param name="direction">Direction (and magnitude) vector</param>
    /// <param name="timeIncrement">Time increment of the samples</param>
    /// <param name="yFloor">Minimum height, below which is clipped</param>
    /// <param name="positions">Buffer to fill with positions</param>
    public int GetPositions(Vector3 startPos, Vector3 direction, float timeIncrement, float yFloor, Vector3[] positions)
    {
        int maxItems = positions.Length;
        int i = 0;
        for (; i < maxItems; i++)
        {
            Vector3 pos = GetPosition(startPos, direction, timeIncrement * i);
            if (pos.y < yFloor)
                break;
            positions[i] = pos;
        }
        return i;
    }*/


    void FixedUpdate()
    {
        // If the ball has been catched by pl, draw the trajectory
        if (catchObjectSrc.IsCatched() /*|| CatchObjectSRC.IsLaunching()*/)
        {
            if(catchObjectSrc.GetCatchedObject() == this.gameObject)
                SimulatePath();
        }
        else
        {
            //sightLine.SetVertexCount(0);
            #if UNITY_5_5
            sightLine.numPositions = 0;
            #else
            sightLine.positionCount = 0;
            #endif
        }
    }

    private void SimulatePath()
    {
        if (catchObjectSrc.catchHelper.transform.childCount == 0) return;

        Vector3[] segments = new Vector3[segmentCount];

        // The first line point is wherever the player's cannon, etc is
        Vector3 startPos = catchObjectSrc.catchHelper.transform.GetChild(0).position;//Camera.main.transform.forward - Camera.main.transform.up;
        GetPositions(startPos, Camera.main.transform.forward, -10f, segments);

        // At the end, apply our simulations to the LineRenderer

        //sightLine.SetVertexCount(segmentCount);
        #if UNITY_5_5
        sightLine.numPositions = segmentCount;
        #else
        sightLine.positionCount = segmentCount;
        #endif
        for (int i = 0; i < segmentCount; i++)
            sightLine.SetPosition(i, segments[i]);
    }

}