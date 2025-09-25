
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CharacterMotorMovement : object
{
    [Tooltip("The maximum horizontal forwars speed when moving")]
    public float maxForwardSpeed = 5f; //10f;
    [Tooltip("The maximum horizontal sideways forwars speed when moving")]
    public float maxSidewaysSpeed = 5f; //10f;
    [Tooltip("The maximum horizontal backwars forwars speed when moving")]
    public float maxBackwardsSpeed = 5f; //10f;

    [Tooltip("Curve for multiplying speed based on slope (negative = downwards)")]
    public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe[] { new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0) });

    [Tooltip("How fast does the character change speeds on the ground?  Higher is faster")]
    public float maxGroundAcceleration = 20f; //30f;
    [Tooltip("How fast does the character change speeds in Air?  Higher is faster")]
    public float maxAirAcceleration = 8f; //20f;

    [Tooltip("The gravity for the character")]
    public float gravity = 20f; //10f;
    [Tooltip("How fast does the character can fall (maximum fall speed)?")]
    public float maxFallSpeed = 60f; //20f;

    // For the next variables, System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!
    // The last collision flags returned from controller.Move
    [System.NonSerialized]
    public CollisionFlags collisionFlags;

    // We will keep track of the character's current velocity,
    [System.NonSerialized]
    public Vector3 velocity;

    // This keeps track of our current velocity while we're not grounded
    [System.NonSerialized]
    public Vector3 frameVelocity = Vector3.zero;

    [System.NonSerialized]
    public Vector3 hitPoint = Vector3.zero;

    [System.NonSerialized]
    public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
}

public enum MovementTransferOnJump
{
    None = 0, // The jump is not affected by velocity of floor at all.
    InitTransfer = 1, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
    PermaTransfer = 2, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
    PermaLocked = 3 // Jump is relative to the movement of the last touched floor and will move together with that floor.
}

// We will contain all the jumping related variables in one helper class for clarity.
[System.Serializable]
public class CharacterMotorJumping : object
{
    [Tooltip("Can the character jump at all?")]
    public bool canJump = true;

    [ShowWhen("jumping.canJump")]
    [Tooltip("Can the character jump right now (or it is temporally disabled (when getting tired, for example)?")]
    public bool enabled = true;

    [ShowWhen("jumping.canJump")]
    [Tooltip("How high do we jump when pressing jump and letting go immediately")]
    public float baseHeight = 1f;

    [ShowWhen("jumping.canJump")]
    [Tooltip("We add extraHeight units (meters) on top when holding the button down longer while jumping")]
    public float extraHeight = 2f; //4.1f;

    [ShowWhen("jumping.canJump")]
    [Tooltip("How much does the character jump out perpendicular to the surface on walkable surfaces?. 0 means a fully vertical jump and 1 means fully perpendicular.")]
    public float perpAmount = 1;

    [ShowWhen("jumping.canJump")]
    [Tooltip("How much does the character jump out perpendicular to the surface on too steep surfaces?. 0 means a fully vertical jump and 1 means fully perpendicular.")]
    public float steepPerpAmount = 0.5f;

    [ShowWhen("jumping.canJump")]
    [Tooltip("Can the character make double jumps?")]
    public bool doubleJump = false;

    [ShowWhen("jumping.canJump")]
    [Tooltip("How many Jumps are allowed?")]
    public int numberJumps = 2;

    [ShowWhen("jumping.canJump")]
    [Tooltip("How high do we jump when pressing jump again and letting go immediately (in double jumps)")]
    public float doubleJumpBaseHeight = 1f;

    [ShowWhen("jumping.canJump")]
    [Tooltip("Time before pressing jump key again will be detected as double jump. Pressing after that time window means ignoring it.")]
    public float timeWindow = 1f;

    // For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!
    // Are we jumping? (Initiated with jump button and not grounded yet)
    // To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
    [System.NonSerialized]
    public bool jumping;

    [System.NonSerialized]
    public bool holdingJumpButton;

    // the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
    [System.NonSerialized]
    public float lastStartTime;

    [System.NonSerialized]
    public float lastButtonDownTime = -100;

    [System.NonSerialized]
    public Vector3 jumpDir = Vector3.up;

    [System.NonSerialized]
    public int jumpCount;

    [System.NonSerialized]
    public float internalTimeWindow;
}

