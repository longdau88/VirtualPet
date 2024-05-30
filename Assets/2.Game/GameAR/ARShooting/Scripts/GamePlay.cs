using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using MainApp.VirtualFriend;

namespace Game.ARShooting
{
    public class GamePlay : MonoBehaviour
    {
        public string sceneName = "ARShooting";
        public Player player;
        public AudioSource audioSource;
        public Text scoreText;
        public Text timeText;
        public Button playBtn;
        public Button shotBtn;

        public GameObject visual;
        public List<Enemy> enemyPrefabs;

        private ScreenOrientation _screenOrientation;
        private bool _lose = false;
        private bool _onPlay = false;
        private List<Enemy> _enemys = new List<Enemy>();
        public List<Enemy> Enemys => _enemys;
        private int _score = 0;
        public bool OnPlay => _onPlay;

        private List<Pose> posEnemy = new List<Pose>();
        private List<PlacementEnemy> placementEnemy = new List<PlacementEnemy>();

        [SerializeField]
        private ARRaycastManager _raycastManager;
        [SerializeField]
        private ARSessionOrigin _sessionOrigin;
        private List<ARRaycastHit> _hits;
        private int maxPosEnemy = 7;
        private void Start()
        {
            _screenOrientation = Screen.orientation;
            Screen.orientation = ScreenOrientation.Portrait;
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(PlayGame);

            visual.SetActive(false);
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
            if (IsInvoking("CreateEnemy"))
                CancelInvoke("CreateEnemy");
            timeText.text = "60";
            //shotBtn.gameObject.SetActive(false);
            //visual.SetActive(false);

        }
        public void RemoveEnemy(Enemy enemy)
        {
            _enemys.Remove(enemy);
            for (int i = 0; i < placementEnemy.Count; i++)
            {
                if (ReferenceEquals(placementEnemy[i].enemy, enemy))
                {
                    placementEnemy[i].enemy = null;
                }
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
            player.Init(this);
            ResetGame();
            playBtn.gameObject.SetActive(false);
            if (IsInvoking("CreateEnemy"))
                CancelInvoke("CreateEnemy");
            InvokeRepeating("CreateEnemy", 0, Random.Range(2, 4));
            StartCountDownTime();
            _onPlay = true;
            //shotBtn.gameObject.SetActive(true);
        }

        public void CreateEnemy()
        {
            if (_enemys.Count > 6) return;
            List<Enemy> enemys = Random<Enemy>.GetList(enemyPrefabs, Random.Range(1, 4));
            List<Pose> pos = Random<Pose>.GetList(posEnemy, enemys.Count);
            if (enemys.Count == pos.Count)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    if (!HasEnemyInPose(pos[i]))
                    {
                        var enemy = Instantiate<Enemy>(enemys[i], GetPosEnemyByPose(pos[i], player.cam), pos[i].rotation);
                        enemy.transform.localEulerAngles = new Vector3(0, enemy.transform.localEulerAngles.y, 0);
                        _enemys.Add(enemy);
                        enemy.Init(this);
                        placementEnemy.Add(new PlacementEnemy(enemy, pos[i]));
                    }
                }
            }

        }
        private Vector3 GetPosEnemyByPose(Pose pose, GameObject camera)
        {
            Vector3 A = camera.transform.position;
            Vector3 B = pose.position;
            return new Vector3(2 * B.x / 3f + A.x / 3f, 2 * B.y / 3f + A.y / 3f, 2 * B.z / 3f + A.z / 3f);
        }

        private bool HasEnemyInPose(Pose pose)
        {
            for (int i = 0; i < placementEnemy.Count; i++)
            {
                if (placementEnemy[i].pose.Equals(pose) && placementEnemy[i].enemy != null)
                {
                    return true;
                }
            }
            return false;
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
            for (int i = 0; i < placementEnemy.Count; i++)
            {
                placementEnemy[i].enemy = null;
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
            shotBtn.gameObject.SetActive(false);
            Debug.Log("Lose");
            TweenControl.GetInstance().DelayCall(transform, 2f, () =>
            {
                DestroyAllEnemy();
                VirtualPetManager.Instance.ShowPanelResultDialog(_score, sceneName);
            });
        }

        private int _countTime = 0;
        public void StartCountDownTime()
        {
            _countTime = 60;
            if (IsInvoking("LoopTime"))
                CancelInvoke("LoopTime");
            InvokeRepeating("LoopTime", 0, 1);
        }
        private void LoopTime()
        {
            if (_countTime <= 5)
            {
                audioSource.Play();
            }
            timeText.text = _countTime.ToString();
            if (_countTime <= 0)
            {
                CancelInvoke("LoopTime");
                Lose();
            }
            _countTime--;

        }


        private void Update()
        {
            if (_onPlay && posEnemy.Count < maxPosEnemy)
            {
                _hits = new List<ARRaycastHit>();
                _raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), _hits, TrackableType.Planes);
                if (_hits.Count > 0)
                {
                    //visual.transform.position = _hits[0].pose.position;
                    //visual.transform.rotation = _hits[0].pose.rotation;
                    //if (!visual.activeInHierarchy)
                    //{
                    //    visual.SetActive(true);
                    //}
                    if (CheckPos(_hits[0].pose.position))
                    {
                        posEnemy.Add(_hits[0].pose);
                        //visual.transform.position = _hits[0].pose.position;
                        //visual.transform.rotation = _hits[0].pose.rotation;
                        //Instantiate(visual).gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                //visual.SetActive(false);
            }
        }

        private bool CheckPos(Vector3 target)
        {
            if (Vector3.Distance(target, player.transform.position) < 1f) return false;

            for (int i = 0; i < posEnemy.Count; i++)
            {
                if (Vector3.Distance(target, posEnemy[i].position) < 1f) return false;
            }
            return true;
        }

        public void ShotEnemy()
        {
            RaycastHit hit;
            Ray ray = _sessionOrigin.camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast->ok: " + hit.transform.name);
                if (hit.transform.parent != null)
                {
                    Enemy enemy = hit.transform.parent.GetComponent<Enemy>();
                    if (enemy)
                    {
                        enemy.SubHP(1);
                    }
                }
            }
        }
    }
    public class PlacementEnemy
    {
        public Enemy enemy;
        public Pose pose;
        public PlacementEnemy(Enemy enemy, Pose pose)
        {
            this.enemy = enemy;
            this.pose = pose;
        }
    }
}

