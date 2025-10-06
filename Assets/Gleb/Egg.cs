using Mirror;
using UnityEngine;

namespace Gleb
{
    public class Egg : NetworkBehaviour
    {
        public AudioSource SpawnSound;

        void Start()
        {
            SpawnSound.Play();
        }
        
        void OnCollisionEnter(Collision collision)
        {
            Debug.LogError(gameObject.name + ": OnCollisionEnter called on player " + collision.gameObject.name);
            Player player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                player.Pickup(this);
                NetworkServer.Destroy(gameObject); //уничтожаем яйцо
            }
            else
            {
                Debug.Log(gameObject.name + ": OnCollisionEnter called on player " + collision.gameObject.name);
            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Player player = hit.gameObject.GetComponent<Player>();
            if (player)
            {
                player.Pickup(this);
                NetworkServer.Destroy(gameObject); //уничтожаем яйцо
            }
            else
            {
                Debug.Log(gameObject.name + ": OnControllerColliderHit called on player " + hit.gameObject.name);
            }
        }
    }
}
