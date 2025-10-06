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
    }
}
