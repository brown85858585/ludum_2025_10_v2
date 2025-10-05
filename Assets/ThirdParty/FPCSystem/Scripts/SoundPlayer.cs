
using UnityEngine;
using System.Collections;

[System.Serializable]
public class StepSnd
{
    public AudioClip[] defaultSteps;
    public AudioClip[] woodSteps;
    public AudioClip[] metalSteps;
    public AudioClip[] concreteSteps;
    public AudioClip[] sandSteps;
    public AudioClip[] waterSteps;
    public AudioClip[] wallSteps;
}

[System.Serializable]
public class TerrainStepSnd
{
    public AudioClip[] texture0Steps;
    public AudioClip[] texture1Steps;
    public AudioClip[] texture2Steps;
    public AudioClip[] texture3Steps;
    public AudioClip[] texture4Steps;
    public AudioClip[] texture5Steps;
    public AudioClip[] texture6Steps;
    public AudioClip[] texture7Steps;
    public AudioClip[] texture8Steps;
    public AudioClip[] texture9Steps;
}

[System.Serializable]
public class WalkSnd
{
    public float inAirDelay = 0.7f;
    public AudioClip jumpSound;
    public AudioClip fallSoundNormal;
    public AudioClip fallSoundHigh;
    public AudioClip fallDamageSound;
    public AudioClip kickSound;
    public AudioClip sideKickSound;
    public AudioClip aboveKickSound;
    public AudioClip sprintBreathSound;
}

[System.Serializable]
public class ProneSnd
{
    public AudioClip downUpSound;
    public AudioClip proneSound;
    public AudioClip slideSound;
}

[System.Serializable]
public class CrouchSnd
{
    public AudioClip downUpSound;
    public AudioClip crouchSound;
    public AudioClip slideSound;
}

[System.Serializable]
public class LadderSnd
{
    public AudioClip ladderSoundDefault;
    public AudioClip ladderSoundConcrete;
    public AudioClip ladderSoundMetal;
    public AudioClip ladderSoundWood;
}

[System.Serializable]
public class LedgeSnd
{
    [Tooltip("Ratio to play the ledge movement sound (used when the moves on a ledge).")]
    public float ledgeInterval = 1.3f;
    public AudioClip ledgeSoundDetect;
    public AudioClip ledgeSoundWalk;
    public AudioClip ledgeSoundClimb;
    public AudioClip lowLedgeSoundClimb;
}

[System.Serializable]
public class SwimSnd
{
    [Tooltip("Ratio to play the swimming sound (used to swimming using a linear speed - not when using strokes).")]
    public float swimInterval = 2;
    [Tooltip("Ratio to play the swimming 'fast' sound (used to swimming using a linear speed - not when using strokes).")]
    public float runInterval = 1;
    public AudioClip swimSound;
    public AudioClip waterIn;
    public AudioClip waterOut;
    public AudioClip headIn;
    public AudioClip headOut;
    public AudioClip drownHurt;
}

[System.Serializable]
public class InteractiveSnd
{
    public AudioClip dragSound;
    public AudioClip catchSound;
    public AudioClip launchSound;
    public AudioClip dropSound;
}


public class SoundPlayer : MonoBehaviour
{
    [Header("Basic Step Sounds")]
	public StepSnd stepSounds = new StepSnd();
	public TerrainStepSnd terrainStepSounds = new TerrainStepSnd();

    [Header("Extended Sounds")]
    public WalkSnd walk = new WalkSnd();
	public ProneSnd prone = new ProneSnd();
	public CrouchSnd crouch = new CrouchSnd();
	public LadderSnd ladder = new LadderSnd();
	public LedgeSnd ledge = new LedgeSnd();
	public SwimSnd swim = new SwimSnd();
	public InteractiveSnd interaction = new InteractiveSnd();

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;
    [Tooltip("Enable/Disable all 'Debug.DrawRay' messages from this script.")]
    public bool showDebugRay = false;

    private GameObject MyObj;
	private Status statusSrc;
    private CharacterMotor motor;
    //private CharacterController controller;
    private Core coreScr;

    private float interval = 0.5f; // Important 0.5 secs in silence.just when everything configures itself.
	private float sprintBreathInterval = 0.5f; // Important 0.5 secs in silence.just when everything configures itself.
    private bool JumpPlayed;

