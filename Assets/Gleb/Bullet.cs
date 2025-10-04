using System;
using Mirror;
using UnityEngine;

namespace Gleb
{
    public class Bullet : NetworkBehaviour
    {
        uint owner;
        bool inited;

        void Start()
        {
        }

        [Server]
        public void Init(uint owner)
        {
            this.owner = owner; //кто сделал выстрел
            inited = true;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!inited)
                return;

            Player player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    Debug.DrawRay(contact.point, contact.normal, Color.red, 5f);
                }

                if (player.netId != owner)
                {
                    player.OnBulletHit();
                    NetworkServer.Destroy(gameObject); //уничтожаем пулю
                }
            }
        }
    }
}