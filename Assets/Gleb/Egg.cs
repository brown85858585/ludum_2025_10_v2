using Mirror;
using UnityEngine;

namespace Gleb
{
    public class Egg : NetworkBehaviour
    {
        public AudioSource SpawnSound;
        public GameObject PickupSoundPrefab;
        public float startTime;
        
        uint owner;
        bool inited;
        void Start()
        {
            SpawnSound.Play();
        }

        [Server]
        public void Init(uint owner)
        {
            startTime = Time.time;
            this.owner = owner; //кто отложил яйцо
            inited = true;
        }

        void OnTriggerEnter(Collider collision)
        {
            if (!inited)
                return;
            
            Player player = collision.gameObject.GetComponent<Player>();
            if (player)
            {
                float elapsedTime = Time.time - startTime;
                if (player.netId == owner && elapsedTime < 1f)
                    return;
                
                player.OnPickupEgg();
                GameObject sound = Instantiate(PickupSoundPrefab, collision.gameObject.transform.position, Quaternion.identity);
                var aso = sound.GetComponent<AudioSource>();
                aso.Play();
                sound.gameObject.AddComponent<AutoDestroy>().DestroyTime = aso.time;
                
                NetworkServer.Destroy(gameObject); //уничтожаем яйцо
            }
        }
    }
}