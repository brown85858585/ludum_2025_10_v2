using UnityEngine;

[System.Serializable]
public class StepColor
{
    public Color defaultStep;
    public Color woodStep;
    public Color metalStep;
    public Color concreteStep;
    public Color sandStep;
    public Color waterStep;
    public Color wallStep;
    public Color platformStep;
}

[System.Serializable]
public class TerrainStepColor
{
    public Color texture0Step;
    public Color texture1Step;
    public Color texture2Step;
    public Color texture3Step;
    public Color texture4Step;
    public Color texture5Step;
    public Color texture6Step;
    public Color texture7Step;
    public Color texture8Step;
    public Color texture9Step;
}

public class FootPrintRenderer : MonoBehaviour {

    [Tooltip("Left footprint prefab that will be used to draw the footprint over the floor.")]
    public GameObject footprintLeft;
    [Tooltip("Right footprint prefab that will be used to draw the footprint over the floor.")]
    public GameObject footprintRight;
    [Space(5)]
    [Tooltip("Footprints displacement from pl center point (at floor level).")]
    public Vector3 localDisplacement = new Vector3(1f, 0.15f, 0.3f); // Footprint displacement in every direction (forward, sideways and upwards).
    [Tooltip("Time to wait to render footprints when pl has stopped. This option exists to avoid players to walk and stop quickly, rendering lot of footprints by doing so.")]
    public float standingWaitTime = 0.5f;
    [Tooltip("Minimum distance to walk before rendering a footprint. This option exists to avoid players to walk a tiny distance, rendering a lot of footprints by doing so.")]
    public float minDistanceWalking = 0.5f;

    [Space(5)]
    [Tooltip("Footprints FX particle effect besides the footprint itself. Just render a 'dust' animated effect when making each step.")]
    public GameObject footPrintTerrainDust;
    [Tooltip("Dust effect may need an alpha correction, so just adjust the alpha using this alpha multiplier. Default value is 0.1f, a value of 1.0f means no change in the alpha. Take into acount that this correction will be executed only in Unity 2018.")]
    public float dustAlphaCorrection = 0.05f;

    [Space(5)]
    [Tooltip("The footprints will be rendered only when walking over objects that have these tags.")]
    public string[] footprintTags;

    [Space(5)]
    [Tooltip("Colors the footprints will be tinted with, depending on standing object's tags.")]
    public StepColor stepColor;
    [Tooltip("Colors footprints will be tinted with, depending on the terrain texture pl is standing over.")]
    public TerrainStepColor terrainStepColor;

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;
    public bool showLineCast = false;
    [ShowWhen("showLineCast")]
    public bool showEveryUpdate = false;

    protected GameObject MyObj;
    protected Status StatusSrc;
    protected Core CoreScr;
    protected CharacterController controller;
    protected CameraBobber cameraBobberSrc;
    protected Transform PlayerTransform;

    private SensorOptions FootPrintSensorOptions;   // Internal raycast for each foot. used to detect the exact point to place the footprint.

    [Header("Runtime Watcher")]
    [SerializeField]
    [Disable]
    protected bool isLeftFoot = false;
    [SerializeField]
    [Disable]
    protected bool isStopped = false;


    // Valiables regarding rendering both footprints when standing (player stopped)
    protected bool disableFwrdDispl = false;  // Used to disbale the foward displacement when standing (just stop). When walking the forwad displ is used again.
    protected float internalWaitTime = 0; // Time to wait before rendering bioth footprint again when the player has stopped.

    // Variables used to avoid rendering the left foot when start walking too often if the player start to walk/stop quickly.
    protected bool isGoingToRenderFirstLeftFoot = false;
    protected float distanceWalking = 0f;
    protected Vector3 stoppedPosition = Vector3.zero;

    [SerializeField]
    [Disable]
    protected Transform referencePlane;


    public virtual void Start()
    {
        MyObj = GameObject.FindWithTag("Player");
        if (MyObj == null)
        {
            Debug.LogError("Player NOT Found!");
        }
        else
        {
            StatusSrc = MyObj.GetComponent<Status>();
            CoreScr = StatusSrc.GetCoreScr();
            controller = StatusSrc.GetController();
            cameraBobberSrc = StatusSrc.GetCameraBobberScr();
            PlayerTransform = MyObj.transform;
        }

        referencePlane = this.transform.GetChild(0);

        FootPrintSensorOptions = new SensorOptions();
        FootPrintSensorOptions.sensorDistance = controller.height;
        FootPrintSensorOptions.sensorLayers = CoreScr.belowSensorOptions.sensorLayers;
    }

