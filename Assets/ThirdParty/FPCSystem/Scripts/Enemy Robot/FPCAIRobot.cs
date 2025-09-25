
using UnityEngine;
using System.Collections;

// States of the enemy.
public enum AIState
{
    Guard = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3,
    Kicked = 4,
    Dying = 5,
    Shoot = 6
}

[System.Serializable]
public class MovementOptions : object
{
    [Tooltip("Normal walk velocity")]
    public float speed = 3;
    [Tooltip("Run velocity")]
    public float runSpeed = 6;
    [Tooltip("Jump Speed (Up Direction")]
    public float jumpSpeed = 4;
    [Tooltip("Rotation velocity")]
    public float rotationSpeed = 5;
    [Space(5)]
    [Tooltip("Minimun distance to reach a point")]
    public float dontComeCloser = 1;
    [Tooltip("Minimun distance to reach the player")]
    public float dontComeCloserPlayer = 5;
}

[System.Serializable]
public class AttackOptions : object
{
    [Tooltip("Attack range. (The enemy will change to 'attack' state if in range)")]
    public float attackRange = 30f;
    [Tooltip("Shoot range (enemy will fire if in range)")]
    public float shootRange = 15f;
    [Tooltip("Shoot angle (enemy will fire if in range)")]
    public float shootAngle = 4f;
}

[System.Serializable]
public class TimeStateOptions : object
{
    [Tooltip("Time staying standing in a point. (Guard state)")]
    public float GuardTime = 3;
    [Tooltip("Time chasing a player (Chase state) before going back to patrol.")]
    public float ChaseTime = 3;
    [Tooltip("Time the enemy have to get closer the player and fire again.")]
    public float ComeCloserTime = 2;
    [Tooltip("Wait time while shooting. The enemy has a shoot ratio.")]
    public float delayShootTime = 0.3f; 
}

[System.Serializable]
public class DamageOptions : object
{
    [Tooltip("How many health will substract to the player by standing close to him.")]
    public int SustractHealth = 1;
    [Tooltip("Wait time before substracting more health from the player.")]
    public float SustractRatio = 1;
}



[RequireComponent(typeof(CharacterController))]
public class EnemyBirdIA : MonoBehaviour
{
    private AIState state = AIState.Patrol;

    [Header("Enemy Main Configuration")]
	public MovementOptions movement = new MovementOptions();
	public AttackOptions attack = new AttackOptions();
	public TimeStateOptions timeStates = new TimeStateOptions();
    public DamageOptions damage;

    [Space(5)]
    [Tooltip("Active Waypoint to go to when patrolling.")]
    public DirectionalWaypoint activeWaypoint;
    [Tooltip("Fire position point (to instantiate the bullet in that position).")]
    public Transform FirePos;
    [Tooltip("Bullet Prefab the enemy will fire (when shooting)")]
    public GameObject Bullet;
    [Tooltip("Robot's ralldog version to make a physical 'die' FX (very cool when it's kicked).")]
    public GameObject DieRobot;

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;
    [Tooltip("Enable/Disable all 'Debug.DrawRay' for avoid obstacles from this enemy.")]
    public bool showDebugRays = false;


    // Private variables
    private float origSpeed = 3f;   // Original velocity speed
    private float GuardTimeCheck;   // Internal var that controls the guard time.
    private float ChaseTimeCheck;   // Internal var to control the time chasing the player
    private float ComeCloserTimeCheck;  // Internal var to control the time the enmy will come closer to the player.
    private float timeRateSustractionCheck; // Control the ratio time of health substraction.

    private Transform target;       // Target the enemy will be attaking and shooting to.
    private GameObject MyObj;    // Look at the player GameObject if there isn't a target yet

    private CharacterController character;   // A character controller is needed tio 'move' our enemy.
    private AudioSource FireSound;
    private Animation myAnimation;

    private float nextFire;                 // Internal var to control the shoot ratio.
    private float waitShootInternalTime;    // controls the time to syncronize the shoot and the 'shooting' animation
    private bool cancelShoot = true;        // has been the shoot cancelled for some reason (or done all right)???
    private Vector3 lastVisiblePlayerPosition;  // Last known player position (to chase him in 'chase' mode).

