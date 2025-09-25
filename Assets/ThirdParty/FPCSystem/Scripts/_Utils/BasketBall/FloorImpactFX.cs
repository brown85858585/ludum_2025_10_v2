using System.Collections;
using UnityEngine;

public class FloorImpactFX : MonoBehaviour {
    [Tooltip("Initial Wait time before start detecting collisions and generating FX.")]
    public float initialWaitTime = 0.5f; // Wait some time before start detecting collisions and generating FX.

    [Space(5)]
    [Tooltip("The visual effect shown when this impact again anything that has a collider. A dust fx made with particles or even an explosion.")]
    public GameObject impactEffect;
    [Tooltip("The impact sound. A bouncing ball or a wooden crate impacting with the floor.")]
    public AudioClip impactClip;

    [Space(5)]
    [Tooltip("Minimal velocity needed to activate the FX in this object. Trying to avoid to generate any sound if the object isn't moving.")]
    public float minVelocity = 0.5f;
    [Tooltip("Minimal time that has to wait to be able to play the effects again. Avoid what happens when more than one (the first) collision " +
        "is detected in a very short time, the sound FX is annoying in that cases.")]
    public float minTimeBetwenEffects = 0.5f;

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;

    protected AudioSource myAS;
    protected bool isInitialized = false;
    protected Rigidbody myRigidbody;
    protected float internalTime = 0;

    
    public virtual IEnumerator Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        yield return new WaitForSeconds(initialWaitTime);
        isInitialized = true;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if(showDebug) Debug.Log("FloorImpactFX -> OnCollisionEnter()-> "+this.name+" is colliding with: "+collision.transform.name);

        if (!isInitialized) return;
        if (myRigidbody.linearVelocity.magnitude < minVelocity) return;
        if (Time.time < internalTime) return;

        if (impactEffect != null)
        {
            CreateImpactFX (collision);
        }
        if (impactClip != null)
        {
            myAS = SoundManager.instance.GetSoundPlayer().PlayLocatedClip(impactClip, transform.position);
            internalTime = Time.time + minTimeBetwenEffects;    // Could be done using a Courutine and a flag (a bool) but this method is easier.
        }
    }

    protected virtual void CreateImpactFX(Collision collision)
    {
        PoolSystem.instance.Spawn(impactEffect, collision.contacts[0].point, Quaternion.identity);
    }
    
}