    public virtual void OnEnable()
    {
        EventManagerv2.instance.StartListening("CameraRachedBobEnd", CreateFootPrint);
    }

    public virtual void OnDisable()
    {
        if (!EventManagerv2.IsDestroyed)
            EventManagerv2.instance.StopListening("CameraRachedBobEnd", CreateFootPrint);
    }


    public virtual void Update()
    {
        // Show graphicast line to see how the footprint raycast works
        if(showLineCast && showEveryUpdate)
        {
            UpdateReferencePlane();
            CheckIfIsInLedge();
        }


        if(isStopped != StatusSrc.isStop)
        {
            isStopped = StatusSrc.isStop;

            if (StatusSrc.isStop)
            {
                stoppedPosition = PlayerTransform.position;
                isGoingToRenderFirstLeftFoot = false;
                distanceWalking = 0;

                if (Time.time > internalWaitTime)        // Have to print both footprint once when standing (being stop).
                {
                    CreateBothFootPrints();
                    internalWaitTime = Time.time + standingWaitTime; // Update the time to wait to render both footprints when the Player stops
                }
            }
            else
            {
                isGoingToRenderFirstLeftFoot = true;    // Enable the flag to know that we are going to render the left foot if we walk a minimal distance.
            }
        }

        // Trying to print the first foot when the Player starts walking. Just look at the distance traveled to do so.
        if(isGoingToRenderFirstLeftFoot)
        {
            distanceWalking = Vector3.Distance(stoppedPosition, PlayerTransform.position);
            if (distanceWalking >= minDistanceWalking)
            {
                CreateFootPrint(new EventParam(this.name, string.Empty, "leftFoot"));
                isGoingToRenderFirstLeftFoot = false;
                internalWaitTime = 0;   // Now that Player is walking, reset the time to wait to render both footprints when the Player stops
            }
        }
    }



    protected virtual void CreateBothFootPrints()
    {
        disableFwrdDispl = true;
        CreateFootPrint (new EventParam (this.name, string.Empty, "rightFoot"));
        disableFwrdDispl = true;
        CreateFootPrint (new EventParam (this.name, string.Empty, "leftFoot"));
    }

    protected virtual void CreateFootPrint (EventParam eventParam)
    {
        string _param = eventParam.data as string;
        if (!(_param.Contains("leftFoot") || _param.Contains("rightFoot"))) { return; }

        if (!StatusSrc.isInGround || StatusSrc.isWallRunning) return; // Security sentence

        if (StatusSrc.GetFloorDistance() > 0 && !StatusSrc.isBeingPushed)
        {
            isLeftFoot = _param.Contains("leftFoot");
            _CreateFootPrintInternal ();
        }
    }

    protected virtual void UpdateReferencePlane()
    {
        if (FootPrintSensorOptions.objectHit != null)
        {
            Quaternion angle = Quaternion.FromToRotation(Vector3.up, FootPrintSensorOptions.hitNormal);
            referencePlane.rotation = angle;
            referencePlane.Rotate(0f, PlayerTransform.localEulerAngles.y, 0f);
        }
    }