    private bool beingStopped;  // Has the enemy being stopped (to shoot)
    private bool jump;          // is the enemy jumping?
    private float vSpeed;       // get the jump velocity (vertical velocity)
    private float gravity = 1;  // Gravity that will affect the enemy (mainly when jumping)
    private Vector3 pushDirection = Vector3.zero;   // Push direction the enemy will go when kicked by the player.

    private bool wasKicked;                 // var to know if enemy was kicked some seconds ago used to 'die' properly die state.
    //private float internalWasKickedTime;    // Internal time used to return waskicked again to false automatically.

    private Vector3 avoidDirection = Vector3.zero;  // Direction the enemy has to go to avoid an obstacle.
    private float internalAvoidTime;    // Ratio that has the enemy to look for obstacles in his path.



    // Public funcion to add  push 'force' to the enemy. It's used inStatusSrc.
    // Cancel any shoot and change to kicked status.
    public void AddKickForce(Vector3 _force)
    {
        cancelShoot = true;
        pushDirection = _force;
        wasKicked = true;
        //internalWasKickedTime = Time.time + 1; // Preserv the waskicked var 'True' for 1 second.
        state = AIState.Kicked;
    }

    // Public funtion called by EnemyManageStatus to enter the die status if health < 0
    public void Die()
    {
		StartCoroutine("TryChangeToDieStatus");
    }

	IEnumerator TryChangeToDieStatus()
	{
		do{
			state = AIState.Dying;
			yield return new WaitForSeconds (0.1f);
		}while(state != AIState.Dying);
	}

    void Start()
    {
        // Search the player if there isn't any target.
		if (target == null){
            MyObj = GameObject.FindWithTag("Player"); // other way: GameObject.Find("Player");
            if (MyObj == null)
            {
                Debug.LogError("AIRobot -> Start() -> Player NOT Found!. -> Enemy:" + this.transform.parent.name);
            }
            else
            {
                target = MyObj.transform;
            }
		}

        

        FireSound = GetComponent<AudioSource>();
		character = GetComponent<CharacterController>();
        myAnimation = GetComponent<Animation>();

        origSpeed = movement.speed; // Save our original speed.

        // Check we have waypoints to patrol (minimun waypoints nedded: 2)
        if (!activeWaypoint)
        {
            Debug.LogError("AIRobot -> Start() -> You need to add any number of waypoint - 2 minimun. -> Enemy:" + this.transform.parent.name);
            return;
        }

        // Check we have a target to chase, attack and shoot (the Player player, of course).
        if (!target)
        {
            Debug.LogError("AIRobot -> Start() -> There is no entity tagged 'Player' in the scene. -> Enemy:" + this.transform.parent.name);
            return;
        }
        
    }

