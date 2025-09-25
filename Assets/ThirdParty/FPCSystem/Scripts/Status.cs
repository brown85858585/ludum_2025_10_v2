
#pragma warning disable  0414   // Used because we have a 'floorStats' and 'isAirStats' variables just as information but it isn't being used really.


using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[System.Serializable]
public class KickOptions
{
    [Tooltip("Enable/Disable the kick feature.")]
    public bool canKick = true;
    [Tooltip("Tap interval needed to perform a kick (pressing jump button twice quickly).")]
    [Range(0.01f, 0.9f)] public float tapSpeed = 0.2f; // Tap speed used to configure the double jump detection.
    [Tooltip("Velocity will go in the opposite direction to actual movement.")]
    public float backwardsVelocity = 10f;
    [Tooltip("Velocity will go in upwards direction after kicking.")]
    public float upVelocity = 8f;
    [Tooltip("Minimum distance to the object/enemy to perform a kick.")]
    public float minDistance = 2f;
    [Tooltip("Minimum distance to the floor needed to perform a high kick.")]
    public float height = 3f;
    [Tooltip("Multiplies the velocity of in high kicks.")]
    [Range(0.1f, 5.0f)] public float heightVelMultiplier = 1.5f;
    [Tooltip("Enable/disable the push functionality using kicks.")]
    public bool canPush = true;
    [Tooltip("Force applied to an object when kicked.")]
    public Vector2 pushPower = new Vector2(10f, 2f);    // kick Force over the object being kicked (so it moves when kicked).
    [Space(10)]
    [Tooltip("Force applied to an enemy when it's kicked.")]
    public float pushPowerEnemy = 60f;
    [Tooltip("Damage applied to an enemy when it get kicked.")]
    public float kickDamage = 33f;
}

[System.Serializable]
public class SideKickOptions
{
    [Tooltip("Enable/Disable the side Kick.")]
    public bool canSideKick = true;
    [Tooltip("Velocity will go in the opposite direction (to the actual lateral movement).")]
    public float sideVelocity = 12f;
    [Tooltip("Velocity will go upwards after kicking.")]
    public float upVelocity = 8f;
    [Tooltip("Minimum distance to an object to perform a side kick.")]
    public float minDistance = 1f;
    [Tooltip("Enable/disable the push functionality using side kicks.")]
    public bool canPush = true;
    [Tooltip("Force applied to an object when side kicked.")]
    public Vector2 pushPower = new Vector2(10f, 3f);	// kick Force
    [Space(10)]
    [Tooltip("Force applied to an enemy when it's kicked.")]
    public float pushPowerEnemy = 40f;
    [Tooltip("Damage applied to an enemy when it get kicked.")]
    public float kickDamage = 10f;
}

[System.Serializable]
public class AboveKickOptions
{
    [Tooltip("Enable/Disable the Above Kick.")]
    public bool canAboveKick = false;
    [Tooltip("Velocity will continue going in the direction of movement.")]
    public float forwardVelocity = 18f;
    [Tooltip("Velocity will go in upwards direction after kicking.")]
    public float upVelocity = 12f;
    [Tooltip("Damage applied to the kicked enemy.")]
    public float kickDamage = 33f;
    [Tooltip("Minimum distance to an enemy to perform an above kick.")]
    public float minDistance = 2f;
    [Tooltip("Auto test time ratio to check if an above kick is being performing.")]
    public float checkRatio = 0.1f;
}

[System.Serializable]
public class PushOptions
{
    [Tooltip("Enable/Disable pushing objects.")]
    public bool canPush = true;
    [Tooltip("Use a key to push the object or do it just walking towards it.")]
    public bool useKey = false;
    [Tooltip("Force applied to the object when pushed.")]
    public Vector2 pushPower = new Vector2(8f, 2f);
}

[System.Serializable]
public class SlideOptions
{
    [Tooltip("Can slide at all? It's an aceleration slifing option used if Prone/Crouch when running.")]
    public bool canSlide = true;
    [Tooltip("pl's minimum velocity to start sliding. Default values: Prone: 7, Crouch: 7")]
    public float mimimunVelocityRequired = 7;
    //[Tooltip("Extra forward push when entering in slide mode. Default values: Prone: 0.2, Crouch: 0.3")]
    //public float initialSlidePush = 0.5f;
    //[Tooltip("Ground acceleration when sliding. Default values: Prone: 0.01, Crouch: 0.01")]
    //public float maxGroundAcceleration = 0.15f;
    //[Tooltip("Air acceleration when sliding. Default values: Prone: 0.1, Crouch: 0.1")]
    //public float maxAirAcceleration = 0.5f;
    [Tooltip("Time the slide status will be active. After time is finished the acceleration values will be changed to the usual default ones." +
             " Default values: Prone: 2, Crouch: 1")]
    public float slideTime = 1;
}

