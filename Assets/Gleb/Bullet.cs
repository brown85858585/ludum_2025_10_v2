using System;
using Mirror;
using UnityEngine;

namespace Gleb
{
    public class Bullet : NetworkBehaviour
    {
        [HideInInspector] 
        public int Damage; // берётся из пушки при спавне пули
        
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
                if (player.netId != owner)
                {
                    player.OnBulletHit(this);
                    NetworkServer.Destroy(gameObject); //уничтожаем пулю
                }
            }
        }
    }
}