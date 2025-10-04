
using Mirror;
using UnityEngine;

public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }

[AddComponentMenu("Camera-Control/Mouse Look")]
public partial class MouseLook : NetworkBehaviour
{
	public RotationAxes axes = RotationAxes.MouseX;
	public float sensitivityX = 5f;
    public float sensitivityY;
	public float minimumX = -360f;
	public float maximumX = 360f;
    public float minimumY;
    public float maximumY;

	private float rotationX;
    private float rotationY;
    private Quaternion originalRotation;

    // Landing/Jumping camera effects vars.
	private float rotateCameraFallTime = 0.15f;
    private float rotationYOrig; // Save the origial rotation before start rotating the camera.
	private float rotationYDest = 45f; // Angles we wanna rotate the camera
    private float i = 1; // Used to lerp using time and not velocity.
    private float j = 1; // The same...

    // Cameral roll parameters when walk or run.
    [Header("Runtime Values")]
    [SerializeField]
    [Disable]
    private float cameraRollSpeed;
    [SerializeField]
    [Disable]
    private float cameraRollAmount;
    private float timerZ;

    // Climbing camera -pingpong- roll effect vars.
	private float rotateCameraClimbTime = 0.15f;
    private float rotationZOrig = 0; // Save the original rotation before start rotating the camera.
	private float rotationZDest = 45f; // Angles we wanna rotate the camera
    private float n = 1; // Used to lerp using time and not velocity.
    private float m = 1; // The same...

    // Smooth vars
    private float xSmooth;
    [SerializeField]
    [Disable]
    private float smoothXTime = 0.005f;
    private float xVelocity;
  
	private float ySmooth;
    [SerializeField]
    [Disable]
    private float smoothYTime = 0.05f;
    private float yVelocity;

	private float flyLateralAngle; // angle used to roll the camera when flying. (is asigned from FlyScript in every Update)
    private float wallRunLateralAngle; // Roll camera angle asugned once when Wallrunning and reseted to zero when exiting.

    [Space(10)]
    [SerializeField]
    [Disable]
    public float wallRunEnterRotationSpeed = 5;
    [SerializeField]
    [Disable]
    public float wallRunExitRotationSpeed = 10;
    [SerializeField]
    [Disable]
    private bool inTiltCameraForWallRunning;
    [SerializeField]
    [Disable]
    private bool inResetCameraForWallRunning;
    [SerializeField]
    [Disable]
    private float wallRunAngleLimit = 0;


    private Status statusSrc;



    public virtual void Start()
    {
		statusSrc = transform.root.GetComponent<Status>();

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        originalRotation = transform.localRotation; 
        //ResetPlayerRotation();    // Not used by now.
    }

