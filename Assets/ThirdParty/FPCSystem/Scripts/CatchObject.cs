
using UnityEngine;
using System.Collections;

public class CatchObject : MonoBehaviour
{
	[Space(5)]
    [Tooltip("Can pl catch an object?")]
    public bool canCatch = true; // Can the player catch objects (only those with rigidbody)

    [Tooltip("Maximum distance to be able to catch an object.")]
    public float maxCatchDistance = 3f; // What is the minimun ditance to be able to catch an object?

    [Tooltip("Can pl drag towards itself an object (like a gravity gun)?")]
    public bool canDrag = true; // Can the player catch objects (only those with rigidbody)

    [Tooltip(": Maximum distance to be able to drag an object.")]
    public float maxDragDistance = 20; // What is the minimun ditance to be able to catch an object?

    [Tooltip("Force applied to the object being 'attract' by pl.")]
    public float dragForce = 5f; // What is the minimun ditance to be able to catch an object?

    [Tooltip("Force applied to the object when launching it.")]
    public float launchForce = 15f; // The force will use to launch the object forward.

    [Tooltip("Force applied to the object when dropping it.")]
    public float dropForce = 2f; // The force will use to drop the object.

    [Space(5)]
    [Tooltip("GameObject that will be used as an anchor, so the catched (dragged) object will be placed in this gameobject's position. NOTE: If no gameobject is provided, it will be created using the following var (Catch Position) otherwise, Catch Position will be ignored.")]
    public GameObject catchHelper; // Helper GameObject that can be created in execution Time.

     // Any cached object will be a 'son' of this object
     // so we can move around with it.
    [Tooltip("Final position where the scene object will be placed when catched or dragged.")]
	public Vector3 catchPosition = new Vector3(0, 0.3f, 1.5f); // Relative position (regarding the player) of the object once catched.

    [Space(5)]
    [Tooltip("Can the catched object detect collision in the scene?. If 'positive' and if a collision is detected pl will lose (drop) the catched object.")]
	public bool detectCollisions = true; // is the object catched be able to detect surrounding collisions?

    [Tooltip("Start detecting collision only when the object is catched (or it can detect them when is being catched, before isCatched is true).")]
	public bool onlyWhenCatched = true; // Start to detect collisions only when isCatched is enabled

     // or do it without waiting (while isCatching is still enable).
    private CatchedCollision catchedCollisionScript; // Script attached to cached Object to detect the collisons.

    [Space(5)]
    [Tooltip("Time the IsCatching status will be active until it changes to false (so IsCatched status will be true).\nNOTE: Included because should be useful if using some character animations.")]
	public float catchingTime = 1f; // The time will use to catch the object.

    [Tooltip("Time the IsLaunching status will be active until it changes to false (so no object will be in player possession, all internal catch, drag status will be false). NOTE: Included because should be useful if using some character animations.")]
	public float launchingTime = 1f; // The time will use to launch the object forward.


    // =======================================
    // Private catching states. 
    // For now there will be private vars and will not be a part of the Status script.
    // The reason why is because i dont wanna mix the usual player status with this kind of object interaction.
    private bool isCatching = false; // is the pl catching an object?
	private bool isDragging = false; // is the pl dragging an object?
	private bool isCatched = false; // Have the pl catched an object?
	private bool isLaunching = false; // is the pl launching an object?
	private bool isDroping = false; // is the pl droping an object?

    // =======================================
    private GameObject catchObject; // Object catched.
	private Rigidbody catchRigidbody; // Object catched.
	private Core coreScr;

    private Transform CameraTransform;  // Get the camera transform for performance reasons.


    //====================================================================================================
    // Public funcions to get the launch status.
    public bool IsCatching(){ return isCatching; }

    public bool IsDragging(){ return isDragging; }

    public bool IsCatched(){ return isCatched; }

    public bool IsLaunching(){ return isLaunching; }

    public bool IsDroping(){ return isDroping; }

    public GameObject GetCatchedObject(){ return catchObject; }


