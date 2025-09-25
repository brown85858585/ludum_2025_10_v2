using UnityEngine;
using System.Collections;


public enum Axis
{
    X = 0,
    Y = 1,
    XY = 2
}

public enum BobType
{
    NormalBob = 0,
    RunBob = 1,
    CrouchBob = 2,
    ProneBob = 3,
    LadderBob = 4,
    WallRunBob = 5,
    None = 6
}

// Camera roll parameters (used to apply rotation on Z axis when running, walking, etc...)
// This class is created as a parameter in all the cases.
// It's exposed in this general Bob parameters because it make sense (this roll camera effect its 
// a bob effect to, but it take affect over camera's rotation, not the camera's position as the ordinary 
// bob effect). The two of them can be combined.
[System.Serializable]
public class CameraRollZ : object
{
    [Tooltip("Is the roll effect  active?.")]
    public bool isCameraRolling;
    [Tooltip("Rotation speed when rolling the camera.")]
	public float rollSpeed = 4.7f;
    [Tooltip("Rotation angle.")]
	public float rollAngle = 0.25f;
}

// Values used to tweak the bob momvements of the camera depending on status.
// Organizing the variables like this, you get a more understanble and easy view in Unity's inspector.
[System.Serializable]
public class NormalBobber : object
{
    [Tooltip("The axis we want the camera to bob when walking (X, Y or both XY). Now it has to be always in XY to be able to sincronize the bob and the step sounds.")]
    [Disable]
	public Axis axisBob = Axis.XY;
    [Tooltip("The speed the camera will move when bobbing.")]
	public float speed = 4;
    [Tooltip("The amount of displacement we want in each axis. Don't use any negative value or zero; if you want almost no bob in an axis, use a very little value instead (0.001f).")]
	public Vector2 amount = new Vector2(0.08f, 0.1f);
    [Tooltip("Camera roll effect  (rotation over camera 'z' axis).")]
    public CameraRollZ cameraRoll;
}

[System.Serializable]
public class SprintBobber : object
{
    [Tooltip("The axis we want the camera to bob when walking (X, Y or both XY). Now it has to be always in XY to be able to sincronize the bob and the step sounds.")]
    [Disable]
    public Axis axisBob = Axis.XY;
    [Tooltip("The speed the camera will move when bobbing.")]
	public float speed = 6;
    [Tooltip("The amount of displacement we want in each axis. Don't use any negative value or zero; if you want almost no bob in an axis, use a very little value instead (0.001f).")]
    public Vector2 amount = new Vector2(0.14f, 0.12f);
    [Tooltip("Camera roll effect  (rotation over camera 'z' axis).")]
    public CameraRollZ cameraRoll;
}

[System.Serializable]
public class ProneBobber : object
{
    [Tooltip("The axis we want the camera to bob when walking (X, Y or both XY). Now it has to be always in XY to be able to sincronize the bob and the step sounds.")]
    [Disable]
    public Axis axisBob = Axis.XY;
    [Tooltip("The speed the camera will move when bobbing.")]
	public float speed = 4;
    [Tooltip("The amount of displacement we want in each axis. Don't use any negative value or zero; if you want almost no bob in an axis, use a very little value instead (0.001f).")]
    public Vector2 amount = new Vector2(0.01f, 0.06f);
    [Tooltip("Camera roll effect  (rotation over camera 'z' axis).")]
    public CameraRollZ cameraRoll;
}

[System.Serializable]
public class CrouchBobber : object
{
    [Tooltip("The axis we want the camera to bob when walking (X, Y or both XY). Now it has to be always in XY to be able to sincronize the bob and the step sounds.")]
    [Disable]
    public Axis axisBob = Axis.XY;
    [Tooltip("The speed the camera will move when bobbing.")]
	public float speed = 4;
    [Tooltip("The amount of displacement we want in each axis. Don't use any negative value or zero; if you want almost no bob in an axis, use a very little value instead (0.001f).")]
    public Vector2 amount = new Vector2(0.25f, 0.07f);
    [Tooltip("Camera roll effect  (rotation over camera 'z' axis).")]
    public CameraRollZ cameraRoll;
}

