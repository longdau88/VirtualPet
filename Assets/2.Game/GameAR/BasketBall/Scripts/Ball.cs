using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.BasketBall
{
    public class Ball : MonoBehaviour
    {
        [SerializeField]
        private GamePlay _gamePlay;
        public void Init(GamePlay gamePlay)
        {
            _gamePlay = gamePlay;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == "CheckCorrect")
            {
                if (transform.position.y < other.transform.position.y)
                {
                    Debug.Log("CheckCorrect -> Correct");
                    _gamePlay.Correct();
                }
                else
                {
                    Debug.Log("CheckCorrect -> Wrong");
                }

            }
        }
        public void CheckNextNextTurn()
        {
            TweenControl.GetInstance().DelayCall(transform, 2.5f, () =>
            {
                if (_gamePlay)
                {
                    if (_gamePlay.IsCorrect)
                    {
                        _gamePlay.NewThrowBall();
                    }
                    else
                    {
                        _gamePlay.ResetGamePlay();
                    }
                }
            });
        }
        //private bool _isTrigger = false;
        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.gameObject.name == "PlaneResetGame" && !_isTrigger)
        //    {
        //        //Debug.Log("Cham plane: PlaneResetGame");
        //        float distance = Vector3.Distance(transform.position, _gamePlay.pointStartBall.position);
        //        Debug.Log(distance);
        //        if (distance > 0.5f)
        //        {
        //            _isTrigger = true;
        //            if (_gamePlay.IsCorrect)
        //            {
        //                //Debug.Log("Cham plane: Correct = true");
        //                TweenControl.GetInstance().DelayCall(transform, 0.5f, () =>
        //                {
        //                    _gamePlay.NewThrowBall();

        //                });

        //            }
        //            else
        //            {
        //                //Debug.Log("Cham plane: Correct = false");
        //                TweenControl.GetInstance().DelayCall(transform, 0.5f, () =>
        //                {
        //                    _gamePlay.ResetGamePlay();
        //                });

        //            }
        //        }
        //    }
        //}
        //private void OnDestroy()
        //{
        //    TweenControl.GetInstance().KillDelayCall(transform);
        //}
    }
}