    //====================================================================================================
    // Creation of our helper object as inicialization.
    void Start()
    {
        CameraTransform = Camera.main.transform;
        coreScr = GetComponent<Core>();

        if (catchHelper == null)
        {
            catchHelper = new GameObject("Catch Helper GameObject");
            catchHelper.transform.parent = Camera.main.transform;
            catchHelper.transform.localPosition = catchPosition;
            catchHelper.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
		if (!canCatch) { return; } // if the player an catch anything, just return.
		if ((isCatching || isLaunching) || isDroping) { return; } // if has passed the time assigned to catch or launch something, just return;


        RaycastHit hit = default(RaycastHit);

        // We catch and launch objects using the 'Fire1' button.
        if (InputManager.instance.fire2Key.isDown)
        {
            // Catch an object ot drag it towards us.
            if (!isCatched)
            {
                Vector3 dir = CameraTransform.forward;
                int layer = ~(1 << LayerMask.NameToLayer("Player"));

                // Mark an object as catched.
                if ((!isCatching && !isDragging) && Physics.Raycast(CameraTransform.position, dir, out hit, maxCatchDistance, layer))
                {
					if (hit.transform.gameObject != coreScr.GetObjectBelow())
                    {
                        Rigidbody body = hit.collider.attachedRigidbody;
                        // no there's rigidbody. It can't be cached.
                        if (body != null)
                        {
                            if (body.isKinematic) { return; }

                            CatchObjectOptions objLaunchOptions = hit.collider.gameObject.GetComponent<CatchObjectOptions>();
                            // Catch the object (so we parent it with our helper object)
                            if ((objLaunchOptions == null) || objLaunchOptions.canBeCatched)
                            {
                                isCatching = true;
                                Invoke("EnableCatch", catchingTime);
                                gameObject.BroadcastMessage("PlayCatch");
                                catchObject = hit.transform.gameObject;
								catchRigidbody = catchObject.GetComponent<Rigidbody>();
                                catchObject.transform.parent = catchHelper.transform;
                                catchObject.transform.localPosition = catchHelper.transform.localPosition;
								catchRigidbody.useGravity = false;
								catchRigidbody.isKinematic = true;
                                if (!onlyWhenCatched)
                                    ConfigureCatchedObjectCollisions();
                            }
                        }
                    }
                }

                // Start dragging the catched object towards us.
                if ((!isCatching && !isDragging) && Physics.Raycast(CameraTransform.position, dir, out hit, maxDragDistance, layer))
                {
					if (!canDrag) { return; } // if the Player can't drag anything, just return.

					if (hit.transform.gameObject != coreScr.GetObjectBelow())
                    {
						Rigidbody body = hit.collider.attachedRigidbody;
                        // no there's rigidbody. It can't be cached.
                        if (body != null)
                        {
                            if (body.isKinematic) { return; }

							CatchObjectOptions objLaunchOptions = hit.collider.gameObject.GetComponent<CatchObjectOptions>();
                            // Catch the object (so we parent it with our helper object)
                            if ((objLaunchOptions == null) || objLaunchOptions.canBeDragged)
                            {
                                isDragging = true;
                                //Invoke("EnableCatch", catchingTime * 3);
                                catchObject = hit.transform.gameObject;
								catchRigidbody = catchObject.GetComponent<Rigidbody>();
								//catchRigidbody.useGravity = false;
								catchRigidbody.linearVelocity = -CameraTransform.forward * launchForce;
                            }

                            gameObject.BroadcastMessage("PlayDrag");
                        }
                    }
                }
            }
        }

        // If we are dragging an object towards us.
        if (isDragging)
        {
            if (InputManager.instance.fire2Key.isPressed)
            {
                if (Vector3.Distance(catchObject.transform.position, catchHelper.transform.position) < 2f)
                {
					catchRigidbody.linearVelocity = Vector3.zero;
                    catchObject.transform.parent = catchHelper.transform;
                    catchObject.transform.localPosition = catchHelper.transform.localPosition;
					catchRigidbody.useGravity = false;
					catchRigidbody.isKinematic = true;
                    gameObject.BroadcastMessage("PlayCatch");
                    if (!onlyWhenCatched)
                        ConfigureCatchedObjectCollisions();

                    EnableCatch();
                }
                else
                {
					CatchObjectOptions objLaunchOptions = catchObject.GetComponent<CatchObjectOptions>();
                    if (objLaunchOptions == null)
						catchRigidbody.AddForce(-CameraTransform.forward * dragForce, ForceMode.Force);
                    else
						catchRigidbody.AddForce(-CameraTransform.forward * objLaunchOptions.dragForce, ForceMode.Force);
                }
            }
            else
            {
                DisableLaunching();
            }
        }

        // if we already have the object in our power.
        if (isCatched)
        {
            ConfigureCatchedObjectCollisions();
            // Launch an already cached object.
            if (InputManager.instance.fire1Key.isDown)
            {
				CatchObjectOptions objLaunchOptions = catchObject.GetComponent<CatchObjectOptions>();
				catchRigidbody.useGravity = true;
				catchRigidbody.isKinematic = false;
                catchObject.GetComponent<Collider>().enabled = true;
                catchObject.transform.parent = null;
                if (catchedCollisionScript != null)
                      UnityEngine.Object.Destroy(catchedCollisionScript);
  
                if (objLaunchOptions == null)
					catchRigidbody.linearVelocity = CameraTransform.forward * launchForce;
                else
					catchRigidbody.linearVelocity = CameraTransform.forward * objLaunchOptions.launchForce;

                isCatched = false;
                isLaunching = true;
                gameObject.BroadcastMessage("PlayLaunch");
                Invoke("DisableLaunching", launchingTime);
            }

            // Drop the cached object with a defined dropforce force.
            if (InputManager.instance.fire2Key.isDown)
            {
				catchRigidbody.useGravity = true;
				catchRigidbody.isKinematic = false;
                catchObject.GetComponent<Collider>().enabled = true;
                catchObject.transform.parent = null;
                if (catchedCollisionScript != null)
                    UnityEngine.Object.Destroy(catchedCollisionScript);

				//catchRigidbody.AddForce(CameraTransform.forward * dropForce , ForceMode.Force);
                catchRigidbody.linearVelocity = CameraTransform.forward * dropForce;
                isCatched = false;
                isDroping = true;
                gameObject.BroadcastMessage("PlayDrop");
                Invoke("DisableLaunching", launchingTime);
            }
        }
    }

    // If detect Collisions is enabled, we modify the catched object to do it.
    private void ConfigureCatchedObjectCollisions()
    {
        if (detectCollisions)
        {
			catchRigidbody.isKinematic = false;
			catchRigidbody.linearVelocity = Vector3.zero;
			catchRigidbody.angularVelocity = Vector3.zero;
            if (catchedCollisionScript == null)
                catchedCollisionScript = catchObject.AddComponent<CatchedCollision>();

        }
        else
        {
            if (catchObject.GetComponent<Collider>().enabled == true)
                catchObject.GetComponent<Collider>().enabled = false;
        }
    }

    // Enable isCatched status and disable the others.
    private void EnableCatch()
    {
        isCatching = false;
        isDragging = false;
        isCatched = true;
        gameObject.BroadcastMessage("StopDrag");
    }

    // Disable all status, because the catched object has benn launched or dropped.
    private void DisableLaunching()
    {
        isCatching = false;
        isDragging = false;
        isCatched = false;
        isLaunching = false;
        isDroping = false;
        catchObject = null;
		catchRigidbody = null;
        catchedCollisionScript = null;
        gameObject.BroadcastMessage("StopDrag");
    }

    // Function called if chached gameobject detects a collision, in that case we drop the object.
    public void DropCachedObject()
    {
        CancelInvoke("EnableCatch");
		catchRigidbody.useGravity = true;
		catchRigidbody.isKinematic = false;
        catchObject.GetComponent<Collider>().enabled = true;
        catchObject.transform.parent = null;
        if (catchedCollisionScript != null)
            UnityEngine.Object.Destroy(catchedCollisionScript);

		//catchRigidbody.AddForce(CameraTransform.forward * dropForce, ForceMode.Force);
        catchRigidbody.linearVelocity = CameraTransform.forward * dropForce;
        isCatched = false;
        isDroping = true;
        Invoke("DisableLaunching", launchingTime);
    }

}