
using UnityEngine;
using System.Collections;

public class Pusher : MonoBehaviour
{
	public bool isEnabled = true; // is the pusher enabled?
	public bool constantPush = true; // is the push constant or just one time push?
	public bool forwardPush = true; // is the push constant or just one time push?
	public float pushForce = 5;
    public AudioClip pushClip;

	private float internalTime = 0;
    private AudioSource myAS;

	private Status statusSrc;
	private Core coreScr;

    void Start()
    {
		GameObject MyObj = GameObject.FindWithTag("Player"); //GameObject.Find("Player");
		if(MyObj == null)
		{
			Debug.LogError("Player NOT Found!");
		}
		else
		{
			statusSrc = MyObj.GetComponent<Status>();
			coreScr = statusSrc.GetCoreScr();
		}
    }

    void Update()
    {
        if (internalTime >= 0)
            internalTime -= Time.deltaTime;

		// When finish playing, we delete the AudioSource reference used to play tthe FX sound.
		// PLay the audio not always will be done in the same AusioSource, the SoundPlayer manage where to play it.
		if (myAS != null)
			if(!myAS.isPlaying)
				myAS = null;
    }

    void OnTriggerStay(Collider other)
    {
        if (!isEnabled) { return; }

		if (other.transform.tag.Contains("Player"))
        {
            if (constantPush || (!constantPush && (internalTime <= 0)))
            {
                Vector3 dir = other.transform.position - transform.position;
                if (forwardPush)
					coreScr.AddImpact(transform.forward, pushForce);
                else
					coreScr.AddImpact(dir, pushForce);

				PlayPusherClip();
                internalTime = 1;
            }
        }
    }

	// Play FX audio clip.
	void PlayPusherClip()
	{
		if (pushClip == null) return;
		if (myAS != null) return;

		myAS = statusSrc.GetSoundPlayerSrc().PlaySound(pushClip);	// Play the sound effect.
	}
}