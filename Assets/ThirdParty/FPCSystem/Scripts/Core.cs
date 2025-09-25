
using UnityEngine;
using System.Collections;

[System.Serializable]
public class MobileSustainedOptions
{
    [Tooltip("Is the sprint button a sutained key (sustained = the key has to be pressed always. NonSustained = the key behaves as a switch it will change On/Off. For mobile is false by default")]
    public bool sprintSustained = false;
    [Tooltip("Is the prone button a sutained key (sustained = the key has to be pressed always. NonSustained = the key behaves as a switch it will change On/Off. For mobile is false by default")]
    public bool proneSustained = false;
    [Tooltip("Is the crouch button a sutained key (sustained = the key has to be pressed always. NonSustained = the key behaves as a switch it will change On/Off. For mobile is false by default")]
    public bool crouchSustained = false;
}

public class Core : MonoBehaviour
{
    [Header("Sustained Keys Detectors")]
    [Tooltip("Used at demo scene, it's not suposse to be used in a real game. Activate the 'F11' key to change the mode to crouch/Prone. Demo purposes feature." +
        "useSustainedKey define if Sprint/Crouch/Prone keys needs to be pressed always to perform that action or they are a 'switch mode' buttons, so only pressing once you change that behavior On/Off.")]
	public bool useSustainedKey = true;
    [Tooltip("Configure in mobile the sustained key you want to use. Usually in mobiles you'll set all to false (use non sustained keys at all)")]
    public MobileSustainedOptions mobileSustainedOptions = new MobileSustainedOptions(); 

    [Header("Sensor Detectors")]
	[Tooltip("Forward sensor cast to 'see' what's in front of us. It uses a Capsule Cast that takes the Character's Height. " +
		"Stairs (not ladders) use this sensor to be able to walk step by step in the stairs. " +
		"If you aren't using stairs and you don't need it, you can deactivate it.")]
	public SensorOptions forwardSensorOptions = new SensorOptions ();
	[Tooltip("Below sensor cast to detect what are we stepping on. It uses a very sort 'Ray' to do it. " +
		"It will be ALWAYS enabled by pl, even if you deactivate it, because it's used by a lot of scripts (ladders, flying, etc...)." +
		"Recommended distance is 0.4f")]
	public SensorOptions belowSensorOptions = new SensorOptions ();
	[Tooltip("Camera Forward sensor cast to detect what we are looking at. It will be used by a crossAir if any. " +
		"If there is a CrossAir enabled, this sensor will be always activated by it, to be able to change its color " +
		"when detecting a target or enemy.. If you don't need it and there isn't any crossair, you can deactivate it.")]
	public SensorOptions forwardCameraSensorOptions = new SensorOptions ();


	[Header("Usual actions")]
    [Tooltip("Push any object in scene that have a rigidbody.")]
	public PushOptions push = new PushOptions();
    [Tooltip("prone options.")]
	public ProneOptions prone = new ProneOptions();
    [Tooltip("crouch options.")]
	public CrouchOptions crouch = new CrouchOptions();
    [Tooltip("run with a time limitation (sprint time).")]
	public SprintOptions sprint = new SprintOptions();

	[Header("Camera movement special effects")]
    [Tooltip("Fall options. When pl falls into the floor a default camera effect is performed and a damage script test is executed to verify if pl gets hurt.")]
	public FallOptions fall = new FallOptions();
    [Tooltip("Moving camera effects configuration when falling. The camera moves down/up at the desired velocity and displacement and rotates a given the angle. There are three types of effects: normal landing, high landing without getting any damage and damage landing. This function has a strong dependency on which values you have on the CharacterMotor script (MaxAirAcceleration and Gravity in the Movement section).")]
	public CameraFallOptions cameraFallEffects = new CameraFallOptions();
    [Tooltip("Moving camera effects configuration when jumping. The camera moves up/down at the desired velocity and displacement, and rotates the angle we want. Notice that this effects are almost the same than the fall effects, but the movement is made first in the 'up' direction.")]
	public CameraJumpOptions cameraJumpEffects = new CameraJumpOptions();

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;
    [Tooltip("Show all 'Debug.Log' falling messages.")]
    [ShowWhen("showDebug")]
    public bool showFallDebug = false;
    [Tooltip("Show all 'Debug.Log' ControllerHit (Above/sideways) messages.")]
    [ShowWhen("showDebug")]
    public bool showControllerHitDebug = false;
    [Tooltip("Show all 'Debug.Log' Crouch messages.")]
    [ShowWhen("showDebug")]
    public bool showCrouchDebug = false;
    [Tooltip("Show all 'Debug.Log' Prone messages.")]
    [ShowWhen("showDebug")]
    public bool showProneDebug = false;
    [Tooltip("Show all 'Debug.Log' Jump messages.")]
    [ShowWhen("showDebug")]
    public bool showJumpDebug = false;


    //===============================================
    // General private vars.
    //===============================================
    // Health Private vars.
    private Status statusSrc;
    private SliderFader HealthScript;
    private CharacterController controller;
    private CharacterMotor motor;
	private Kick kickScr;
	private WallParkour wallParkourScr;

    // Prone Private vars.
    //private var isPerformingProne : boolean = false;
	private float minFrwdWalkSpeed;	// Minimum (Walk) Forward velocity of the player (used to change the velocity when crouching, Proning  or jumping)
    private float minBackWalkSpeed; // Backward wak velocity
    private float minSideWalkSpeed; // Sideways walk velocity
    //private bool isInAirPrev;

    // Crouch Private vars.
    //private var isPerformingCrouch : boolean = false;
    // Sprint Private vars.
    private SliderFader SprintScript;
    private float sprintInternalTime;
	private bool OrigCanSprint = true;
    private float CameraFovOrig; // Original FOV value of the camera.
	//private float dirtyLensOrigPosition = 0.3f; // Original local position of the DirtyLens Plane.

    // Damage private vars
	private float DamageInternalTime = 1; // One second of silence while everything inicializes.

    // Crouch/prone private vars
    private bool hasPlayedOnce; // Used to play crouch/prone going down/up sound.

    // Internal Taptime used to detect when user press double jump.
	[HideInInspector]
	public float lastTapTime;
	[HideInInspector]
    public float testJumpStateTime;
    //public static bool isJumpReleased = true;

    private bool isSprintNotSustainedActive;
    private bool isProneNotSustainedActive;
    private bool isCrouchNotSustainedActive;

    [HideInInspector]
    public float motorGroundAccelOrig = 30f;
    [HideInInspector]
    public float motorAirAccelOrig = 30f;
    [HideInInspector]
	public bool GUISustainedMode = true;

     
    private bool isAutojump;    // after perfoming a jump from a platform (so we know we are autojumping from a jump platform).
    private bool isPlatformJumpSafe; // To be used on Fall damage (if autojump is not safe you can die, depending on fall time).

	// Private values used by the forward sensor (to detect what is in front of us).
	private Vector3 sensorDirFwd;
	private Vector3 sensorP1;
	private Vector3 sensorP2;
	private RaycastHit sensorHitInfo;

	private Transform cameraTransform;
	private Vector3 sensorCameraDirFwd;
	private RaycastHit sensorCameraHitInfo;

