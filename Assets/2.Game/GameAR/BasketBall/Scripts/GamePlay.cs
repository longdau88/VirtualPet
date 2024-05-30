using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using Game;
using MainApp.VirtualFriend;

namespace Game.BasketBall
{
    public class GamePlay : MonoBehaviour
    {
        public string sceneName = "BasketBall";
        public float throwForceZ = 0.4f;

        [SerializeField]
        private ARRaycastManager _raycastManager;
        [SerializeField]
        private ARSessionOrigin _sessionOrigin;
        [SerializeField]
        public Camera cam;
        public Transform pointStartBall;
        [SerializeField]
        private Text scoreText;

        [SerializeField]
        private GameObject backetBallPole;
        [SerializeField]
        private GameObject backet3D;
        [SerializeField]
        private Ball ballPrefabs;
        [SerializeField]
        private Transform pointAddEffect;
        [SerializeField]
        private AddText addText;
        [Space]
        [SerializeField] AudioClip soundWin;
        [SerializeField] AudioClip soundWrong;

        private List<Ball> _balls;
        private List<ARRaycastHit> _hits;
        private bool _onDetectPlacement = false;
        private ScreenOrientation _screenOrientation;
        private bool _putBacketBallPoleDone = false;
        private bool _alowThrow = false;
        private int _score = 0;
        private bool _isCorrect = false;

        public bool IsCorrect => _isCorrect;

        private void Start()
        {
            _onDetectPlacement = true;
#if !UNITY_EDITOR
                backetBallPole.SetActive(false);
#endif
            _screenOrientation = Screen.orientation;
            Screen.orientation = ScreenOrientation.Portrait;
            _score = 0;
            scoreText.text = _score.ToString();
            _balls = new List<Ball>();

            ClockController.Instance.ClockRun(60, 1, OnEndGame);
        }


        public void OnEndGame()
        {
            VirtualPetManager.Instance.ShowPanelResultDialog(_score, sceneName);
        }
        public void BackToMainApp()
        {
            DataManager.Instance.UnloadSceneGame(sceneName);
        }

        private void Update()
        {
            if (_onDetectPlacement)
            {
                _hits = new List<ARRaycastHit>();
                _raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), _hits, TrackableType.Planes);
                if (_hits.Count > 0)
                {
                    backetBallPole.transform.position = _hits[0].pose.position;
                    backetBallPole.transform.rotation = _hits[0].pose.rotation;
                    if (!backetBallPole.activeInHierarchy)
                    {
                        backetBallPole.SetActive(true);
                    }
                }
                else
                {
#if !UNITY_EDITOR
                backetBallPole.SetActive(false);
#endif
                }
                if (GetMouseButtonDown() && backetBallPole.activeInHierarchy)
                {
                    CreateBall();
                    _onDetectPlacement = false;
                    TweenControl.GetInstance().DelayCall(transform, 0.5f, () =>
                    {
                        _alowThrow = true;
                    });
                }
            }

            if (_alowThrow && !_onDetectPlacement)
            {
                if (GetMouseButtonDown())
                {
                    startPos = (Vector2)MousePosition();
                }
                if (GetMouseButtonUp())
                {
                    _isCorrect = false;
                    _alowThrow = false;
                    distanceTocuh = Vector2.Distance(startPos, (Vector2)MousePosition());
                    ThrowBall(distanceTocuh);
                }
            }

        }
        private Vector2 startPos;
        private float distanceTocuh;
        public void ThrowBall(float distanceTocuh)
        {
            if (_balls.Count > 0)
            {
                _balls[_balls.Count - 1].GetComponent<Rigidbody>().isKinematic = false;
                //Debug.Log(cam.transform.forward);
                _balls[_balls.Count - 1].GetComponent<Rigidbody>().AddForce((cam.transform.forward + new Vector3(0, 1f, 0)) * distanceTocuh * throwForceZ);
                _balls[_balls.Count - 1].CheckNextNextTurn();
            }
        }
        private void CreateBall()
        {
            Ball ball = Instantiate(ballPrefabs, pointStartBall.position, Quaternion.identity, pointStartBall.transform);
            ball.Init(this);
            ball.GetComponent<Rigidbody>().isKinematic = true;
            _balls.Add(ball);
        }

        public void ResetGamePlay()
        {
            GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundWrong, false);
            //_score = 0;
            //scoreText.text = _score.ToString();
            NewThrowBall();
        }

        public void NewThrowBall()
        {
            for (int i = _balls.Count - 1; i >= 0; i--)
            {
                Destroy(_balls[i].gameObject);
            }
            _balls.Clear();
            CreateBall();
            _alowThrow = true;
        }
        public void Correct()
        {
            GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundWin, false);
            _isCorrect = true;
            _score++;
            scoreText.text = _score.ToString();
            ShowEffectAddText();
        }

        private void OnDestroy()
        {
            Screen.orientation = _screenOrientation;
        }

        public void ShowEffectAddText()
        {
            AddText adText = Instantiate(addText, pointAddEffect.position, Quaternion.identity, pointAddEffect);
            adText.Init(1);
        }

        private bool GetMouseButtonDown()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonDown(0);
#else
        return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began;
#endif
        }

        private bool GetMouseButtonUp()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonUp(0);
#else
        return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended;
#endif

        }
        private Vector3 MousePosition()
        {
#if UNITY_EDITOR
            return Input.mousePosition;
#else
        return Input.GetTouch(0).position;
#endif
        }

    }
}

