using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Game.ZombieShoting
{
    public class Player : MonoBehaviour
    {
        public int attack;
        public float speedRotate;
        public GameObject cam;
        public Slider healthSlider;
        public Text healthText;
        public GameObject gun;
        public Rigidbody rigidbody;
        public Bullet bullet;
        public Transform target;
        public Transform targetEnemy;
        public AudioClip shotClip;
        private GamePlay _gamePlay;
        private Gyroscope _gyro;
        private bool _gyroActive = false;
        private int _health;


        public float speedH = 2.0f;
        public float speedV = 2.0f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;

        float startAttitudeX;
        float startAttitudeY;
        float startGravityX;
        private Quaternion _rot;

        public void Init(GamePlay gamePlay, int health)
        {
            _health = health;
            _gamePlay = gamePlay;
            if (SystemInfo.supportsGyroscope)
            {
                _gyroActive = true;
                _gyro = Input.gyro;
                _gyro.enabled = true;
                startAttitudeX = _gyro.attitude.eulerAngles.x;
                startGravityX = _gyro.gravity.y;
                startAttitudeY = _gyro.attitude.eulerAngles.y;
                _rot = new Quaternion(0, 0, 0, 0);

            }
            healthSlider.value = _health;
            healthSlider.maxValue = _health;
            healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
        }
        void Update()
        {

            if (_gamePlay == null) return;
            if (!_gamePlay.OnPlay) return;

            if (_gyroActive)
            {
                //transform.Rotate(0, - Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.x);
                //transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y - Input.gyro.rotationRateUnbiased.y * Time.deltaTime * Mathf.Rad2Deg, transform.eulerAngles.x - Input.gyro.rotationRateUnbiased.x * Time.deltaTime * Mathf.Rad2Deg);
                transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y - Input.gyro.rotationRateUnbiased.y * Time.deltaTime * Mathf.Rad2Deg, transform.eulerAngles.z);
                transform.Rotate(0.0f, 0f, Input.gyro.rotationRateUnbiased.x);

                //transform.Rotate(-_gyro.gravity.x * Time.deltaTime * speedRotate, _gyro.gravity.x * Time.deltaTime * speedRotate, 0, Space.World);
                //cam.transform.localEulerAngles = new Vector3(/*75 + */(_gyro.attitude.eulerAngles.x - startAttitudeX), 90 + (_gyro.gravity.y - startGravityX) * Time.deltaTime * speedRotate * 2f, 0);
            }

            //#if UNITY_EDITOR
            //            //if (Input.touchCount > 0 || Input.GetMouseButton(0))
            //            //{
            //            yaw += speedH * Input.GetAxis("Mouse X");
            //            pitch += speedV * Input.GetAxis("Mouse Y");
            //            transform.eulerAngles = new Vector3(0, yaw, pitch);
            //            //}
            //#else
            // if (Input.touchCount > 0 || Input.GetMouseButton(0))
            //            {
            //            yaw += speedH * Input.GetAxis("Mouse X");
            //            pitch += speedV * Input.GetAxis("Mouse Y");
            //            transform.eulerAngles = new Vector3(0, yaw, pitch);
            //            }
            //#endif

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Shot();
            }
        }
        private void Shot()
        {
            Debug.Log("Shot");
            TweenControl.GetInstance().ValueTo(transform, (param) =>
            {

                gun.transform.localEulerAngles = new Vector3(param, 90, 0);
            }, -90, -91, 0.05f, () =>
            {
                TweenControl.GetInstance().ValueTo(transform, (param) =>
                {
                    gun.transform.localEulerAngles = new Vector3(param, 90, 0);
                }, -91, -90, 0.05f, () =>
                {

                });
            });
            Bullet bu = Instantiate(bullet, cam.transform.position, Quaternion.identity, _gamePlay.transform);
            bu.transform.eulerAngles = cam.transform.eulerAngles;
            bu.transform.LookAt(target);
            bu.Move(target.position, 1);
            GetComponent<AudioSource>().PlayOneShot(shotClip);
        }

        public void SubHP(int value)
        {
            _health -= value;
            healthSlider.value = _health;
            healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
            if (_health <= 0)
            {
                _health = 0;
                _gamePlay.Lose();
            }
        }
    }
}