[System.Serializable]
public class ProneOptions
{
    [Tooltip("Enable/Disable prone functionality.")]
    public bool canProne = true;
    [Tooltip("Enable/Disable prone functionality when jumping")]
    public bool canProneInAir = true;
    [Tooltip("Prone key needs to be pressed all the time?.")]
    public bool isSustained = true; // The Prone key must be pressed all the time???
    [Tooltip("The height the charactercontroller will have when it's proned.")]
    [Range(0.01f, 4.0f)] public float height = 1f;
    [Tooltip("CharacterController's height velocity change.")]
    public float proneSpeed = 3f;
    [Tooltip("walk forward velocity when proned.")]
    public float walkForwardSpeed = 3f;
    [Tooltip("walk sideways velocity when proned.")]
    public float walkSidewaysSpeed = 2f;
    [Tooltip("walk backwards velocity when proned.")]
    public float walkBackwardSpeed = 2f;
    [Tooltip("Ground & Air CharacterMotor's acceleration values when proned. Used to slide when proning after running.")]
    public SlideOptions proneSlideOptions = new SlideOptions();
    [System.NonSerialized]
    public float cameraMovTime = 1f;
}

[System.Serializable]
public class CrouchOptions
{
    [Tooltip("Enable/Disable crouch functionality.")]
    public bool canCrouch = true;
    [Tooltip("Enable/Disable crouch functionality when jumping")]
    public bool canCrouchInAir = true;
    [Tooltip("Crouch key needs to be pressed always?.")]
    public bool isSustained = true; // The Crouch key must be pressed all the time???
    [Tooltip("The height the charactercontroller will have when it's crouched.")]
    [Range(0.01f, 4.0f)] public float height = 0.1f;
    [Tooltip("Change Velocity of the CharacterController's height.")]
    public float crouchSpeed = 10f;
    [Tooltip("walk forward velocity when crouched.")]
    public float walkForwardSpeed = 1f;
    [Tooltip("walk sideways velocity when crouched.")]
    public float walkSidewaysSpeed = 1f;
    [Tooltip("walk backwards velocity when crouched.")]
    public float walkBackwardSpeed = 1f;
    [Tooltip("Ground & Air CharacterMotor's acceleration values when crouched.")]
    public SlideOptions crouchSlideOptions = new SlideOptions();
    [System.NonSerialized]
    public float cameraMovTime = 1f;
}

// Aceleration option to use it in Sprint status.
[System.Serializable]
public class SlideOptions1
{
    [Tooltip("Can slide at all when running?")]
    public bool canSlide = true;
    [Tooltip("Ground acceleration when sprinting. Default: 40")]
    public float maxGroundAcceleration = 40;
    [Tooltip("Air acceleration when sprinting. Default: 5")]
    public float maxAirAcceleration = 5;
    [Tooltip("Time the Slide will be active. After time is finished the acceleration values will be changed to the usual default ones. Default: 0.5")]
    public float slideTime = 0.5f;
}

[System.Serializable]
public class SprintOptions
{
    [Tooltip("Enable/Disable sprint functionality.")]
    public bool canSprint = true;
    [Tooltip("Sprint key needs to be pressed always?.")]
    public bool isSustained = true; // The sprint key must be pressed all the time???
    [Tooltip("velocity when running forward.")]
    public float runForwardSpeed = 8f;
    [Tooltip("velocity when running sideways.")]
    public float runSidewaysSpeed = 7f;
    [Tooltip("velocity when running backward.")]
    public float runBackwardSpeed = 6f;
    [Tooltip("Ground & Air CharacterMotor's acceleration values when sprinting.")]
    public SlideOptions1 sprintSlideOptions = new SlideOptions1();
    [Tooltip("Time can stay running.")]
    public float sprintTime = 50;
    [Tooltip("Recovery ratio of sprint time.")]
    public float recoveryRatio = 1;
    [Tooltip("Minimum sprint time needed to start or keep running.")]
    public float minSprintValue = 2f;
    [Tooltip("Sprint bar GUITexture (sprint time) in  screen.")]
    public Slider SprintGUI;
    [Space(5)]
    [Tooltip("Time to subtract to sprint time when jumps.")]
    public float jumpSustractTime = 2f;
    [Tooltip("Minimum sprint time needed to be able to jump.")]
    public float minJumpValue = 2f;
    [Space(5)]
    [Tooltip("Can the pl sprint when it's in the air?")]
    public bool canSprintInAir = true;
    [Tooltip("Change the camera Fov when sprinting?")]
    public bool cameraFovChange = false;
    [Tooltip("Value of the camera Fov when sprinting.")]
    public float cameraFovValue = 68;
    [Tooltip("The changing speed of the camera Fov value.")]
    public float cameraFovSpeed = 2f;
    //var dirtyLensObject : GameObject;
    [Tooltip("Target position (Z axis) of the DirtyLens Plane, when Fov is active. It has to be done by trial & error when playing the scene. Depending on the amount of FOV, you'll need to move the Dirty Lens plane more or less towards the main camera.")]
    public float dirtyLensFovCompensation = 0.32f;
}