	private float rocketInterval = 0.5f;
    private AudioSource rocketAS;

    private AudioSource flyAirAS;
    private AudioSource LedgeAS;
    private AudioClip platformClip;
    private AudioSource dragAS;

    ///=================================================================
    ///
    /// Public Functions called by other scripts.
    ///
    ///=================================================================
    public void SetPlatformSound(string cTag)
    {
        if (cTag.Contains("wood"))
            platformClip = stepSounds.woodSteps[Random.Range(0, stepSounds.woodSteps.Length)];
		else if (cTag.Contains("metal"))
			platformClip = stepSounds.metalSteps[Random.Range(0, stepSounds.metalSteps.Length)];
		else if (cTag.Contains("concrete"))
			platformClip = stepSounds.concreteSteps[Random.Range(0, stepSounds.concreteSteps.Length)];
		else if (cTag.Contains("dirt"))
			platformClip = stepSounds.sandSteps[Random.Range(0, stepSounds.sandSteps.Length)];
		else  if (cTag.Contains("sand"))
			platformClip = stepSounds.sandSteps[Random.Range(0, stepSounds.sandSteps.Length)];
		else  if (cTag.Contains("water"))
			platformClip = stepSounds.waterSteps[Random.Range(0, stepSounds.waterSteps.Length)];
		else
			platformClip = stepSounds.sandSteps[Random.Range(0, stepSounds.sandSteps.Length)];
    }

    // generic function called from whatever you need to play any kind of sound effect.
    public AudioSource PlaySound(AudioClip _clip) { return PlayClip(_clip); }

    public AudioSource PlayLocatedSound(AudioClip _clip, Vector3 _position) { return PlayLocatedClip(_clip, _position); }
    
    // Function called by Status when jumping.
    public void PlayJump() { PlayClip(walk.jumpSound); }

    // Function called by Status when kick happens.
    public void PlaySimpleKick() { PlayClip(walk.kickSound); }

    // Function called by Status when side kick happens.
    public void PlaySideKick() { PlayClip(walk.sideKickSound); }

    // Function called by Status when kick happens over an enemy.
    public void PlayAboveKick() { PlayClip(walk.aboveKickSound); }

    // Function called by Status when Fall Hurt happens.
    public void PlayFallHurt() { PlayClip(walk.fallDamageSound); }

    // Function called by Status when Fall Hurt happens.
    public void PlayFallHigh() { PlayClip(walk.fallSoundNormal); }

    // Function called by Status when Fall Hurt happens.
    public void PlayFallNormal() { PlayClip(walk.fallSoundNormal); }

    // Function called by FOCStatus script when player is crouching .
    public void CrouchGoingDownUP() { PlayClip(crouch.downUpSound); }

    // Function called by FOCStatus script when player is crouching .
    public void ProneGoingDownUP() { PlayClip(prone.downUpSound); }

    // Function called by Underwater script when player get the surface .
    public void PlayBreath() { PlayClip(swim.headOut); }

    // Function called by Underwater script when player start diving.
    public void PlayDrown() { PlayClip(swim.headIn); }

    // Function called by Status when Fall Hurt happens.
    public void DrownHurt() { PlayClip(swim.drownHurt); }

    // Function called by LedgeJS when ledge is detected (hit sound?).
    public void LedgeDetect() { PlayClip(ledge.ledgeSoundDetect); }

    // Function called by LedgeJS when the player is climbing in the ledge.
    public void LedgeClimb() { PlayClip(ledge.ledgeSoundClimb); }

    // Function called by LedgeJS when the pl is climbing in the lowledge.
    public void LowLedgeClimb() { PlayClip(ledge.lowLedgeSoundClimb); }

    // Function called by Core when the pl is sliding & proning.
    public void PlayProneSlide() { PlayClip(prone.slideSound); }

    // Function called by Core when the player is sliding & proning.
    public void PlayCrouchSlide() { PlayClip(crouch.slideSound); }



    // Function call by Swim script, when player enter in the water (or get out).
    public void PlayWaterInOut(bool _value)
    {
        if (_value)
            PlayClip(swim.waterIn);
        else
            PlayClip(swim.waterOut);
    }