[System.Serializable]
public class CharacterMotorMovingPlatform : object
{
    [Tooltip("Can the character stand in a moving platform?")]
    public bool enabled = true;

    [ShowWhen("movingPlatform.enabled")]
    [Tooltip("What kind of movement will have the character when standing over a moving platform?\n" +
        "None = 0, // The jump is not affected by velocity of floor at all.\n" +
        "InitTransfer = 1, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.\n" +
        "PermaTransfer = 2, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.\n" +
        "PermaLocked = 3 // Jump is relative to the movement of the last touched floor and will move together with that floor.")]
    public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;

    [System.NonSerialized]
    public Transform hitPlatform;

    [System.NonSerialized]
    public Transform activePlatform;

    [System.NonSerialized]
    public Vector3 activeLocalPoint;

    [System.NonSerialized]
    public Vector3 activeGlobalPoint;

    [System.NonSerialized]
    public Quaternion activeLocalRotation;

    [System.NonSerialized]
    public Quaternion activeGlobalRotation;

    [System.NonSerialized]
    public Matrix4x4 lastMatrix;

    [System.NonSerialized]
    public Vector3 platformVelocity;

    [System.NonSerialized]
    public bool newPlatform;
}




[System.Serializable]
public class CharacterMotorSliding : object
{
    [Tooltip("Does the character slide on too steep surfaces?")]
    public bool enabled = true;

    [ShowWhen("sliding.enabled")]
    [Tooltip("How fast does the character slide on steep surfaces?")]
    public float slidingSpeed = 10f; //15f;

    [ShowWhen("sliding.enabled")]
    [Tooltip("How much can the player control the sliding direction?. If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.")]
    public float sidewaysControl = 1f;

    [ShowWhen("sliding.enabled")]
    [Tooltip("How much can the player influence the sliding speed?. If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.")]
    public float speedControl = 0.4f;
}

[System.Serializable]
public class CharacterMotorProperCollisions : object
{
    [Tooltip("Does the character perform manually moving collision detections? The CharacterController doesn't detect properly moving objects collisions so, " +
        "we need this extra collision detection feature. Use this if you have moving/rotating objects in your scene.")]
    public bool enabled = false;

    [ShowWhen("advancedCollisions.enabled")]
    public bool useFixedUpdate = false;

    [ShowWhen("advancedCollisions.enabled")]
    [Tooltip("Moving platform Layers to test collisions against.")]
    public LayerMask movingPlatformsLayers;

    [ShowWhen("advancedCollisions.enabled")]
    [Tooltip("Raycast number to check colliders around the Character. Higher is more accurate & expensive.")]
    public int raycastNumber = 10;

    [Space(10)]
    [Tooltip("Check the players feet and push them up if something clips through their feet. (Useful for vertical moving platforms).")]
    public bool testCharacterFeet = false;

    [ShowWhen("advancedCollisions.testCharacterFeet")]
    public bool useFeetFixedUpdate = false;
}


