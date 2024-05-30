using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Game.ARShooting
{
    public class Player : MonoBehaviour
    {
        public GameObject cam;
        public Animator gunAnimator;
        public Transform startPos;
        public Transform target;
        public AudioClip shotClip;
        private GamePlay _gamePlay;

        private bool _onShot = false;

        public void Init(GamePlay gamePlay)
        {
            _gamePlay = gamePlay;
            _onShot = false;
        }
        void Update()
        {
            if (_gamePlay == null) return;
            if (!_gamePlay.OnPlay) return;

            if (Input.GetMouseButtonDown(0))
            {
                Shot();
            }
        }
        public void Shot()
        {
            if (_gamePlay == null) return;
            if (!_gamePlay.OnPlay) return;
            if (_onShot) return;
            _onShot = true;
            Debug.Log("Shot");
            _gamePlay.ShotEnemy();
            GetComponent<AudioSource>().PlayOneShot(shotClip);
            gunAnimator.SetTrigger("shot");
            TweenControl.GetInstance().DelayCall(transform, 0.3f, () =>
            {
                _onShot = false;
            });
        }
        
    }
}
