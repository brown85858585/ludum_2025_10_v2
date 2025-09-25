
#pragma warning disable  0414   // Used because we have a 'grounded' flag just as information but it isn't being used really.

using UnityEngine;
using System.Collections;


[System.Serializable]
public class LedgeOptions
{
    [Tooltip("Minimum speed when moving laterally.")]
	public float minSpeed = 0.3f; // Minimun speed swiming or diving.
    [Tooltip("Maximum speed when moving laterally.")]
	public float maxSpeed = 2; // aceleration when swiming.
    [Tooltip("Acceleration/deceleration ratio.")]
	public float accelRatio = 3; // aceleration/deceleration ratio (how quickly we reach max/min speed).
}

[System.Serializable]
public class CameraLegdeOptions : object
{
    [Tooltip("Camera movement activator.")]
	public bool enableEffects = true;
    [Tooltip("Time the camera will have to move (going down first and later upwards).")]
	[Range(0.1f, 2.0f)] public float moveTime = 1f;
    [Tooltip("The vertical position the camera will reach going down.")]
	public float targetPos = 0.2f;
    [Tooltip("Time the camera will have to rotate itself over its X axis (looking down first).")]
    [Range(0.1f, 2.0f)] public float RotTime = 1f;
    [Tooltip("The amount of degrees the camera will rotate over its right vector.")]
	public int RotAngle = 80;
    [Tooltip("Time the camera will have to rotate itself over its Z axis (camera roll).")]
    [Range(0.1f, 2.0f)] public float RollTime = 0.7f;
    [Tooltip("The amount of degrees the camera will rotate over its forward vector (camera roll).")]
	public int RollAngle = 60;
}


[RequireComponent(typeof(CharacterController))]
public class Ledge : MonoBehaviour
{
    [Header("Ledge Climbing Setup")]
    //@Label("Auto Climb Ledge", "player will climb the ledge by itself (without  pressing forward key again)", "Color.red", false)
    [Tooltip("player will climb the ledge by itself (without  pressing forward key again")]
    public bool autoClimbLedge = false;

    [Space(5)]
    [Tooltip("Relative 'Y' position of the player respect the ledge.")]
	public float relHeight = -0.9f;	// Relative Y position respect the ledge's center.
    [Tooltip("Extra time to climb when the player has reached the ledge's top.")]
	public float climbExtraTime = 0.2f; // Extra time to continue climbing when exitin the legde.
    [Tooltip("Speed used to continue moving a little bit once pl has climbed the ledge. That way it can move forward a little bit from the edge of the wall.")]
    public float scapeSpeed = 8f;

    [Space(5)]
    [Tooltip("Speed of the player when climbing.")]
	public float climbSpeed = 3f;	// The speed of the player up and down the ledge. Roughly equal to walking speed is a good starting point.
    [Tooltip("Backwards jump speed from the ledge.")]
	public float jumpSpeed = 4f;

    [Space(5)]
    [Tooltip("Can the player move laterally when hanging from the ledge?")]
	public bool isLateralMovement = true;
    [Tooltip("Lateral movement options. It has minimum and maximum speed because pl will not move at a constant speed. It will accelerate/decelerate until reaches the maximum/minimum speed at a given acceleration ratio.")]
	public LedgeOptions movLedgeOptions = new LedgeOptions();

	//@Comment("Camera Ledge Configuration", "", "Color.blue", 15)
	//public var Comment2 : int;
    [Header("Camera Ledge Configuration")]
    [Tooltip("Moving/Rotating camera effects configuration when climbing a ledge. The camera moves up/down at the desired velocity and displacement, and rotates the angle we want. Notice that this effects are almost the same than the fall effects.")]
	public CameraLegdeOptions cameraLedgeEffects = new CameraLegdeOptions();

	// In the range -1 to 1 where -1 == -90 deg, 1 = 90 deg, angle of view camera at which the user climbs down rather than up when moving with the forward key.
	//private float climbDownThreshold = -0.4f;
	private float reclimbTime = 1;

    [Tooltip("The camera rotation over (X,Y) axis will be restricted to some degrees.(No limit values are:  X: -360, Y: 360).")]
	public Vector2 cameraRotationLimits = new Vector2(-45, 45);

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;

    private float internalTime;			// Time to retry to climb the ledge.
	private float climbExtraTimeInternal;	// Time to climb (going up at certain speed) the ledge.

