using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Game.ARShooting
{
    public class Enemy : MonoBehaviour
    {
        public int health;
        private GamePlay _gamePlay;
        public Animator animatorMove;
        public AudioClip dieClip;
        public GameObject effectBreak;
        public bool Die => health <= 0;
        private bool onDie = false;
        public void Init(GamePlay gamePlay)
        {
            _gamePlay = gamePlay;
            onDie = false;
            if (animatorMove)
            {
                if (Random.Range(0, 1000) % 2 == 0)
                    animatorMove.speed = 1f;
                else
                    animatorMove.speed = 0.7f;
            }
         
            if (transform.position.y < _gamePlay.player.cam.transform.position.y)
            {
                if (animatorMove)
                    animatorMove.enabled = false;
                float offsetY = _gamePlay.player.cam.transform.position.y - transform.position.y;
                TweenControl.GetInstance().Move(transform, new Vector3(transform.position.x, Random.Range(_gamePlay.player.cam.transform.position.y - offsetY / 2f, _gamePlay.player.cam.transform.position.y + offsetY / 2f), transform.position.z), 5f, () =>
                {
                    if (Random.Range(0, 1000) % 2 == 0)
                    {
                        if (animatorMove)
                            animatorMove.enabled = true;
                    }
                });
            }
            else
            {
                if (animatorMove)
                    animatorMove.enabled = true;
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
                if (dieClip)
                {
                    GetComponent<AudioSource>().clip = dieClip;
                    GetComponent<AudioSource>().Play();
                }
                _gamePlay.RemoveEnemy(this);
                _gamePlay.AddScore(1);
                EffectDie();
                TweenControl.GetInstance().DelayCall(transform, 2f, () =>
                {
                    Destroy(gameObject);
                });
            }
        }
        public void EffectDie()
        {
            TweenControl.GetInstance().KillTweener(transform);
            TweenControl.GetInstance().DelayCall(transform, 0.5f, () =>
            {
                gameObject.SetActive(false);
            });
            if (effectBreak)
                Instantiate(effectBreak, transform.position, Quaternion.identity, transform);
        }
    }
}