[System.Serializable]
public class LadderBobber : object
{
    [Tooltip("The axis we want the camera to bob when walking (X, Y or both XY). Now it has to be always in XY to be able to sincronize the bob and the step sounds.")]
    [Disable]
    public Axis axisBob = Axis.XY;
    [Tooltip("The speed the camera will move when bobbing.")]
	public float speed = 6;
    [Tooltip("The amount of displacement we want in each axis. Don't use any negative value or zero; if you want almost no bob in an axis, use a very little value instead (0.001f).")]
    public Vector2 amount = new Vector2(0.15f, 0.12f);
    [Tooltip("Camera roll effect  (rotation over camera 'z' axis).")]
    public CameraRollZ cameraRoll;
}

[System.Serializable]
public class WallRunBobber : object
{
    [Tooltip("The axis we want the camera to bob when walking (X, Y or both XY). Now it has to be always in XY to be able to sincronize the bob and the step sounds.")]
    [Disable]
    public Axis axisBob = Axis.XY;
    [Tooltip("The speed the camera will move when bobbing.")]
	public float speed = 9;
    [Tooltip("The amount of displacement we want in each axis. Don't use any negative value or zero; if you want almost no bob in an axis, use a very little value instead (0.001f).")]
    public Vector2 amount = new Vector2(0.2f, 0.2f);
    [Tooltip("Camera roll effect  (rotation over camera 'z' axis).")]
    public CameraRollZ cameraRoll;
}

[System.Serializable]
public class CameraBobber : MonoBehaviour
{
    [Header("Camera Bobbing Configuration")]
    [Tooltip("Camera bobber configuration when walking.")]
	public NormalBobber normalBob = new NormalBobber();

    [Tooltip("Camera bobber configuration when running.")]
	public SprintBobber sprintBob = new SprintBobber();

    [Tooltip("Camera bobber configuration when proned.")]
	public ProneBobber proneBob = new ProneBobber();

    [Tooltip("Camera bobber configuration when crouched.")]
	public CrouchBobber crouchBob = new CrouchBobber();

    [Tooltip("Camera bobber configuration when using a ladder.")]
	public LadderBobber ladderBob = new LadderBobber();

    [Tooltip("Camera bobber configuration when wall running.")]
	public WallRunBobber wallRunBob = new WallRunBobber();

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;
    [Tooltip("Show all 'Debug.Log' bobbing messages.")]
    [ShowWhen("showDebug")]
    public bool showBobDebug = false;
    [Tooltip("Show all 'Debug.Log' PingPong (up/down) movement messages.")]
    [ShowWhen("showDebug")]
    public bool showPingPongDebug = false;

    [Header("Runtime Values")]
    [Tooltip("Camera Bob type it's enabled right now.")]
    [SerializeField]
    [Disable]
    private BobType bobType = BobType.None;


    // General private vars.
    [SerializeField]
    [Disable]
    private Axis AxisMov = Axis.X; // Axis to use in the Bob movement
    [SerializeField]
    [Disable]
    private float bobbingSpeed; // Speed of the bob
    [SerializeField]
    [Disable]
    private Vector2 bobbingAmount = Vector2.zero; // Bobbibg effect amount
    [SerializeField]
    [Disable]
    private float midpointX; // Midpoints from where we are going to start making the bob
    [SerializeField]
    [Disable]
    private float midpointY;

    //private var midangleZ : float = 0.0;
    private float midpointXOriginal; // Original (scene start point) of the camera.
    private float midpointYOriginal; // Original (scene start point) of the camera.

    private float timer; // Internal timer for frame independent bob movement (deltaTime).

    private Transform MainCamera;
	private Status statusSrc;
    //private CharacterController controller;
    private MouseLook mouseLookSrc;

    // Crouch and prone private vars.
    private float cameraMoveTime; // Camera movement time when crouching or proning.
    private float i = 1; // Needed to use a based lerp function when crouching (instead of speed).
    private float m = 1; // Needed to use a based lerp function when wallrunning (instead of speed).