    // Function called by Status when sprint stops consuming all the time.
    public void PlaySprintBreath()
    {
        if (statusSrc.isClimbing || statusSrc.isJumping) { return; }

        if (Time.time > sprintBreathInterval)
        {
            PlayClip(walk.sprintBreathSound);
            sprintBreathInterval = (Time.time + walk.sprintBreathSound.length) + 0.1f;
        }
    }

    // Interaction sound calls.
    // Function called by CatchObjectJS when the player has catched an object.
    public void PlayCatch() { PlayClip(interaction.catchSound); }

    // Function called by CatchObjectJS when the player has catched an object.
    public void PlayDrag() { dragAS = PlayClip(interaction.dragSound); }

    // Function called by CatchObjectJS when the player has catched an object.
    public void StopDrag()
    {
        if (dragAS != null)
        {
            dragAS.Stop();
            dragAS = null;
        }
    }

    // Function called by CatchObjectJS when the player has catched an object.
    public void PlayLaunch() { PlayClip(interaction.launchSound); }

    // Function called by CatchObjectJS when the player has catched an object.
    public void PlayDrop() { PlayClip(interaction.dropSound); }

    ///=================================================================
    ///
    /// Regular Functions of Sound Manager.
    ///
    ///=================================================================
    void Awake()
    {
		MyObj = GameObject.FindWithTag("Player"); // other way: GameObject.Find("Player");
		if(MyObj == null)
		{
			Debug.LogError("SoundPlayer -> Player NOT Found!");
            return;
		}

		transform.parent = MyObj.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		statusSrc = MyObj.GetComponent<Status>();
		statusSrc.SetSoundPlayerSrc(this);
    }

	void Start()
	{
		coreScr = statusSrc.GetCoreScr();
		//controller = StatusSrc.GetController();
        motor = statusSrc.GetMotor();
    }

    void OnEnable()
    {
        EventManagerv2.instance.StartListening("CameraRachedBobEnd", PlayFootStep);
    }

    void OnDisable()
    {
        if (!EventManagerv2.IsDestroyed)
        {
            EventManagerv2.instance.StopListening("CameraRachedBobEnd", PlayFootStep);
        }
    }


    void PlayFootStep(EventParam eventParam)
    {
        // Make sure we move are the are grounded or wallrunning (not in air, flying, ladder or ledge).
        if (!statusSrc.isStop && !statusSrc.isInLadder && !statusSrc.isInLedge &&
            (motor.IsGrounded() || statusSrc.isWallRunning))
        {
            // walk & run sounds. Prone & crouch too
            if (!statusSrc.isCrouched && !statusSrc.isProned)
            {
                AudioClip MyAudio = null;

                // Choose the audio & sound interval if we are wallrunning or in the ground
                if (!statusSrc.isWallRunning)
                {
                    MyAudio = GetStepAudio();
                    PlayClip(MyAudio);
                }
                else
                {
                    MyAudio = stepSounds.wallSteps[Random.Range(0, stepSounds.wallSteps.Length)];
                    PlayClip(MyAudio);
                }
            }
            else if (statusSrc.isProned && !statusSrc.isStop)
            {
                if (!statusSrc.isSliding) PlayClip(prone.proneSound);
            }
            else if (statusSrc.isCrouched && !statusSrc.isStop)
            {
                if (!statusSrc.isSliding) PlayClip(crouch.crouchSound);
            }

            if (JumpPlayed)
                JumpPlayed = false;
        }
        else
        {
            // Ladder sound
            if (statusSrc.isInLadder && statusSrc.isWalking)
            {
                AudioClip ladderAudio = GetLadderAudio();
                PlayClip(ladderAudio);
                if (JumpPlayed)
                    JumpPlayed = false;
            }
        }
    }

