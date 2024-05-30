using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Game.SpaceInfiniteRunner
{
    public class Player : MonoBehaviour
    {
        public GameObject cam;
        public GameObject spaceShip;
        public GameObject smoke;
        public Rigidbody rigidbody;
        private GamePlay _gamePlay;
        private Gyroscope _gyro;
        private bool _gyroActive = false;

        float startAttitudeX;
        float startAttitudeY;
        float startGravityX;
        public void Init(GamePlay gamePlay)
        {
            if (SystemInfo.supportsGyroscope)
            {
                _gyroActive = true;
                _gyro = Input.gyro;
                _gyro.enabled = true;
                startAttitudeX = _gyro.attitude.eulerAngles.x;
                startGravityX = _gyro.gravity.y;
                startAttitudeY = _gyro.attitude.eulerAngles.y;
            }
            _gamePlay = gamePlay;
            spaceShip.SetActive(true);
            smoke.SetActive(false);
        }
        void Update()
        {
            if (_gamePlay == null) return;
            if (_gamePlay.OnMove)
            {
                rigidbody.velocity = transform.right * _gamePlay.speedMove;
                if (_gyroActive)
                {
                    transform.Rotate(new Vector3(Mathf.Clamp(-_gyro.gravity.x * Time.deltaTime * _gamePlay.speedRotate, -30f, 30f), _gyro.gravity.x * Time.deltaTime * _gamePlay.speedRotate, 0), Space.World);
                    cam.transform.localEulerAngles = new Vector3(/*75 + */(_gyro.attitude.eulerAngles.x - startAttitudeX), 90 + (_gyro.gravity.y - startGravityX) * Time.deltaTime * _gamePlay.speedRotate * 2f, 0);
                }
            }

#if UNITY_EDITOR

            if (Input.GetKey(KeyCode.RightArrow))
            {
                //Rotate the sprite about the Y axis in the positive direction
                transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * _gamePlay.speedRotate, Space.World);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //Rotate the sprite about the Y axis in the negative direction
                transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * _gamePlay.speedRotate, Space.World);
            }
#endif

        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.gameObject.name);
            if (collision.transform.GetComponent<Obj>() != null)
            {
                switch (collision.transform.GetComponent<Obj>().objType)
                {
                    case ObjType.Rock:
                        _gamePlay.Lose();
                        break;
                    case ObjType.UFO:
                        _gamePlay.Lose();
                        break;
                }
            }
        }
    }
}
