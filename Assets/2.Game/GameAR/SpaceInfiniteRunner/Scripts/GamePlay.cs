using MainApp.VirtualFriend;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SpaceInfiniteRunner
{

    public class GamePlay : MonoBehaviour
    {
        public string sceneName = "SpaceInfiniteRunner";
        public Material skybox;
        public float speedMove;
        public float speedRotate;
        public Player player;

        public Text distanceText;
        public Button playBtn;
        public GameObject breakFX;
        public List<Transform> points;
        public List<Obj> objPrefabs;

        private ScreenOrientation _screenOrientation;
        private bool _onMove = false;
        private bool _lose = false;

        public bool OnMove => _onMove;

        private List<Obj> objs = new List<Obj>();
        public List<Obj> Objs => objs;
        private void Start()
        {
            _screenOrientation = Screen.orientation;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            //player.Init(this);
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(PlayGame);

            LoadSkyBox();
        }

        private void ResetGame()
        {
            _lose = false;
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i] != null)
                {
                    Destroy(objs[i].gameObject);
                }
            }
            objs.Clear();
            SetDistance(0);
            distanceText.transform.localScale = Vector3.one;
            distanceText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120f);
            playBtn.gameObject.SetActive(true);
            player.spaceShip.SetActive(true);
            player.transform.position = Vector3.zero;
            player.transform.localEulerAngles = Vector3.zero;
            player.cam.transform.localEulerAngles = new Vector3(0, 90, 0);
            _onMove = false;
            player.rigidbody.isKinematic = true;
            if (IsInvoking("CreateObject"))
                CancelInvoke("CreateObject");

            
        }

        private void LoadSkyBox()
		{
            RenderSettings.skybox = skybox;
        }

        public void SetDistance(float value)
        {
            distanceText.text = "Distance: " + Mathf.Round(value).ToString();
        }

        public void PlayGame()
        {
            player.Init(this);
            ResetGame();
            playBtn.gameObject.SetActive(false);
            player.rigidbody.isKinematic = false;
            player.smoke.SetActive(true);
            _onMove = true;
            InvokeRepeating("CreateObject", 0, 4f);
            CreateObject();
        }

        public void CreateObject()
        {
            List<Obj> obj = Random<Obj>.GetList(objPrefabs, points.Count);
            if (obj.Count == points.Count)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var g = Instantiate<Obj>(obj[i], points[i].position, Quaternion.identity);
                    objs.Add(g);
                    g.Init(this, 10f);
                }
            }
        }

        private void Update()
        {
            if (_onMove)
            {
                SetDistance(player.transform.position.x);
            }
        }

        public void BackToMainApp()
        {
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
            Debug.Log("Lose");
            player.spaceShip.SetActive(false);
            player.smoke.SetActive(false);
            _onMove = false;
            MoveText();
            if (breakFX)
            {
                player.GetComponent<AudioSource>().Play();
                GameObject fx = Instantiate(breakFX, player.transform.position, Quaternion.identity);
                Destroy(fx, 2f);
            }
            TweenControl.GetInstance().DelayCall(transform, 2f, () =>
            {
                VirtualPetManager.Instance.ShowPanelResultDialog(5, sceneName);
            });
        }
        private void MoveText()
        {
            TweenControl.GetInstance().MoveRectY(distanceText.GetComponent<RectTransform>(), -450f, 0.5f);
            TweenControl.GetInstance().Scale(distanceText.gameObject, Vector3.one * 1.5f, 0.5f, () =>
            {
            });
        }

        public void AddPower()
        {

        }
    }
}