[System.Serializable]
public class WallRunOptions
{
    [Header("General Setting")]
    [Tooltip("Enable/Disable wallrun functionality.")]
    public bool canWallrun = true;
    [Tooltip("Layer Mask used to detect where can wallrun.")]
    public LayerMask wallMasks;
    [Tooltip("Jump first is needed to execute a wallrun?.")]
    public bool jumpFirst = false; // Need to jump First to a wall to be able to wall walk.
    [Tooltip("Minimum wall distance needed to execute wallrun.")]
    public float minWallDistance = 0.5f;
    [Tooltip("Detects if the player is looking to the wall when running. If not, cancel the wall run and fall.\n\n" +
       "If we are not looking at the wall when running, it means that maxLookAway goes to 1.0f, a parallel view is an 0.5f, and looking directly to the wall means a 0 value.\n\n" +
       "So Keeping this value betwen 0.7 - 1.0 force the player to look to the wall when running. If this value if reached when the player moves the camera, then the wallrun will be cancelled.")]
    public float maxLookAway = 0.8f;
    [Tooltip("Gravity multiplier used while we are wallrunning. ussually it would be a value lower than 1, so the gravity will be lower.")]
    public float gravityMult = 0.5f;
    [Tooltip("When grounded after a wallrun has done, this is the tie to wait until we can do another wallrun again.")]
    public float wallrunGroundedTime = 2f; // Time to wait to WallRun again once is grounded again.
    [Tooltip("Initial velocity added to when starting a wallrun.")]
    public Vector3 initialWallrunSpeed = new Vector3(15, 15, 15);
    [Tooltip("Velocity multiplier used to continue a fraction of movement when the wall ends (or when you reach the top). A 0.3f value will make it jumpy when exit walls that ends (reaching the roof could be crazy!)")]
    [Range(0.05f, 0.5f)] public float veloScapeMult = 0.1f;
    [Tooltip("Jump force applied when jumping while is wallrunning.")]
    public Vector2 jumpScapeSpeed = new Vector3(8, 12);

    [Header("Camera Settings")]
    [Tooltip("Time used to move the camera to the side of (regarding in which side is the wall).")]
    public float cameraMovTime = 1f;
    [Tooltip("Local camera's displacement point in X axis to make the bobbing effect in there.")]
    public float cameraLateralMov = 0.5f;
    [Tooltip("Camera rotation speed (in Z axis) when entering wall running.")]
    public float wallRunEnterRotationSpeed = 5;
    [Tooltip("Camera rotation speed (in Z axis) when exiting wall running.")]
    public float wallRunExitRotationSpeed = 10;
    [Tooltip("Fixed camera's rotation angle (in Z axis ) while wallrunning.")]
    public float cameraAngleRotation = 20;
}

//@Comment("Camera movement special effects","Configure the different kick options.", "Color.blue", 10)
//public var Comment5 : int;
[System.Serializable]
public class FallOptions : object
{
    [Tooltip("Do always some kind of landing camera FX everytime we touch the ground.")]
    public bool alwaysDoLandFX = true;
    [Tooltip("Can get damage when falling from high.")]
    public bool canGetDamage = true;
    /*[Tooltip("Minimum speed needed to activate some special camera & sound FX effects.")]
	public int fallMinimunSpeed = 1;
	[Tooltip("High speed falls activates some special camera & sound FX effects (different from 'normal' fall at minimum speed).")]
	public int fallHighSpeed = 10;
	[Tooltip("Minimum Damage speed (in the upwards coordinate) that will trigger some damage when landing.")]
	public int fallDamageSpeed = 18;
    [Tooltip("Amount of damage gets when landing depending on its velocity.")] 
    public float damagePercent = 0.25f; // Must be a value betwen 0-1. Zero is No damage, 1 is total damage.*/

    [Space(10)]
    [Tooltip("Minimum fall time needed to activate some special camera & sound FX effects. Depending on this minimal fall time all" +
             "fall damage and effects will be fired combining it with the damage curve.")]
    public float fallMinimunTime = 0.5f;
    [Tooltip("Amount of damage gets when landing depending on its fall time. In the curve in inspector think of each value is " +
             "multiplied by 10 or 100. So, 0.1 will be 1 second in X axis (x10) and 0.1 will be 10 damage (x100) in Y axis")]
    public AnimationCurve timeDamagePercent = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
}

[System.Serializable]
public class CameraFallOptions
{
    [Tooltip("We can disable this camera effects if we want to.")]
    public bool enableEffects = true;
    [Space(5)]
    [Tooltip("Time the camera will have to move (first down and later up) when falling a normal height")]
    public float NormalTime = 1;
    [Tooltip("The vertical position the camera will reach (usually going down).")]
    public float NormalPos = 0.6f;
    [Tooltip("Time the camera will have to rotate itself over its X axis.")]
    public float NormalRotTime = 1;
    [Tooltip("The amount of degrees the camera will rotate over its right vector.")]
    public int NormalRotAngle = 0;
    [Space(5)]
    [Tooltip("Time the camera will have to move up/down when falling a high height")]
    public float HighTime = 1.2f;
    [Tooltip("The vertical position the camera will reach (usually going down).")]
    public float HighPos = 0.4f;
    [Tooltip("Time the camera will have to rotate itself over its X axis.")]
    public float HighRotTime = 1.2f;
    [Tooltip("The amount of degrees the camera will rotate over its right vector.")]
    public int HighRotAngle = 9;
    [Space(5)]
    [Tooltip("Time the camera will have to move up/down when falling a high height and get damage")]
    public float DamageTime = 1.6f;
    [Tooltip("The vertical position the camera will reach (usually going down).")]
    public float DamagePos = 0.2f;
    [Tooltip("Time the camera will have to rotate itself over its X axis.")]
    public float DamageRotTime = 1.6f;
    [Tooltip("The amount of degrees the camera will rotate over its right vector.")]
    public int DamageRotAngle = 18;
}