    //=================================
    //
    // AI States control.
    //
    //=================================
    void Update()
    {
		if (!activeWaypoint && !target){ return; } // Security Sentence.
			
        switch (state)
        {
            case AIState.Patrol:
                // Action :  Move towards
                MoveTowards(activeWaypoint.transform.position, true);

                // Change status : I'm near the waypoint -> Guard. I can see the Player -> attack him.
                if (CanSeeTarget())
                {
                    if (showDebug) Debug.Log("AIRobot -> Update() -> Changing to Attack Status. -> Enemy:" + this.transform.parent.name);
                    state = AIState.Attack;
                }
                else if (activeWaypoint.CalculateDistance(transform.position) < movement.dontComeCloser)
                {
                    if (showDebug) Debug.Log("AIRobot -> Update() -> Changing to Guard Status. -> Enemy:" + this.transform.parent.name);
                    state = AIState.Guard;
                }

                break;
            case AIState.Guard:
                // Action :  Apply Idle animation and set speed to zero.
                if (movement.speed > 0)
                {
                    movement.speed = 0;
                    SendMessage("SetSpeed", 0f);
                    GuardTimeCheck = Time.time + timeStates.GuardTime;
                }

                // Change status : Has pass enought time in guard state? -> patrol. I'm seeing the player? -> Attack
                if (Time.time > GuardTimeCheck)
                {
                    state = AIState.Patrol;
                    movement.speed = origSpeed;
                    SendMessage("SetSpeed", movement.speed);
                    activeWaypoint = activeWaypoint.CalculateTargetPosition(transform.position, movement.dontComeCloser);
                }

                if (CanSeeTarget())
                {
                    if (showDebug) Debug.Log("AIRobot -> Update() -> Changing to Attack Status. -> Enemy:" + this.transform.parent.name);
                    state = AIState.Attack;
                }

                break;
            case AIState.Chase:
                // Action :  Go to the last known player position.
                SearchPlayer(lastVisiblePlayerPosition);
                
				// Change status condition: Player not visible anymore -> Patrol, i'm seing the player -> Attack.
                if (Time.time >= ChaseTimeCheck)
                {
                    if (!CanSeeTarget())
                    {
                        if (showDebug) Debug.Log("AIRobot -> Update() -> From Chasing to Patrol Status. -> Enemy:" + this.transform.parent.name);
                        movement.speed = origSpeed;
                        state = AIState.Patrol;
                        return;
                    }
                    else
                    {
                        if (showDebug) Debug.Log("AIRobot -> Update() -> From Chasing to Attack Status. -> Enemy:" + this.transform.parent.name);
                        state = AIState.Attack;
                    }
                }
                break;
            case AIState.Attack:
                // Action :  Attack the player.
                AttackPlayer();
                
				// Change status condition: The target (player) is null (he's dead), go to patrol.
                if (target == null)
                {
                    if (showDebug) Debug.Log("AIRobot -> Update() -> Player is Dead, back to Patrol. -> Enemy:" + this.transform.parent.name);
                    beingStopped = false;
                    movement.speed = origSpeed;
                    SendMessage("SetSpeed", movement.speed);
                    state = AIState.Patrol;
                }
                // Change status condition: I cant see the payer -> Chase him.
                if (!CanSeeTarget())
                {
                    if (showDebug) Debug.Log("AIRobot -> Update() -> Changing to Chase Status. -> Enemy:" + this.transform.parent.name);

                    beingStopped = false;
                    movement.speed = movement.runSpeed;
                    ChaseTimeCheck = Time.time + timeStates.ChaseTime;
                    state = AIState.Chase;
                }
                break;
            case AIState.Shoot:
                // Action : Perform one shoot.
                Shoot();
                WaitAndFire();

                // Change status condition: shoot is done or canceled.
                if (cancelShoot)
                {
                    if (showDebug) Debug.Log("AIRobot -> Update() -> Changing to Attack Status. -> Enemy:" + this.transform.parent.name);
                    state = AIState.Attack;
                }

                break;
            case AIState.Kicked:
                // Action : Perform a kick in the enemy (move it backwards).
                if (movement.speed != movement.runSpeed)
                {
                    movement.speed = movement.runSpeed;
                    SendMessage("SetSpeed", movement.speed);
                }
                MoveTowards(lastVisiblePlayerPosition, false);

                // Change status condition: if push velocity < 1 -> Chase the player (velocity lowing in every update a little bit).
                if (pushDirection.magnitude <= 1)
                {
                    if (showDebug) Debug.Log("AIRobot -> Update() -> Changing to Chase Status. -> Enemy:" + this.transform.parent.name);
                    state = AIState.Chase;
                }

                break;
            case AIState.Dying:
                // Action : Kill the enemy.
                // Change status : NONE. The gameObject will be destroyed in several seconds (in ManageEnemyStatus.cs).

                if (showDebug) Debug.Log("AIRobot -> Update() -> Enemy is Dying. -> Enemy:" + this.transform.parent.name);

                // Step1: Disable the actual robot model.
                character.enabled = false;
                //SkinnedMeshRenderer mySkinRender = (SkinnedMeshRenderer) gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                //MeshRenderer myMeshrender = (MeshRenderer) gameObject.GetComponentInChildren<MeshRenderer>();
                GetComponentInChildren<MeshRenderer>().enabled = false;
                
				// Step2: Enable the robot ralldog instead (to die falling to the ground)
                DieRobot.SetActive(true);
                DieRobot.transform.position = transform.position;
                DieRobot.transform.rotation = transform.rotation;
                // If the robot was kicked, apply a force to the ralldog.
                if (wasKicked)
                {
                    Vector3 dir = DieRobot.transform.TransformDirection(-Vector3.forward);
                    dir *= movement.runSpeed * 6;
                    DieRobot.GetComponentInChildren<Rigidbody>().AddForce(dir, ForceMode.VelocityChange);
                    wasKicked = false;
                    //internalWasKickedTime = Time.time;
                }
                break;
        }
    }