	private Vector3 sensorBelowOrigin;
	private RaycastHit sensorBelowHitInfo;

	private CharacterOriginalData characterControllerOrig; // Save the initial CharacterController values.




    // Public functions called by Swim function to enable/disable sprint capabilities
    // when swiming (swim has a variable to enable/disable when swiming).

    public float GetMinimumWalkVelocity() { return minFrwdWalkSpeed; }

    public void EnableSprint(bool _value){ sprint.canSprint = _value; }

    public bool CheckSprint(){ return sprint.canSprint == OrigCanSprint; }

    public void ResetSprint(){ sprint.canSprint = OrigCanSprint; }

    public void SetAutoJump(bool _jump){ isAutojump = _jump; }

    public void SetPlatformJumpSafe(bool _isSafe) { isPlatformJumpSafe = _isSafe; }
  

    //==================================================================================================
    //
    // General Section - Start, Update, Fixed, Update,  InputController and ControllerColliderHit
    //
    //==================================================================================================
    
	void Start()
    {
		// Get the controller component and save it values.
		statusSrc = GetComponent<Status>();
		controller = statusSrc.GetController();
		motor = statusSrc.GetMotor();
		kickScr = statusSrc.GetKickScr();
		wallParkourScr = statusSrc.GetWallParkourScr();

		characterControllerOrig = new CharacterOriginalData ();
		characterControllerOrig.slopeLimit = controller.slopeLimit;
		characterControllerOrig.stepOffset = controller.stepOffset;
		characterControllerOrig.skinWidth = controller.skinWidth;
		characterControllerOrig.center = controller.center;
		characterControllerOrig.radius = controller.radius;
		characterControllerOrig.height = controller.height;
       
        minFrwdWalkSpeed = motor.movement.maxForwardSpeed;
        minBackWalkSpeed = motor.movement.maxBackwardsSpeed;
        minSideWalkSpeed = motor.movement.maxSidewaysSpeed;
        motorGroundAccelOrig = motor.movement.maxGroundAcceleration;
        motorAirAccelOrig = motor.movement.maxAirAcceleration;
        

        CameraFovOrig = Camera.main.fieldOfView;
        sprintInternalTime = sprint.sprintTime;
        OrigCanSprint = sprint.canSprint; // used by swim script.
        if (sprint.SprintGUI)
        {
            SprintScript = (SliderFader) sprint.SprintGUI.GetComponent(typeof(SliderFader));
            sprint.SprintGUI.maxValue = sprintInternalTime;
            sprint.SprintGUI.value = sprintInternalTime;
        }
        else
            Debug.LogWarning("Core -> SprintGUI has not been assigned in the Status.");

        // The below sensor detector is always active. It's used by a lot of processes (ladders, when flying, etc...)
        belowSensorOptions.useSensorDetector = true;

		// Get the camera Transform to be used by the Forward Camera sensor.
		cameraTransform = Camera.main.transform;

        // ========================================================================================
        // In mobile platforms be sure that the isSustained key is false.
        #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        GUISustainedMode = useSustainedKey;
        sprint.isSustained = mobileSustainedOptions.sprintSustained;
        prone.isSustained = mobileSustainedOptions.proneSustained;
        crouch.isSustained = mobileSustainedOptions.crouchSustained;
        #else
        GUISustainedMode = useSustainedKey;
        sprint.isSustained = GUISustainedMode;
        prone.isSustained = GUISustainedMode;
        crouch.isSustained = GUISustainedMode;
        #endif
        // ========================================================================================
    }


    void Update()//DetectMouse(); // Not used for now.
    {
        // ========================================================================================
        // TODO.
        // USED FOR DEMO PURPOSES. Can be commented at any time.
        //
        // Change sustained mode used to crouch/prone/sprint using the 'F9' key.
        // 
        if (InputManager.instance.f11Key.isDown && useSustainedKey)
        {
            GUISustainedMode = !GUISustainedMode;
            sprint.isSustained = GUISustainedMode;
            prone.isSustained = GUISustainedMode;
            crouch.isSustained = GUISustainedMode;
        }

        //
        // ========================================================================================
        InputController();

		// Check the Raycast Sensor
		if(!belowSensorOptions.useSensorFixedUpdate)
			SensorBelowCast ();
		if(!forwardSensorOptions.useSensorFixedUpdate)
			SensorForwadCast ();
		if(!forwardCameraSensorOptions.useSensorFixedUpdate)
			SensorCameraForwadCast ();

		DetectGroundStairs ();

        DoSprint();

        // Detect change in sprint sustained key when pressing the 'run' key (can be done underwater to walk/Run with NON sustained configuration)
        if (InputManager.instance.runKey.isDown)
            isSprintNotSustainedActive = !isSprintNotSustainedActive;

        // Perform jump & kicks.
        // Note: Not allow jumps or kicks in the water (exception on underwater ladders jumps backwards). Dont allow when flying either
        if ( !((statusSrc.isInLadder || statusSrc.isInLedge)) )
        {
			DetectJumpInput();

			// Check if we can run on a wall.
			if(wallParkourScr != null && wallParkourScr.enabled)
			{
				wallParkourScr.ProcessWallParkour ();
			}

            if (motor.jumping.canJump)
                UpdateJumpStates();

			if(kickScr != null && kickScr.enabled)
            {
				if (kickScr.sideKick.canSideKick)
					kickScr.UpdateSideKickState();

				if (kickScr.aboveKick.canAboveKick)
					kickScr.AboveKick();
			}
        }
    }


	//=======================================================================================
	//
	// RayCast Section : Get the hit of any object below and in front of player
	//
	//=======================================================================================

	// Physics cast forward to detect what is ahead (used to detect stairs to be able to walk on them)
	private void SensorForwadCast()
	{
		// Hole forward detection. Using all the player capsule height.
		if(forwardSensorOptions.useSensorDetector)
		{
			sensorDirFwd = transform.TransformDirection(Vector3.forward);
            Vector3 orig = transform.position + controller.center + (-sensorDirFwd * 0.1f);

            sensorP1 = orig + (Vector3.up * (-controller.height * 0.5f));
			sensorP2 = sensorP1 + (Vector3.up * controller.height);

            // Debug.Line to visualize the capsulecast
            //Debug.DrawLine(sensorP1, sensorP2, Color.red);
            //Vector3 dest = orig + (sensorDirFwd * forwardSensorOptions.sensorDistance);
            //Debug.DrawLine(orig, dest, Color.red);

            // Detect if we are facing something.
            if (Physics.CapsuleCast(sensorP1, sensorP2, controller.radius, sensorDirFwd, out sensorHitInfo, forwardSensorOptions.sensorDistance, forwardSensorOptions.sensorLayers))
			{
				forwardSensorOptions.sensorHitTag = sensorHitInfo.transform.tag;
				forwardSensorOptions.sensorHitDistance = sensorHitInfo.distance;
				forwardSensorOptions.objectHit = sensorHitInfo.collider.gameObject;
                forwardSensorOptions.hitNormal = sensorHitInfo.normal;
                forwardSensorOptions.hitPosition = sensorHitInfo.point;
            }
			else{
				forwardSensorOptions.sensorHitTag = string.Empty;
				forwardSensorOptions.sensorHitDistance = 0f;
				forwardSensorOptions.objectHit = null;
                forwardSensorOptions.hitNormal = Vector3.zero;
                forwardSensorOptions.hitPosition = Vector3.zero;
            }
		}
	}