[System.Serializable]
public class CameraJumpOptions
{
    [Tooltip("We can disable this effects if we want to.")]
    public bool enableEffects = true;
    [Tooltip("Time the camera will have to move (going up first and later downwards).")]
    public float JumpTime = 2;
    [Tooltip("The vertical position the camera will reach (usually going up first).")]
    public float JumpPos = 1.2f;
    [Tooltip("Time the camera will have to rotate itself over its X axis.")]
    public float JumpRotTime = 2;
    [Tooltip("The amount of degrees the camera will rotate over its right vector.")]
    public int JumpRotAngle = 2;
}

// To save the CharacterController values.
// So always knows the original height (when crouching) or slopeLimit (when using the stairs the slope has to be 90 always).
[System.Serializable]
public class CharacterOriginalData
{
    public float slopeLimit = 45f;
    public float stepOffset = 0.3f;
    public float skinWidth = 0.001f;
    public Vector3 center = Vector3.zero;
    public float radius = 0.5f;
    public float height = 2.0f;
}

[System.Serializable]
public class SensorOptions
{
    [Tooltip("Enable/Disable this sensor.")]
    public bool useSensorDetector = true;
    [Tooltip("Set to true to perform the Physics cast in the FixedUpdate function.")]
    public bool useSensorFixedUpdate = false;
    [Tooltip("Sensor detection distance.")]
    public float sensorDistance = 10f;
    [Tooltip("Layers to be detected by the sensor.")]
    public LayerMask sensorLayers;
    [Tooltip("If the sensor returns a hit, save its tag.")]
    [Disable]
    public string sensorHitTag = string.Empty;
    [Tooltip("If the sensor returns a hit, we get the distance where that objects is from us.")]
    [Disable]
    public float sensorHitDistance = 0f;
    [Tooltip("If the sensor returns a hit, get the hitted gameobject")]
    [Disable]
    public GameObject objectHit;
    [Tooltip("If the sensor returns a hit, get the hitted surface normal")]
    [Disable]
    public Vector3 hitNormal = Vector3.zero;
    [Tooltip("If the sensor returns a hit, get the hitted position in World Coordinates")]
    [Disable]
    public Vector3 hitPosition = Vector3.zero;
}


public enum FloorStats
{
    Flat = 0,
    DownHill = 1,
    UpHill = 2,
    Stairs = 3,
    None = 4
}

public enum InAirStats
{
    Floating = 0,
    GoingUp = 1,
    Falling = 2,
    None = 3
}

public enum Preset
{
    Normal = 0,
    Parkeur = 1,
    Minimal = 2
}

public class Status : MonoBehaviour
{
    [PlayerButton("OpenWindow")]  // ==  [SpriteDraw("hidden", "center")]
    public Sprite FpcSprite;

    public int comment1 = 1;

    [HelpBox("Status will keep track of status in runtime. Also initializes all the visual elements " +
             "that can be used at any time. Status is always present (it's NOT optional).", HelpBoxMessageType.Info)]

    [Header("Presets")]
    [SerializeField]
    public Preset _preset = Preset.Normal;
    [InspectorButton("ChangeToPreset")]
    public bool changeIt = false;

    [Space(10)]
    [Header("Health")]
    public int maxHealth = 100;
    public Slider healthGUI;
    public int health = 100;
    private int healthPrev = 100;

    [Header("GUI & visual effects")]
    [Tooltip("GUI Damage is used when get hurts when falling or it's getting any kind of damage.")]
    public GameObject damageGUI;
    [Tooltip("How much time to wait before the Damage effect dissapear?.")]
    [Range(0.1f, 5.0f)] public float damageFadeTime = 1f;

    public bool isDirtyLens = true;
    [SerializeField]
    [Disable]
    private Transform dirtyLensPlane;
    private DirtyLensEffect dirtyLensSrc;
    private AnimatedRenderer dirtyLensAnimSrc;
    private float dirtyLensOrigPosition = 0.3f;     // Original local position of the DirtyLens Plane.

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;

    [Header("Runtime Stats")]
    [SerializeField]
    [Disable]
    private FloorStats floorStats = FloorStats.Flat;
    [SerializeField]
    [Disable]
    private float floorDistance = 0;

    [Space(5)]
    [SerializeField]
    [Disable]
    private InAirStats inAirStats = InAirStats.None;
    [SerializeField]
    [Disable]
    private float inAirTime = 0f;
    [SerializeField]
    [Disable]
    private float fallTime = 0f;


    // Player possible status.
    // Movement Status.
    [HideInInspector] public bool isStop; // When player is NOT moving at all.
    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isSliding;

