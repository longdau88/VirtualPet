using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Game.ZombieShoting
{
    public class Enemy : MonoBehaviour
    {
        public int health;
        public int attack;
        public float speedMove;
        private GamePlay _gamePlay;
        public Animator animator;
        public AudioClip walkClip;
        public AudioClip dieClip;
        public AudioClip attackClip;
        public bool Die => health <= 0;
        private Rigidbody _rigidbody;
        private bool onDie = false;
        public void Init(GamePlay gamePlay)
        {
            _gamePlay = gamePlay;
            _rigidbody = GetComponent<Rigidbody>();
            _onMove = true;
            animator.SetTrigger("walk");
            onDie = false;
            if (walkClip)
            {
                GetComponent<AudioSource>().clip = walkClip;
                GetComponent<AudioSource>().Play();
            }
        }
        public void SubHP(int value)
        {
            Debug.Log("SubHP " + gameObject.name);
            health -= value;
            if (health < 0)
                health = 0;
            if (health <= 0 && !onDie)
            {
                onDie = true;
                animator.SetTrigger("die");
                if (dieClip)
                {
                    GetComponent<AudioSource>().clip = dieClip;
                    GetComponent<AudioSource>().Play();
                }
                if (IsInvoking("Attack"))
                    CancelInvoke("Attack");
                TweenControl.GetInstance().DelayCall(transform, 1.4f, () =>
                {
                    _gamePlay.Enemys.Remove(this);
                    _gamePlay.AddScore(1);
                    Destroy(gameObject);
                });
            }
        }

        private bool _onMove = false;
        private void Update()
        {
            if (_onMove && Vector3.Distance(transform.position, _gamePlay.player.targetEnemy.transform.position) < 10f)
            {
                animator.SetTrigger("attack");
                Attack();
                _onMove = false;
            }

            if (_onMove)
            {
                _rigidbody.transform.LookAt(_gamePlay.player.targetEnemy.transform);
                _rigidbody.velocity = (_gamePlay.player.targetEnemy.transform.position - transform.position) * speedMove;
            }
        }

        private void Attack()
        {
            if (Die) return;
            GetComponent<AudioSource>().clip = attackClip;
            GetComponent<AudioSource>().Play();
            _gamePlay.player.SubHP(attack);
            Invoke("Attack", 3f);
        }
    }
}

