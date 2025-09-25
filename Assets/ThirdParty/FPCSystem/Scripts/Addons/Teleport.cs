
using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour
{
    [Tooltip("Enable/Disable the teleport platform.")]
	public bool isEnabled = true; // is the platform enabled?

	private bool isActive = true; // is the platform active? The platform reactivates itself at a given time.

    [Tooltip("Teleport detination point. Can be another teleport platform or not.")]
    public Transform destination; // Destination position	

    [Tooltip("This teleport platform will work once (so it can be used again)")]
    public bool OneUseOnly = false;

    [ShowWhen("OneUseOnly", ShowWhenAttribute.Condition.Equals, false)]
    [Tooltip("This teleport platform will work only in one directon?")]
    public bool OneDirection = false;

    [Tooltip("Wait time to make the teleportation.")]
	public float waitTime = 0.1f; // Wait time to make the Teleport.

    [Tooltip("Wait time reactivate the platform after making a teleportation.")]
	public float reactivateTime = 1; // Platform reactivate time when a teleportation has been made.

    [Space(10)]
    [Tooltip("Teleportation sound effect.")]
    public AudioClip teleportClip;

    [Tooltip("Full screen visual effect fade time when teleporting.")]
	public float effectFadeTime = 2; // Fade time of the fullscreen teleportation visual effect

    private GameObject MyObj;
	private Core coreScr;
    //private Status StatusSrc;
    private Transform MyTransform;
    private bool isOverTeleport; // is the pl over this jumper platform?
    private Vector3 MyPosition;
    private GameObject MiGUITransport;


    // This function activate/deactivate the platform.
    // Is it is being deactivate, it will reactivate again some time later.
    public void ActivateTeleport(bool _active)
    {
        //Debug.Log("Deactivating"+name+" Destination: "+destination);
        isActive = _active;
        if (!_active && !OneDirection)
            StartCoroutine("ReactivateTeleport" ,reactivateTime*3);
    }

    // Initialize our platform, scotty
    void Start()
    {
        MiGUITransport = GameObject.Find("Teleport Image");
        if (!MiGUITransport)
            Debug.LogWarning("Can't find Teleport GUI element!!");

        if (destination == null)
            Debug.LogWarning("Can't find the Teleport destination!!");

		MyObj = GameObject.FindWithTag("Player"); //GameObject.Find("Player");
		if(MyObj == null)
		{
			Debug.LogError("Player NOT Found!");
		}
		else
		{
			coreScr = MyObj.GetComponent<Core>();
	        MyTransform = MyObj.transform;
	        MyPosition = destination.position + (destination.up * 2);
		}

        // If the Teleport can be used only once, then One Direction has to be false 
        // (you can't use it twice, so you can return from destination).
        if (OneUseOnly)
            OneDirection = true;
    }

    // Detect if pl is over tthe platform
    void Update()
    {
		if (coreScr.GetObjectBelow() == gameObject && !isOverTeleport)
            SetOverCube(true);
        else if (coreScr.GetObjectBelow() != gameObject && isOverTeleport)
            SetOverCube(false);
    }

    // Cange the platform status and decide if make a Teleport or reactivate the patform (if it is deactivated previously).
    public void SetOverCube(bool _isOver)
    {
        isOverTeleport = _isOver;
        if (!isEnabled) { return; }
        if (destination == null) { return; }

        if (isActive && isOverTeleport)
            StartCoroutine(MakeTeleTransport());
        else 
		if (!isActive && !isOverTeleport)
            StartCoroutine(ReactivateTeleport(0.5f));
    }

    // Timed platform reactivator
    private IEnumerator ReactivateTeleport(float _reactivateTime)
    {
        yield return new WaitForSeconds(_reactivateTime);
        isActive = true;
    }

    // Make the teleTransport.
    // Move our Player.
    // Deactivate for some time the Teleport (origin and destiny is it exists)
	private IEnumerator MakeTeleTransport()
    {
        if (!isEnabled) { yield break; }

        // Deactivate the destination platform (if it is a platform).
        // Otherwaise it will teleport us back again to this platform (the origin)	
        //Debug.Log("MakeTeleTransport"+name+" Destination: "+destination);
        destination.SendMessage("ActivateTeleport", false, SendMessageOptions.DontRequireReceiver);
        yield return new WaitForSeconds(waitTime);
        MyTransform.position = MyPosition; // Move pl to the destination point. LOL.
		MyObj.BroadcastMessage("PlaySound", teleportClip); // Play the sound effect.
        
		// Full Screen Effect
        if (MiGUITransport)
        {
            MiGUITransport.SendMessage("SetDamageFadeTime", effectFadeTime);
            MiGUITransport.SendMessage("ShowGUIDamage");
        }
        // Deactivate the origin teleport platform for a short time.
        isOverTeleport = false;
        isActive = false;

        // If it's only to be used once, deactivate the teleportPlatform
        if(OneUseOnly)
            isEnabled = false;
        else 
            StartCoroutine(ReactivateTeleport(reactivateTime));
    }

    public void OnDrawGizmosSelected()
    {
        if (destination != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, destination.position);
        }
    }

}
 