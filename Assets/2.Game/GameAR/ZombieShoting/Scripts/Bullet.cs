using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.ZombieShoting
{
    public class Bullet : MonoBehaviour
    {
        public float speed;
        private int _damage = 1;
        private bool _destroy = false;
        public void Move(Vector3 direction, int damage)
        {
            _damage = damage;
            GetComponent<Rigidbody>().AddForce(direction * speed, ForceMode.Impulse);
            TweenControl.GetInstance().DelayCall(transform, 2, () =>
            {
                if (!_destroy)
                {
                    _destroy = true;
                    Destroy(gameObject);
                }
            });
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other != null && other.GetComponent<Enemy>() != null)
            {
                other.GetComponent<Enemy>().SubHP(_damage);
                if (!_destroy)
                {
                    _destroy = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}

