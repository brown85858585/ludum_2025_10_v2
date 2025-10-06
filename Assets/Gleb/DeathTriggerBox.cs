using Mirror;
using UnityEngine;

namespace Gleb
{
    public class DeathTriggerBox : NetworkBehaviour
    {
        void Start()
        {
        }


        
        void OnTriggerEnter(Collider collision)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                player.needResp = true;
            }
        }
        
        void OnTriggerStay(Collider other)
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player)
            {
                player.needResp = true;
            }
        }
        
        
    }
}