[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Character/Character Motor")]
public class CharacterMotor : MonoBehaviour
{
    [Tooltip("Does the character currently respond to input?")]
    public bool canControl = true;

    [Tooltip("Use the FixedUpdate (more precise but more expensive too) to perform the character internal movement calculations?")]
    public bool useFixedUpdate = true;

    // For the next variables, System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!

    [System.NonSerialized]
    public Vector3 inputMoveDirection = Vector3.zero;   // The current global direction we want the character to move in.

    // Is the jump button held down? We use this interface instead of checking
    // for the jump button directly so this script can also be used by AIs.
    [System.NonSerialized]
    public bool inputJump;

    public CharacterMotorMovement movement = new CharacterMotorMovement();
    public CharacterMotorJumping jumping = new CharacterMotorJumping();
    public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();
    public CharacterMotorSliding sliding = new CharacterMotorSliding();
    public CharacterMotorProperCollisions advancedCollisions = new CharacterMotorProperCollisions();

    [System.NonSerialized]
    public bool grounded = true;

    [System.NonSerialized]
    public Vector3 groundNormal = Vector3.zero;

    [System.NonSerialized]
    public Vector3 wallNormal = Vector3.zero;

    private Vector3 lastGroundNormal = Vector3.zero;
    private Transform tr;
    private CharacterController controller;


    [System.NonSerialized]
    public bool stairsDetected = false; // To check if encounter stairs done by the sensors (we change the movement behavior in that case to be able to walk over the stairs).


    private float distanceProperCollisions = -1f;

    private Status statusSrc;
    private Core coreScr;


    private void Awake()
    {
        tr = transform;
        controller = GetComponent<CharacterController>();
        distanceProperCollisions = controller.radius + 0.2f;
    }

    private void Start()
    {
        statusSrc = GetComponent<Status>();
        coreScr = statusSrc.GetCoreScr();
    }

    private void UpdateFunction()
    {
        // We copy the actual velocity into a temporary variable that we can manipulate.
        Vector3 velocity = movement.velocity;

        // Update velocity based on input
        velocity = ApplyInputVelocityChange(velocity);

        // Apply gravity and jumping force
        velocity = ApplyGravityAndJumping(velocity);

        // Moving platform support
        Vector3 moveDistance = Vector3.zero;
        if (MoveWithPlatform())
        {
            Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
            moveDistance = newGlobalPoint - movingPlatform.activeGlobalPoint;
            if (moveDistance != Vector3.zero)
                ControllerMove(moveDistance);

            // Support moving platform rotation as well:
            Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
            Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);

            float yRotation = rotationDiff.eulerAngles.y;
            if (yRotation != 0)
                // Prevent rotation of the local up vector
                tr.Rotate(0, yRotation, 0);
        }

        // Save lastPosition for velocity calculation.
        Vector3 lastPosition = tr.position;

        // We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
        Vector3 currentMovementOffset = velocity * Time.deltaTime;

        // Find out how much we need to push towards the ground to avoid loosing grouning
        // when walking down a step or over a sharp change in slope.
        float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
        if (grounded)
            currentMovementOffset = currentMovementOffset - (pushDownOffset * Vector3.up);

        // Reset variables that will be set by collision function
        movingPlatform.hitPlatform = null;
        groundNormal = Vector3.zero;

        // Move our character!
        movement.collisionFlags = ControllerMove(currentMovementOffset);

        movement.lastHitPoint = movement.hitPoint;
        lastGroundNormal = groundNormal;

        if (movingPlatform.enabled && (movingPlatform.activePlatform != movingPlatform.hitPlatform))
        {
            if (movingPlatform.hitPlatform != null)
            {
                movingPlatform.activePlatform = movingPlatform.hitPlatform;
                movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
                movingPlatform.newPlatform = true;
            }
        }

        /**/
        // We get the last known movement velocity before start calculations.
        // This velocity will be passed to the Status OnFall() function (FX fall sounds and damage calculations)
        Vector3 oldVelocity = velocity;

        // Calculate the velocity based on the current and previous position.  
        // This means our velocity will only be the amount the character actually moved as a result of collisions.
        Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);

        // Security sentence to avoid zero division (can happen when pausing ==> timeScale = 0, so deltaTime is zero too!!!) /**/
        float _deltaTimeSecured = Time.deltaTime;
        if (_deltaTimeSecured == 0)
        {
            _deltaTimeSecured = 0.001f;
            //Debug.LogWarning("Something went wrong because deltaTime was zero!!! Fixing the problem...");
        }

        movement.velocity = (tr.position - lastPosition) / _deltaTimeSecured;
        /*if (float.IsNaN(movement.velocity.x) || float.IsNaN(movement.velocity.y) || float.IsNaN(movement.velocity.z))
            Debug.Log("Something went wrong1 because Velocity: " + movement.velocity + " deltaTime: " + Time.deltaTime);*/

        Vector3 newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);

        // The CharacterController can be moved in unwanted directions when colliding with things.
        // We want to prevent this from influencing the recorded velocity.
        if (oldHVelocity == Vector3.zero)
        {
            movement.velocity = new Vector3(0, movement.velocity.y, 0);
        }
        else
        {
            float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
            movement.velocity = (oldHVelocity * Mathf.Clamp01(projectedNewVelocity)) + (movement.velocity.y * Vector3.up);
        }

        if (movement.velocity.y < (velocity.y - 0.001f))
        {
            if (movement.velocity.y < 0)
            {
                // Something is forcing the CharacterController down faster than it should. Ignore this
                movement.velocity.y = velocity.y;
            }
            else
            {
                // The upwards movement of the CharacterController has been blocked.
                // This is treated like a ceiling collision - stop further jumping here.
                jumping.holdingJumpButton = false;
            }
        }

        // We were grounded but just loosed grounding
        if (grounded && !IsGroundedTest())
        {
            grounded = false;

            // Apply inertia from platform
            if (movingPlatform.enabled && ((movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
            {
                movement.frameVelocity = movingPlatform.platformVelocity;
                movement.velocity = movement.velocity + movingPlatform.platformVelocity;
                if (float.IsNaN(movement.velocity.x) || float.IsNaN(movement.velocity.y) || float.IsNaN(movement.velocity.z))
                    Debug.Log("Something went wrong because Velocity: " + movement.velocity + " deltaTime: " + Time.deltaTime);
            }
            SendMessage("OnFall", oldVelocity, SendMessageOptions.DontRequireReceiver);

            // We pushed the character down to ensure it would stay on the ground if there was any.
            // But there wasn't so now we cancel the downwards offset to make the fall smoother.
            tr.position += pushDownOffset * Vector3.up;
        }
        // We were not grounded but just landed on something
        else if (!grounded && IsGroundedTest())
        {
            grounded = true;
            jumping.jumping = false;
            StartCoroutine(SubtractNewPlatformVelocity());
            SendMessage("OnLand", oldVelocity, SendMessageOptions.DontRequireReceiver);
        }

        // Moving platforms support
        if (MoveWithPlatform())
        {
            // Use the center of the lower half sphere of the capsule as reference point.
            // This works best when the character is standing on moving tilting platforms. 
            movingPlatform.activeGlobalPoint = tr.position + (Vector3.up * ((controller.center.y - (controller.height * 0.5f)) + controller.radius));
            movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);

            // Support moving platform rotation as well:
            movingPlatform.activeGlobalRotation = tr.rotation;
            movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation;
        }

        if (advancedCollisions.enabled && !advancedCollisions.useFixedUpdate)
        {
            ManageProperCollisions();
        }

        if (advancedCollisions.testCharacterFeet && !advancedCollisions.useFeetFixedUpdate)
        {
            ManageProperFeetCollisions();
        }
    }

    void FixedUpdate()
    {
        if (Pause.instance.IsPaused()) { return; }

        if (movingPlatform.enabled)
        {
            if (movingPlatform.activePlatform != null)
            {
                if (!movingPlatform.newPlatform)
                {
                    //Vector3 lastVelocity = movingPlatform.platformVelocity;
                    movingPlatform.platformVelocity = (movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint) - movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)) / Time.deltaTime;
                }
                movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
                movingPlatform.newPlatform = false;
            }
            else
            {
                movingPlatform.platformVelocity = Vector3.zero;
            }
        }

        if (useFixedUpdate)
        {
            UpdateFunction();
        }

        if (advancedCollisions.enabled && advancedCollisions.useFixedUpdate)
        {
            ManageProperCollisions();
        }

        if (advancedCollisions.testCharacterFeet && advancedCollisions.useFeetFixedUpdate)
        {
            ManageProperFeetCollisions();
        }
    }


    void Update()
    {
        if (Pause.instance.IsPaused()) { return; }

        if (!useFixedUpdate)
        {
            UpdateFunction();
        }
    }

    private Vector3 ApplyInputVelocityChange(Vector3 velocity)
    {
        Vector3 desiredVelocity = default(Vector3);
        if (!canControl)
            inputMoveDirection = Vector3.zero;

        // Find desired velocity
        if (grounded && TooSteep())
        {
            // The direction we're sliding in
            desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
            // Find the input movement direction projected onto the sliding direction
            Vector3 projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
            // Add the sliding direction, the speed control, and the sideways control vectors
            desiredVelocity = (desiredVelocity + (projectedMoveDir * sliding.speedControl)) + ((inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl);
            // Multiply with the sliding speed
            desiredVelocity = desiredVelocity * sliding.slidingSpeed;
        }
        else
            desiredVelocity = GetDesiredHorizontalVelocity();

        if (movingPlatform.enabled && (movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
        {
            desiredVelocity = desiredVelocity + movement.frameVelocity;
            desiredVelocity.y = 0;
        }

        if (grounded)
        {
            if (!stairsDetected)
                desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal); // If we are in sdome stairs, don't adjust nothing to normal.
        }
        else
            velocity.y = 0;

        //=============================================================================================
        // if we hit a wall while moving, adjust the trayectory of our player.
        //=============================================================================================
        if (controller.collisionFlags == CollisionFlags.None)
            wallNormal = Vector3.one;
        desiredVelocity = AdjustWallVelocityToNormal(desiredVelocity, wallNormal);
        //=============================================================================================

        // Enforce max velocity change
        float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
        Vector3 velocityChangeVector = desiredVelocity - velocity;
        if (velocityChangeVector.sqrMagnitude > (maxVelocityChange * maxVelocityChange))
            velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;

        // If we're in the air and don't have control, don't apply any velocity change at all.
        // If we're on the ground and don't have control we do apply it - it will correspond to friction.
        if (grounded || canControl)
            velocity = velocity + velocityChangeVector;

        if (grounded)
        {
            // When going uphill, the CharacterController will automatically move up by the needed amount.
            // Not moving it upwards manually prevent risk of lifting off from the ground.
            // When going downhill, DO move down manually, as gravity is not enough on steep hills.
            velocity.y = Mathf.Min(velocity.y, 0);
        }

        return velocity;
    }

    private Vector3 ApplyGravityAndJumping(Vector3 velocity)
    {
        if (!inputJump || !canControl)
        {
            jumping.holdingJumpButton = false;
            jumping.lastButtonDownTime = -100;
        }

        if ((inputJump && !jumping.holdingJumpButton) && (Time.time < jumping.internalTimeWindow))
            jumping.jumpCount++;

        if (grounded)
        {
            jumping.internalTimeWindow = 0;
            jumping.jumpCount = 0;
        }

        if ((inputJump && (jumping.lastButtonDownTime < 0)) && canControl)
            jumping.lastButtonDownTime = Time.time;

        if (grounded)
            velocity.y = Mathf.Min(0, velocity.y) - (movement.gravity * Time.deltaTime);
        else
        {
            velocity.y = movement.velocity.y - (movement.gravity * Time.deltaTime);

            // When jumping up we don't apply gravity for some time when the user is holding the jgrounded = falseump button.
            // This gives more control over jump height by pressing the button longer.
            if (jumping.jumping && jumping.holdingJumpButton)
            {
                // Calculate the duration that the extra jump force should have effect.
                // If we're still less than that duration after the jumping time, apply the force.
                if (Time.time < (jumping.lastStartTime + (jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight))))
                {
                    // Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
                    velocity = velocity + ((jumping.jumpDir * movement.gravity) * Time.deltaTime);
                }
            }

            // Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
            velocity.y = Mathf.Max(velocity.y, -movement.maxFallSpeed);
        }

        if (grounded ||
            (((((!grounded && !jumping.holdingJumpButton) && inputJump) && jumping.doubleJump) && (jumping.jumpCount < jumping.numberJumps)) && (Time.time < jumping.internalTimeWindow)))
        // Jump only if the jump button was pressed down in the last 0.2 seconds.
        // We use this check instead of checking if it's pressed down right now
        // because players will often try to jump in the exact moment when hitting the ground after a jump
        // and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
        // it's confusing and it feels like the game is buggy.
        {
            if (((jumping.canJump && jumping.enabled) && canControl) && ((Time.time - jumping.lastButtonDownTime) < 0.2f))
            {
                grounded = false;
                jumping.jumping = true;
                jumping.lastStartTime = Time.time;
                jumping.lastButtonDownTime = -100;
                jumping.holdingJumpButton = true;
                jumping.internalTimeWindow = Time.time + jumping.timeWindow;

                // Calculate the jumping direction
                if (TooSteep())
                    jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
                else
                    jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);

                if (jumping.jumpCount > 0)
                    jumping.jumpDir = Vector3.Slerp(Vector3.up, Vector3.up, jumping.steepPerpAmount);

                // Apply the jumping force to the velocity. Cancel any vertical velocity first.
                velocity.y = 0;
                if (jumping.jumpCount == 0)
                    velocity = velocity + (jumping.jumpDir * CalculateJumpVerticalSpeed(jumping.baseHeight));
                else
                    velocity = velocity + (jumping.jumpDir * CalculateJumpVerticalSpeed(jumping.doubleJumpBaseHeight));

                // Apply inertia from platform
                if (movingPlatform.enabled && ((movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
                {
                    movement.frameVelocity = movingPlatform.platformVelocity;
                    velocity = velocity + movingPlatform.platformVelocity;
                }
                SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                jumping.holdingJumpButton = false;
            }
        }
        return velocity;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0)
        {
            if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001f || lastGroundNormal == Vector3.zero)
                groundNormal = hit.normal;
            else
                groundNormal = lastGroundNormal;

            movingPlatform.hitPlatform = hit.collider.transform;
            movement.hitPoint = hit.point;
            movement.frameVelocity = Vector3.zero;
        }

        //=============================================================================================
        // Get the normal if we hit a wall, and save in our 'wallNormal' var.
        //=============================================================================================
        if ((controller.collisionFlags & CollisionFlags.Sides) != (CollisionFlags)0)
            // only check lateral collisions (ideally normal.y should be 0, a vertical wall)
            wallNormal = (Mathf.Abs(hit.normal.y) < 0.2f) ? hit.normal : Vector3.one;
        else if (wallNormal != Vector3.one)
            wallNormal = Vector3.one;
        //=============================================================================================
    }

    private IEnumerator SubtractNewPlatformVelocity()
    {
        // When landing, subtract the velocity of the new ground from the character's velocity
        // since movement in ground is relative to the movement of the ground.
        if (movingPlatform.enabled && ((movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
        {
            // If we landed on a new platform, we have to wait for two FixedUpdates
            // before we know the velocity of the platform under the character
            if (movingPlatform.newPlatform)
            {
                Transform platform = movingPlatform.activePlatform;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                if (grounded && (platform == movingPlatform.activePlatform))
                    yield return 1;
            }
            movement.velocity = movement.velocity - movingPlatform.platformVelocity;
        }
    }

    private bool MoveWithPlatform()
    {
        return (movingPlatform.enabled && (grounded || (movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked))) && (movingPlatform.activePlatform != null);
    }

    private Vector3 GetDesiredHorizontalVelocity()
    {
        // Find desired velocity
        Vector3 desiredLocalDirection = tr.InverseTransformDirection(inputMoveDirection);
        float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
        if (grounded)
        {
            // Modify max speed on slopes based on slope speed multiplier curve
            float movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y) * Mathf.Rad2Deg;
            maxSpeed = maxSpeed * movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
        }
        return tr.TransformDirection(desiredLocalDirection * maxSpeed);
    }

    private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
    {
        Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
        return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
    }

    //=============================================================================================
    // Adjust trayectory when hitting a wall. It'll go in the perpendicular direction of the wall
    // tat will be adjusted using the actual forward direction off the player.
    //=============================================================================================
    private Vector3 AdjustWallVelocityToNormal(Vector3 hVelocity, Vector3 wallNormal)
    {
        Vector3 dir = hVelocity;
        if (wallNormal != Vector3.one)
        {
            Vector3 perp = Vector3.Cross(wallNormal, Vector3.up);
            perp = perp / perp.magnitude;
            dir = Vector3.Project(hVelocity, perp);
        }
        //Debug.DrawRay(this.transform.position, dir*20, Color.green);
        return dir;
    }
    //=============================================================================================

    private bool IsGroundedTest() { return groundNormal.y > 0.01f; }


    //=============================================================================================
    //
    // Trying to avoid physics penetration in the CharacterController (generates weird movements).
    //
    //=============================================================================================
    private int overlappingCollidersCount = 0;
    private Collider[] overlappingColliders = new Collider[256];
    private List<Collider> ignoredColliders = new List<Collider>(256);

    private void PreCharacterControllerUpdate()
    {
        Vector3 center = transform.TransformPoint(controller.center);
        Vector3 delta = (0.5f * controller.height - controller.radius) * Vector3.up;
        Vector3 bottom = center - delta;
        Vector3 top = bottom + delta;

        overlappingCollidersCount = Physics.OverlapCapsuleNonAlloc(bottom, top, controller.radius, overlappingColliders);
        for (int i = 0; i < overlappingCollidersCount; i++)
        {
            Collider overlappingCollider = overlappingColliders[i];
            if (overlappingCollider.gameObject.isStatic)
                continue;
            ignoredColliders.Add(overlappingCollider);
            Physics.IgnoreCollision(controller, overlappingCollider, true);
        }
    }

    private void PostCharacterControllerUpdate()
    {
        for (int i = 0; i < ignoredColliders.Count; i++)
            Physics.IgnoreCollision(controller, ignoredColliders[i], false);

        ignoredColliders.Clear();
    }

    // Move the CharacterController trying to avoid physics penetration.
    private CollisionFlags ControllerMove(Vector3 _motion)
    {
        PreCharacterControllerUpdate();
        CollisionFlags flags = controller.Move(_motion);
        PostCharacterControllerUpdate();
        return flags;
    }


    //=============================================================================================
    //
    // Trying to fix the Collision detection problem.
    // https://forum.unity.com/threads/proper-collision-detection-with-charactercontroller.292598/
    //
    //=============================================================================================
    private void ManageProperCollisions()
    {
        RaycastHit hit;

        //Bottom of controller. Slightly above ground so it doesn't bump into slanted platforms. (Adjust to your needs)
        //Vector3 p1 = tr.position + Vector3.up * 0.1f;
        //Top of controller
        //Vector3 p2 = p1 + Vector3.up * controller.height;

        Vector3 p1 = (tr.position + controller.center) + (Vector3.down * controller.height * 0.5f);
        Vector3 p2 = p1 + (Vector3.up * controller.height);

        //Check around the character in a 360, 10 times (increase if more accuracy is needed)
        int incr = 360 / advancedCollisions.raycastNumber;
        for (int i = 0; i < 360; i += incr)
        {
            //Check if anything with the platform layer touches this object
            if (Physics.CapsuleCast(p1, p2, 0, new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)), out hit, distanceProperCollisions, advancedCollisions.movingPlatformsLayers))
            {
                //Debug.Log("ManageProperCollisions -> Raycast detected: "+hit.transform.name);
                //If the object is touched by a platform, move the object away from it
                controller.Move(hit.normal * (distanceProperCollisions - hit.distance));
            }
        }
    }

    private void ManageProperFeetCollisions()
    {
        if (coreScr.belowSensorOptions.sensorHitTag.Contains("Platform")) return;

        RaycastHit hit;

        //Check the players feet and push them up if something clips through their feet. (Useful for vertical moving platforms)
        Vector3 p1 = (tr.position + controller.center) + (Vector3.up * controller.height * 0.5f);
        if (Physics.Raycast(p1, Vector3.down, out hit, controller.height, advancedCollisions.movingPlatformsLayers))
        {
            Debug.Log("ManageProperFeetCollisions -> Feet raycast detected: " + hit.transform.name);
            controller.Move(Vector3.up * (1 - hit.distance));
        }
    }

    //=============================================================================================
    //
    // Public functions
    //
    //=============================================================================================



    // Maximum acceleration on ground and in air
    public float GetMaxAcceleration(bool grounded) { return grounded ? movement.maxGroundAcceleration : movement.maxAirAcceleration; }
    /*{
        if (grounded)
            return movement.maxGroundAcceleration;
        else
            return movement.maxAirAcceleration;
    }*/

    // From the jump height and gravity we deduce the upwards speed 
    // for the character to reach at the apex.
    public float CalculateJumpVerticalSpeed(float targetJumpHeight) { return Mathf.Sqrt((2 * targetJumpHeight) * movement.gravity); }

    public bool IsJumping() { return jumping.jumping; }

    public bool IsSliding() { return (grounded && sliding.enabled) && TooSteep(); }

    public bool IsTouchingCeiling() { return (movement.collisionFlags & CollisionFlags.CollidedAbove) != (CollisionFlags)0; }

    public bool IsGrounded() { return grounded; }

    public bool TooSteep() { return groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad); }

    public Vector3 GetDirection() { return inputMoveDirection; }

    public void SetControllable(bool controllable) { canControl = controllable; }

    // Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
    // The function returns the length of the resulting vector.
    public float MaxSpeedInDirection(Vector3 desiredMovementDirection)
    {
        if (desiredMovementDirection == Vector3.zero) { return 0; }
        else
        {
            float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
            Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
            float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
            return length;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        grounded = false;
        movement.velocity = velocity;
        movement.frameVelocity = Vector3.zero;
        //SendMessage("OnExternalVelocity");
    }

    public void AddVelocity(Vector3 velocity)
    {
        grounded = false;
        movement.velocity = movement.velocity + velocity;
        movement.frameVelocity = Vector3.zero;
    }
    //=============================================================================================

}