    //private var a : float = 1;					// Needed to use a based lerp function when wallrunning -Z Rotation- (instead of speed).
    // Jump/fall private vars.
    private float cameraPingPongMoveTime = 1; // camera movement when landing (falling or jumping).
    private float DestCameraPosition;

    private float j = 1;
    private float n = 1;

    //====================================================================================================
    //
    // Public functions called from other scripts
    //
    //====================================================================================================
    // Reset height of the camera when proning or crouching are finished. Called by Status Crouch & Prone functions.
    public void ResetMidpoint(){ midpointY = midpointYOriginal; i = 0; }

    // Change height of the camera when proning or crouching. Called by Status Crouch & Prone functions.
    public void SetMidpoint(float _newvalue){ midpointY = _newvalue; i = 0; }

    // Reset the lateral position of the camera when wallRunLateralAngle is finished. Called by Status WallRun function.
    public void ResetMidpointX(){ midpointX = midpointXOriginal; m = 0; }

    // Change lateral position of the camera when wallrunning. Called by Status WallRun function.
    public void SetMidpointX(float _newvalue){ midpointX = _newvalue; m = 0; }

    // Change height of the camera when proning or crouching. Called by Status Crouch & Prone functions.
    public void SetCameraMoveTime(float _time){ cameraMoveTime = _time; }

    // Change position of the camera when jump or fall. Called by Status when jumping/touching the floor.
    public void SetCameraPingPongMoveTime(float _time){ cameraPingPongMoveTime = _time; }

    // Change position of the camera when jump or fall. Called by Status when jumping/touching the floor.
    public void SetCameraDestPosition(float _posY){ DestCameraPosition = _posY; j = 0; }

    // Cancel any camera fall movement that maybe it's been playing.
    // Used to cancel camera movements when Crouching/proning in Status to start moving camera to target height position.
    // If not, the camera will not reach the target position when croucing/proning.
    public void CancelAnyCameraMovement()
    {
		Vector3 camLocalPos = MainCamera.localPosition;
		camLocalPos.x = midpointXOriginal;
		camLocalPos.y = midpointYOriginal;
		MainCamera.localPosition = camLocalPos;

        SetMidpoint(midpointYOriginal);
        SetMidpointX(midpointXOriginal);
        j = 1;
        n = 1;
        m = 1;
    }

    // Function called for several scripts to abort any camera bob movement.
	public void NullMovement(){ timer = 0f; totalAxesPrev = 0; bobInternalMsgFlag = false; bobType = BobType.None; } // Null Movement. Stop any bob.

    //====================================================================================================
    //
    // Internal & system functions.
    //
    //====================================================================================================
    void Start()
    {
        MainCamera = Camera.main.transform;
        midpointX = MainCamera.transform.localPosition.x;
        midpointY = MainCamera.transform.localPosition.y;
        midpointYOriginal = midpointY;
        midpointXOriginal = midpointX;
        AxisMov = normalBob.axisBob;
        bobbingSpeed = normalBob.speed;
        bobbingAmount = normalBob.amount;
		statusSrc = GetComponent<Status>();
        mouseLookSrc = MainCamera.GetComponent<MouseLook>();
    }

