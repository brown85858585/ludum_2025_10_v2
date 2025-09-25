
using UnityEngine;
using System.Collections;


[System.Serializable] 
public class CameraLowLegdeOptions
{
    [Tooltip("Camera movement activator.")]
    public bool enableEffects = true; // Enable camera effects
    [Tooltip("Time the camera will have to move (going down first and later upwards).")]
    [Range(0.1f, 4.0f)] public float moveTime = 2; // Time to move the camera down and up again.
    [Tooltip("The vertical position the camera will reach going down.")]
	public float targetPos = 0.3f; // Target position to reach after startgoing up again.
    [Tooltip("Time the camera will have to rotate itself over its X axis (looking down first).")]
    [Range(0.1f, 4.0f)] public float RotTime = 2; // Rotation time (looking downwards to the 'floor').
    [Tooltip("The amount of degrees the camera will rotate over its right vector.")]
	public int RotAngle = 30; // Rootation angle.
    [Tooltip("Time the camera will have to rotate itself over its Z axis (camera roll).")]
    [Range(0.1f, 2.0f)] public float RollTime = 0.4f; // Roll time (roll the camera using its Z axis)
    [Tooltip("The amount of degrees the camera will rotate over its forward vector (camera roll).")]
	public int RollAngle = 40; 	// Roll angle before going back to normal orientation.
}


[RequireComponent(typeof(CharacterController))]
public partial class LedgeLow : MonoBehaviour
{
    //@Label("Auto Climb LowLedge", "will jump over  the obstacle by itself (without pressing jump key?)", "Color.red", false)
    [Header("LowLedge Climbing Setup")]
    [Tooltip("will jump over  the obstacle by itself (without pressing jump key?)")]
    public bool autoClimbLowLedge = false;	// Always climbs the ledge without being hangin from it.

    [Space(5)]
    [Tooltip("Extra time to needed when has reached the obstacle's top.")]
    public float climbExtraTime = 1f;    // Extra time to continue climbing when exitin the legde.
    [Tooltip("Time to jump when starting to climb the lowLedge (jump time to reach the obstacle top level).")]
    public float climbJumpTime = 0.3f;    // Extra time to continue climbing when exitin the legde.
    //[Tooltip("Velocity used to move forward over the obstacle if the player is not pressing the forward button. ")]
    //public float walkVelocity = 0.15f;
    [Tooltip("Moving/Rotating camera effects configuration when jumping over a lowledge. The camera moves up/down at the desired velocity and displacement, and rotates the angle we want. Notice that this effects are almost the same we are using when climbing a wall.")]
    public CameraLowLegdeOptions cameraLowLedgeEffects = new CameraLowLegdeOptions();

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;

    private float reclimbTime = 1;
    private float internalTime; // Time to retry to climb the ledge.
    private float climbExtraTimeInternal;	// Time to climb (going up at certain speed) the ledge
    private float climbJumpTimeInternal;	// Time to climb (going up at certain speed) the ledge.

    private Transform PlayerTransform;
    private Vector3 Fwd;
    private Vector3 ledgeBackwd;

    private GameObject mainCamera;
    private Status statusSrc;

    void Start()
    {
        mainCamera = Camera.main.gameObject;
        statusSrc = GetComponent<Status>();
        PlayerTransform = transform;
    }

