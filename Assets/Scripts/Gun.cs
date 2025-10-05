using Gleb;
using Mirror;
using UnityEngine;

namespace Demo
{
    public class Gun : NetworkBehaviour
    {
        [SerializeField]
        float _startSpeed = 10f;

        [SerializeField]
        float _bulletLifetime = 3f;

        [SerializeField]
        GameObject _bullet;

        [SerializeField]
        Transform _origin;
    
        [SerializeField]
        int _damage = 20;
        
        public AudioSource ShootSound;

        void Update()
        {
            if (!isOwned) return; //проверяем, есть ли у нас права изменять этот объект
            if (!Input.GetMouseButtonDown(0)) return;
            
            if (isServer)
                SpawnBullet(netId);
            else
                CmdSpawnBullet(netId);
        }


        [Command]
        public void CmdSpawnBullet(uint owner)
        {
            SpawnBullet(owner);
        }

        [Server]
        public void SpawnBullet(uint owner)
        {
            GameObject bullet = Instantiate(_bullet, _origin.position, Quaternion.identity); //Создаем локальный объект пули на сервере
            if (!bullet.TryGetComponent(out Rigidbody rb))
            {
                return;
            }
            
            if (bullet.TryGetComponent(out Bullet component))
            {
                component.Damage = _damage;
            }

            rb.AddForce(_startSpeed * _origin.forward);
            rb.gameObject.AddComponent<AutoDestroy>().DestroyTime = _bulletLifetime;

            NetworkServer.Spawn(bullet); //отправляем информацию о сетевом объекте всем игрокам.
            bullet.GetComponent<Bullet>().Init(owner); //инициализируем поведение пули

            ShootSound.Play();
        }
    }
}