    private Vector3 myPos;
    void Update()
    {
        myPos = MainCamera.localPosition;

        // Assign the bobbing speed & amount depending of the player status.
        if (statusSrc.isCrouched && bobType != BobType.CrouchBob)
        {
            if (showDebug) Debug.Log("CameraBobber -> Update() -> Crouched BOB");
            bobType = BobType.CrouchBob;
            AxisMov = crouchBob.axisBob;
            bobbingSpeed = crouchBob.speed;
            bobbingAmount = crouchBob.amount;
            SetCameraRollParameters(crouchBob.cameraRoll.isCameraRolling, crouchBob.cameraRoll.rollSpeed, crouchBob.cameraRoll.rollAngle);
        }
        else if (statusSrc.isProned && bobType != BobType.ProneBob)
        {
            if (showDebug) Debug.Log("CameraBobber -> Update() -> Proned BOB");
            bobType = BobType.ProneBob;
            AxisMov = proneBob.axisBob;
            bobbingSpeed = proneBob.speed;
            bobbingAmount = proneBob.amount;
            SetCameraRollParameters(proneBob.cameraRoll.isCameraRolling, proneBob.cameraRoll.rollSpeed, proneBob.cameraRoll.rollAngle);
        }
        else if (statusSrc.isInLadder && statusSrc.isClimbing && bobType != BobType.LadderBob)
        {
            if (showDebug) Debug.Log("CameraBobber -> Update() -> Ladder BOB");
            bobType = BobType.LadderBob;
            AxisMov = ladderBob.axisBob;
            bobbingSpeed = ladderBob.speed;
            bobbingAmount = ladderBob.amount;
            SetCameraRollParameters(ladderBob.cameraRoll.isCameraRolling, ladderBob.cameraRoll.rollSpeed, ladderBob.cameraRoll.rollAngle);
        }
        else if (statusSrc.isWallRunning && bobType != BobType.WallRunBob)
        {
            if (showDebug) Debug.Log("CameraBobber -> Update() -> WallRunning BOB");
            bobType = BobType.WallRunBob;
            AxisMov = wallRunBob.axisBob;
            bobbingSpeed = wallRunBob.speed;
            bobbingAmount = wallRunBob.amount;
            SetCameraRollParameters(wallRunBob.cameraRoll.isCameraRolling, wallRunBob.cameraRoll.rollSpeed, wallRunBob.cameraRoll.rollAngle);
	    }

        // Check the bob if any of previous hasn't been asigned.
        if (!statusSrc.isCrouched && !statusSrc.isProned && !statusSrc.isWallRunning && !statusSrc.isInLadder && !statusSrc.isClimbing)
        {
            if (statusSrc.isStop && bobType != BobType.None)
            {
                if (showDebug) Debug.Log("CameraBobber -> Update() -> NO BOB");
                bobType = BobType.None;
                AxisMov = normalBob.axisBob;
                bobbingSpeed = 0;
                bobbingAmount = Vector2.zero;
                SetCameraRollParameters(false, 0f, 0f);
            }
            if (statusSrc.isWalking && bobType != BobType.NormalBob)
            {
                if (showDebug) Debug.Log("CameraBobber -> Update() -> Normal BOB");
                bobType = BobType.NormalBob;
                AxisMov = normalBob.axisBob;
                bobbingSpeed = normalBob.speed;
                bobbingAmount = normalBob.amount;
                SetCameraRollParameters(normalBob.cameraRoll.isCameraRolling, normalBob.cameraRoll.rollSpeed, normalBob.cameraRoll.rollAngle);
            }
            else if (statusSrc.isRunning && bobType != BobType.RunBob)
            {
                if (showDebug) Debug.Log("CameraBobber -> Update() -> Running BOB");
                bobType = BobType.RunBob;
                AxisMov = sprintBob.axisBob;
                bobbingSpeed = sprintBob.speed;
                bobbingAmount = sprintBob.amount;
                SetCameraRollParameters(sprintBob.cameraRoll.isCameraRolling, sprintBob.cameraRoll.rollSpeed, sprintBob.cameraRoll.rollAngle);
            }
        }

        // Change camera position move when Crouch or Prone.
        // Normal camera movement (No pingpong) when crouching or proning going up or down
        if (i < 1f)
        {
            if (showDebug) Debug.Log("CameraBobber -> Update() -> Move Camera position when Crouch or Prone");
            myPos.y = TimedLerpi(myPos.y, midpointY, cameraMoveTime);
			MainCamera.localPosition = myPos;

            AxisMov = Axis.XY;
        }

        // Change camera position move when WallRunning.
        // Normal camera movement (No pingpong) when Wall Running going left or right
        if (m < 1f)
        {
            if (showDebug) Debug.Log("CameraBobber -> Update() -> Move Camera position when WallRunning");
            myPos.x = TimedLerpm(myPos.x, midpointX, cameraMoveTime);
			MainCamera.localPosition = myPos;

            AxisMov = Axis.XY;
        }

        // Move the camera down & up to make the landing effect.
        PingPongCameraMovement();

        // Jumping & Swiming  & standing or moving in a ledge doesn't have Bob.
        if (statusSrc.isJumping && !statusSrc.isWallRunning || 
            statusSrc.isStop || statusSrc.isInLedge || statusSrc.isSliding)
        {
            SetCameraRollParameters(false, 0f, 0f);
            NullMovement(); // Null Movement. Stop any bob.
        }
        else
            MakeBob();
    }