    // If we enter the lowledge trigger change our Status to isInLowLedge.
    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Contains("LowLedge")) { return; }

        if (Time.time < internalTime || statusSrc.isInLowLedge) { return; }

        if (statusSrc.isInAir || statusSrc.isJumping) { return; } // If the player is jumping, dont detect the lowledge.

        if (showDebug) Debug.Log("LowLedge -> TriggerEnter() -> Entering LowLedge. Tag: " + other.tag);	
        // The ledge's Z axis is always facing the player. This is because the way the 3d ledge model is oriented.
        ledgeBackwd = other.transform.TransformDirection(-Vector3.forward);

        // The current forward vector of pl.
        Fwd = PlayerTransform.TransformDirection(Vector3.forward);

        // Make sure we are looking at the ledge and not in the opposite dir.
        if (Vector3.Dot(Fwd, ledgeBackwd) > 0.7f)
        {
            // Set up the climb direction. Up vector relative to the ledge.
            statusSrc.isInLowLedge = true;
            statusSrc.isLowClimbing = false;
        }
    }

    // if we are already on the lowledge's trigger, check if we are facing the obstacle
    // or looking any other place to deactivate the isInLowLedge status if so.
    void OnTriggerStay(Collider other)
    {
        if (!other.tag.Contains("LowLedge")) { return; }

        // If the player is jumping, dont detect the ledge.
        if (statusSrc.isInAir || statusSrc.isJumping || statusSrc.isLowClimbing) { return; }

        // Don't show LedgeStay in console, it will generate a ton of msg.
        //if (showDebug) Debug.Log("LowLedge -> TriggerStay() -> Debug.Log("Entering LowLedge. Tag: "+other.tag);	

        // The ledge's Z axis is always facing the player. This is because the way the 3d ledge model is oriented.
        ledgeBackwd = other.transform.TransformDirection(-Vector3.forward);
        // The current forward vector of pl.
        Fwd = PlayerTransform.TransformDirection(Vector3.forward);
        // Make sure we are looking at the ledge and not in the opposite dir.
        statusSrc.isInLowLedge = Vector3.Dot(Fwd, ledgeBackwd) > 0.7f ? true : false;
    }

    // We leave the lowledge, return to normal state-
    void OnTriggerExit(Collider other)
    {
        if (!other.tag.Contains("LowLedge")) { return; }
        if (!statusSrc.isInLowLedge) { return; }

        if (showDebug) Debug.Log("LowLedge -> TriggerExit() -> Exiting LowLedge. Tag: " + other.tag);

        statusSrc.isInLowLedge = false;
        internalTime = Time.time + reclimbTime;
        climbExtraTimeInternal = Time.time + climbExtraTime;
    }

    //
    // Detect the jump in the lowledge to make our own camera effects.
    // if autoClimbLowLedge is active, create a manual jump by code (no need to press the Jump button to make
    // the jump, obviously).
    //
    void Update()
    {
        // Detect if we are not in a lowledge but we are in a ladder (or a normal ledge) and do nothing if so.
        if (!statusSrc.isInLowLedge && (statusSrc.isInLadder || statusSrc.isInLedge)) { return; }

        if (statusSrc.isLowClimbing)
        {
            // Continue jumping if we are autocimbing the lowledge.
            if (autoClimbLowLedge && Time.time < climbExtraTimeInternal)
            {
                if (showDebug) Debug.Log("LowLedge -> Update() -> AutoJump.");
                statusSrc.isJumpDetected = true;
                statusSrc.isJumpReleased = true;
            }

            // if no autojump, always force jumping an fixed time (1 sec by default)
            if (Time.time <= climbJumpTimeInternal)
            {
                if (showDebug) Debug.Log("LowLedge -> Update() -> Jump initial Time.");
                statusSrc.isJumpDetected = true;
                statusSrc.isJumpReleased = false;
            } // Force disable the jump and start an extra time to move the player forward.
            else if (Time.time < climbExtraTimeInternal && statusSrc.isJumpDetected)
            {
                statusSrc.isJumpDetected = false;
                statusSrc.isJumpReleased = false;
                climbExtraTimeInternal = Time.time + climbExtraTime;
            }

            // Check if climb_extra_time has finished to return to 'normal' status.
            // Improvement: Disable the climb status always after the extra time has passed.
            if (Time.time > climbJumpTimeInternal && Time.time > climbExtraTimeInternal)
            {
                if (showDebug) Debug.Log("LowLedge -> Update() -> Disable Climbing.");
                DisableClimbingLowLedge();
            }
        }


        // We are in a lowledge.
        if (statusSrc.isInLowLedge && !statusSrc.isLowClimbing)
        {
            // Start to auclimb if selected by user or Climb the lowledge when jumping.
            if (autoClimbLowLedge)
            {
                if (showDebug) Debug.Log("LowLedge -> Update() -> Start AutoClimbing the LowLedge.");
                statusSrc.isJumpDetected = true;	// being jumping if is autoclimb
                statusSrc.isJumpReleased = true;
                InputManager.instance.SetForceForwardButton(true);
                StartClimbingLowLedge();
            }
             else if (statusSrc.isJumpDetected /*&& !StatusSrc.isSimpleKickDetected*/)
            {
                if (showDebug) Debug.Log("LowLedge -> Update() -> Start Manual Climbing the LowLedge.");
                statusSrc.isJumpReleased = true;
                InputManager.instance.SetForceForwardButton(true);
                StartClimbingLowLedge ();
            }
        }
    }

    private void StartClimbingLowLedge()
    {
        statusSrc.isLowClimbing = true;
        statusSrc.GetSoundPlayerSrc().LowLedgeClimb();   //  Play the sound when the player climbs the ledge.
        climbJumpTimeInternal = Time.time + climbJumpTime;

        // Moving the camera down when climbing.
        statusSrc.GetCameraBobberScr().SetCameraPingPongMoveTime(cameraLowLedgeEffects.moveTime);
        statusSrc.GetCameraBobberScr().SetCameraDestPosition(cameraLowLedgeEffects.targetPos);

        // Rotate the camera a little bit too.
        mainCamera.SendMessage("SetRotationCameraFallTime", cameraLowLedgeEffects.RotTime);
        mainCamera.SendMessage("RotateClimbingLedge", cameraLowLedgeEffects.RotAngle);
        // Roll the camera.
        mainCamera.SendMessage("SetRollCameraClimbTime", cameraLowLedgeEffects.RollTime);
        mainCamera.SendMessage("RollClimbing", cameraLowLedgeEffects.RollAngle);
    }

    private void DisableClimbingLowLedge()
    {
        statusSrc.isLowClimbing = false;
        statusSrc.isJumping = false;
        statusSrc.isJumpDetected = false;
        statusSrc.isJumpReleased = true;
        InputManager.instance.SetForceForwardButton (false);
        internalTime = Time.time + reclimbTime;
    }

}