    // Position Status.
    [HideInInspector] public bool isInGround;
    [HideInInspector] public bool isInAir;
    [HideInInspector] public bool isInLadder;
    [HideInInspector] public bool isInLedge;
    [HideInInspector] public bool isInLowLedge;

    // Action Status
    [HideInInspector] public bool isDoingNothing; // When all the other action vars are false, this one will be turn true.
    [HideInInspector] public bool isCrouched;
    [HideInInspector] public bool isPerformingCrouch;
    [HideInInspector] public bool isProned;
    [HideInInspector] public bool isPerformingProne;
    [HideInInspector] public bool isClimbing; // Using a ladder or a ledge
    [HideInInspector] public bool isLowClimbing; // Using a lowledge.
    [HideInInspector] public bool isBeingPushed;

    // Jump is a little more complicated because it has to detect kicks or side kicks.
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isJumpingFromLadder;
    [HideInInspector] public bool isJumpingFromLedge;
    [HideInInspector] public bool isSimpleKicking;
    [HideInInspector] public bool isSideKicking;
    [HideInInspector] public bool isJumpDetected; // Has the player press Jump key just once time?
    [HideInInspector] public bool isSimpleKickDetected; // Has the player press Jump key twice?
    [HideInInspector] public bool isSideKickDetected; // Has the player press Jump key and 'horizontal' key?
    [HideInInspector] public bool isWallRunning;
    [HideInInspector] public bool isWallRunDetected; // Has the player press Jump key, 'horizontal' and 'vertical' key?
    [HideInInspector] public bool isWallRunEndNow;  // Has the wallrun ended just a moment ago? (used to detect ledges in the walls).

    [HideInInspector] public bool isOverPlatform;     // Player interaction with scene platforms. Platforms has to have a Platform tag.
    [HideInInspector] public bool isJumpReleased = true;




    // Catch all posible Player Scripts (Excluding Player GUIs on screen)
    private MouseLook mouseLookScr;
    private CharacterController controller;
    private CharacterMotor motor;
    private Core coreScr;
    private CameraBobber cameraBobberScr;
    private Kick kickScr;
    private Ledge ledgeScr;
    private LedgeLow ledgeLowScr;
    private WallParkour wallParkourScr;
    private FlashLight flashLightScr;
    private CatchObject catchObjectScr;
    private Crosshair crosshairScr;
    private SoundPlayer soundPlayerSrc;

    private CatchStatusGUI myCatchStatusGUI;
    


    //==================================================================================================
    //
    // Public function to get diferent values from Status or Core
    //
    //==================================================================================================

    public MouseLook GetMouseLookSrc() { return mouseLookScr; }
    public CharacterController GetController() { return controller; }
    public CharacterMotor GetMotor() { return motor; }
    public Core GetCoreScr() { return coreScr; }
    public CameraBobber GetCameraBobberScr() { return cameraBobberScr; }
    public Kick GetKickScr() { return kickScr; }
    public Ledge GetFPCLedgeScr() { return ledgeScr; }
    public LedgeLow GetFPCLedgeLowScr() { return ledgeLowScr; }
    public WallParkour GetWallParkourScr() { return wallParkourScr; }
    public FlashLight GetFlashLightScr() { return flashLightScr; }
    public CatchObject GetCatchObjectScr() { return catchObjectScr; }
    public Crosshair GetCrosshairScr() { return crosshairScr; }

    public SoundPlayer GetSoundPlayerSrc() { return soundPlayerSrc; }
    public void SetSoundPlayerSrc(SoundPlayer _sndPlayer) { soundPlayerSrc = _sndPlayer; }

    public Transform GetDirtyLensPlane() { return dirtyLensPlane; /*isDirtyLens ? dirtyLensPlane : null;*/ }
    public DirtyLensEffect GetDirtyLensSrc() { return dirtyLensSrc; /*isDirtyLens ? dirtyLensSrc : null;*/ }
    public AnimatedRenderer GetDirtyLensAnimSrc() { return dirtyLensAnimSrc; /*isDirtyLens ? dirtyLensAnimSrc : null;*/ }
    public float GetDirtyLensOrigPosition() { return dirtyLensOrigPosition; }

    public void SetHealth(int _health) { health = _health; CheckIfIsDeath(); }
    public void AddHealth(int _moreHealth) { health += _moreHealth; CheckIfIsDeath(); }
    public void SetDamage(int _damage) { health -= _damage; CheckIfIsDeath(); }

    public float GetFloorDistance() { return floorDistance; }

    // If the Player has death, send a Death message to the DeathManager using or new Message system.
    private void CheckIfIsDeath()
    {
        if (health <= 0)
        {
            EventManagerv2.instance.TriggerEvent("Death");
        }
    }

    public float GetFallTime() { return fallTime; }


    //==================================================================================================
    //
    // Math functions to compare Vector3 Approximately.
    //
    //==================================================================================================
    public bool IsEqual(Vector3 a, Vector3 b, float _range = 0.01f)
    {
        return IsEqual(a.x, b.x, _range) && IsEqual(a.y, b.y, _range) && IsEqual(a.z, b.z, _range);
    }