    protected virtual void _CreateFootPrintInternal()
    {
        // Check if the floor has a tag and is in the list.
        bool checkTag = false;
        string objectBelowTag = CoreScr.GetTagBelow();
        foreach (string tag in footprintTags)
        {
            if (tag.Contains(objectBelowTag))
            {
                checkTag = true;
                break;
            }
        }

        if (!checkTag) return;      // Can print the footprint because the floor tag isn't in the list.

        // Launch a Raycast to see exactly where is the ground.
        float lateralDispl = isLeftFoot ? -localDisplacement.x : localDisplacement.x;

        // The footstep foward displacememnt render point is to be able to see the step effect when looking down.
        // When walking backwards, it should start behind the , so you don't see how the step is created.
        float fwrdDispl = InputManager.instance.VerticalValue > 0 ? localDisplacement.z : -localDisplacement.z * 3.0f;

        // In case we are moving ONLY laterally disable the forward (or backward) displacement
        if (InputManager.instance.VerticalValue == 0 && InputManager.instance.HorizontalValue != 0)
            disableFwrdDispl = true;

        if (disableFwrdDispl)
            FootprintSensorCast (0f, lateralDispl);
        else
            FootprintSensorCast(fwrdDispl, lateralDispl);

        disableFwrdDispl = false; // If we have enabled the fwrdDispl (to draw the footprint when Player is stopped), we disbale that

        UpdateReferencePlane();


        if (CheckIfIsInLedge()) return; // If the Player is in a ledge dont render the footprints (example: in a roof close to the edge).


        // Calculate the fottprint displacement from the Player center (where the below sensor hit the ground)
        GameObject footPrintObj = null;
        Vector3 pos = FootPrintSensorOptions.hitPosition;
        Quaternion angle = Quaternion.FromToRotation(Vector3.up, FootPrintSensorOptions.hitNormal);

        Color myColor = GetTerrainStepColor (objectBelowTag, FootPrintSensorOptions.hitPosition);

        if (isLeftFoot)
            footPrintObj = PoolSystem.instance.Spawn(footprintLeft, pos, angle);
        else
            footPrintObj = PoolSystem.instance.Spawn(footprintRight, pos, angle);

        footPrintObj.GetComponent<Renderer>().material.color = myColor;


        Transform footPrintTransform = footPrintObj.transform;
        footPrintTransform.Rotate(0f, PlayerTransform.localEulerAngles.y, 0f);

        // Set the position on Y axis of the footprint right!
        float upwardsDispl = localDisplacement.y;
        pos += footPrintTransform.up * upwardsDispl;
        footPrintTransform.position = pos;

        if (footPrintTerrainDust != null)
        {
            GameObject myTerrainDust = PoolSystem.instance.Spawn (footPrintTerrainDust, pos, angle);
            myTerrainDust.transform.Rotate(0f, PlayerTransform.localEulerAngles.y, 0f);
            //#if UNITY_2018
            myColor.a *= dustAlphaCorrection;
            //#endif
            myTerrainDust.SendMessage("ChangeParticleColor", myColor);
            
        }

        isLeftFoot = !isLeftFoot;
    }

    protected virtual bool CheckIfIsInLedge()
    {
        bool isInCorner = false;

        RaycastHit sensorBelowHitInfo;

        float sensorDistance = CoreScr.GetTagBelow().Contains("Terrain") ? controller.height * 0.65f : controller.height * 0.55f;   // The Ray's lenght is a little bit larger if we are in a terrain
        Vector3 sensorBelowOrigin = (PlayerTransform.position + controller.center);// + (Vector3.up * (-controller.height * 0.3f));
        sensorBelowOrigin += referencePlane.forward * 0.5f; // The ray will start foward or backward the CharacterController radius

        Vector3 direction = -transform.up;
        if (FootPrintSensorOptions.objectHit != null)
        {
            direction = -FootPrintSensorOptions.hitNormal;
            direction = direction.normalized;
        }


        if (showLineCast) Debug.DrawRay(sensorBelowOrigin, direction * sensorDistance, Color.red);
        if (!Physics.Raycast(sensorBelowOrigin, -transform.up, out sensorBelowHitInfo, sensorDistance))
        {
            isInCorner = true;
            if (showDebug) Debug.Log("FootPrintRenderer -> CheckIfLedge(Ray 1) -> Is in a Corner. Don't render footprints.");
        }

        if (!isInCorner)
        {
            sensorBelowOrigin = (PlayerTransform.position + controller.center);// + (Vector3.up * (-controller.height * 0.3f));
            sensorBelowOrigin -= referencePlane.forward * 0.5f;
            if (showLineCast) Debug.DrawRay(sensorBelowOrigin, direction  * sensorDistance, Color.red);
            if (!Physics.Raycast(sensorBelowOrigin, -transform.up, out sensorBelowHitInfo, sensorDistance))
            {
                isInCorner = true;
                if (showDebug) Debug.Log("FootPrintRenderer -> CheckIfLedge(Ray 2) -> Is in a Corner. Don't render footprints.");
            }
        }

        if (!isInCorner)
        {
            sensorBelowOrigin = (PlayerTransform.position + controller.center);// + (Vector3.up * (-controller.height * 0.3f));
            sensorBelowOrigin += referencePlane.right * 0.5f;
            if (showLineCast) Debug.DrawRay(sensorBelowOrigin, direction * sensorDistance, Color.red);
            if (!Physics.Raycast(sensorBelowOrigin, -transform.up, out sensorBelowHitInfo, sensorDistance))
            {
                isInCorner = true;
                if (showDebug) Debug.Log("FootPrintRenderer -> CheckIfLedge(Ray 3) -> Is in a Corner. Don't render footprints.");
            }
        }

        if (!isInCorner)
        {
            sensorBelowOrigin = (PlayerTransform.position + controller.center);// + (Vector3.up * (-controller.height * 0.3f));
            sensorBelowOrigin -= referencePlane.right * 0.5f;
            if (showLineCast) Debug.DrawRay(sensorBelowOrigin, direction * sensorDistance, Color.red);
            if (!Physics.Raycast(sensorBelowOrigin, -transform.up, out sensorBelowHitInfo, sensorDistance))
            {
                isInCorner = true;
                if (showDebug) Debug.Log("FootPrintRenderer -> CheckIfLedge(Ray 4) -> Is in a Corner. Don't render footprints.");
            }
        }

        return isInCorner;
    }


