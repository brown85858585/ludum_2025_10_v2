using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour
{
	public int healthDamage = 1;
	public float healthRatio = 1.0f;
    private float healthInternalTime;
    public AudioClip damageClip;

    void OnTriggerStay(Collider other)
    {
		if (other.transform.tag.Contains("Player") && Time.time >= healthInternalTime)
        {
			Status statusSrc = other.GetComponent<Status>();
            if (statusSrc.health > 0)
            {
                statusSrc.SetDamage(Mathf.FloorToInt(healthDamage)); // Damage the player
                if (damageClip != null) statusSrc.GetSoundPlayerSrc().PlaySound(damageClip); // Play the sound effect.
                healthInternalTime = Time.time + healthRatio;   // wait a moment to damage Player again.
            }
        }
    }

}