    public bool IsEqual(float a, float b, float _range = 0.01f)
    {
        return (a >= b - _range && a <= b + _range);
    }



    //==================================================================================================
    //
    // Player Preset Changes.
    //
    //==================================================================================================
    public void ChangeToPreset()
    {
#if UNITY_EDITOR
        // Make sure we have all component references
        if (mouseLookScr == null)
            GetFpcComponents();

        // Enabled always all basic components (they must be always present)
        mouseLookScr.enabled = true;
        motor.enabled = true;
        this.enabled = true;    // this is StatusSrc
        coreScr.enabled = true;
        cameraBobberScr.enabled = true;

        // 
        switch (_preset)
        {
            case Preset.Normal:
                ChangeToNormal();
                break;
            case Preset.Parkeur:
                ChangeToParkeur();
                break;
            case Preset.Minimal:
                ChangeToMinimal();
                break;
        }
#endif
    }

    public void ChangeToNormal()
    {
#if UNITY_EDITOR
        StoreMotorFloatValue(motor, "movement.maxForwardSpeed", 4);
        StoreMotorFloatValue(motor, "movement.maxSidewaysSpeed", 3);
        StoreMotorFloatValue(motor, "movement.maxBackwardsSpeed", 2);
        StoreMotorFloatValue(motor, "movement.maxGroundAcceleration", 40);
        StoreMotorFloatValue(motor, "movement.maxAirAcceleration", 40);

        StoreMotorBoolValue(motor, "jumping.doubleJump", false);
        StoreMotorIntValue(motor, "jumping.numberJumps", 1);

        StoreCoreFloatValue(coreScr, "sprint.runForwardSpeed", 8);
        StoreCoreFloatValue(coreScr, "sprint.runSidewaysSpeed", 7);
        StoreCoreFloatValue(coreScr, "sprint.runBackwardSpeed", 6);

        EnableOptionalComponents(true);
#endif
    }

    public void ChangeToParkeur()
    {
#if UNITY_EDITOR

        StoreMotorFloatValue(motor, "movement.maxForwardSpeed", 4);
        StoreMotorFloatValue(motor, "movement.maxSidewaysSpeed", 4);
        StoreMotorFloatValue(motor, "movement.maxBackwardsSpeed", 4);
        StoreMotorFloatValue(motor, "movement.maxGroundAcceleration", 40);
        StoreMotorFloatValue(motor, "movement.maxAirAcceleration", 40);

        StoreMotorBoolValue(motor, "jumping.doubleJump", true);
        StoreMotorIntValue(motor, "jumping.numberJumps", 2);

        StoreCoreFloatValue(coreScr, "sprint.runForwardSpeed", 8);
        StoreCoreFloatValue(coreScr, "sprint.runSidewaysSpeed", 8);
        StoreCoreFloatValue(coreScr, "sprint.runBackwardSpeed", 8);

        EnableOptionalComponents(true);
#endif
    }

    public void ChangeToMinimal()
    {
#if UNITY_EDITOR
        StoreMotorFloatValue(motor, "movement.maxForwardSpeed", 5);
        StoreMotorFloatValue(motor, "movement.maxSidewaysSpeed", 4);
        StoreMotorFloatValue(motor, "movement.maxBackwardsSpeed", 3);
        StoreMotorFloatValue(motor, "movement.maxGroundAcceleration", 20);
        StoreMotorFloatValue(motor, "movement.maxAirAcceleration", 80);

        StoreMotorBoolValue(motor, "jumping.doubleJump", false);
        StoreMotorIntValue(motor, "jumping.numberJumps", 1);

        StoreCoreFloatValue(coreScr, "sprint.runForwardSpeed", 9);
        StoreCoreFloatValue(coreScr, "sprint.runSidewaysSpeed", 7);
        StoreCoreFloatValue(coreScr, "sprint.runBackwardSpeed", 5);

        EnableOptionalComponents(false);
#endif
    }

    //==================================================================================================
    //
    // Fanncy way to change from inspector all pl's values making all changes persistent.
    // If not doing in this way, you lose all those changes when hit Play in Unitym because there 
    // isn't an ApplyModifiedProperties() funtion.
    //
    //==================================================================================================
    private static void StoreMotorFloatValue(CharacterMotor _motor, string _propertyName, float _value)
    {
#if UNITY_EDITOR
        UnityEditor.SerializedObject obj = new UnityEditor.SerializedObject(_motor);
        obj.FindProperty(_propertyName).floatValue = _value;
        obj.ApplyModifiedPropertiesWithoutUndo();
#endif
    }

    private static void StoreMotorIntValue(CharacterMotor _motor, string _propertyName, int _value)
    {
#if UNITY_EDITOR
        UnityEditor.SerializedObject obj = new UnityEditor.SerializedObject(_motor);
        obj.FindProperty(_propertyName).intValue = _value;
        obj.ApplyModifiedPropertiesWithoutUndo();
#endif
    }

    private static void StoreMotorBoolValue(CharacterMotor _motor, string _propertyName, bool _value)
    {
#if UNITY_EDITOR
        UnityEditor.SerializedObject obj = new UnityEditor.SerializedObject(_motor);
        obj.FindProperty(_propertyName).boolValue = _value;
        obj.ApplyModifiedPropertiesWithoutUndo();
#endif
    }