    public virtual void Update()
    {
        if (Pause.instance.IsPaused()) { return; }

        if (!isLocalPlayer) return;

        //============================================================================================
        //
        // Generic Camera Rotation using the mouse (made by the player)
        //
        //============================================================================================
        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            rotationX = rotationX + (InputManager.instance.MouseXValue * sensitivityX * AppManager.instance.sensitivity); //InputManager.instance.MouseXValue
            rotationY = rotationY + (InputManager.instance.MouseYValue * sensitivityY * AppManager.instance.sensitivity); //InputManager.instance.MouseYValue
            // We do the smooth using a smoothdamp function.
            xSmooth = Mathf.SmoothDamp(xSmooth, rotationX, ref xVelocity, smoothXTime); // Smooth the rotation.
            xSmooth = ClampAngle(xSmooth, minimumX, maximumX); // Clamp the angle (360º problem).
            ySmooth = Mathf.SmoothDamp(ySmooth, rotationY, ref yVelocity, smoothYTime);
            ySmooth = ClampAngle(ySmooth, minimumY, maximumY);
            // Everything else is as always (same code).
            Quaternion xQuaternion = Quaternion.AngleAxis(xSmooth, Vector3.up); // Creates a given rotation over up axis)
            Quaternion yQuaternion = Quaternion.AngleAxis(-ySmooth, Vector3.right); // Creates a rotation over right axis.
            transform.localRotation = (originalRotation * xQuaternion) * yQuaternion; // Assign the rotation to our gameobject.
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX = rotationX + (InputManager.instance.MouseXValue * sensitivityX * AppManager.instance.sensitivity);
            if (rotationX < -360f)
                xSmooth = xSmooth + 360f;
            else if (rotationX > 360f)
                xSmooth = xSmooth - 360f;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            xSmooth = Mathf.SmoothDamp(xSmooth, rotationX, ref xVelocity, smoothXTime);
            xSmooth = ClampAngle(xSmooth, minimumX, maximumX);
			Quaternion xQuaternion = Quaternion.AngleAxis(xSmooth, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotationY = rotationY + (InputManager.instance.MouseYValue * sensitivityY * AppManager.instance.sensitivity);
            if (rotationY < -360f)
                ySmooth = ySmooth + 360f;
            else if (rotationY > 360f)
                ySmooth = ySmooth - 360f;

            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            ySmooth = Mathf.SmoothDamp(ySmooth, rotationY, ref yVelocity, smoothYTime);
            ySmooth = ClampAngle(ySmooth, minimumY, maximumY);
			Quaternion yQuaternion = Quaternion.AngleAxis(-ySmooth, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }


        //============================================================================================
        //
        // Make the rotation down & up of the manera when caracter fall and touch the ground (or jump)
        // Cancel all the fall/jump rotations if the user moves the mouse in the axis we are using.
        // But let the rotation start a little bit before allowing to move the mouse.
        //
        //============================================================================================
        if (InputManager.instance.MouseYValue != 0f)
        {
            if (i > 0.1f)
                i = 1;

            j = 1;
        }

        //============================================================================================
        //
        // Camera Rotation
        //
        //============================================================================================

        // Make the rotation itself, doing an rotation down and then back to the original rotation.
        // Used to make a fall effect and when climbing a ledge.
        if (i < 1f)
        {
            //Debug.Log("RotationY-i: "+rotationY);
            rotationY = TimedLerpi(rotationY, rotationYDest, rotateCameraFallTime);
        }
        else if (j < 1f)
        {
            //Debug.Log("RotationY-j: "+rotationY);
            rotationY = TimedLerpj(rotationY, rotationYOrig, rotateCameraFallTime);
        }

        if (i < 1f || j < 1f)
        {
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }
        else
        {
            timedLerpRatio = 0;
        }

        //============================================================================================
        //
        // Camera Roll
        //
        //============================================================================================
        // Camera Roll rotation: doing an rotation to one side and then back to the original rotation.
        // Used to make a fall effect and when climbing a ledge.

        Vector3 MyRot = transform.localEulerAngles;
        if (n < 1f)
        {
            MyRot.z = TimedLerp2n(rotationZOrig, rotationZDest, rotateCameraClimbTime);
        }
        else if (m < 1f)
        {
            //Debug.Log("RotationZ-n: "+n+" - Rot: "+MyRot.z+" - Time: "+rotateCameraClimbTime);
            MyRot.z = TimedLerp2m(rotationZDest, rotationZOrig, rotateCameraClimbTime);
        }

        if (n < 1f || m < 1f)
        {
            transform.localEulerAngles = MyRot;
        }
        else
        {
            //Debug.Log("RotationZ-m: "+MyRot.z);
            timedLerpRatio2 = 0;
        }

        //============================================================================================
        //
        // Camera Roll when running
        //
        //============================================================================================
        // If some of the two camera rolling parameters needed to make the effect
        // are <1, dont do anything at all. So, we dont do this effect if we are 
        // already rolling the camera (ledge/lowledge climb effect/flying)
        if (cameraRollSpeed > 0 && cameraRollAmount > 0 && n >= 1 && m >= 1)
        {
            CameraRoll(cameraRollSpeed, cameraRollAmount);
        }

        //============================================================================================
        //
        // Interpolate Camera Roll when wallrunning entering/exiting.
        // (Original code from Federico Barbero). Thanks, Man.!!!!
        //
        //============================================================================================
        if (inTiltCameraForWallRunning)
        {
            RotateCameraForWallRunning();
        }

        if (inResetCameraForWallRunning)
        {
            ResetCameraForWallRunning();
        }
    }

    //==================================================================
    //
    // Public functions called by other scripts.
    //
    //==================================================================
    // Called by CameraBobber, ledge and lowledge scripts.
    public virtual float GetCameraRollSpeed()
    {
        return cameraRollSpeed;
    }

    public virtual float GetCameraRollAngle()
    {
        return cameraRollAmount;
    }


    public virtual void SetCameraRollSpeed(float rollSpeed)
    {
        cameraRollSpeed = rollSpeed;
    }

    // Called by CameraBobber, ledge and lowledge scripts.
    public virtual void SetCameraRollAngle(float rollAmount)
    {
        cameraRollAmount = rollAmount;
    }

    // Used to make the move towards a target (giving the rotation needed to look that at target).
    // Is similar to SetRotationX (angle)
    public virtual void SetFacing(Quaternion pos)
    {
        rotationX = 0;
        xSmooth = 0;
        rotationY = 0;
        ySmooth = 0;
        transform.localRotation = pos;
        originalRotation = transform.localRotation;
    }


    // Used in ladder & ledge to rotate the camera to look towards the ladder.
    public virtual void SetRotationX(float angle)
    {
        Vector3 EulerAux = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(EulerAux.x, EulerAux.y + angle, EulerAux.z);
        rotationX = 0;
        xSmooth = 0;
        rotationY = 0;
        ySmooth = 0;
        originalRotation = transform.localRotation;
    }

    // Used in wallrunning to rotate the camera over Z a little bit.
    // Reset the Y rotation also, to avoid strange rotation issues.
    public virtual void SetRotationZ(float angle)
    {
        Vector3 EulerAux = transform.localEulerAngles;
        wallRunAngleLimit = angle;
        wallRunLateralAngle = angle > 0 ? 0.2f : -0.2f;
        transform.localEulerAngles = new Vector3(EulerAux.x, EulerAux.y, wallRunLateralAngle);
        inTiltCameraForWallRunning = true;

        // Old Reset code (without interpolation)
        /* transform.localEulerAngles = new Vector3(EulerAux.x, EulerAux.y, wallRunLateralAngle);
        rotationX = 0;
        xSmooth = 0;
        rotationY = 0;
        ySmooth = 0;
        wallRunLateralAngle = angle;*/
    }

    // Reset the Z rotation to zero again (after rotating it at the wallrun start -if enabled-).
    // Reset the Y rotation also, to avoid strange rotation issues.
    public virtual void ResetRotationZ()
    {
        inTiltCameraForWallRunning = false;
        inResetCameraForWallRunning = true;

        // Old Reset code (without interpolation)
        /*Vector3 EulerAux = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(EulerAux.x, EulerAux.y, 0);
        rotationX = 0;
        xSmooth = 0;
        rotationY = 0;
        ySmooth = 0;
        wallRunLateralAngle = 0;*/
    }


    protected virtual void RotateCameraForWallRunning()
    {
        Vector3 EulerAux = transform.localEulerAngles;
        wallRunLateralAngle  = Mathf.LerpAngle(wallRunLateralAngle, wallRunAngleLimit, wallRunEnterRotationSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(EulerAux.x, EulerAux.y, wallRunLateralAngle);

        if(Mathf.Abs(wallRunLateralAngle - wallRunAngleLimit) < 0.1f)
        {
            inTiltCameraForWallRunning = false;
        }
    }

    protected virtual void ResetCameraForWallRunning()
    {
        Vector3 EulerAux = transform.localEulerAngles;
        wallRunLateralAngle = Mathf.LerpAngle(wallRunLateralAngle, 0,  wallRunExitRotationSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(EulerAux.x, EulerAux.y, wallRunLateralAngle);

        if (Mathf.Abs(wallRunLateralAngle) < 0.1f)
        {
            inResetCameraForWallRunning = false;
            wallRunAngleLimit = 0;
            wallRunLateralAngle = 0;
        }
    }

    // Set the time of the rotation effect.
    public virtual void SetRotationCameraFallTime(float _time)
    {
        rotateCameraFallTime = _time;
    }

    // Rotates the camera when player fall after jump and touch the floor.
    // It rotates some degrees (usually looking at the floor) and return to its original rotation.
    // Saves the actual rotation before starting to make the effect and assign the final rotation
    // using the angles the user has chosen.
    public virtual void RotateFall(float _RotY)//Debug.Log("Landing- rotation: "+rotationY+" Rot Dest: "+rotationYDest);
    {
        rotationYOrig = rotationY;
		rotationYDest = (rotationY >= 0) ? rotationY - _RotY : rotationY + -_RotY;

        i = 0;
        j = 0;
    }

    // Rotates the camera when player jump. (going up first, the opposite of rotatefall).
    public virtual void RotateJump(float _RotY)//Debug.Log("Landing- rotation: "+rotationY+" Rot Dest: "+rotationYDest);
    {
        rotationYOrig = rotationY;
        rotationYDest = (rotationY >= 0) ? rotationY + _RotY : rotationY - -_RotY;
        /*if (rotationY >= 0)
            rotationYDest = rotationY + _RotY;
        else
            rotationYDest = rotationY - -_RotY;*/

        i = 0;
        j = 0;
    }

    // Rotates the camera when climbing a ledge.
    public virtual void RotateClimbingLedge(float _RotY)//Debug.Log("Landing- rotation: "+rotationY+" Rot Dest: "+rotationYDest);
    {
        rotationYOrig = 0;
        rotationYDest = (rotationY >= 0) ? rotationY - _RotY : rotationY + -_RotY;
        /*if (rotationY >= 0)
            rotationYDest = rotationY - _RotY;
        else
            rotationYDest = rotationY + -_RotY;*/

        i = 0;
        j = 0;
    }

    // Set the time of the roll camera effect when climbing a ledge or lowLedge.
    public virtual void SetRollCameraClimbTime(float _time)
    {
        rotateCameraClimbTime = _time;
    }

    // Roll the camera (rotate over Z axis) when player jumps over a LowLedge (an obstacle) or when climbing a ledge. 
    // It rotates some degrees and return to its original rotation (that always is zero).
    public virtual void RollClimbing(float _RotZ)
    {
        rotationZDest = _RotZ;
        n = 0;
        m = 0;
    }

    // Roll the camera (rotate over Z axis) when player jumps over a LowLedge (an obstacle) or when climbing a ledge. 
    // It rotates some degrees and return to its original rotation (that always is zero).
    public virtual void RollFlying(float angle)
    {
        flyLateralAngle = angle;
    }

    // Set limits to the camera rotation in X axis. Called by ladder script.
    public virtual void SetXLimits(float min, float max)
    {
        if (min == -360 && max == 360)
        {
            minimumX = min;
            maximumX = max;
        }
        else
        {
            minimumX = rotationX + min;
            maximumX = rotationX + max;
        }

        /*minimumX = transform.eulerAngles.x + min;
		maximumX = transform.eulerAngles.x + max;*/
        rotationX = 0;
        xSmooth = 0;
        rotationY = 0;
        ySmooth = 0;
        originalRotation = transform.localRotation;
    }

    // Set limits to the camera rotation in Y axis. Called by ladder script.
    public virtual void SetYLimits(float min, float max)
    {
        if (min == -360 && max == 360)
        {
            minimumY = min;
            maximumY = max;
        }
        else
        {
            minimumY = transform.eulerAngles.y + min;
            maximumY = transform.eulerAngles.y + max;
        }

        rotationX = 0;
        rotationY = 0;
        originalRotation = transform.localRotation;
    }

    //==================================================================
    //
    // TEST PURPOSES. NOT USED FOR NOW IN PLAYER.
    // Weaponized Functions used to rotate UP the camera if firing a weapon. 
    //
    //==================================================================
    // Used in GunBobboer to create the recoil of the weapon when fired.
    public virtual void AddRecoil(float angle, bool _isFirtTime)
    {
        if (_isFirtTime)
            rotationYOrig = rotationY;

        rotationY = rotationY + angle;
        j = 1;
    }

    // Used in GunBobboer to create the recoil of the weapon when fired.
    public virtual void ReturnRecoil(float _time)
    {
        rotateCameraFallTime = _time;
        j = 0;
    }

    //==================================================================
    //
    // Private functions.
    //
    //==================================================================
    // Use a sinus function to roll the camera some degrees when walking, running, etc...
    // The parameters to tweak the effect are in CameraBobber.
    protected virtual void CameraRoll(float rollSpeed, float rollAmount)
    {
        //float wavesliceZ = 0f;
        Vector3 MyRot = transform.localEulerAngles;
        float totalAxesZ = InputManager.instance.HorizontalValue + InputManager.instance.VerticalValue;
        totalAxesZ = Mathf.Clamp01(totalAxesZ);
        MyRot.z = BobGenerator(MyRot.z, totalAxesZ, rollAmount, rollSpeed);
        MyRot.z = MyRot.z + wallRunLateralAngle; // 
        transform.localEulerAngles = MyRot;
    }

    // Internal function to clamp, dealing with the 360ª problem.
    protected virtual float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle = angle + 360f;
        else if (angle > 360f)
            angle = angle - 360f;
        return Mathf.Clamp(angle, min, max);
    }

    // Reset the player rotation to zero.
    public virtual void ResetPlayerRotation()
    {
        rotationX = 0;
        rotationY = 0;
        originalRotation = Quaternion.identity;
    }


    // NOT USED for now. Rotates the player over itself.
    /*public virtual void RotatePlayer (){
	    rotationX += 1 * sensitivityX;
	    rotationX = ClampAngle (rotationX, minimumX, maximumX);

	    var xQuaternion : Quaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
	    transform.localRotation = originalRotation * xQuaternion;
    }*/


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
        if (timedLerpRatio == 0)
            timedLerpRatio = 1 / time;

        float val = dest;
        if (j < 1f)
        {
            j = j + (Time.deltaTime * timedLerpRatio);
            if (Mathf.Abs(origin - dest) > 0.01f)
                val = Mathf.Lerp(origin, dest, j);
            else
            {
                j = 1;
                timedLerpRatio = 0;
            }
        }
        return val;
    }

    // Another TimedLerp function, just the same than the regular TimeLerp.
    // Is used to make the camera move at the same time doing to diffrent things,
    // for example, when climbing a ledge, the camera can do down/up and perform a roll at the same time
    // using two timelerped funcions at once.
    public float TimedLerp2n(float origin, float dest, float time)
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

    public float TimedLerp2m(float origin, float dest, float time)
    {
        if (timedLerpRatio2 == 0)
            timedLerpRatio2 = 1 / time;

        float val = dest;
        if (m < 1f)
        {
            m = m + (Time.deltaTime * timedLerpRatio2);
            if (Mathf.Abs(origin - dest) > 0.01f)
                val = Mathf.Lerp(origin, dest, m);
            else
            {
                m = 1;
                timedLerpRatio2 = 0;
            }
        }
        return val;
    }

    // Generates a BOB movement (for the camera or maybe weapons) using a sinus function.
    // It checks if totalAxes are presed to return Zero (no Bob at all)
    public float BobGenerator(float val, float totalAxes, float amount, float speed)
    {
        if (totalAxes == 0)
        {
            timerZ = 0f;
            val = 0;
        }
        else
        {
            val = (Mathf.Sin(timerZ) * amount) * totalAxes;
            timerZ = timerZ + (speed * Time.deltaTime);
            if (timerZ > (Mathf.PI * 2))
                timerZ = timerZ - (Mathf.PI * 2);
        }
        return val;
    }
		
}