    private AudioSource swimFXAS;
    void Update(){

        // Emergency code to stop a ledge movement sound when we stop moving laterally or when jump is detected 
        if (LedgeAS != null && (statusSrc.isJumping || statusSrc.isStop))
        {
            LedgeAS.Stop();
            LedgeAS = null;
            interval = 0;
        }

        // if Player is not moving, let all sounds to finish.
        if (statusSrc.isStop && !statusSrc.isJumping)
        {
            JumpPlayed = false;
            
            // If we were walking laterally in a ledge, stop the walk sound. (Again: emergency code to rescue!).
            if (statusSrc.isInLedge && LedgeAS != null)
                LedgeAS.Stop();
           
			return;
        }
        else
        {

            // Walk, prone, crouch, climb(ladder), swim & dive effects
            // All depends on an interval that makes them repeat to achieve the desired step effect or depends on camera bobbing movement
            // to sincronize the steps sound with the camera movement.

            // Sounds that depends on an interval (with no camera bob to tell us when to pay an step sound).
            // Flying, hang on ledge lateral movements, swimming & diving).
            if (Time.time > interval)
            {
                if (statusSrc.isInLedge && statusSrc.isWalking && !statusSrc.isJumping)
                {
                    //var ladderAudio : AudioClip = GetLadderAudio();
                    LedgeAS = PlayClip(ledge.ledgeSoundWalk);
                    interval = Time.time + ledge.ledgeInterval;
                    if (JumpPlayed)
                        JumpPlayed = false;
                }
            }
        }
    }

    // Plays a sound clip. If it isn't any free, create one to play the effect and destroy it.
    public AudioSource PlayClip(AudioClip _clip)
    {
        if (_clip == null) { return null; }

		AudioSource AS = SoundManager.instance.FindFreeAS();
        if (AS != null)
        {
            AS.transform.parent = this.transform;
            AS.transform.localPosition = Vector3.zero;
            AS.clip = _clip;
            if (AS.enabled)
                AS.Play();
            else
                Debug.Log("WARNING: This AS is Disabled. I can't not play it. Ignoring...");
        }
        else
        {
            if(showDebug) Debug.Log("SoundPlayer -> Playing clip :"+_clip.name);
            AS = SoundManager.instance.ForceAS();
            if (AS != null)
            {
                if (showDebug) Debug.LogWarning("SoundPlayer -> NOT free AS to play sound effect. Force Play");
                AS.transform.localPosition = Vector3.zero;
                ForcePlay(AS, _clip);
            }
        }
        return AS;
    }

    // Plays a sound clip. If it isn't any free, create one to play the effect and destroy it.
    public AudioSource PlayLocatedClip(AudioClip _clip, Vector3 position)
    {
        if (_clip == null) { return null; }

        AudioSource AS = SoundManager.instance.FindFreeAS();
        if (AS != null)
        {
            AS.transform.parent = null;
            AS.transform.position = position;
            AS.clip = _clip;
            if (AS.enabled)
                AS.Play();
            else
                Debug.Log("WARNING: This AS is Disabled. I can't not play it. Ignoring...");
        }
        else
        {
            if (showDebug) Debug.Log("SoundPlayer -> Playing clip :" + _clip.name);
            
            AS = SoundManager.instance.ForceAS();
            if (AS != null)
            { 
                if (showDebug) Debug.LogWarning("SoundPlayer -> NOT free AS to play sound effect. Force Play");
                AS.transform.position = position;
                ForcePlay(AS, _clip);
            }
        }
        return AS;
    }



    // Play an effect even if there isn't any AudioSource available.
    // It will create an AudioSource, play the clip and destroy the AudioSource when the clip finished playing.
    public void ForcePlay(AudioSource AS, AudioClip clip)
    {
        AS.clip = clip;
        AS.volume = 0.7f;
        if (AS.enabled)
            AS.Play();
        else
            Debug.Log("WARNING: This AS is Disabled. I can't not play it. Ignoring...");
        UnityEngine.Object.Destroy(AS.gameObject, clip.length);
    }