	private void SensorBelowCast()
	{
		if(belowSensorOptions.useSensorDetector)
		{
			sensorBelowOrigin = transform.position + controller.center + (Vector3.down * controller.height * belowSensorOptions.sensorDistance * 0.5f);
            //Debug.Log("SensorBelowCast -> SensorBelowOrigi: " + sensorBelowOrigin + " Center: " + controller.center + " height: " + controller.height);

            if (Physics.SphereCast(sensorBelowOrigin, controller.radius-0.1f, -transform.up, out sensorBelowHitInfo, belowSensorOptions.sensorDistance, belowSensorOptions.sensorLayers))
			{
				belowSensorOptions.sensorHitTag = sensorBelowHitInfo.transform.tag;
				belowSensorOptions.sensorHitDistance = sensorBelowHitInfo.distance;
				belowSensorOptions.objectHit = sensorBelowHitInfo.collider.gameObject;
                belowSensorOptions.hitNormal = sensorBelowHitInfo.normal;
                belowSensorOptions.hitPosition = sensorBelowHitInfo.point;
            }
			else{
				belowSensorOptions.sensorHitTag = string.Empty;
				belowSensorOptions.sensorHitDistance = 0f;
				belowSensorOptions.objectHit = null;
                belowSensorOptions.hitNormal = Vector3.zero;
                belowSensorOptions.hitPosition = Vector3.zero;
            }
		}
	}

	private void SensorCameraForwadCast()
	{
		// Detect what are you pointing with the camera.
		if(forwardCameraSensorOptions.useSensorDetector)
		{
			if(cameraTransform == null) cameraTransform = Camera.main.transform; // Security sentence. Just in case...

			sensorCameraDirFwd = cameraTransform.TransformDirection(Vector3.forward);
			if (Physics.Raycast(cameraTransform.position, sensorCameraDirFwd, out sensorCameraHitInfo, forwardCameraSensorOptions.sensorDistance, forwardCameraSensorOptions.sensorLayers))
			{
				forwardCameraSensorOptions.sensorHitTag = sensorCameraHitInfo.transform.tag;
				forwardCameraSensorOptions.sensorHitDistance = sensorCameraHitInfo.distance;
				forwardCameraSensorOptions.objectHit = sensorCameraHitInfo.collider.gameObject;
                forwardCameraSensorOptions.hitNormal = sensorCameraHitInfo.normal;
                forwardCameraSensorOptions.hitPosition = sensorCameraHitInfo.point;
            }
			else{
				forwardCameraSensorOptions.sensorHitTag = string.Empty;
				forwardCameraSensorOptions.sensorHitDistance = 0f;
				forwardCameraSensorOptions.objectHit = null;
                forwardCameraSensorOptions.hitNormal = Vector3.zero;
                forwardCameraSensorOptions.hitPosition = Vector3.zero;
            }
		}
	}


	// Check if forward capsule cast has detected stairs and change slopelimit to be able to walk over them
	private void DetectGroundStairs()
	{
		if(!forwardSensorOptions.useSensorDetector) return;

		if(forwardSensorOptions.sensorHitTag.Contains("Stairs") || belowSensorOptions.sensorHitTag.Contains("Stairs"))
		{
			if (controller.slopeLimit != 90f)
				controller.slopeLimit = 90f;
            if(!motor.stairsDetected)
                motor.stairsDetected = true;
		}
		else
		{
			if (controller.slopeLimit != characterControllerOrig.slopeLimit)
				controller.slopeLimit = characterControllerOrig.slopeLimit;
            if (motor.stairsDetected)
                motor.stairsDetected = false;
        }
	}

