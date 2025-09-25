using UnityEngine;
using System.Collections;

public class EnemyBulletMngr : MonoBehaviour
{
    public float aceleracion = 1;
    public float shootRatio = 1;
    public int damage = 45;

    [Space (5)]
    public float kickbackForce = 10;
    public bool autoRotation = true;
	public Vector3 rotationVelocity = new Vector3(2, 2, 2);

    [Space(5)]
    public GameObject splashPrefab;
    public bool splashRotated = false;
    public GameObject decallPrefab;

    private RaycastHit hit;

    

    
    void Update()
    {
		if (autoRotation == true) // Generate AutoRotation
        {
            transform.Rotate(Vector3.right, rotationVelocity.x, Space.World);
            transform.Rotate(Vector3.up, rotationVelocity.y, Space.World);
            transform.Rotate(Vector3.forward, rotationVelocity.z, Space.World);
        }
    }

    // Get raycast information. So we can instantiate a splah and a decal if we want to.
    void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.forward, out hit, 5f);
    }

    // The bullet uses trigger + raycast now.
    public void OnTriggerEnter(Collider other)
    {
        // If bullet hit a player, apply damage to player's health.
		if (other.transform.tag.Contains("Player"))
        {
			other.transform.SendMessage("SetDamage", damage);
            //StatusSrc.health =StatusSrc.health - damage;
			// VERY, very, very bad code here. must catch the player status first in the Start and the if we it, applyDaage
			other.transform.GetComponent<Core>().AddImpact(transform.forward, kickbackForce); 
        }
        else if (!other.transform.tag.Contains("EnemyBullet"))
        {
            // if bullet hits anything else, create a splash (explision or something) and a decall in the impact point.
			// This is an old code as well, the colision matrix should prevent bullet detection (with another bullet) 
			
            // Create the splash. It can be created oriented in the normal of the surface where the bullet hitted
            Vector3 pos = hit.point;
            Quaternion rot = Quaternion.identity;
            if (splashPrefab != null)
            {
                if (splashRotated == true)
                {
                    rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                }
                PoolSystem.instance.Spawn(splashPrefab, pos, rot);
            }

            // Create a decall.
            if (decallPrefab != null)
            {
                Quaternion rotDecall = Quaternion.FromToRotation(Vector3.up, hit.normal);
                GameObject decallObj = PoolSystem.instance.Spawn(decallPrefab, pos, rotDecall);

                // Move the decall a little bit in the up (Y) direction.
                Vector3 _posAux = decallObj.transform.localPosition;
                _posAux += decallObj.transform.up * 0.1f;
                decallObj.transform.localPosition = _posAux;
            }
        }

        this.gameObject.SetActive(false);   // Disable the bullet
    }

    private void OnDisable()
    {
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

}