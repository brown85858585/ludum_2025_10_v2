
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour {

	[Tooltip("Simple forward kick it's made by pressing twice the Jump button while walking forward againts any object. The kick reaction will send the up & backwards. If the object kicked have a rigidbody it will be pushed.")]
	public KickOptions simpleKick = new KickOptions();
	[Tooltip("Side kick it's made by pressing once the jump button while walking sideways againts any object. The kick reaction will send up & sideways. If the object kicked have a rigidbody it will be pushed.")]
	public SideKickOptions sideKick = new SideKickOptions();
	[Tooltip("Above kick it's made jumping over an enemy (over his 'head'). The kick reaction will send the up & forward.")]
	public AboveKickOptions aboveKick = new AboveKickOptions();

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;
    [Tooltip("Show all 'Debug.Log' normal (frontal) kicks messages.")]
    [ShowWhen("showDebug")]
    public bool showKickDebug = false;
    [Tooltip("Show all 'Debug.Log' lateral kicks messages.")]
    [ShowWhen("showDebug")]
    public bool showSidekickDebug = false;
    [Tooltip("Show all 'Debug.Log' above kicks messages.")]
    [ShowWhen("showDebug")]
    public bool showAbovekickDebug = false;

    private float aboveKickInternalTime = 1;

	private Status statusSrc;
	private Core coreScr;
	private CharacterController controller;
	private CharacterMotor motor;


	// Detect normal kicks by detecting double taps (using the jump button).
	private void DetectSimpleKickInput()
	{
		if ((Time.time - coreScr.lastTapTime) < simpleKick.tapSpeed) // Detect simple kick.
		{
			statusSrc.isSimpleKickDetected = true;
			statusSrc.isJumpDetected = false;
		}
		else
		{
			coreScr.lastTapTime = Time.time; // no kick it is a normal jump.
			statusSrc.isSimpleKickDetected = false;
			statusSrc.isJumpDetected = true;
		}
	}

	// Detect side kicks by looking the lateral keyboard movement buttons.
	private void DetectSideKickInput()
	{
		if ((Mathf.Abs(InputManager.instance.HorizontalValue) == 1) && statusSrc.isJumpDetected)
			statusSrc.isSideKickDetected = true;
		else
			statusSrc.isSideKickDetected = false;

		if (Mathf.Abs(InputManager.instance.HorizontalValue) != 1)
			statusSrc.isSideKickDetected = false;
	}


	void Start ()
    {
		statusSrc = GetComponent<Status>();
		coreScr = statusSrc.GetCoreScr();
		controller = statusSrc.GetController();
		motor = statusSrc.GetMotor();
	}


	// Detect the jump button to perform a kick.
	void Update () 
	{
		if (!InputManager.instance.jumpKey.isDown) return;	// If the jump button isn't pressed do nothing

		// Detect the kick or simple kick  when player press the jump button
		if (!(statusSrc.isCrouched || statusSrc.isProned || (statusSrc.isInLedge && statusSrc.isClimbing)))
		{
			if(simpleKick.canKick)
				DetectSimpleKickInput ();

			if(sideKick.canSideKick)
				DetectSideKickInput ();
		}
	}

	// Detect a regular kick and do the movement.
	public void UpdateSimpleKickState()
	{
		if(!statusSrc.isSimpleKickDetected) return;	// Security sentence

		RaycastHit hit = default(RaycastHit);
		RaycastHit hitAbove = default(RaycastHit);

		// Dont allow kick in ladders and Ledges.
		if ((((statusSrc.isInLadder || statusSrc.isInLedge) || statusSrc.isJumpingFromLadder) || statusSrc.isJumpingFromLedge))
			statusSrc.isSimpleKickDetected = false;

		if (!statusSrc.isSimpleKicking && statusSrc.isSimpleKickDetected)
		{
			// Setup the values needed for a Capsulecast to see if we are actually kicking something in the scene.
			int enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
			int hitLayer = 1 << enemyBulletLayer; // Ignore enemy Bullets.
			hitLayer = ~hitLayer;
			Vector3 dirFwd = transform.TransformDirection(Vector3.forward);
			Vector3 p1 = (transform.position + controller.center) + (Vector3.up * (-controller.height * 0.5f));
			Vector3 p2 = p1 + (Vector3.up * controller.height);

			// Detect if we are facing something to kick to.
			if (Physics.CapsuleCast(p1, p2, controller.radius, dirFwd, out hit, simpleKick.minDistance, hitLayer))
			{
                if (showDebug && showKickDebug) Debug.Log("Kick -> UpdateSimpleKickState() -> Kicking object : " + hit.transform.name + " tag: "+ hit.collider.tag.ToLower());
                statusSrc.isSimpleKickDetected = false;
				statusSrc.isSimpleKicking = true;
				statusSrc.isJumping = false;
				coreScr.testJumpStateTime = Time.time + 0.1f; /**/

				// Get the direction of the movement of pl when kicking. (Response direction).
				Vector3 dirBackwrd = transform.TransformDirection(-Vector3.forward);
				dirBackwrd.Normalize();
				dirBackwrd = dirBackwrd + Vector3.up;
				// Detect if we are doing the kick at some height to apply more velocity to the player. Height Kick.
				Vector3 dirAbove = transform.TransformDirection(-Vector3.up);
				if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), dirAbove, out hitAbove, simpleKick.height * 2))
				{
					if (Vector3.Distance(hitAbove.point, transform.position) >= simpleKick.height)
						motor.SetVelocity(new Vector3((dirBackwrd.x * simpleKick.backwardsVelocity) * simpleKick.heightVelMultiplier, dirBackwrd.y * simpleKick.upVelocity, (dirBackwrd.z * simpleKick.backwardsVelocity) * simpleKick.heightVelMultiplier));
					else
						motor.SetVelocity(new Vector3(dirBackwrd.x * simpleKick.backwardsVelocity, dirBackwrd.y * simpleKick.upVelocity, dirBackwrd.z * simpleKick.backwardsVelocity));
				}
				else
					motor.SetVelocity(new Vector3(dirBackwrd.x * simpleKick.backwardsVelocity, dirBackwrd.y * simpleKick.upVelocity, dirBackwrd.z * simpleKick.backwardsVelocity));

				// Play kick sound effect.
				gameObject.BroadcastMessage("PlaySimpleKick");

				// Can we push objects when kicking something.
				if (!simpleKick.canPush) { return; }

				// Perform the push if the object has a rigidbody.
				Rigidbody body = hit.collider.attachedRigidbody;
				CharacterController enemyController = hit.collider.gameObject.GetComponent<CharacterController>();
				
                // no there's rigidbody and it isn't Kinematic
				if (body != null)
				{
					if (body.isKinematic) { return; }

					Vector3 dirPush = dirFwd;	// Get the kick force direction to be used

					// Check if there are new kick options (and override the kick values if so).
					ObjectPushOptions objPushOptions = hit.collider.GetComponent<ObjectPushOptions> ();
					if(objPushOptions != null){
						if(objPushOptions.canKick && objPushOptions.overrideKickPower){
							body.linearVelocity = dirPush * objPushOptions.kickPower.x + Vector3.up * objPushOptions.kickPower.y;
						}
						else if(objPushOptions.canKick && !objPushOptions.overrideKickPower){
							body.linearVelocity = dirPush * simpleKick.pushPower.x + Vector3.up * simpleKick.pushPower.y;
						}
					}
					else
						body.linearVelocity = dirPush * simpleKick.pushPower.x + Vector3.up * simpleKick.pushPower.y;
				}
				
				// Perform a kick on an enemy an decrease his health.
				if (enemyController != null)
				{
					hit.collider.gameObject.SendMessage("ApplyDamage", simpleKick.kickDamage);
					hit.collider.gameObject.SendMessage("AddKickForce", dirFwd * simpleKick.pushPowerEnemy);
				}
			}
			else
				statusSrc.isSimpleKickDetected = false;
		}
	}

    // Detect a sidekick and do the movement.
    public void UpdateSideKickState()
    {
        if (!sideKick.canSideKick) { return; }

        if (statusSrc.isSideKicking || !statusSrc.isJumping || !statusSrc.isSideKickDetected) { return; }

        // if is really 'isSideKickDetected' active, test if we are pressing the keyboard yet and do it.
        float sideKey = InputManager.instance.HorizontalValue;
        if (sideKey == 0) { statusSrc.isSideKickDetected = false; return; }


        RaycastHit hit;
        Vector3 dirRight, p1, p2;
        bool IsHit = false;

        // Get values for the CapsuleCast.
        dirRight = transform.TransformDirection(Vector3.right);
        // p1 is the capsule's lowest point, p2 is the highest point
        p1 = (transform.position + controller.center) + (Vector3.up * (-controller.height * 0.3f));
        p2 = p1 + (Vector3.up * controller.height);

        dirRight = sideKey < 0 ? -dirRight : dirRight;
        if (Physics.CapsuleCast(p1, p2, controller.radius, dirRight, out hit, sideKick.minDistance))
            IsHit = true;

        // CapsuleCast detects a hit, generate the side kick in pl.
        if (IsHit)
        {
            if (showDebug && showSidekickDebug) Debug.Log("Kick -> UpdateSideKickState() -> Kicking object : " + hit.transform.name + " tag: " + hit.collider.tag.ToLower());

            statusSrc.isSideKickDetected = false;
            statusSrc.isSideKicking = true;
            statusSrc.isJumping = false;
            coreScr.testJumpStateTime = Time.time + 0.1f;

            // Get the direction of the movement
            Vector3 dir = -dirRight;
            dir.Normalize();
            dir = dir + Vector3.up;
            // Apply the movement in that direction (as response of the sidekick).
            motor.SetVelocity(new Vector3(dir.x * sideKick.sideVelocity, dir.y * sideKick.upVelocity, dir.z * sideKick.sideVelocity));
            // Play the sound effect.
            gameObject.BroadcastMessage("PlaySideKick");

            // Push the object if we can do it and it has a rigidbody.
            if (!sideKick.canPush) { return; }

            Rigidbody body = hit.collider.attachedRigidbody;

            Vector3 dirPush = dirRight;

            // no there's rigidbody and it isn't Kinematic
            if (body != null)
            {
                if (body.isKinematic) { return; }
               
                // Check if there are new kick options (and override the kick values if so).
                ObjectPushOptions objPushOptions = hit.collider.GetComponent<ObjectPushOptions>();
                if (objPushOptions != null)
                {
                    if (objPushOptions.canSideKick && objPushOptions.overrideSideKickPower)
                    {
                        body.linearVelocity = dirPush * objPushOptions.kickSidePower.x + Vector3.up * objPushOptions.kickSidePower.y;
                    }
                    else if (objPushOptions.canSideKick && !objPushOptions.overrideSideKickPower)
                    {
                        body.linearVelocity = dirPush * sideKick.pushPower.x + Vector3.up * sideKick.pushPower.y;
                    }
                }
                else
                    body.linearVelocity = dirPush * sideKick.pushPower.x + Vector3.up * sideKick.pushPower.y;
            }

            CharacterController enemyController = hit.collider.gameObject.GetComponent<CharacterController>();
            // If we are kicking over an enemy, perform the kick and decrease his health.
            if (enemyController != null)
            {
                hit.collider.gameObject.SendMessage("ApplyDamage", sideKick.kickDamage);
                hit.collider.gameObject.SendMessage("AddKickForce", dirPush * sideKick.pushPowerEnemy);
            }
        }
    }

	public void AboveKick()
	{
		RaycastHit hit = default(RaycastHit);
		if (Time.time < aboveKickInternalTime) { return; }

		// CapsuleCast Below us to see in pl is over something.
		int enemyLayer = LayerMask.NameToLayer("Enemy");
		int hitLayer = 1 << enemyLayer; // Only check agains Enemy's layer.
		//hitLayer = ~hitLayer;

		Vector3 dirFwd = transform.TransformDirection(-Vector3.up);
		Vector3 p1 = (transform.position + controller.center) + (Vector3.up * (-controller.height * 0.5f));
		Vector3 p2 = p1 + (Vector3.up * controller.height);

		//if(Physics.Raycast(transform.position - Vector3(0,0.5,0) , dirFwd, hit, aboveKick.minDistance, hitLayer)){
		if (Physics.CapsuleCast(p1, p2, controller.radius, dirFwd, out hit, aboveKick.minDistance, hitLayer))
		{
            if (showDebug && showAbovekickDebug) Debug.Log("Kick -> AboveKick() -> Kicking enemy : " + hit.transform.name + " tag: " + hit.collider.tag.ToLower());
            // The is we collided with an enemy
            CharacterController enemyController = hit.collider.gameObject.GetComponent<CharacterController>();
			// If we are kicking over an enemy, perform the kick.
			if (enemyController != null)
			{
				// Apply Damage decreasing the enemy's health.
				hit.collider.gameObject.SendMessage("ApplyDamage", aboveKick.kickDamage);
				// Play Kick sound effect.
				gameObject.BroadcastMessage("PlayAboveKick");
				// Move pl forward and up at a selected velocity.
				Vector3 dirFwrd = transform.TransformDirection(Vector3.forward);
				dirFwrd.Normalize();
				dirFwrd = dirFwrd + Vector3.up;
				motor.SetVelocity(new Vector3(dirFwrd.x * aboveKick.forwardVelocity, dirFwrd.y * aboveKick.upVelocity, dirFwrd.z * aboveKick.forwardVelocity));
				aboveKickInternalTime = Time.time + aboveKick.checkRatio;

				// TODO
				// Set the default acceleration values when landing (for now i suposse is always stop or walking)
				motor.movement.maxGroundAcceleration = coreScr.motorGroundAccelOrig;
				motor.movement.maxAirAcceleration = coreScr.motorAirAccelOrig;
			}
		}
	}

}