    // Push the controller in a direction with a force (internally it just add velocity to the CharacterMotor).
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0)
            dir.y = -dir.y; // reflect down force on the ground
        motor.AddVelocity(dir.normalized * force /* / mass */); //impact = dir.normalized * force / mass;
        statusSrc.isBeingPushed = true;
        StartCoroutine("_DisablePusherTimed", 1f);
    }


    private IEnumerator _DisablePusherTimed(float _time)
    {
        yield return new WaitForSeconds(_time);
        statusSrc.isBeingPushed = false;
    }

    // it is called once per frame. Original Unity's function.
    private void InputController()
    {
        // Get the input vector from keyboard or analog stick
        Vector3 directionVector = new Vector3(InputManager.instance.HorizontalValue, 0, InputManager.instance.VerticalValue);
        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;
            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1, directionLength);
            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;
            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }

        // Apply the direction to the CharacterMotor
        motor.inputMoveDirection = transform.rotation * directionVector;
        if (!isAutojump)
            motor.inputJump = statusSrc.isJumpDetected;
    }

   

    public GameObject GetObjectBelow()
    {
		return belowSensorOptions.objectHit;
    }

    public string GetTagBelow()
    {
		return belowSensorOptions.sensorHitTag;
    }


    // Needed to be in FixedUpdate because internally they used a raycast
    void FixedUpdate()
    {

		// Check the Raycast Sensor
		if(belowSensorOptions.useSensorFixedUpdate)
			SensorBelowCast ();
		if(forwardSensorOptions.useSensorFixedUpdate)
			SensorForwadCast ();
		if(forwardCameraSensorOptions.useSensorFixedUpdate)
			SensorCameraForwadCast ();

        if (prone.canProne)
        {
            DoProne();
        }


        if (crouch.canCrouch)
        {
            DoCrouch();
        }

        // Slide when running
        if (!statusSrc.isPerformingProne && !statusSrc.isProned && !statusSrc.isPerformingCrouch && !statusSrc.isCrouched)
        {
            if (statusSrc.isRunning)
            {
                if (motor.movement.maxGroundAcceleration != sprint.sprintSlideOptions.maxGroundAcceleration)
                    motor.movement.maxGroundAcceleration = sprint.sprintSlideOptions.maxGroundAcceleration;
                if (motor.movement.maxAirAcceleration != sprint.sprintSlideOptions.maxAirAcceleration)
                    motor.movement.maxAirAcceleration = sprint.sprintSlideOptions.maxAirAcceleration;
                if (statusSrc.isSliding) statusSrc.isSliding = false;
            }
            else
            {
                ChangeAccelValues();
            }
        }
    }

    // It's called from the CharacterMotor (line 304).
    // When landing on the floor we look at current vertical speed of pl.
    // Depending on this velocity, we activate the apropiatte camera & sound effect.
    // If is greater than the Damage Speed, subtract to pl's health a % of the current vertical speed value.
    public void OnFall(Vector3 velocity)
    {
        // TODO
        // Set the default acceleration values when landing (for now i suposse is always stopped or walking)
        motor.movement.maxGroundAcceleration = motorGroundAccelOrig;
        motor.movement.maxAirAcceleration = motorAirAccelOrig;

        // Render a footprint if we have some velocity (always start with the left foot, 
        // because the camera will start sending a render msg for the righht foot)
        Vector3 velo = velocity;
        velo.y = 0;
        if (velo.magnitude > 0.9f)   // We are not stopped.
        {
            statusSrc.isInGround = true; // Make sure that we are grounded or the footprint will NOT be rendered.
            EventManagerv2.instance.TriggerEvent("CameraRachedBobEnd", new EventParam(this.name, string.Empty, "leftFoot"));
            if (showDebug && showFallDebug) Debug.Log("Core -> OnFall() -> Render a left footprint in the ground");
        }


        // Play landing sounds depending on velocity & assign damage to the player.
        if (Time.time > DamageInternalTime) // && isInAirPrev != StatusSrc.isInAir)
        {
            //if(showDebug && showFallDamageDebug) Debug.Log("Core -> OnFall() -> Motor Velo (old): "+motor.movement.velocity+", Mag: "+motor.movement.velocity.magnitude+" - Velo (new): "+velocity+", Mag: "+velocity.magnitude);
            // No damage if we fall in the water or if we are in air (colliding laterally againts something)
            if (!statusSrc.isInLadder && !statusSrc.isInLedge && !statusSrc.isInLowLedge)
            {
                float fallTime = statusSrc.GetFallTime();

                if (motor.movement.collisionFlags != CollisionFlags.CollidedSides && 
                    motor.movement.collisionFlags != CollisionFlags.CollidedAbove && !motor.IsSliding() &&
                    fallTime >= fall.fallMinimunTime)
                {
                    //Debug.Log(motor.movement.velocity.y+", "+motor.movement.velocity.magnitude);
                    // Get Damage.
                    float damage = fall.timeDamagePercent.Evaluate(fallTime * 0.1f) * 100; // Time has a x10 multiplier, damage a x100 mult.
                    if (showDebug && showFallDebug) Debug.Log("Core -> OnFall() -> Damage: " + damage + " FallTime: "+ fallTime);

                    // If there is some damage, that is it.
                    if (damage >= 1f)
                    {
                        // If player can get damage when fall, then apply damage to his health and play the GUI damage and the hurt sound.
                        if (fall.canGetDamage && !isPlatformJumpSafe) // Jumping from autoJump platforms is safe!!
                        {
                            statusSrc.SetDamage(Mathf.FloorToInt(damage));
                            DoDamageLandEffects ();
                        }
                        else
                            DoDamageSafeLandEffects();

                        //isPlatformJumpSafe = false; // Make sure that autojump gets false (jumper - The jumper itself is doing it too)
                    }
                    else
                    // Fall from  high altitude but no damage. Just make camera land effects.
                    if (fallTime < fall.fallMinimunTime * 2)
                    {
                        DoHighLandEffects ();
                    }
                    else
                    // Normal fall, if we land a too low speed, do nothing but a simple camera movement and a easy landing sound.
                    if (fallTime >= fall.fallMinimunTime)
                    {
                        DoNormalLandEffects ();
                    }
                }

                else if(fall.alwaysDoLandFX && fallTime > fall.fallMinimunTime*0.5f)
                {
                    DoNormalLandEffects ();
                }

                DamageInternalTime = Time.time + 1;
                statusSrc.isInAir = false;
            }
        }
    }

    // Called from CharacterMotor when the player falls landing in the ground (it lands in a platform or something). 
    public void OnLand(Vector3 velocity)
    {
        OnFall(velocity);
    }

    // Normal landing FX. 
    private void DoNormalLandEffects()
    {
        BroadcastMessage("PlayFallNormal"); // Play Sound FX.
        if (cameraFallEffects.enableEffects && !statusSrc.isCrouched && !statusSrc.isPerformingCrouch && !statusSrc.isProned && !statusSrc.isPerformingProne)
        {
            // Moving the camera down when landing. (Send message to the CameraBobber script)
            SendMessage("SetCameraPingPongMoveTime", cameraFallEffects.NormalTime);
            SendMessage("SetCameraDestPosition", cameraFallEffects.NormalPos);
            // Rotation the camera when landing.
            Camera.main.gameObject.SendMessage("SetRotationCameraFallTime", cameraFallEffects.NormalRotTime);
            Camera.main.gameObject.SendMessage("RotateFall", cameraFallEffects.NormalRotAngle);
        }
    }

    // Falling from high altitude FX.
    private void DoHighLandEffects()
    {
        BroadcastMessage("PlayFallNormal");   // Play Sound FX.
        if (cameraFallEffects.enableEffects && !statusSrc.isCrouched && !statusSrc.isPerformingCrouch && !statusSrc.isProned && !statusSrc.isPerformingProne)
        {
            // Moving the camera down when landing. (Send message to the CameraBobber script)
            SendMessage("SetCameraPingPongMoveTime", cameraFallEffects.HighTime);
            SendMessage("SetCameraDestPosition", cameraFallEffects.HighPos);
            // Rotation the camera more angles because the hard landing.
            Camera.main.gameObject.SendMessage("SetRotationCameraFallTime", cameraFallEffects.HighRotTime);
            Camera.main.gameObject.SendMessage("RotateFall", cameraFallEffects.HighRotAngle);
        }
    }

    // The same FX as Damage but without the damage hurting sound.
    private void DoDamageSafeLandEffects()
    {
        BroadcastMessage("PlayFallNormal");
        if (cameraFallEffects.enableEffects && !statusSrc.isCrouched && !statusSrc.isPerformingCrouch && !statusSrc.isProned && !statusSrc.isPerformingProne)
        {
            // Moving the camera down when landing.
            SendMessage("SetCameraPingPongMoveTime", cameraFallEffects.DamageTime);
            SendMessage("SetCameraDestPosition", cameraFallEffects.DamagePos);
            // Rotation the camera a little big too.
            Camera.main.gameObject.SendMessage("SetRotationCameraFallTime", cameraFallEffects.DamageRotTime);
            Camera.main.gameObject.SendMessage("RotateFall", cameraFallEffects.DamageRotAngle);
        }
    }

    // Rake full damage FX with hurt sound.
    private void DoDamageLandEffects()
    {
        gameObject.BroadcastMessage("PlayFallHurt");
        if (cameraFallEffects.enableEffects && !statusSrc.isCrouched && !statusSrc.isPerformingCrouch && !statusSrc.isProned && !statusSrc.isPerformingProne)
        {
            // Moving the camera down when landing.
            SendMessage("SetCameraPingPongMoveTime", cameraFallEffects.DamageTime);
            SendMessage("SetCameraDestPosition", cameraFallEffects.DamagePos);
            // Rotation the camera a little big too.
            Camera.main.gameObject.SendMessage("SetRotationCameraFallTime", cameraFallEffects.DamageRotTime);
            Camera.main.gameObject.SendMessage("RotateFall", cameraFallEffects.DamageRotAngle);
        }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
		if (hit.transform.name.Contains("Floor") || hit.transform.tag.Contains("Terrain")) { return; } // Ignore the floor
		if (hit.transform.tag.Contains("Water")) { return; } // Ignore the water
		if (hit.transform.tag.Contains("EnemyBullet")) { return; } // Ignore the bullets. They can have a rigidbody and we dont want to push them.

        if (motor.movement.collisionFlags == CollisionFlags.Sides)
        {
            if (showDebug && showControllerHitDebug) Debug.Log("Core -> OnControllerColliderHit() -> Name: " + hit.transform.name + " Normal :" + hit.normal + " Coliding Side.");
        }
        else if (motor.movement.collisionFlags == CollisionFlags.Above)
        {
            if (showDebug && showControllerHitDebug) Debug.Log("Core -> OnControllerColliderHit() -> Name: " + hit.transform.name + " Normal :" + hit.normal + " Coliding Above.");
        }
        /*else if (motor.movement.collisionFlags == CollisionFlags.Below)
        {
            if (showDebug && showControllerHitDebug) Debug.Log("Core -> OnControllerColliderHit() -> Name: " + hit.transform.name + " Normal :" + hit.normal + " Coliding Below.");
        }*/


        // Detect if we are over a platform								
        if (hit.collider.tag.Contains("Platform") && !statusSrc.isOverPlatform)
            statusSrc.isOverPlatform = true;


        //==============================================
        // Push any rigidbody in the scene.
        //==============================================
        if (statusSrc.isCrouched || statusSrc.isProned) { return; } // Dont push anything if not standing.

        if (!push.canPush) { return; } // Check if we can push objects
        if (push.canPush && push.useKey && !InputManager.instance.pushKey.isPressed) { return; } // Check if we can push objects pressing a 'push key' but the key hasn't been pressed.

        if (hit.moveDirection.y < -0.3f) { return; }    // We dont want to push objects below us

        Rigidbody body = hit.collider.attachedRigidbody;    // If there isn't a rigidbody or isKinematic then we can't push it.
        if (body == null || body.isKinematic) { return; }


        ObjectPushOptions objPushOptions = hit.gameObject.GetComponent<ObjectPushOptions>();
        // If no pushOptions in the object, use the Player options
        if (objPushOptions == null)
        {
            if (!push.useKey || (push.useKey && InputManager.instance.pushKey.isPressed))
            {
                // Calculate push direction from move direction.
                //Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                Vector3 pushDir = hit.moveDirection;
                pushDir.y = 0f;

                // If you know how fast your character is trying to move, then you can also multiply the push velocity by that.
                body.linearVelocity = pushDir * push.pushPower.x + Vector3.up * push.pushPower.y;  // Apply the push
            }
        }
        else
        {
            // If the object has PushOptions, push the object with that options.
            if (!objPushOptions.useKey || (objPushOptions.useKey && InputManager.instance.pushKey.isPressed))
            {
                // Calculate push direction from move direction,
                //Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                Vector3 pushDir = hit.moveDirection;
                pushDir.y = 0f;

                // Apply the push
                if (objPushOptions.overridePushPower)
					body.linearVelocity = pushDir * objPushOptions.pushPower.x + pushDir * push.pushPower.y;
				else
					body.linearVelocity = pushDir * push.pushPower.x + Vector3.up * push.pushPower.y;
            }
        }
    }

    //=======================================================================================
    //
    // Jump & KeyBoard Section : DetectKeyboard, Jump, SimpleKick, SideKick Section.
    //
    //=======================================================================================
    // Keyboard detection of jumps, kicks and sidekicks.
    private void DetectJumpInput()
    {
        if (InputManager.instance.proneKey.isDown)
        {
            isProneNotSustainedActive = !isProneNotSustainedActive;
            isCrouchNotSustainedActive = false;
        }
        if (InputManager.instance.crouchKey.isDown)
        {
            isCrouchNotSustainedActive = !isCrouchNotSustainedActive;
            isProneNotSustainedActive = false;
        }
        /*if (InputManager.instance.runKey.isDown)
            isSprintNotSustainedActive = !isSprintNotSustainedActive;*/

        // Detect Jumps.(not allowed when we are in prone/crouch position.)
        if (InputManager.instance.jumpKey.isDown)
        {
            if (!(statusSrc.isCrouched || statusSrc.isProned || 
                 (statusSrc.isInLedge && statusSrc.isClimbing)))
            {
				if(kickScr == null || !kickScr.enabled){
                    if (showDebug && showJumpDebug) Debug.Log("Core -> DetectJumpInput() -> Pressed Jump Key Detected");
                    lastTapTime = Time.time; // no kick it is a normal jump.
					statusSrc.isSimpleKickDetected = false;
					statusSrc.isJumpDetected = true;
				}
            }
        }
        else
        {
            if (InputManager.instance.jumpKey.isUp) // Detect when release the jump button.
            {
                if (showDebug && showJumpDebug) Debug.Log("Core -> DetectJumpInput() -> Release Jump Key Detected");
                statusSrc.isJumpDetected = false;
                statusSrc.isJumpReleased = true;
            }
        }
    }

    // Jump related methods
    // Called by CharacterMotor when a jump is performed.
    public void OnJump()
    {
        PerformJumpVisualEffects();
        BroadcastMessage("PlayJump", SendMessageOptions.DontRequireReceiver);
    }

    public void PerformJumpVisualEffects()
    {
        if (cameraJumpEffects.enableEffects && !statusSrc.isInLowLedge)
        {
            // Moving the camera down when jumping.
            gameObject.SendMessage("SetCameraPingPongMoveTime", cameraJumpEffects.JumpTime);
            gameObject.SendMessage("SetCameraDestPosition", cameraJumpEffects.JumpPos);
            // Rotate the camera a little bit too over itself.
            Camera.main.gameObject.SendMessage("SetRotationCameraFallTime", cameraJumpEffects.JumpRotTime);
            Camera.main.gameObject.SendMessage("RotateJump", cameraJumpEffects.JumpRotAngle);
        }
    }

    private void EnableJumpFlags()
    {
        statusSrc.isJumpReleased = false;
        statusSrc.isJumping = true;
        testJumpStateTime = Time.time + 0.1f;
        if (!motor.jumping.enabled)
            motor.jumping.enabled = true;
    }

    private void DisableJumpFlags()
    {
        if (motor.jumping.enabled)
        {
            statusSrc.isJumping = false;
            motor.jumping.enabled = false;
        }
    }

    // Perform the jump effects if jump is detected (keyboard detected).
    // Perform a kick is user has pressed twice the jump button.
    private void UpdateJumpStates()
    {
        
		// Perform the jump if posible.
        if (statusSrc.isJumpReleased && statusSrc.isJumpDetected)
        {
            if (!statusSrc.isJumping)
            {
                // Case 1.- Sprint bar detected: Detect if we have enought 'energy' to perform the jump.
                if (SprintScript && sprintInternalTime > sprint.minJumpValue)
                {
                    if (showDebug && showJumpDebug) Debug.Log("Core -> UpdateJumpStates() -> Jump and substrack stamina time");
                    EnableJumpFlags();	// Execute the jump.
					sprintInternalTime = sprintInternalTime - sprint.jumpSustractTime; // Decrease the run & jump energy a user-seleted amount.
                    sprint.SprintGUI.value = sprintInternalTime;
                }	
				else if (!SprintScript)	// Case 2.- NO Sprint bar: perform the jump anyways..
				{
                    if (showDebug && showJumpDebug) Debug.Log("Core -> UpdateJumpStates() -> Jump");
                    EnableJumpFlags(); // Execute the jump.
				}
				else // Case 3.- If we dont have enought 'energy' to jump, disable it.
				{
                    if (showDebug && showJumpDebug) Debug.Log("Core -> UpdateJumpStates() -> No Energy to Jump!");
                    DisableJumpFlags();	// Stop the jump.
				}
            }
        }

        // Can we kick? (user selected in inspector vars).
		if(kickScr != null && kickScr.enabled)
		{
			if (kickScr.simpleKick.canKick)
				kickScr.UpdateSimpleKickState ();
		}
       
    }

  

    //=======================================================================================
    //
    // States Section : Crouch, Prone, Sprint andShowStatus
    //
    //=======================================================================================
    private void DoCrouch()
    {
        RaycastHit hit = default(RaycastHit);
        // Look if we are preesing Crouch button to make the Player crouch.
        // if is sustained wee need to press the 'crouch' key all the time.
        // if not just pressing a leaving the key will do the trick.
        if ((crouch.isSustained && InputManager.instance.crouchKey.isPressed) || (!crouch.isSustained && isCrouchNotSustainedActive))
        {
            // No Crouch on ladders, ledges and water.
			//if (StatusSrc.isInLadder ||StatusSrc.isInLedge ||StatusSrc.isInWater ||StatusSrc.isFlying) { return; }
            if (statusSrc.isInLadder || statusSrc.isInLedge)
            {
                return;
            }

            if (!crouch.canCrouchInAir && statusSrc.isInAir) return;


            //Disable the prone status and set the camera movement speed.
            gameObject.SendMessage("SetCameraMoveTime", crouch.cameraMovTime);
            if (statusSrc.isPerformingProne || statusSrc.isProned)
            {
                if (statusSrc.isProned) statusSrc.isProned = false;
                if (statusSrc.isPerformingProne) statusSrc.isPerformingProne = false;
                //StopCoroutine("ChangeMovementValues");
            }

            // Going down and lowering the speed too.
            if (!statusSrc.isCrouched || statusSrc.isPerformingCrouch)
            {
                /*motor.movement.maxForwardSpeed = crouch.walkForwardSpeed;
                motor.movement.maxSidewaysSpeed = crouch.walkSidewaysSpeed;
                motor.movement.maxBackwardsSpeed = crouch.walkBackwardSpeed;*/
                gameObject.SendMessage("SetMidpoint", crouch.height + 0.1f); // Set camera midpoint for camera's bob.
                if (!hasPlayedOnce)
                {
                    gameObject.SendMessage("CancelAnyCameraMovement"); // Cancel any previous camerafall effect
                    gameObject.BroadcastMessage("CrouchGoingDownUP"); // Play sound effect.
                    hasPlayedOnce = true;
                }

                if (controller.height > crouch.height)
                {
                    if (showDebug && showCrouchDebug) Debug.Log("Core -> DoCrouch() -> Going down (crouching)");
                    statusSrc.isPerformingCrouch = true;
                    controller.height = controller.height - (crouch.crouchSpeed * Time.deltaTime);
                }
                else
				// It's done. Now we are crouched.
                if (controller.height < crouch.height)
                {
                    controller.height = crouch.height;
                    hasPlayedOnce = false;
                    statusSrc.isCrouched = true;
                    statusSrc.isPerformingCrouch = false;
                }

                if (statusSrc.isPerformingCrouch || statusSrc.isCrouched)
                {
                    if (crouch.crouchSlideOptions.canSlide && CheckIfCanSlide())
                    {
                        //Debug.Log("Sliding from crouch");
                        statusSrc.isSliding = true;
                        statusSrc.GetSoundPlayerSrc().PlayCrouchSlide();
                        StartCoroutine("ChangeMovementValues", crouch.crouchSlideOptions.slideTime); // Change the walk speed to prone values (maxForwardSpeed, maxSidewaysSpeed and maxBackwardsSpeed
                    }
                    else if (!statusSrc.isSliding)
                    {
                        //Debug.Log("NO Sliding from crouch");
                        StartCoroutine("ChangeMovementValues", 0);
                    }
                }
            }
        }
        else
        {
            // We are leaving the crouch position (going up to prone o normal stantard position).
            if ((crouch.isSustained && !InputManager.instance.crouchKey.isPressed) || (!crouch.isSustained && !isCrouchNotSustainedActive))
            {
                // When raising from crouch we need to know if prone its activate (the user is pressing that
                // button too). If not we just raise to normal height, if its been pressing, we raise to prone directly.
                // Prone is not active. Raise normally to the original height 
                if ((prone.isSustained && !InputManager.instance.proneKey.isPressed) || (!prone.isSustained && !isProneNotSustainedActive))
                {
                    if (statusSrc.isCrouched || statusSrc.isPerformingCrouch)
                    {
						if (controller.height < characterControllerOrig.height)
                        {
                            if (showDebug && showCrouchDebug) Debug.Log("Core -> DoCrouch() -> Going Up");
                            if (!hasPlayedOnce)
                            {
                                gameObject.BroadcastMessage("CrouchGoingDownUP"); // Play sound effect.
                                hasPlayedOnce = true;
                            }
                            gameObject.SendMessage("SetMidpoint", crouch.height + 0.1f);
                            statusSrc.isPerformingCrouch = true;
                            float dir = controller.height * 0.6f;
                            if (dir <= 0)
                                dir = 0.1f;

                            // Check if there is something over us, so we can stand in normal status.
							if (Physics.Raycast(transform.position, Vector3.up, out hit, dir))
                            {
                                if (showDebug && showCrouchDebug) Debug.Log("Core -> DoCrouch() -> Player hit something above when going up: " + hit.transform.name);
							}
                            else
                            {
                                controller.height = controller.height + (crouch.crouchSpeed * Time.deltaTime);
                                Vector3 myPos = transform.position;
								myPos.y += crouch.crouchSpeed * Time.deltaTime;
								transform.position = myPos;
                            }
                        }
                        else
                        {
							controller.height = characterControllerOrig.height;
                            gameObject.SendMessage("ResetMidpoint");
                            /*motor.movement.maxForwardSpeed = minFrwdWalkSpeed;
                            motor.movement.maxSidewaysSpeed = minSideWalkSpeed;
                            motor.movement.maxBackwardsSpeed = minBackWalkSpeed;*/
                            hasPlayedOnce = false;
                            statusSrc.isCrouched = false;
                            statusSrc.isPerformingCrouch = false;
                            StartCoroutine("ChangeMovementValues", 0);
                        }
                    }
                }
                else
                {
                    // Prone IS active. Raise raise only to prone height and update theStatus properly.
                    if ((prone.isSustained && InputManager.instance.proneKey.isPressed) || (!prone.isSustained && isProneNotSustainedActive))
                    {
                        if (statusSrc.isCrouched || (!statusSrc.isCrouched && statusSrc.isPerformingCrouch))
                        {
                            if (controller.height < prone.height)
                            {
                                if (showDebug && showCrouchDebug) Debug.Log("Core -> DoCrouch() -> We are crouched but changing to Prone directly");
                                if (!hasPlayedOnce)
                                {
                                    gameObject.BroadcastMessage("CrouchGoingDownUP");
                                    hasPlayedOnce = true;
                                }
                                gameObject.SendMessage("SetMidpoint", prone.height * 0.4f);
                                statusSrc.isPerformingCrouch = true;
                                float dir = controller.height * 0.6f;
                                if (dir <= 0)
                                    dir = 0.1f;

                                // Check if there is something over us, so we can stand in normal status.
                                if (Physics.Raycast(transform.position, Vector3.up, out hit, dir))
                                {
                                    if (showDebug && showCrouchDebug) Debug.Log("Core -> DoCrouch() -> Player hit something above when going up (to Prone position): " + hit.transform.name);
                                }
                                else
                                {
                                    controller.height = controller.height + (crouch.crouchSpeed * Time.deltaTime);
                                    Vector3 myPos = transform.position;
                                    myPos.y += crouch.crouchSpeed * Time.deltaTime;
                                    transform.position = myPos;
                                }
                            }
                            else
                            {
                                controller.height = prone.height;
                                SendMessage("SetMidpoint", prone.height * 0.4f);
                                /*motor.movement.maxForwardSpeed = prone.walkForwardSpeed;
                                motor.movement.maxSidewaysSpeed = prone.walkSidewaysSpeed;
                                motor.movement.maxBackwardsSpeed = prone.walkBackwardSpeed;*/
                                hasPlayedOnce = false;
                                statusSrc.isCrouched = false;
                                statusSrc.isPerformingCrouch = false;
                                statusSrc.isProned = true;
                                statusSrc.isPerformingProne = false;
                                StopCoroutine("ChangeMovementValues");
                                StartCoroutine("ChangeMovementValues", 0);
                            }
                        }
                    }
                }
            }
        }
    }

    private void DoProne()
    {
        if (statusSrc.isPerformingCrouch || statusSrc.isCrouched) { return; } // Security sentence.

        RaycastHit hit = default(RaycastHit);

        if ((prone.isSustained && InputManager.instance.proneKey.isPressed) || (!prone.isSustained && isProneNotSustainedActive))
        {
            if (statusSrc.isInLadder || statusSrc.isInLedge)
            { return; }

            if (!prone.canProneInAir && statusSrc.isInAir) return;

            gameObject.SendMessage("SetCameraMoveTime", prone.cameraMovTime);
            if (!statusSrc.isProned || statusSrc.isPerformingProne)
            {
                if (statusSrc.isPerformingCrouch) statusSrc.isPerformingCrouch = false;

                if (!hasPlayedOnce)
                {
                    gameObject.SendMessage("CancelAnyCameraMovement"); // Cancel any previous camerafall effect
                    gameObject.BroadcastMessage("ProneGoingDownUP");
                    gameObject.SendMessage("SetMidpoint", prone.height * 0.4f);
                    hasPlayedOnce = true;
                }

                if (controller.height > prone.height)
                {
                    if (showDebug && showProneDebug) Debug.Log("Core -> DoProne() -> Going down (to Prone position)");
                    controller.height = controller.height - (prone.proneSpeed * Time.deltaTime);
                    statusSrc.isPerformingProne = true;
                }
                else if (controller.height < prone.height)
                {
                    controller.height = prone.height;
                    hasPlayedOnce = false;
                    statusSrc.isProned = true;
                    statusSrc.isPerformingProne = false;
                }

                if (statusSrc.isPerformingProne || statusSrc.isProned)
                {
                    if (prone.proneSlideOptions.canSlide && CheckIfCanSlide())
                    {
                        //Debug.Log("Sliding from prone");
                        statusSrc.isSliding = true;
                        statusSrc.GetSoundPlayerSrc().PlayProneSlide();
                        StartCoroutine("ChangeMovementValues", prone.proneSlideOptions.slideTime); // Change the walk speed to prone values (maxForwardSpeed, maxSidewaysSpeed and maxBackwardsSpeed
                    }
                    else if (!statusSrc.isSliding)
                    {
                        //Debug.Log("NO Sliding from prone");
                        StartCoroutine("ChangeMovementValues", 0);
                    }
                }
            }
        }
        else // Going up from prone.
        {
            if ((prone.isSustained && !InputManager.instance.proneKey.isPressed) || (!prone.isSustained && !isProneNotSustainedActive))
            {
                if (((statusSrc.isProned && !statusSrc.isPerformingProne) || (statusSrc.isProned && statusSrc.isPerformingProne)) || (!statusSrc.isProned && statusSrc.isPerformingProne))
                {
					if (controller.height < characterControllerOrig.height)
                    {
                        if (showDebug && showProneDebug) Debug.Log("Core -> DoProne() -> Going Up");
                        if (!hasPlayedOnce)
                        {
                            gameObject.BroadcastMessage("ProneGoingDownUP");
                            hasPlayedOnce = true;
                            statusSrc.isPerformingProne = true;
                        }
                        // Check if there is something over us, so we can stand in normal status.
                        if (Physics.Raycast(transform.position, Vector3.up, out hit, controller.height * 0.6f))
                        {
                            if (showDebug && showProneDebug) Debug.Log("Core -> DoProne() -> Player hit something above when going up: " + hit.transform.name);    // DEBUG PURPOSES TO SEE WHAT WE ARE COLLIDING WITH.
                        }
                        else
                        {
                            controller.height = controller.height + (prone.proneSpeed * Time.deltaTime);
							Vector3 myPos = transform.position;
							myPos.y += prone.proneSpeed * Time.deltaTime;
							transform.position = myPos;
                        }
                    }
                    else
                    {
                        gameObject.SendMessage("ResetMidpoint");
						controller.height = characterControllerOrig.height;
                        /*motor.movement.maxForwardSpeed = minFrwdWalkSpeed;
                        motor.movement.maxSidewaysSpeed = minSideWalkSpeed;
                        motor.movement.maxBackwardsSpeed = minBackWalkSpeed;*/
                        hasPlayedOnce = false;
                        statusSrc.isProned = false;
                        statusSrc.isPerformingProne = false;
                        StopCoroutine("ChangeMovementValues");
                        StartCoroutine("ChangeMovementValues", 0);
                    }
                }
            }
        }
    }


    // Check if pl can do a slide
    private bool CheckIfCanSlide()
    {
        bool doSlideAction = false;
        if ((statusSrc.isPerformingCrouch || statusSrc.isCrouched) &&
            motor.movement.velocity.magnitude >= crouch.crouchSlideOptions.mimimunVelocityRequired && !statusSrc.isSliding && statusSrc.isInGround) doSlideAction = true;
        else if ((statusSrc.isPerformingProne || statusSrc.isProned) &&
            motor.movement.velocity.magnitude >= prone.proneSlideOptions.mimimunVelocityRequired && !statusSrc.isSliding && statusSrc.isInGround) doSlideAction = true;

        return doSlideAction;
    }

    // Set when we can sprint at a given Player status.
    private void DoSprint()
    {
        if (!sprint.canSprint) return;

        // Notice that you can sprint in water (Swimming) if you want.
        if ( (!statusSrc.isRunning && !statusSrc.isCrouched && !statusSrc.isProned && !statusSrc.isInLadder && !statusSrc.isInLedge && !statusSrc.isInLowLedge) &&
             ((sprint.isSustained && InputManager.instance.runKey.isPressed) || (!sprint.isSustained && isSprintNotSustainedActive)) )
        {
            // Normal sprint when Player is grounded. Notice it check if we have sprint stamina.
            // Sprint in the air...
            if (!statusSrc.isJumping || (statusSrc.isJumping && sprint.canSprintInAir))
                if (motor.movement.velocity.magnitude > 0.3f && sprintInternalTime > sprint.minSprintValue)
                    StartRunning (!sprint.sprintSlideOptions.canSlide);
        }
        else
        {
            if ((sprint.isSustained && InputManager.instance.runKey.isPressed) || (!sprint.isSustained && isSprintNotSustainedActive && statusSrc.isRunning)) // Security code to keep it running.
            {
                if (motor.movement.velocity.magnitude < 0.3f)
                    StopRunning();
            }
            else if ((sprint.isSustained && InputManager.instance.runKey.isUp) || (!sprint.isSustained && !isSprintNotSustainedActive && statusSrc.isRunning)) // Stop running if we release the 'run' key.
            { 
                StopRunning();
            }
        }

        // Stop running if we are already in the following cases. (With no velocity interpolation).
        if (statusSrc.isRunning && (statusSrc.isInLadder || statusSrc.isInLedge || statusSrc.isInLowLedge/* ||StatusSrc.isCrouched ||StatusSrc.isProned*/))
            StopRunning();

        // Sprint activation/deactivation. Change FOV if selected and inicialize sprint time (stamina).
        // Dont use FOV if Swimming, Diving or Flying
        if (statusSrc.isRunning)
        {
            if (sprint.cameraFovChange)
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, sprint.cameraFovValue, sprint.cameraFovSpeed * Time.deltaTime);
				if (statusSrc.GetDirtyLensPlane() != null)
					statusSrc.GetDirtyLensPlane().localPosition = Vector3.Lerp(statusSrc.GetDirtyLensPlane().localPosition, new Vector3(0, 0, sprint.dirtyLensFovCompensation), sprint.cameraFovSpeed * Time.deltaTime);
            }
            sprintInternalTime = sprintInternalTime - Time.deltaTime;
            if (sprintInternalTime <= 0)
            {
                StopRunning();
                sprintInternalTime = 0;
            }

            if (SprintScript)
                sprint.SprintGUI.value = sprintInternalTime;
        }
        else
        {
            //SprintScript.SetCurrentValue(sprintInternalTime);
             // FOV back to normal (original value)
            if (sprint.cameraFovChange)
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, CameraFovOrig, (sprint.cameraFovSpeed * Time.deltaTime) * 2);
				if (statusSrc.GetDirtyLensPlane() != null)
					statusSrc.GetDirtyLensPlane().localPosition = Vector3.Lerp(statusSrc.GetDirtyLensPlane().localPosition, new Vector3(0, 0, statusSrc.GetDirtyLensOrigPosition()), sprint.cameraFovSpeed * Time.deltaTime);
            }
            // Check if we are tired.
            if (sprintInternalTime < sprint.minSprintValue)
                gameObject.BroadcastMessage("PlaySprintBreath");

            // Sprint (stamina bar) recovery.
            if (sprintInternalTime < sprint.sprintTime)
                sprintInternalTime = sprintInternalTime + (sprint.recoveryRatio * Time.deltaTime);
            else
                sprintInternalTime = sprint.sprintTime;

            if (SprintScript != null)
                sprint.SprintGUI.value = sprintInternalTime;
        }
    }

    // Change walk velocity to sprint. Can do it directly or interpolating it in character motor.
    private void StartRunning(bool _defaultAccel)
    {
        if (_defaultAccel)
        {
            motor.movement.maxGroundAcceleration = motorGroundAccelOrig;
            motor.movement.maxAirAcceleration = motorAirAccelOrig;
        }
        else
        {
            motor.movement.maxGroundAcceleration = sprint.sprintSlideOptions.maxGroundAcceleration;
            motor.movement.maxAirAcceleration = sprint.sprintSlideOptions.maxAirAcceleration;
        }

        motor.movement.maxForwardSpeed = sprint.runForwardSpeed;
        motor.movement.maxSidewaysSpeed = sprint.runSidewaysSpeed;
        motor.movement.maxBackwardsSpeed = sprint.runBackwardSpeed;
        statusSrc.isRunning = true;
    }

    // Change the movement velocity. Can be delayed in time if we want to slide (when crouching or pronning).
    // This function is called from DoProne / Do Crouch
    private IEnumerator ChangeMovementValues(float _time)
    {
        //Debug.Log("Start Sliding (Time): "+_time);

        float frwWalkAux, backWalkAux, sideWalkAux = 0;

        yield return new WaitForSeconds(_time);

        if (statusSrc.isCrouched)
        {
            frwWalkAux = crouch.walkForwardSpeed;
            sideWalkAux = crouch.walkSidewaysSpeed;
            backWalkAux = crouch.walkBackwardSpeed;
        }
        else if (statusSrc.isProned)
        {
            frwWalkAux = prone.walkForwardSpeed;
            sideWalkAux = prone.walkSidewaysSpeed;
            backWalkAux = prone.walkBackwardSpeed;
        }
        else if(statusSrc.isRunning)
        {
            frwWalkAux = sprint.runForwardSpeed;
            sideWalkAux = sprint.runSidewaysSpeed;
            backWalkAux = sprint.runBackwardSpeed;
        }
        else //if (StatusSrc.isWalking)
        {
            frwWalkAux = minFrwdWalkSpeed;
            sideWalkAux = minSideWalkSpeed;
            backWalkAux = minBackWalkSpeed;
        }

        if (motor.movement.maxForwardSpeed != frwWalkAux) motor.movement.maxForwardSpeed = frwWalkAux;
        if (motor.movement.maxSidewaysSpeed != sideWalkAux) motor.movement.maxSidewaysSpeed = sideWalkAux;
        if (motor.movement.maxBackwardsSpeed != backWalkAux) motor.movement.maxBackwardsSpeed = backWalkAux;

        statusSrc.isSliding = false;
    }

    // Change walk velocity to normal walk speed. Can do it directly or interpolating it in character motor.
    // Called from Swim script too, to disable script if it's disabled to sprint in water in the inspector.
    // (That's why this function is public).
    public void StopRunning()
    {
        if (!statusSrc.isRunning) return; // Security Sentence

        StartCoroutine("ChangeMovementValues", 0);
        Invoke("ChangeAccelValues", sprint.sprintSlideOptions.slideTime);
        statusSrc.isRunning = false;
    }

    private void ChangeAccelValues()
    {
        if(motor.movement.maxGroundAcceleration != motorGroundAccelOrig) motor.movement.maxGroundAcceleration = motorGroundAccelOrig;
        if(motor.movement.maxAirAcceleration != motorAirAccelOrig) motor.movement.maxAirAcceleration = motorAirAccelOrig;
        if(statusSrc.isSliding) statusSrc.isSliding = false;
    }

    /*void OnExternalVelocity(){
		// Do Nothing
	}*/// Needed to avoid sendmessage error from charatermotor. NOT USED FOR NOW.

}