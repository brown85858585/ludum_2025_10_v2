using UnityEngine;

namespace Demo
{
    public class Gun : MonoBehaviour
    {
        [SerializeField]
        float _startSpeed = 10f;

        [SerializeField]
        float _bulletLifetime = 3f;
    
        [SerializeField]
        GameObject _bullet;

        [SerializeField]
        Transform _origin;
    
        void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var bullet = Instantiate(_bullet, _origin.position, Quaternion.identity);
            if (!bullet.TryGetComponent(out Rigidbody rb))
            {
                return;
            }
        
            rb.AddForce(_startSpeed * _origin.forward);
            rb.gameObject.AddComponent<AutoDestroy>().DestroyTime = _bulletLifetime;
        }
    }

}