    protected virtual void FootprintSensorCast(float fwrdDispl, float lateralDispl)
    {

        RaycastHit sensorBelowHitInfo;
        Vector3 sensorBelowOrigin = (PlayerTransform.position + controller.center);// + (Vector3.up * (-controller.height * 0.3f));

        sensorBelowOrigin += PlayerTransform.forward * fwrdDispl;
        sensorBelowOrigin += PlayerTransform.right * lateralDispl;

        if (Physics.Raycast(sensorBelowOrigin, -transform.up, out sensorBelowHitInfo, FootPrintSensorOptions.sensorDistance, FootPrintSensorOptions.sensorLayers))
        {
            FootPrintSensorOptions.sensorHitTag = sensorBelowHitInfo.transform.tag;
            FootPrintSensorOptions.sensorHitDistance = sensorBelowHitInfo.distance;
            FootPrintSensorOptions.objectHit = sensorBelowHitInfo.collider.gameObject;
            FootPrintSensorOptions.hitNormal = sensorBelowHitInfo.normal;
            FootPrintSensorOptions.hitPosition = sensorBelowHitInfo.point;
            if (showDebug) Debug.Log("FootPrintRenderer -> FootprintSensorCast() -> GetGroundTag: " + FootPrintSensorOptions.sensorHitTag);
        }
        else
        {
            FootPrintSensorOptions.sensorHitTag = string.Empty;
            FootPrintSensorOptions.sensorHitDistance = 0f;
            FootPrintSensorOptions.objectHit = null;
            FootPrintSensorOptions.hitNormal = Vector3.zero;
            FootPrintSensorOptions.hitPosition = Vector3.zero;
            if (showDebug) Debug.Log("FootPrintRenderer -> FootprintSensorCast() -> No Hit Detected.");
        }
    }

    protected virtual Color GetTerrainStepColor(string cTag, Vector3 position)
    {
        Color myColor = Color.black;

        cTag = cTag.ToLower();
        if (showDebug) Debug.Log("FootPrintRenderer -> GetTerrainStepColor() -> GetGroundTag: " + cTag);

        if (cTag.Contains("wood"))
            myColor = stepColor.woodStep;
        else if (cTag.Contains("metal"))
            myColor = stepColor.metalStep;
        else if (cTag.Contains("concrete"))
            myColor = stepColor.concreteStep;
        else if (cTag.Contains("dirt"))
            myColor = stepColor.sandStep;
        else if (cTag.Contains("sand"))
            myColor = stepColor.sandStep;
        else if (cTag.Contains("water"))
            myColor = stepColor.waterStep;
        else if (cTag.Contains("platform"))
            myColor = stepColor.platformStep;
        else if (cTag.Contains("terrain"))
        {
            int index = TerrainTextureDetector.GetMainTexture(position);
            if (showDebug) Debug.Log("FootPrintRenderer -> GetTerrainStepColor() -> Terrain Texture Index: " + index.ToString());
            switch (index)
            {
                case 0:
                    myColor = terrainStepColor.texture0Step;
                    break;
                case 1:
                    myColor = terrainStepColor.texture1Step;
                    break;
                case 2:
                    myColor = terrainStepColor.texture2Step;
                    break;
                case 3:
                    myColor = terrainStepColor.texture3Step;
                    break;
                case 4:
                    myColor = terrainStepColor.texture4Step;
                    break;
                case 5:
                    myColor = terrainStepColor.texture5Step;
                    break;
                case 6:
                    myColor = terrainStepColor.texture6Step;
                    break;
                case 7:
                    myColor = terrainStepColor.texture7Step;
                    break;
                case 8:
                    myColor = terrainStepColor.texture8Step;
                    break;
                case 9:
                    myColor = terrainStepColor.texture9Step;
                    break;
                default:
                    myColor = terrainStepColor.texture0Step;
                    break;
            }
        }
        else
            myColor = stepColor.defaultStep;

        return myColor;
    }
}
