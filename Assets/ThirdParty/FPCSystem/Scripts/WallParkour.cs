using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallParkour : MonoBehaviour {

	[Tooltip("Wall Run options (Parkour!).")]
	public WallRunOptions wallRun = new WallRunOptions();

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;

    //
    // Private vars
    //
    
    // Wall Walk region
    private Vector3 currentVelocity; // WallRun Current velocity (used internally to interpolate de gravity)
	private Transform lastObjectWallRunned; // Last scene object that the wall-walk (side walk if you like)
    private Vector3 wallNormal;     // The normal of the wall we are running over.

	// not allowed to wallwalk in the same object twice if not ground touched.
	private float wallrunTime = 0.5f; // Internal time to avoid an error when jumping out the wall (is detected twice sometimes).
	private float internalWallrunTime; // This will not allow to wallwalk again in 0.2 secs.
	private bool isWallLeft = true; // To Detect at what side is the wall (to move the camera)
	private bool isWallJumpExited; // To know if we finished the wallRun Jumping.

	// Internal time to apply force when walking on the wall
	private float sideWalkInternalTime;
	private Vector3 dirWalkWall = Vector3.zero;
	private Vector3 perp = Vector3.zero;

    private GameObject mainCamera;
    private Status statusSrc;
	private Core coreScr;
	private CharacterController controller;
	private CharacterMotor motor;
    private MouseLook myMouseLook;

    void Start () {
        mainCamera = Camera.main.gameObject;
        myMouseLook = Camera.main.GetComponent<MouseLook>();
        statusSrc = GetComponent<Status>();
		coreScr = statusSrc.GetCoreScr();
		controller = statusSrc.GetController();
		motor = statusSrc.GetMotor();
	}


	public void ProcessWallParkour () {
        if (!enabled) return;

        if (wallRun.canWallrun)
		{ 
			PreProcessWallRunState ();
			WallRunState ();
		}
	}


    // Detect is we can make wall running & detect we we touch the ground (to exit a current wall-running)..
    private void PreProcessWallRunState()
    {
        // Security sentence
        if ((!statusSrc.isWallRunning && !statusSrc.isWallRunDetected && lastObjectWallRunned == null) &&
             statusSrc.isInLadder || statusSrc.isInLedge || statusSrc.isInLowLedge  || statusSrc.isProned || statusSrc.isCrouched)
            return;

        // If not running, wallrun is imposible.
        if (!statusSrc.isRunning)
        {
            statusSrc.isWallRunDetected = false;
        }
        else
        { 
            if (!statusSrc.isWallRunDetected && !statusSrc.isWallRunning)
            {
                if (wallRun.jumpFirst)
                {
                    statusSrc.isWallRunDetected = statusSrc.isJumping && statusSrc.isInAir;
                }
                else
                {
                    statusSrc.isWallRunDetected = statusSrc.isInGround || statusSrc.isJumping;
                }
            }
            else
                statusSrc.isWallRunDetected = false;
        }

        // Exit  wallrun if ground is detected or try to run in the same object.
        if (statusSrc.isInGround && lastObjectWallRunned != null)
        { 
            ExitWallRun ();
			isWallJumpExited = false;
            internalWallrunTime = Time.time + wallRun.wallrunGroundedTime;
            statusSrc.isWallRunEndNow = true;
            if (showDebug) Debug.Log("WallParkour -> PreProcessWallRunState() -> Exit wallrun because pl is grounded");
        }

        // Exit  wallrun if entered in a not compatible state.
        if (statusSrc.isInLadder || statusSrc.isInLedge || statusSrc.isProned || statusSrc.isCrouched)
        {
            ExitWallRun();
            isWallJumpExited = false;
            internalWallrunTime = Time.time + wallRun.wallrunGroundedTime;
            if (showDebug) Debug.Log("WallParkour -> PreProcessWallRunState() -> Exit wallrun because player has entered in other state");
        }
    }


    private void WallRunState()
	{
		//if (!wallRun.canWallrun) { return; }	 // Security checks to do nothing if that's possible.
		if (!statusSrc.isWallRunning && !statusSrc.isWallRunDetected) { return; }
		if (Time.time <= internalWallrunTime) { return; }


		RaycastHit hit;
		Vector3 dirRight, p1, p2;
		bool IsHit = false;

		// Check against what we are going to WallRun.
		// Objects with rigidbodies are ignored.
		dirRight = transform.TransformDirection(Vector3.right) * 2f;
        p1 = (transform.position + controller.center);// + (Vector3.up * (-controller.height * 0.3f));
		p2 = p1 + (Vector3.up * controller.height * 0.5f);
		isWallLeft = false;

		if (Physics.CapsuleCast(p1, p2, controller.radius, -dirRight, out hit, wallRun.minWallDistance, (int) wallRun.wallMasks))
		{
			if (hit.rigidbody == null)
			{
				IsHit = true;
				isWallLeft = true;
			}
		}

		if (!IsHit)
		{
			if (Physics.CapsuleCast(p1, p2, controller.radius, dirRight, out hit, wallRun.minWallDistance, (int) wallRun.wallMasks))
			{
				if (hit.rigidbody == null)
					IsHit = true;
			}
		}

        // We are wall running but there is not a wall anymore.
        /*if (StatusSrc.isWallRunning && !IsHit)
        {
            if(showDebug) Debug.Log("NO WALL");
            ExitWallRun();
            Vector3 velo = motor.movement.velocity;
            velo.y = 0f;
            motor.SetVelocity(velo);
        }*/


        // We detect an object that we can use to wall walk.
        if (IsHit)
        {
            //if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Height:" + hit.transform.gameObject.GetComponent<Renderer>().bounds.extents);
            if (hit.transform.gameObject.GetComponent<Renderer>().bounds.extents.y < controller.height * 1.3f)
            {
                if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Height requeriments NOT fullfilled:" + hit.transform.gameObject.GetComponent<Renderer>().bounds.extents.y);
                statusSrc.isWallRunDetected = false;
                return;
            }

            // Inicialize procedure.
            if (!statusSrc.isWallRunning)
			{
                
                if (!(statusSrc.IsEqual(hit.normal, hit.transform.TransformDirection(Vector3.left), 0.1f) || 
                    statusSrc.IsEqual(hit.normal, hit.transform.TransformDirection(Vector3.right), 0.1f) ||
                    statusSrc.IsEqual(hit.normal, hit.transform.TransformDirection(Vector3.forward), 0.1f) || 
                    statusSrc.IsEqual(hit.normal, hit.transform.TransformDirection(Vector3.back), 0.1f)) )
                {
                    if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Normal requeriments NOT fullfilled:" + hit.normal);
                    statusSrc.isWallRunDetected = false;
                    return;
                }

                // Dont allow to walk against the same object over a over again.
                if (lastObjectWallRunned == null || (lastObjectWallRunned != hit.transform && isWallJumpExited))
				{
                    if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Entering wallrun for first time:" + hit.normal);

                    lastObjectWallRunned = hit.transform;
                    wallNormal = hit.normal;
                    currentVelocity = wallRun.initialWallrunSpeed;
					statusSrc.isWallRunDetected = false;
					statusSrc.isWallRunning = true;
					coreScr.testJumpStateTime = Time.time + 0.1f;
                    myMouseLook.wallRunEnterRotationSpeed = wallRun.wallRunEnterRotationSpeed;
                    myMouseLook.wallRunExitRotationSpeed = wallRun.wallRunExitRotationSpeed;

                    // Make the camera effects.
                    SendMessage("SetCameraMoveTime", wallRun.cameraMovTime); // Move the camera's center for bobbing FX in that new position.
					if (isWallLeft)
					{
						SendMessage("SetMidpointX", wallRun.cameraLateralMov);
                        mainCamera.SendMessage("SetRotationZ", -wallRun.cameraAngleRotation);
					}
					else
					{
						SendMessage("SetMidpointX", -wallRun.cameraLateralMov);
                        mainCamera.SendMessage("SetRotationZ", wallRun.cameraAngleRotation);
					}
				}
				else
				{

                    ExitWallRun();
                    if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Exit wallrun because running the same wall again");
                    return;
                }

                // Low velocity can happen by running towards the wall (even if you stay running your velocity wll decrease for sure)
                if (controller.velocity.magnitude < coreScr.GetMinimumWalkVelocity())
				{
                    ExitWallRun();
                    if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Exit wallrun because low velocity");
                    return;
				}
			}

			// Change the vertical velocity to emulate gravity.
			currentVelocity.y = Mathf.Lerp(currentVelocity.y, -20, wallRun.gravityMult * Time.deltaTime);
			
            // Figure out the direction of pl movenent (parallel to the wall).
			Vector3 dirFwrd = transform.TransformDirection(Vector3.forward);
			perp = Vector3.Cross(hit.normal, Vector3.up).normalized;
            
			dirWalkWall = Vector3.Cross(dirFwrd.normalized, perp);
			dirWalkWall = (dirFwrd + Vector3.up) + -hit.normal;
			dirWalkWall.Normalize();

            // DEBUG PURPOSES ONLY TO SEE HOW THESE VALUES CHANGE WHEN MOVING THE CAMERA WHEN WALL RUNNING
            // we are not looking at the wall when running, means that dirWalkWall goes to 1.0f in 'y' coord
            // looking directly to the wall is 0.0f
            //Debug.Log("dirFwrd"+ dirFwrd+" dirWalkWall: "+ dirWalkWall);

            // If we are in the wall, we are already running and the Jump Button is released, then we force the isJumping state to false.
            // Such thing doesn't happen in the normal jump where if you release the jump button, isJumping remain true until you touch the ground.
            if (statusSrc.isJumpReleased && statusSrc.isJumping)
				statusSrc.isJumping = false;

            // Check if we stop running or if a jump is performed by the user.
            // If that happen the wallwalk has to stop (and cant not be performed again in THIS object
            // until pl hits ground, other object are allowed).
            if (statusSrc.isWallRunning)
            {
                // we are not looking at the wall when running, means that dirWalkWall goes to 1.0f in 'y' coord
                // looking directly to the wall is 0.0f
                if (dirWalkWall.y >= wallRun.maxLookAway || controller.velocity.magnitude < coreScr.GetMinimumWalkVelocity() || !statusSrc.isRunning || (statusSrc.isJumpDetected && !statusSrc.isJumping))
                {
                    ExitWallRun();

                    // Add jump force if a jump is detected while wall running
                    if (statusSrc.isJumpDetected && !statusSrc.isJumping)
                    {
                        Vector3 scapeVelo = (hit.normal.normalized * wallRun.jumpScapeSpeed.y) + (transform.up * wallRun.jumpScapeSpeed.x);
                        motor.AddVelocity(scapeVelo);
                        isWallJumpExited = true;
                        if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Exit wallrun. Jump detected");
                    }
                    else
                        if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Exit wallrun for stop running or for low velocity");

                    return;
                }
                else if (!statusSrc.IsEqual(hit.normal, wallNormal, 0.5f))
                {
                    if (showDebug) Debug.Log("WallParkour -> WallRunState() -> Exit wallrun because the wall has a hard edge: " + hit.normal + " WallNormal: " + wallNormal);
                    Vector3 scapeNormal = hit.normal.normalized;
                    float spaceVelo = wallRun.veloScapeMult;

                    // Detecting the roof (the wall ends becuase we reach the top).
                    if (statusSrc.IsEqual(hit.normal.y, 0.7f, 0.3f))
                    {
                        scapeNormal = -scapeNormal;
                        scapeNormal.y = -scapeNormal.y;
                        spaceVelo *= 2.0f;
                    }
                   
                    Vector3 scapeVelo = (scapeNormal * currentVelocity.x) + (Vector3.up * currentVelocity.y);
                    motor.AddVelocity(scapeVelo * spaceVelo);
                    ExitWallRun();
                    return;
                }
            }

			// Assign to the motor the velocity we decided to  apply to wall walk (is in the inspector).
			motor.SetVelocity(new Vector3(dirWalkWall.x * currentVelocity.x, dirWalkWall.y * currentVelocity.y, dirWalkWall.z * currentVelocity.z));
		}
	}


    private void ExitWallRun()
    {
        statusSrc.isWallRunning = false;
        statusSrc.isWallRunDetected = false;
        statusSrc.isWallRunEndNow = true;
        StartCoroutine("_DisableFlagWallRunEndNow");  // Disable the state of the wallRun_Leaving_status in 0.5 seconds.

        lastObjectWallRunned = null;
        wallNormal = Vector3.zero;
        SendMessage("ResetMidpointX"); // Reset the camera effects.
        //SendMessage("NullMovement");    // Stop any bob
        //SendMessage("CancelAnyCameraMovement");
        mainCamera.SendMessage("ResetRotationZ");
        internalWallrunTime = Time.time + wallrunTime;
    }


    IEnumerator _DisableFlagWallRunEndNow()
    {
        yield return new WaitForSeconds(1.0f);
        statusSrc.isWallRunEndNow = false;
    }

}