    // Moverse hacia un punto rotando a la vez hacia el blanco.
    private void MoveTowards(Vector3 position, bool avoidObstacles)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0;
        
		if (direction.magnitude < 0.5f) { SendMessage("SetSpeed", 0f); return; } // If it is close then stop moving and return.

        // Rotate towards the target
        RotateTowards(position, avoidObstacles);

        // Modify speed so we slow down when we are not facing the target
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float speedModifier = Vector3.Dot(forward, direction.normalized);
        speedModifier = Mathf.Clamp(speedModifier, 0.5f, 1);
        if (character.isGrounded) // if is grounded...
        {
            vSpeed = 0; // vertical speed  is zero
            if (jump) // if should jump...
            {
                vSpeed = movement.jumpSpeed; // aplly jump speed
                jump = false;
            }
        }
        else
        {
            jump = false;
            vSpeed = vSpeed - gravity; // apply gravity
        }

        // Move the character
        direction = (forward * movement.speed) * speedModifier;
        direction.y = direction.y + vSpeed;

        // Push direction of a doublekick from the player.
        if (pushDirection != Vector3.zero)
        {
            direction +=  pushDirection;
            pushDirection *= 0.98f;
            if (pushDirection.magnitude <= 1)
                pushDirection = Vector3.zero;
        }

        if (character.enabled)
            character.Move(direction * Time.deltaTime);