    public AudioClip GetStepAudio()
    {
		string cTag = string.Empty;
        AudioClip myClip = null;

		cTag = coreScr.GetTagBelow().ToLower();
        if (showDebug) Debug.Log("SoundPlayer -> GetStepAudio() -> GetGroundTag: " + cTag);

        if (cTag.Contains("Untagged"))
            myClip = stepSounds.defaultSteps[Random.Range(0, stepSounds.defaultSteps.Length)];
        else if (cTag.Contains("") || cTag.IsNullOrEmpty() || cTag.IsNullOrWhiteSpace())
            myClip = stepSounds.defaultSteps[Random.Range(0, stepSounds.defaultSteps.Length)];
        else if (cTag.Contains("wood"))
            myClip = stepSounds.woodSteps[Random.Range(0, stepSounds.woodSteps.Length)];
        else  if (cTag.Contains("metal"))
			myClip = stepSounds.metalSteps[Random.Range(0, stepSounds.metalSteps.Length)];
		else if (cTag.Contains("concrete"))
			myClip = stepSounds.concreteSteps[Random.Range(0, stepSounds.concreteSteps.Length)];
		else  if (cTag.Contains("dirt"))
			myClip = stepSounds.sandSteps[Random.Range(0, stepSounds.sandSteps.Length)];
		else if (cTag.Contains("sand"))
			myClip = stepSounds.sandSteps[Random.Range(0, stepSounds.sandSteps.Length)];
		else  if (cTag.Contains("water"))
			myClip = stepSounds.waterSteps[Random.Range(0, stepSounds.waterSteps.Length)];
		else if (cTag.Contains("platform"))
			myClip = platformClip;
		else if (cTag.Contains("terrain"))
		{
            int index = TerrainTextureDetector.GetMainTexture(transform.position);
            switch (index)
            {
                case 0:
                    myClip = terrainStepSounds.texture0Steps[Random.Range(0, terrainStepSounds.texture0Steps.Length)];
                    break;
                case 1:
                    myClip = terrainStepSounds.texture1Steps[Random.Range(0, terrainStepSounds.texture1Steps.Length)];
                    break;
                case 2:
                    myClip = terrainStepSounds.texture2Steps[Random.Range(0, terrainStepSounds.texture2Steps.Length)];
                    break;
                case 3:
                    myClip = terrainStepSounds.texture3Steps[Random.Range(0, terrainStepSounds.texture3Steps.Length)];
                    break;
                case 4:
                    myClip = terrainStepSounds.texture4Steps[Random.Range(0, terrainStepSounds.texture4Steps.Length)];
                    break;
                case 5:
                    myClip = terrainStepSounds.texture5Steps[Random.Range(0, terrainStepSounds.texture5Steps.Length)];
                    break;
                case 6:
                    myClip = terrainStepSounds.texture6Steps[Random.Range(0, terrainStepSounds.texture6Steps.Length)];
                    break;
                case 7:
                    myClip = terrainStepSounds.texture7Steps[Random.Range(0, terrainStepSounds.texture7Steps.Length)];
                    break;
                case 8:
                    myClip = terrainStepSounds.texture8Steps[Random.Range(0, terrainStepSounds.texture8Steps.Length)];
                    break;
                case 9:
                    myClip = terrainStepSounds.texture9Steps[Random.Range(0, terrainStepSounds.texture9Steps.Length)];
                    break;
                default:
                    myClip = terrainStepSounds.texture0Steps[Random.Range(0, terrainStepSounds.texture0Steps.Length)];
                    break;
            }
        }
		else
			myClip = stepSounds.defaultSteps[Random.Range(0, stepSounds.defaultSteps.Length)];
		
        return myClip;
    }

    public AudioClip GetLadderAudio()
    {
        RaycastHit hit = default(RaycastHit);
        int ladderLayer = LayerMask.NameToLayer("Ladder");
        int hitLayer = 1 << ladderLayer; // Ignore ladder
        hitLayer = ~hitLayer;
        string cTag = "";
        Vector3 dir = transform.TransformDirection(Vector3.forward);

        if(showDebugRay) Debug.DrawRay(transform.position + dir*0.5f, dir*5.0f);
        if (Physics.Raycast(transform.position,  dir, out hit, 1, hitLayer))
            cTag = hit.collider.tag.ToLower();

        if (showDebug) Debug.Log("SoundPlayer -> GetLadderAudio() -> GetLadderTag: " + cTag +" in the object: "+hit.collider.name);
        AudioClip myClip = null;
        switch (cTag)
        {
            case "wood":
                myClip = ladder.ladderSoundWood;
                break;
            case "metal":
                myClip = ladder.ladderSoundMetal;
                break;
            case "concrete":
                myClip = ladder.ladderSoundConcrete;
                break;
            default:
                myClip = ladder.ladderSoundDefault;
                break;
        }
        return myClip;
    }

}