	private Vector3 climbDirection = Vector3.zero;
	private Vector3 lateralMove = Vector3.zero;
	private Vector3 ledgeMovement = Vector3.zero;
    private bool grounded;
	private float lateralStartTime;	// Start lateral time to reset ping-pong movement.

    private Vector3 ledgeBackwd;
    private Vector3 moveJumpDirection;

    private GameObject mainCamera;
	private Status statusSrc;
    private CharacterController controller;
    private CharacterMotor motor;
    private MouseLook mouseScript;

	private Vector2 minMaxXOrig = Vector2.zero;


    void Start()
    {
		mainCamera = Camera.main.gameObject;
		statusSrc = GetComponent<Status>();
		controller = statusSrc.GetController();
		motor = statusSrc.GetMotor();
		mouseScript = statusSrc.GetMouseLookSrc();
        minMaxXOrig = new Vector2(mouseScript.minimumX, mouseScript.maximumX);
    }

	// Get the relative position of an object respect another (origin).
	// Used to position de Y coord position of pl respect the ledge position
    public Vector3 getRelativePosition(Transform origin, Vector3 position)
    {
        Vector3 distance = position - origin.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
        relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
        relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);
        return relativePosition;
    }

    private float RotationY = 0f;
    private Vector3 PlayerFwd;
    void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (Time.time < internalTime || statusSrc.isInLedge) { return; }

        // If the player isnt in the air, dont detect the ledge.
        if ( !statusSrc.isInAir || (!statusSrc.isJumping && !statusSrc.isWallRunEndNow)) { return; }	

		if (!other.tag.Equals("Ledge")) { return; } // It's a ledge???

        if (showDebug) Debug.Log("Ledge -> TriggerEnter() -> Entering Ledge. Tag: "+other.tag);	
		// The ledge's Z axis is always facing the player. This is because the way the 3d ledge model is oriented.
        ledgeBackwd = other.transform.TransformDirection(-Vector3.forward);
		// The current forward vector of pl.
        PlayerFwd = transform.TransformDirection(Vector3.forward);

        // Make sure we are looking at the ledge and not in the opposite dir.
        if (Vector3.Dot(PlayerFwd, ledgeBackwd) <= 0.7f)
        {
            if(showDebug) Debug.Log("Ledge -> TriggerEnter() -> pl not looking towards the ledge: " + Vector3.Dot(PlayerFwd, ledgeBackwd) );
            return;
        }

        HangFromLedge(other.transform);
    }

    void OnTriggerStay(Collider other)
    {
        if (!enabled) return;
        if (Time.time < internalTime || statusSrc.isInLedge) { return; }

        if ((!statusSrc.isInAir || (!statusSrc.isJumping && !statusSrc.isWallRunEndNow)))
        { return; }	// If the player isnt in the air, dont detect the ledge.

        if (!other.tag.Equals("Ledge")) { return; }

        // Don't show LedgeStay in console, it will generate a ton of msg.
        //if (showDebug) Debug.Log("Ledge -> TriggerStay() -> Staying in Ledge. Tag: "+other.tag);	

        // The ledge's Z axis is always facing the player. This is because the way the 3d ledge model is oriented.
        ledgeBackwd = other.transform.TransformDirection(-Vector3.forward);
        // The current forward vector of pl.
        PlayerFwd = transform.TransformDirection(Vector3.forward);

        // Make sure we are looking at the ledge and not in the opposite dir.
        if (Vector3.Dot(PlayerFwd, ledgeBackwd) <= 0.7f)
        {
            //if (showDebug) Debug.Log("Ledge -> TriggerStay() -> pl not looking towards the ledge: " + Vector3.Dot(Fwd, ledgeBackwd));
            return;
        }

        HangFromLedge(other.transform);
    }


    void OnTriggerExit(Collider other)
    {
        if (!enabled) return;
        // If i'm exiting in the ledge and i'm not climbing it, most likely it's because i'm not detecting it well.
        // Maybe the relative positioning of pl, is moving it outside the ledge's trigger.
        if (!other.tag.Equals("Ledge")) { return; }
        if (!statusSrc.isInLedge) { return; }

        if (showDebug) Debug.Log("Ledge -> TriggerExit() -> Exiting Ledge. Tag: " + other.tag);

        if (!statusSrc.isClimbing)
        {
            if (!statusSrc.isWalking) Debug.LogWarning("Not hanging in the ledge, decrease the Relative Height parameter.");

            statusSrc.isInLedge = false;
            motor.enabled = true;
            mouseScript.SetXLimits(minMaxXOrig.x, minMaxXOrig.y);
        }
        else
        {
            statusSrc.isInLedge = false;
            internalTime = Time.time + reclimbTime;
            climbExtraTimeInternal = Time.time + climbExtraTime;
        }
    }

	//
	//	Convert the player's normal forward and backward motion into up and down motion on the ledge.
	//
    void Update()
    {
		// Detect if we are not in ledge but we are in a ladder and do nothing if so.
        if (!statusSrc.isInLedge && (statusSrc.isInLadder || statusSrc.isInLowLedge)) { return; }

		// If we are not more in a ledge, check is we were before to jump backwards or finish climbing that ledge
		// until we reach the top.
        if (!statusSrc.isInLedge)
        {
            if (statusSrc.isJumpingFromLedge)
            {
                // Control the jump of the player if it has jumped from the ledge.
                if (statusSrc.isInAir)
                {
                    // perform off-ledge hop
                    /*if(ledgeLimits == null)*/
                    controller.Move((moveJumpDirection * jumpSpeed) * Time.deltaTime);
                    /*else{
                        var dir : Vector3 = Camera.main.transform.TransformDirection(Vector3.forward);
                        dir.Normalize();
                        dir += moveJumpDirection;
                        controller.Move(dir * ledgeLimits.jumpSpeed * 0.5 * Time.deltaTime);
                    }*/
                }
                else
                // We landed after jumping from ledge, back to normal state.
                if (!statusSrc.isInAir)
                    statusSrc.isJumpingFromLedge = false;
            }
            else if (statusSrc.isClimbing)
            {
                // Control the extra-climb on the ledge to reach the top.
                if (Time.time <= climbExtraTimeInternal)
                {
                    // Move the player forward making it move forward if player is not pressing the forward button.
                    if (!InputManager.instance.IsFowardButtonPressed())
                    {
                        motor.SetVelocity(ledgeBackwd * scapeSpeed);
                    }
                    else
                    {
                        ClimbLedge(); // If pressing the froward button climb a little more (the extra time assigned in the inspector) to make it goes forward.
                    }
                }
                else  // Extra climb time is finished, back to normal state.
                {
                    statusSrc.isClimbing = false;
                    motor.enabled = true;
                    mouseScript.SetXLimits(minMaxXOrig.x, minMaxXOrig.y);
                }
            }

            return;
        }
        else
        {
			// We are in a ledge. Climb the ledge when pressing forward key if isn't activated the autoclimb in inspector.
            if (!statusSrc.isClimbing && 
               (autoClimbLedge || (!autoClimbLedge && InputManager.instance.verticalKey.isDown && InputManager.instance.VerticalValue > 0)) )
            {
                if (!statusSrc.isClimbing && !statusSrc.isJumpingFromLedge)
                {
                    if (showDebug) Debug.Log("Ledge -> Update() -> Start Climbing the Ledge.");
                    statusSrc.isClimbing = true;
                    statusSrc.isJumping = false;
                    statusSrc.GetSoundPlayerSrc().LedgeClimb(); //  Play the sound when the player climbs the ledge.

                    // Moving the camera down when landing.
                    statusSrc.GetCameraBobberScr().SetCameraPingPongMoveTime(cameraLedgeEffects.moveTime);
                    statusSrc.GetCameraBobberScr().SetCameraDestPosition(cameraLedgeEffects.targetPos);
                    // Rotation the camera a little big too.
                    mainCamera.SendMessage("SetRotationCameraFallTime", cameraLedgeEffects.RotTime);
                    mainCamera.SendMessage("RotateClimbingLedge", cameraLedgeEffects.RotAngle);
                    // Roll the camera.
                    mainCamera.SendMessage("SetRollCameraClimbTime", cameraLedgeEffects.RollTime);
                    mainCamera.SendMessage("RollClimbing", cameraLedgeEffects.RollAngle);
                }
            }


            if (statusSrc.isClimbing)
            {
                ClimbLedge();
            }
            else
            {
                // Lateral movement when hanging from a ledge.
                // get our lateral component of movement if it's allowed. if not, lateralmove will be zero.
                if (isLateralMovement)
                {
                    if (InputManager.instance.horizontalKey.isDown)
                        lateralStartTime = Time.time;

                    ledgeMovement = lateralMove * InputManager.instance.HorizontalValue;
                }
                if (ledgeMovement.magnitude >= 0)
                {
                    // Update the Player Status.
                    statusSrc.isStop = false;
                    statusSrc.isWalking = false;
                    statusSrc.isJumping = false;
                    if (ledgeMovement.magnitude < 0.3f)
                        statusSrc.isStop = true;
                    else
                        statusSrc.isWalking = true;

                    // Move using a ping-pong funcion (accel/decel lateral movement).
                    float vel = movLedgeOptions.minSpeed + Mathf.PingPong((Time.time - lateralStartTime) * movLedgeOptions.accelRatio, movLedgeOptions.maxSpeed);
                    CollisionFlags flags = controller.Move(ledgeMovement * vel * Time.deltaTime);
                    grounded = (flags & CollisionFlags.CollidedBelow) != (CollisionFlags)0;
                }
            }

			// If we jumpped, then go back to the usual pl behavior (jumping, landing, walking, ...)
            if (InputManager.instance.jumpKey.isDown)
            {
                if (statusSrc.isClimbing) { return; }

				// perform off-ledge hop
                //Vector3 hop = mainCamera.transform.TransformDirection(Vector3.forward);
				//if(ledgeLimits == null){
	                moveJumpDirection = -ledgeBackwd.normalized * climbSpeed;
	                controller.Move((moveJumpDirection * jumpSpeed) * Time.deltaTime);
                /*}
				else{
					moveJumpDirection = (ledgeMovement.normalized + -ledgeBackwd.normalized) * climbSpeed;
					controller.Move(moveJumpDirection * ledgeLimits.jumpSpeed * Time.deltaTime);
				}*/

                if (showDebug) Debug.Log("Ledge -> Update() -> Jump from ledge Start: " + moveJumpDirection);

                mouseScript.SetXLimits(minMaxXOrig.x, minMaxXOrig.y);
                statusSrc.isJumpingFromLedge = true;
                motor.SetVelocity((moveJumpDirection * jumpSpeed) * Time.deltaTime);
                motor.enabled = true;
                statusSrc.isInLedge = false;
                statusSrc.isClimbing = false;
                statusSrc.isJumping = true;
                motor.jumping.jumping = true;
                internalTime = Time.time + reclimbTime;
                return;
            }
        }
    }

    private void HangFromLedge(Transform wall)
    {
        // Set up the climb direction. Up vector relative to the ledge.
        climbDirection = wall.TransformDirection(Vector3.up);
        lateralMove = wall.TransformDirection(Vector3.left);

        // Position pl to be at certain height respect the ledge
        Vector3 posAux = getRelativePosition(wall.transform, transform.position);
        transform.position = transform.position - (transform.up * (posAux.y - relHeight));

        // Rotate pl to look exactly forward aligned with the ledge
        Quaternion newRotation = Quaternion.FromToRotation(PlayerFwd, ledgeBackwd);
        RotationY = newRotation.eulerAngles.y;

        // Don't allow pl to rotate a huge angle value (it does not work).
        // Instead, make it a litle negative angle to rotate in the other direction.
        if (RotationY > 180)
            RotationY = RotationY - 360;
        mouseScript.SetRotationX(RotationY);

        // Enable camera rotation ledge limits. /**/ // This is an internal debug log, to look for angle errors (hard to find). Do not activate, pls!
        //Vector3 EulerAux = transform.localEulerAngles;
        //Debug.Log("Pre AngleY: " + EulerAux.y);
        //mouseScript.SetXLimits(EulerAux.y+cameraRotationLimits.x, EulerAux.y+cameraRotationLimits.y); /**/
        //Debug.Log("Post AngleY: " + EulerAux.y);

        mouseScript.SetXLimits(cameraRotationLimits.x, cameraRotationLimits.y);

        statusSrc.GetSoundPlayerSrc().LedgeDetect();	//  Play the sound when the Player hits the ledge.
        motor.enabled = false;
        motor.jumping.jumping = false;
        statusSrc.isInLedge = true;
        statusSrc.isJumping = false;
        statusSrc.isClimbing = false;
    }


    // Move pl upwards at certain speed.
    private void ClimbLedge()
    {
        Vector3 verticalMove = climbDirection.normalized;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        Vector3 dir = verticalMove +fwd;
        dir = dir / dir.magnitude;

        if (showDebug) Debug.DrawRay(this.transform.position, dir*20, Color.green);

        CollisionFlags flags = controller.Move((dir * climbSpeed) * Time.deltaTime);
        grounded = (flags & CollisionFlags.CollidedBelow) != (CollisionFlags) 0;
    }

}