    private static void StoreCoreFloatValue(Core _core, string _propertyName, float _value)
    {
#if UNITY_EDITOR
        UnityEditor.SerializedObject obj = new UnityEditor.SerializedObject(_core);
        obj.FindProperty(_propertyName).floatValue = _value;
        obj.ApplyModifiedPropertiesWithoutUndo();
#endif
    }




    public void EnableOptionalComponents(bool _enabled)
    {
        if (kickScr != null)
        {
            kickScr.enabled = _enabled;
        }
        if (ledgeScr != null)
        {
            ledgeScr.enabled = _enabled;
        }
        if (ledgeLowScr != null)
        {
            ledgeLowScr.enabled = _enabled;
        }
        if (wallParkourScr != null)
        {
            wallParkourScr.enabled = _enabled;
        }
        if (flashLightScr != null)
        {
            flashLightScr.enabled = _enabled;
        }
        if (catchObjectScr != null)
        {
            catchObjectScr.enabled = _enabled;
        }
        if (crosshairScr != null)
        {
            crosshairScr.enabled = _enabled;
        }
    }



    //==================================================================================================
    //
    // Initialization
    //
    //==================================================================================================

    private void GetFpcComponents()
    {
        mouseLookScr = GetComponent<MouseLook>();
        controller = GetComponent<CharacterController>();
        motor = GetComponent<CharacterMotor>();
        coreScr = GetComponent<Core>();
        cameraBobberScr = GetComponent<CameraBobber>();
        kickScr = GetComponent<Kick>();
        ledgeScr = GetComponent<Ledge>();
        ledgeLowScr = GetComponent<LedgeLow>();
        wallParkourScr = GetComponent<WallParkour>();
        flashLightScr = GetComponent<FlashLight>();
        catchObjectScr = GetComponent<CatchObject>();
        crosshairScr = GetComponent<Crosshair>();
        //SoundPlayerSrc 	= GetComponentInChildren<SoundPlayer>();	// Don't. The soundPlayer will do this itself
    }

    private void GetFpcGUIComponents()
    {
        myCatchStatusGUI = GetComponent<CatchStatusGUI>();
    }


    // Player Status keyboard detector, to show Status GUI. Pressing F6
    private void CheckStatusGUI()
    {
        // myCatchStatusGUI
        if (myCatchStatusGUI != null)
        {
            if (InputManager.instance.f6Key.isDown)
            {
                myCatchStatusGUI.showGUIStatus = !myCatchStatusGUI.showGUIStatus;
            }
            myCatchStatusGUI.enabled = myCatchStatusGUI.showGUIStatus;
        }
    }

    void Awake()
    {
        GetFpcComponents();
        GetFpcGUIComponents();
    }

    void Start()
    {
        healthPrev = health;
        if (healthGUI != null)
        {
            healthGUI.maxValue = maxHealth;
            healthGUI.value = health;
        }
        else
        {
            Debug.LogWarning("Status -> HealthGUI has not been assigned in the Status.");
        }

        if (damageGUI == null)
            Debug.LogWarning("Status -> DamageGUI has not been assigned in the Status.");
        else if(damageGUI.activeInHierarchy)
            damageGUI.SendMessage("SetDamageFadeTime", damageFadeTime, SendMessageOptions.DontRequireReceiver);
    }


    //==================================================================================================
    //
    // Update Health
    //
    //==================================================================================================
    void Update()
    {
        CheckStatusGUI();
        CheckFpcStatusLogic();

        // Check & Update the Health GUI & Danage FX.
        if (healthPrev != health)
            UpdateHealth();

        // Check how much time we are in Air (when jumping. Doesn't keep track of it if we are flying around)
        UpdateInAirStats();

        // Check all floor stats
        UpdateFloorStats();
    }


    private void UpdateInAirStats()
    {
        if (isInAir && !isInLedge && !isWallRunning)
        {
            inAirTime += Time.deltaTime;

            if (IsEqual(motor.movement.velocity.y, 0f, 0.5f))
            {
                inAirStats = InAirStats.Floating;
            }
            else if (motor.movement.velocity.y > 0)
            {
                inAirStats = InAirStats.GoingUp;
            }
            else if (motor.movement.velocity.y < 0)
            {
                inAirStats = InAirStats.Falling;
                fallTime += Time.deltaTime;
            }
        }
        else if (inAirTime > 0)
        {
            if (showDebug) Debug.Log("Status -> FallTime: " + fallTime);
            inAirTime = 0;
            StartCoroutine("_ResetFallTimeTimed");
            inAirStats = InAirStats.None;
        }
    }

    // Reset the Falltime after waiting a tiny period of time, so we can get this fall time value and see if we took some damage when landing.
    IEnumerator _ResetFallTimeTimed()
    {
        yield return new WaitForSeconds(0.1f);
        fallTime = 0;
    }

