using UnityEngine;
using System.Collections;

public class Jumper : MonoBehaviour
{
    //[Label("isAutoJumper", "Is this platform an auto jumper platform? If so, pl will automatically make a jump when detects it.", "Color.blue", true)]
    [Space(5)]
    [Tooltip("Is this platform an auto jumper platform? If so, pl will automatically make a jump when detects it.")]
	public bool isAutoJumper = true;
    [ShowWhen("isAutoJumper")]
    [Tooltip("Time to wait before the Jump is performed once the Player gets over the platform jumper.")]
    public float jumpWaitTime = 0.15f;
    [Tooltip("Is this jump platform safe? If so, will can fall from high places without getting any damage or die.")]
    public bool isJumperSafe = true;

    [Space(5)]
    [Tooltip("How much high can player Get?.")]
	public float jumpForce = 12;

    [Tooltip("Jump forward normal (walk) displacement. Usually it should be the same as regular walk forward value.")]
	public float forwardSpeed = 6;

    [Tooltip("Jump forward sprinting (running) displacement. Could be the same as regular sprint forward value.")]
	public float forwardRunSpeed = 12;

    private bool isOverJumper; // is the pl over this jumper platform?
    private bool isAutoJumping; // is pl AutoJumping right now?
    private bool isFirstTimeJumpDetected; // is the pl over this jumper platform?
    
	private GameObject MyObj;
	private Status statusSrc;
	private Core coreScr;
    private CharacterMotor motor;
   
	private float OrigJumpForce;
    private float OrigWalkForwardSpeed;
    private float OrigRunForwardSpeed;

    void Start()
    {
		MyObj = GameObject.FindWithTag("Player"); //GameObject.Find("Player");
		if(MyObj == null)
		{
			Debug.LogError("Player NOT Found!");
		}
		else
		{
			statusSrc = MyObj.GetComponent<Status>();
			coreScr = statusSrc.GetCoreScr();
			motor = statusSrc.GetMotor();
		}

		OrigJumpForce = motor.jumping.baseHeight;
        OrigWalkForwardSpeed = motor.movement.maxForwardSpeed;
		OrigRunForwardSpeed = coreScr.sprint.runForwardSpeed;
    }

    void Update()
    {

        // UpdateControllerMotor status and the jumper flag if Player is over the jumper (right now and for first time).
        if (coreScr.GetObjectBelow() == gameObject && !isOverJumper) { SetOverCube(true); return; }
		else if (coreScr.GetObjectBelow() != gameObject && isOverJumper){ SetOverCube(false); return; }

        // If we detect a jumper start auto jumping.
        if (isAutoJumper)
        {
            if (isOverJumper && !isAutoJumping)
                StartCoroutine(MakeSuperJump());
            else if (isAutoJumping && coreScr.GetObjectBelow() != null && coreScr.GetTagBelow() != "Jumper")
            	StartCoroutine(DisableAutoJump());
        }
        else
        {
            if (statusSrc.isJumping && !isFirstTimeJumpDetected)
                isFirstTimeJumpDetected = true;
			
			if (isFirstTimeJumpDetected && coreScr.GetObjectBelow() != null && coreScr.GetTagBelow() != "Jumper")
                StartCoroutine(DisableAutoJump());
        }
    }

    public void SetOverCube(bool _isOver)
    {
        isOverJumper = _isOver;
        if (isOverJumper)
            SetNewJumpValues();
    }

    public void SetNewJumpValues()
    {
        motor.jumping.baseHeight = jumpForce;
        motor.movement.maxForwardSpeed = statusSrc.isRunning ? forwardRunSpeed : forwardSpeed;
    }

    public void SetOriginalJumpValues()
    {
        motor.jumping.baseHeight = OrigJumpForce;
        motor.movement.maxForwardSpeed = statusSrc.isRunning ? OrigRunForwardSpeed : OrigWalkForwardSpeed;
    }

    private IEnumerator MakeSuperJump()
    {
        yield return new WaitForSeconds(jumpWaitTime);
        motor.inputJump = true;
        isAutoJumping = true;
		coreScr.SetAutoJump(true);
        coreScr.SetPlatformJumpSafe(isJumperSafe);
    }

    private IEnumerator DisableAutoJump()
    {
        yield return new WaitForSeconds(0.1f);
        if (motor.IsGrounded())
        {
            SetOriginalJumpValues();
            motor.inputJump = false;
            isAutoJumping = false;
            isOverJumper = false;
			coreScr.SetAutoJump(false);
            coreScr.SetPlatformJumpSafe(false);
        }
    }

}