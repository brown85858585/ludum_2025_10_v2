using UnityEngine;

public enum pickupType
{
    Health = 0,
    DoubleJump = 2,
    Ammo = 3,
    Mana = 4
}

public class DetectPickup : MonoBehaviour
{
	public pickupType type = pickupType.Health;

    [ShowWhen("type", ShowWhenAttribute.Condition.Equals, 0)]
    public int healthAmount = 10;

    [ShowWhen("type", ShowWhenAttribute.Condition.Equals, 2)]
    public int numberOfJumps = 2;

    [ShowWhen("type", ShowWhenAttribute.Condition.Equals, 3)]
    public int numBullets = 20;

    [ShowWhen("type", ShowWhenAttribute.Condition.Equals, 4)]
    public int energy = 10;

    public AudioClip catchSound;

    private GameObject MyObj;
    protected Status StatusSrc;


    public virtual void Start()
    {
        MyObj = GameObject.FindWithTag("Player"); //GameObject.Find("Player");
        if (MyObj == null)
        {
            Debug.LogError("Player NOT Found!");
        }
        else
        {
            StatusSrc = MyObj.GetComponent<Status>();
        }
    }


    public virtual void OnTriggerEnter(Collider other)
    {
		if (other.tag.Contains("Player"))
        {
            PickupResponse();
        }
    }


    // This is virtual, so you can create a hole new script that derives from this one and reprogram this function (example below).
    protected virtual void PickupResponse()
    {
        switch (type)
        {
            case pickupType.Health:
                //Debug.Log("DetectPickup : Health, yum, yum!.");
                StatusSrc.AddHealth(healthAmount);
                break;
            case pickupType.DoubleJump:
                //Debug.Log("DetectPickup : DoubleJump just for you, baby!.");
                StatusSrc.GetMotor().jumping.doubleJump = true;
                StatusSrc.GetMotor().jumping.numberJumps = numberOfJumps;
                break;
            case pickupType.Ammo:
                // TO-DO : Add ammo to the main player.
                Debug.Log("DetectPickup : Ammo picked!.");
                break;
            case pickupType.Mana:
                // TO-DO : Add mana to the main player.
                Debug.Log("DetectPickup : Mana picked!.");
                break;
            default:
                // TO-DO : ¿do nothing? ¿Display a warning msg?.
                Debug.Log("DetectPickup : This Pickup doesnt have a type.");
                break;
        }

        SoundManager.instance.GetSoundPlayer().PlaySound(catchSound); // Play the sound effect.

        Destroy(gameObject); // Destroy this pickup
    }
}



