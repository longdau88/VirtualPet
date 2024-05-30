using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using MainApp.VirtualFriend;

namespace Game.ZombieShoting
{
    public class GamePlay : MonoBehaviour
    {
        public string sceneName = "ZombieShoting";
        public Player player;

        public Text scoreText;
        public Button playBtn;
        public List<Transform> points;
        public List<Enemy> enemyPrefabs;
        public GameObject targetGun1;
        public GameObject targetGun2;
        public RectTransform targetGun2_1;
        public RectTransform targetGun2_2;
        private ScreenOrientation _screenOrientation;
        private bool _lose = false;
        private bool _onPlay = false;
        private List<Enemy> _enemys = new List<Enemy>();
        public List<Enemy> Enemys => _enemys;
        private int _score = 0;
        public bool OnPlay => _onPlay;

        private void Start()
        {
            _screenOrientation = Screen.orientation;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(PlayGame);
        }

        private void ResetGame()
        {
            _score = 0;
            _lose = false;
            _onPlay = false;
            DestroyAllEnemy();
            _enemys.Clear();
            SetScore(0);
            playBtn.gameObject.SetActive(true);
            player.transform.position = Vector3.zero;
            player.transform.localEulerAngles = Vector3.zero;
            player.cam.transform.localEulerAngles = new Vector3(0, 90, 0);
            player.rigidbody.isKinematic = true;
            if (IsInvoking("CreateEnemy"))
                CancelInvoke("CreateEnemy");


        }
        public void ShowTatget(bool TwoCam)
        {
            if (TwoCam)
            {
                targetGun1.SetActive(false);
                targetGun2.SetActive(true);
                float anchoreX = targetGun2.GetComponent<RectTransform>().rect.width / 4f;
                targetGun2_1.anchoredPosition = new Vector2(-anchoreX, 0);
                targetGun2_2.anchoredPosition = new Vector2(anchoreX, 0);
            }
            else
            {
                targetGun1.SetActive(true);
                targetGun2.SetActive(false);
            }
        }

        public void SetScore(int value)
        {
            _score = value;
            scoreText.text = "Score: " + _score.ToString();
        }
        public void AddScore(int value)
        {
            _score += value;
            scoreText.text = "Score: " + _score.ToString();
        }
        public void PlayGame()
        {
            ClockController.Instance.ClockRun(60, 1, () =>
            {
                DestroyAllEnemy();
                VirtualPetManager.Instance.ShowPanelResultDialog(_score, sceneName);
            });
            player.Init(this, 10);
            ResetGame();
            playBtn.gameObject.SetActive(false);
            CreateEnemy();
            InvokeRepeating("CreateEnemy", 0, Random.Range(10, 20));
            _onPlay = true;
        }

        public void CreateEnemy()
        {
            List<Enemy> enemys = Random<Enemy>.GetList(enemyPrefabs, Random.Range(1, 4));
            List<Transform> pos = Random<Transform>.GetList(points, enemys.Count);
            if (enemys.Count == pos.Count)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    var enemy = Instantiate<Enemy>(enemys[i], pos[i].position, Quaternion.identity);
                    _enemys.Add(enemy);
                    enemy.Init(this);
                }
            }

        }
        private void DestroyAllEnemy()
        {
            for (int i = _enemys.Count - 1; i >= 0; i--)
            {
                if (_enemys[i] != null)
                {
                    Destroy(_enemys[i].gameObject);
                }
            }
        }
        public void BackToMainApp()
        {
            DestroyAllEnemy();
            DataManager.Instance.UnloadSceneGame(sceneName);
        }

        private void OnDestroy()
        {
            Screen.orientation = _screenOrientation;
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

        public void Lose()
        {
            if (_lose) return;
            _lose = true;
            _onPlay = false;
            Debug.Log("Lose");
            TweenControl.GetInstance().DelayCall(transform, 2f, () =>
            {
                VirtualPetManager.Instance.ShowPanelResultDialog(_score, sceneName);
            });
        }
    }
}