    // Assign the paramenters for thecamera rolling effect (over Z axis) in MouseLook.
    private void SetCameraRollParameters(bool _isCameraRolling, float _speed, float _amount)
    {
         if(mouseLookSrc.GetCameraRollSpeed() != _speed)
            mouseLookSrc.SetCameraRollSpeed(_speed);
        if (mouseLookSrc.GetCameraRollAngle() != _amount)
            mouseLookSrc.SetCameraRollAngle(_amount);
    }

    // Moves the camera up&down when jumping or falling touching the floor.
    private void PingPongCameraMovement()
    {
        if (j >= 1f && n >= 1f) { return; } // Security Sentence

		Vector3 myPos = MainCamera.localPosition;

        // Move the camera to dest position (going down).
        if (j < 1f)
        {
			myPos.y = TimedLerpj(myPos.y, DestCameraPosition, cameraPingPongMoveTime);
			MainCamera.localPosition = myPos;

            if (showDebug && showPingPongDebug) Debug.Log("CameraBobber -> PingPongCamera() -> Camera LocalPosition: " + MainCamera.localPosition);
            if (Mathf.Abs(myPos.y - DestCameraPosition) < 0.01f)
            {
                n = 0;
                j = 1;
            }

            AxisMov = Axis.X;
        }
        // Move the camera again to the midpoint position (going up to original position).
        if (n < 1f)
        {
			myPos.y = TimedLerpn(myPos.y, midpointYOriginal, cameraPingPongMoveTime);
			MainCamera.localPosition = myPos;

            if (showDebug && showPingPongDebug) Debug.Log("CameraBobber -> PingPongCamera() -> Camera LocalPosition: " + MainCamera.localPosition);
            if (Mathf.Abs(midpointYOriginal - myPos.y) < 0.01f)
            {
                n = 1;
                SetMidpoint(midpointYOriginal);
            }

            AxisMov = Axis.X;
        }
    }

    // The wave function used a sinus call that do the trick based on timedeltatime
    private float totalAxesPrev = 0;
    private void MakeBob()
    {
        Vector3 myPos = MainCamera.localPosition;
        float totalAxes = Mathf.Abs(InputManager.instance.HorizontalValue) + Mathf.Abs(InputManager.instance.VerticalValue);
        totalAxes = Mathf.Clamp01(totalAxes);

        if (totalAxes != 0 && totalAxesPrev == 0)   // Start walking or running (from being stopped).
        {
            EventManagerv2.instance.TriggerEvent("CameraRachedBobEnd", new EventParam (this.name, string.Empty, string.Empty) );
        }

        if (AxisMov == Axis.X)
        {
            myPos.x = midpointX + BobGenerator(myPos.x, totalAxes, bobbingAmount.x, bobbingSpeed);
        }
        else if (AxisMov == Axis.Y)
        {
            myPos.y = midpointY + BobGeneratorX2(myPos.y, totalAxes, bobbingAmount.y, bobbingSpeed, true);
        }
        else if (AxisMov == Axis.XY)
        {
            myPos.x = midpointX + BobGenerator(myPos.x, totalAxes, bobbingAmount.x, bobbingSpeed);
            myPos.y = midpointY + BobGeneratorX2(myPos.y, totalAxes, bobbingAmount.y, bobbingSpeed, false);
        }

        MainCamera.localPosition = myPos;
        totalAxesPrev = totalAxes;
    }

    private float timedLerpRatio;
    private float timedLerpRatio2;