        SendMessage("SetSpeed", movement.speed * speedModifier, SendMessageOptions.DontRequireReceiver);
    }

    // Rotate until we look at some destination point (only rotation, without forward movement).
    private void RotateTowards(Vector3 position, bool avoidObstacles)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0;
		if (direction.magnitude < 0.1f) { return; } // If it is close then stop rotating and return.

        // Look if we have to change our rotation direction because its an obstacle in front of us.
        if (avoidObstacles)
            direction = AvoidObstacles(direction);

        // Rotate to the target point interpolating using a rotationspeed.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), movement.rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    // Avoid static obstacles, launching three raycast and turning to one side if hit something.
    private Vector3 AvoidObstacles(Vector3 direction)
    {
        RaycastHit hit = default(RaycastHit);
        float rayLength = 3f;
        Vector3[] rayArray = new Vector3[3];
        
		// Stop making a raycast some time and return the last direction we calculate.
        if (Time.time < internalAvoidTime)
        {
            if (showDebugRays) Debug.DrawRay(transform.position, avoidDirection, Color.magenta);
            return avoidDirection;
        }

        // Define the directions we're gonna make our raycasts.
        rayArray[0] = transform.TransformDirection(-0.2f, 0, 1); //ray pointed slightly left
        rayArray[2] = transform.TransformDirection(0.2f, 0, 1); //ray pointed slightly right
        rayArray[1] = transform.TransformDirection(Vector3.forward); //ray 1 is pointed straight ahead

        if (showDebugRays) Debug.DrawRay(transform.position, rayArray[0] * rayLength, Color.green);
        if (showDebugRays) Debug.DrawRay(transform.position, rayArray[1] * rayLength, Color.green);
        if (showDebugRays) Debug.DrawRay(transform.position, rayArray[2] * rayLength, Color.green);
        
		//loop through the rays
        int i = 0;
        while (i < 3)
        {
            // if the enemy hit something with the ray......
            if (Physics.Raycast(transform.position, rayArray[i], out hit, rayLength))
            {
                if (showDebugRays) Debug.DrawLine(transform.position, hit.point, Color.magenta);
               
				// get the perpendicular vector of the 'wall'
                Vector3 perp = Vector3.Cross(hit.normal, Vector3.up);
                perp.Normalize();
               
				// Get the avoid direction, by adding the ray direction and the perpendicular vector.
                if (i < 2)
                    avoidDirection = rayArray[i] + (perp * (15 / Vector3.Distance(transform.position, hit.point)));
                else
                    avoidDirection = rayArray[i] - (perp * (15 / Vector3.Distance(transform.position, hit.point)));

                // add our original direction to our new raycast direction.
                avoidDirection = avoidDirection + direction;
                if (showDebugRays) Debug.DrawRay(transform.position, avoidDirection, Color.magenta);
               
				// Setup an internal time to avoid doing the raycast every update, so the enemy
                // will go in that avoiddirection for a given time (1 sec).
                internalAvoidTime = Time.time + 1;
                return avoidDirection;
            }
            i++;
        }
        return direction;
    }

    // if collided with some block, jump. The obstacle should be at most, lower than the enemy.
    // So if it's a wall, he will try to avoid the obstacle and not to jump over it.
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        RaycastHit hitInfo = default(RaycastHit);
        if (character.collisionFlags != CollisionFlags.Sides) { return; } // Security sentence
		if (hit.gameObject.tag == "Player") { return; }	// Security sentence

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        // Launch one ray from the enemys position. If true, it's a obstacle in his way.
        if (Physics.Raycast(transform.position, forward, out hitInfo, 1))
        {
            // Launch a seconf ray, from the head of the enemy. If it's false, then the obstacle hasn't too much height, jump over it.
            // If it's true, he will try to avoid the obstacle using "AvoidObstacles" function.
            if (!Physics.Raycast(transform.position + new Vector3(0, 1, 0), forward, out hitInfo, 1))
            {
                // only check lateral collisions
                if (Mathf.Abs(hitInfo.normal.y) < 0.5f)
                {
                    jump = true; // jump if collided laterally
                    internalAvoidTime = Time.time; // Start doing the avoidObstacles function again.
                }
            }
        }
    }

    // Funcion de ataque al payer. Se compone de tres fases
    // 1.- Correr hacia el player hasta alcanzar la distancia minima de disparo.
    // 2.- Disparar hasta matarlo o hasta que huye.
    // 3.- Si huye buscarlo otra vez y si no lo encuentra, volver a patrullar.
    private void AttackPlayer()
    {
        if (CanSeeTarget())
        {
            // Get the angle and distance to the player.
            float distance = Vector3.Distance(transform.position, target.position);
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 targetDirection = lastVisiblePlayerPosition - transform.position;
            targetDirection.y = 0;
            float angle = Vector3.Angle(targetDirection, forward);
            lastVisiblePlayerPosition = target.position;
            
			// Decrease health in pl by proximity (just because the nemy is close to the player).
            if (distance <= movement.dontComeCloserPlayer && Time.time >= timeRateSustractionCheck)
            {
				target.SendMessage("SetDamage", damage.SustractHealth);
                timeRateSustractionCheck = Time.time + damage.SustractRatio;
                ComeCloserTimeCheck = Time.time;
            }

            // If the enemy is in the right position and angle to make a shoot.
            if (distance < attack.shootRange && angle < attack.shootAngle)
            {
                if (!character.isGrounded)
                    ComeCloser(target.position);
                else
                {
                    if (!beingStopped && (Time.time >= ComeCloserTimeCheck))
                        state = AIState.Shoot;
                    else
                    {
                        if (beingStopped && (distance <= movement.dontComeCloserPlayer))
                        {
                            if (showDebug) Debug.Log("AIRobot -> AttackPlayer() -> Firing the Player. -> Enemy:" + this.transform.parent.name);
                            FireEnemy(); // Instantiate the bullet (without animation).
                        }
                        else
                        {
                            if (distance > movement.dontComeCloserPlayer)
                            {
                                if (showDebug) Debug.Log("AIRobot -> AttackPlayer() -> Enemy Coming Closer. -> Enemy:" + this.transform.parent.name);
                                if (Time.time >= ComeCloserTimeCheck)
                                {
                                    beingStopped = false;
                                    movement.speed = movement.runSpeed;
                                    ComeCloserTimeCheck = Time.time + timeStates.ComeCloserTime;
                                    ComeCloser(target.position);
                                }
                                else
                                {
                                    if (showDebug) Debug.Log("AIRobot -> AttackPlayer() -> Coming Closer TimeCheck :" + ComeCloserTimeCheck + " Time : " + Time.time + " -> Enemy:" + this.transform.parent.name);
                                    ComeCloser(target.position);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Chase the player coming close to him.
                if (showDebug) Debug.Log("AIRobot -> AttackPlayer() -> Run After Player. -> Enemy:" + this.transform.parent.name);
                beingStopped = false;
                movement.speed = movement.runSpeed;
                ComeCloser(lastVisiblePlayerPosition);
            }
        }
    }

    // Can I see the Player from here (from my position)
    private bool CanSeeTarget()
    {
        // Return if it is too far from player
        if (Vector3.Distance(transform.position, target.position) > attack.attackRange) { return false; }

		RaycastHit hit = default(RaycastHit);
        int layerMask = 1 << 29;
        layerMask = ~layerMask;
        if (Physics.Linecast(transform.position, target.position, out hit, layerMask))
        {
            // This function is executing every single frame, so there will be a ton of messages in console... 
            // activate ONLY if it's mandatory to find an error or something like that.
            // if (showDebug) Debug.Log("AIRobot -> CanSeeTarget() -> Enemy has a hit in: " + hit.transform.name +" ->Enemy:" + this.transform.parent.name);
            return hit.transform == target;
        }

       	return false;
    }

    // Funcion that search for the player
    private void SearchPlayer(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        if (distance > movement.dontComeCloserPlayer)
            MoveTowards(position, true);
        else
            RotateTowards(position, false);

        if (CanSeeTarget())
            ChaseTimeCheck = Time.time;
    }

    // Funcion that makes the enemy to come a little closer towards the player.
    private void ComeCloser(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        if (distance > movement.dontComeCloserPlayer)
            MoveTowards(position, true);
        else
            RotateTowards(position, false);
    }

    // Change to a "Search Status" knowing the player position even if he never attack before.
    // It will be used to responf attacking the player in case if taking damage from a players shoot
    private void StartChaseStatus()
    {
        lastVisiblePlayerPosition = target.position;
        beingStopped = false;
        movement.speed = movement.runSpeed;
        ChaseTimeCheck = Time.time + timeStates.ChaseTime;
        state = AIState.Chase;
    }

    // Shoot one bullet, doing the 'shoot' animation.
    private void Shoot()
    {
        // Only shoot if cancelShoot is true, if gets false once the shoot animation starts.
        if (cancelShoot)
        {
            movement.speed = 0;
            SendMessage("SetSpeed", 0f);
            beingStopped = true;
            // Start shoot animation
            myAnimation.CrossFade("shoot", timeStates.delayShootTime);
            // Wait until half the animation has played
            waitShootInternalTime = Time.time + (myAnimation["shoot"].length - timeStates.delayShootTime);
            cancelShoot = false;
        }
    }

    // Wait until half the animation has played, stop the animation and shoot one bullet.
    private void WaitAndFire()
    {
        if (cancelShoot) { return; }

        if (Time.time >= waitShootInternalTime)
        {
            myAnimation.Stop();
            FireEnemy();
            cancelShoot = true;
        }
    }

    // Perform the bullet instantiation.
    private void FireEnemy()
    {
        if (Time.time > nextFire)
        {
            GameObject CloneFire = PoolSystem.instance.Spawn(Bullet, FirePos.position, this.transform.rotation);
            CloneFire.transform.LookAt(target.position);
            if (FireSound != null)
                FireSound.Play();

			EnemyBulletMngr aiScript = CloneFire.GetComponent<EnemyBulletMngr>();

			Rigidbody bulletBody = CloneFire.GetComponent<Rigidbody>();
            bulletBody.transform.LookAt(target.position);
            bulletBody.linearVelocity = Vector3.zero;
            bulletBody.angularVelocity = Vector3.zero;

            Vector3 fireDirection = CloneFire.transform.forward * aiScript.aceleracion;

            bulletBody.AddForce (fireDirection, ForceMode.VelocityChange);
            nextFire = Time.time + aiScript.shootRatio;
        }
    }

}