    private void UpdateFloorStats()
    {
        if (coreScr.belowSensorOptions.sensorHitTag.Contains("Stairs")) { floorStats = FloorStats.Stairs; floorDistance = coreScr.belowSensorOptions.sensorHitDistance; }
        else if (coreScr.belowSensorOptions.sensorHitTag == string.Empty) { floorStats = FloorStats.None; floorDistance = 0f; }
        else
        {
            floorDistance = coreScr.belowSensorOptions.sensorHitDistance;
            Vector3 floorNormal = coreScr.belowSensorOptions.hitNormal;
            if (IsEqual(floorNormal, Vector3.up)) { floorStats = FloorStats.Flat; }
            else
            {
                Vector3 dirFwrd = transform.TransformDirection(Vector3.forward);    // Direccion del Player.
                Vector3 perp = Vector3.Cross(floorNormal, Vector3.up);  // Perp direction to both vectors(floor & dir UP)
                Vector3 dirWalk = Vector3.Cross(dirFwrd.normalized, perp.normalized);   //Perp direction to the Player vector & the floor Perp.
                                                                                        // The resulting vector returns a value that depends on if we are looking to a hill (UpHill) or backwards to it. 

                if (showDebug) Debug.Log("Status -> UpdateFloorStats() -> Floor Normal : " + floorNormal + " perp: " + perp + " dirWalk: " + dirWalk);

                if (!IsEqual(dirWalk.y, 0f, 0.05f)) floorStats = (dirWalk.y > 0) ? FloorStats.UpHill : FloorStats.DownHill;
                else floorStats = FloorStats.Flat;
            }
        }
    }


    // Health value has changed. Update health bar if exists & show GUI Damage if health decreases.
    private void UpdateHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth); // Check health is not below zero or reaches the maximum value.

        if (healthGUI)  // Update the health bar in screen only if exits.
            healthGUI.value = health;

        // Show the GUI Damage effect (only if exist).
        if (healthPrev > health && damageGUI != null)
        {  
            if (damageGUI.activeInHierarchy) damageGUI.SendMessage("ShowGUIDamage");
        }

        healthPrev = health;
    }


    private void CheckFpcStatusLogic()
    {
        // Check the position status logic of pl. 
        // The goal is to avoid duplicate position status.
        if (!motor.IsGrounded())
        {
            isInGround = false;
            isInAir = true;
        }
        else
        {
            isInGround = true;
            isInAir = false;
        }

        if (isInLadder)
            isInAir = false;

        if (!isInAir)
            //isInWater = false; // now we can be in the water without swimming neither diving.
            isInGround = true;

        // If we are wall Running and we detect an Object below, stop the wall run because we are grounded.
        if (isInAir && isWallRunning && isRunning && coreScr.GetObjectBelow() != null)
            isInGround = true;

        // Update Jumping State. If Jump, you can be standing, walking or running in the jump
        // Just to know the inicial state before jump executes.
        if (Time.time > coreScr.testJumpStateTime)
        {
            if ((motor.IsGrounded() && !motor.IsJumping()) && isJumping)
                isJumping = false;

            if (motor.IsGrounded() && isSimpleKicking)
            {
                isSimpleKicking = false;
                isSimpleKickDetected = false; // avoid making a double jump in the air or just when landing.
            }

            if ((motor.IsGrounded() && !motor.IsJumping()) && isSideKicking)
                isSideKicking = false;
        }

        if (!isSliding)
            isSliding = motor.IsSliding();

        // Not allow walk or stand if we are running (obviously).
        if (isRunning)
        {
            isStop = false;
            isWalking = false;
        }

        // Is the Player walking or standing.
        // Note : Climb ladders o ledges will update themselves these two states.
        // Note : Climb will not update itself these two states if we are swiming./**/
        if (!isRunning && !isInLadder && !isInLedge)
        {
            // if moving at 0.3 is so low that we think thats is standing and not walking. its a micro-move!!!
            if (motor.movement.velocity.magnitude < 0.9f)
            {
                isStop = true;
                isWalking = false;
                isBeingPushed = false;
            }
            else
            {
                isStop = false;
                isWalking = true;
            }
        }

        // Take into account if no action is being performed. This is just to show it in the Status GUI.
        if (isJumping || isSimpleKicking || isSideKicking || isCrouched || isProned ||
            isClimbing || isLowClimbing || isPerformingCrouch ||
            isPerformingProne || isWallRunning)
            isDoingNothing = false;
        else
            isDoingNothing = true;
    }


    private void FindDirtyLens()
    {
        dirtyLensSrc = GetComponentInChildren<DirtyLensEffect>();
        if (dirtyLensSrc == null)
        {
            Debug.LogWarning("Status -> I can't find the DirtyLens Plane or the DirtyLensEffectJS script is missing.");
        }
        else
        {
            dirtyLensPlane = dirtyLensSrc.transform;
            dirtyLensAnimSrc = dirtyLensPlane.GetComponent<AnimatedRenderer>();
            dirtyLensSrc.isDirtyLens = isDirtyLens;// ? true : false;
                                                   //dirtyLensSrc.ActivateDirtyLens();
            dirtyLensOrigPosition = dirtyLensPlane.localPosition.z;
        }
    }


    private void FindUnderWater()
    {
    }

}