    public float TimedLerpi(float origin, float dest, float time)
    {
        if (timedLerpRatio == 0)
            timedLerpRatio = 1 / time;

        float val = dest;
        if (i < 1f)
        {
            i = i + (Time.deltaTime * timedLerpRatio);
            if (Mathf.Abs(origin - dest) > 0.01f)
                val = Mathf.Lerp(origin, dest, i);
            else
            {
                i = 1;
                timedLerpRatio = 0;
            }
        }
        return val;
    }

    public float TimedLerpj(float origin, float dest, float time)
    {
        if (timedLerpRatio2 == 0)
            timedLerpRatio2 = 1 / time;

        float val = dest;
        if (j < 1f)
        {
            j = j + (Time.deltaTime * timedLerpRatio2);
            if (Mathf.Abs(origin - dest) > 0.01f)
                val = Mathf.Lerp(origin, dest, j);
            else
            {
                j = 1;
                timedLerpRatio2 = 0;
            }
        }
        return val;
    }

    public float TimedLerpm(float origin, float dest, float time)
    {
        if (timedLerpRatio == 0)
            timedLerpRatio = 1 / time;

        float val = dest;
        if (m < 1f)
        {
            m = m + (Time.deltaTime * timedLerpRatio);
            if (Mathf.Abs(origin - dest) > 0.01f)
                val = Mathf.Lerp(origin, dest, m);
            else
            {
                m = 1;
                timedLerpRatio = 0;
            }
        }
        return val;
    }

    public float TimedLerpn(float origin, float dest, float time)
    {
        if (timedLerpRatio2 == 0)
            timedLerpRatio2 = 1 / time;

        float val = dest;
        if (n < 1f)
        {
            n = n + (Time.deltaTime * timedLerpRatio2);
            if (Mathf.Abs(origin - dest) > 0.01f)
                val = Mathf.Lerp(origin, dest, n);
            else
            {
                n = 1;
                timedLerpRatio2 = 0;
            }
        }
        return val;
    }

    // Generates a BOB movement (for the camera or maybe weapons) using a sinus function.
    // It checks if totalAxes are presed to return Zero (no Bob at all)
    private bool bobInternalMsgFlag = false;    // Flag used to show the Debug "Bob changing directions messages".
    public float BobGenerator(float val, float totalAxes, float amount, float speed)
    {
		if (totalAxes == 0) { timer = 0f; val = 0; return 0; }

        val = Mathf.Sin(timer) * amount * totalAxes;
        timer = timer + (speed * Time.deltaTime);
        if (timer >= Mathf.PI && timer < Mathf.PI * 2 && !bobInternalMsgFlag)
        {
            EventManagerv2.instance.TriggerEvent("CameraRachedBobEnd", new EventParam(this.name, string.Empty, "rightFoot"));
            if (showDebug && showBobDebug) Debug.Log("CameraBobber -> CameraBobbing() -> BOB is changing direction in X axis");
            bobInternalMsgFlag = true;
        }

        if (timer >= Mathf.PI * 2)
        {
            timer = timer - (Mathf.PI * 2);
            EventManagerv2.instance.TriggerEvent("CameraRachedBobEnd", new EventParam(this.name, string.Empty, "leftFoot"));
            bobInternalMsgFlag = false;
            if (showDebug && showBobDebug) Debug.Log("CameraBobber -> CameraBobbing() -> BOB is changing direction in X axis returning back to the original position");
        }

        return val;
    }

    // Generates a BOB movement that moves at double speed.
    // (is usually used to move the camera up/down twice quickly than left/side creating the bob movement).
    // It is twice quickly than the normal camera bob, thats why the sinus is multiplied by 2.
    // It can be used too to create weapons that bobs twice quickly (normally in the Y axis).
    public float BobGeneratorX2(float val, float totalAxes, float amount, float speed, bool updateTimer)
    {
		if (totalAxes == 0) { timer = 0f; val = 0; return 0; } 

        val = Mathf.Sin(timer*2) * amount * totalAxes; // Twice quickly function is done right here.
        if (updateTimer)
        {
            timer = timer + (speed * Time.deltaTime);
            if (timer > Mathf.PI*2)
                timer = timer - (Mathf.PI * 2);
        }

        return val;
    }

    

}