using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MainApp;
using Spine.Unity;
using UnityEngine.XR.ARSubsystems;
using Game;
using System;
using MainApp.VirtualFriend;

public class ARController : MonoBehaviour
{
    public static ARController Instance;
    [SerializeField] Camera cameraAR;
    [SerializeField] ARPlaneManager planeManager;

    [SerializeField] ARRaycastManager arRaycast;

    [SerializeField] Transform targetSpawn;
    [SerializeField] ObjectAR objectSpawnPref;

    [SerializeField] Button btnBack;
    [SerializeField] SkeletonAnimation animTracking;
    [Space]
    [SerializeField] float zoomOutMin;
    [SerializeField] float zoomOutMax;
    [Space]
    [SerializeField] List<AudioClip> lstVoiceNoti;
    [SerializeField] AudioClip flashSound;
    [SerializeField] AudioClip yeahSound;
    [SerializeField] GameObject effectShow;

    private List<ARRaycastHit> hitResults;
    private Pose placementPose;
    int countSpawn;

    int targetFrameRate;
    int maxObjectSpawn = 30;
    GameObject objectAr;

    GameObject objectSpawn;

    bool isFirstInstance = true;
    int idLanguage;

    bool isPlaneDetected = false;
    bool animTrackingHided = false;

    private bool _init = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnClickButtonBack);

        var valueGraphicMemory = (float)SystemInfo.graphicsMemorySize / 1024f;
        var valueSystemMemory = (float)SystemInfo.systemMemorySize / 1024f;

#if UNITY_ANDROID
        if (valueSystemMemory >= 6)
        {
            targetFrameRate = 60;
        }
        else if (valueSystemMemory >= 4)
        {
            targetFrameRate = 55;
        }
        else if (valueSystemMemory >= 2)
        {
            targetFrameRate = 40;
        }
        else
        {
            targetFrameRate = 35;
        }
#elif UNITY_IOS
				if (valueSystemMemory >= 4)
				{
					targetFrameRate = 60;
				}
				else if (valueSystemMemory >= 3)
				{
					targetFrameRate = 55;
				}
				else if (valueSystemMemory >= 2)
				{
					targetFrameRate = 55;
					//MainView.youtubeControl.videoQuality = YoutubeSettings.YoutubeVideoQuality.STANDARD;
				}
				else if (valueSystemMemory > 1)
				{
					targetFrameRate = 50;
					//MainView.youtubeControl.videoQuality = YoutubeSettings.YoutubeVideoQuality.STANDARD;
				}
				else
				{
					targetFrameRate = 45;
					MainView.youtubeControl.videoQuality = YoutubeSettings.YoutubeVideoQuality.STANDARD;
				}
#endif

        SetTargetFrameRate();
        InitStart();
    }
    public void SetTargetFrameRate()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    public void InitARViewVocab(GameObject objectAR)
    {
        Init(objectAR);
    }

    private void Init(GameObject objectAR)
    {
        isPlaneDetected = false;

        if (LanguageController.Instance)
            idLanguage = LanguageController.Instance.GetIdLanguage();

        maxObjectSpawn = 2;
        if (hiddenMeshPlane)
        {
            arRaycast.GetComponent<ARPlaneManager>().planePrefab = plane.gameObject;
        }
        else
        {
            arRaycast.GetComponent<ARPlaneManager>().planePrefab = planeDebug.gameObject;
        }

        _init = true;
        PlayerControl.Instance.Init();

        objectSpawnPref = objectAR.GetComponent<ObjectAR>();

        animTracking.gameObject.SetActive(true);
    }


    private void OnClickButtonBack()
    {
        ClearAllAROject();
        TweenControl.GetInstance().KillDelayCallNew(this.transform);
        DataManager.Instance.UnloadSceneAr(() =>
        {
            // Destroy Scenes AR

            VirtualPetManager.Instance.isPlayGame = false;
            VirtualPetManager.Instance.ShowPanelResultDialog(5);
        });
    }

    void Update()
    {
        if (!_init) return;

        SpawnObjectAR();
        CheckToZoomObject();

        if (IsPlaneDetected()) CheckToHideAnimTracking();
    }

    private bool IsMaxObjectSpawn()
    {
        return (countSpawn >= maxObjectSpawn);
    }

    private bool IsPlaneDetected()
    {
        if (isPlaneDetected) return true;
        if (planeManager.trackables.count > 0)
        {
            isPlaneDetected = true;
            return true;
        }
        return false;
    }

    private void CheckToHideAnimTracking()
    {
        if (animTrackingHided) return;
        if (animTracking.gameObject.activeInHierarchy)
        {
            animTracking.gameObject.SetActive(false);
            animTrackingHided = true;
        }
    }

    public Vector3 GetCameraPostion()
    {
        return cameraAR.transform.position;
    }

    private void ZoomObject(float increment)
    {
        objectSpawn.transform.localScale = new Vector3(Mathf.Clamp(objectSpawn.transform.localScale.x + increment, zoomOutMin, zoomOutMax),
            Mathf.Clamp(objectSpawn.transform.localScale.y + increment, zoomOutMin, zoomOutMax),
            Mathf.Clamp(objectSpawn.transform.localScale.z + increment, zoomOutMin, zoomOutMax));
    }

    private void CheckToZoomObject()
    {
        if (objectSpawn == null) return;

        if (Input.touchCount == 2 && !_arObjectSelected.isclick && !_arObjectSelected.OnMove)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevManitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float offset = currentMagnitude - prevManitude;

            ZoomObject(offset * 0.001f);
        }
    }


    #region Type Has Control

    public GameObject visual;
    public bool hiddenMeshPlane = true;
    public ARPlane plane;
    public ARPlane planeDebug;

    [Header("Demo setup")]
    public bool demo;
    public ObjectAR arObjectDemo;
    public List<PlayerActionName> playerActionNameDemo;


    private List<ARRaycastHit> _hits;
    private bool _onDetectPlacement = false;
    private bool _firstFindPlacement = false;

    private List<ObjectAR> _arObjects = new List<ObjectAR>();
    private ObjectAR _arObjectSelected;

    private ScreenOrientation _screenOrientation;

    private List<PlayerActionName> _playerActionName;
    public List<PlayerActionName> PlayerActionName => _playerActionName;

    private float distanceRotateObjectAR = 1f;
    private void InitStart()
    {
        _onDetectPlacement = true;
        _arObjects.Clear();
#if UNITY_EDITOR
        visual.SetActive(true);
#else
            visual.SetActive(false);
#endif
        if (demo)
        {
            Init(arObjectDemo.gameObject);
        }

        _firstFindPlacement = false;
    }

    public void Play(ObjectAR arObject)
    {
        _arObjectSelected = arObject;
        _arObjects.Add(arObject);
    }

    public void AddAROject(ObjectAR arObject)
    {
        _arObjectSelected = arObject;
        _arObjects.Add(arObject);
    }

    public void Clear()
    {
        _arObjectSelected = null;
        _arObjects.Clear();
    }

    private void SpawnObjectAR()
    {
        if (_onDetectPlacement)
        {
            _hits = new List<ARRaycastHit>();
            if (arRaycast.Raycast(cameraAR.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), _hits, TrackableType.PlaneWithinPolygon))
            {
                if (_hits.Count > 0)
                {
                    visual.transform.position = _hits[0].pose.position;
                    visual.transform.rotation = _hits[0].pose.rotation;

                    if (!visual.activeInHierarchy)
                        visual.SetActive(true);

                    if (!_firstFindPlacement)
                    {
                        var arObj = demo ? arObjectDemo : objectSpawnPref;
                        ObjectAR obj = Instantiate(arObj, visual.transform);
                        obj.IsPreview = true;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localEulerAngles = Vector3.zero;
                        if (obj.rb == null)
                        {
                            obj.rb = obj.GetComponent<Rigidbody>();
                        }
                        if (obj.rb != null)
                        {
                            obj.rb.isKinematic = true;
                        }

                        TweenControl.GetInstance().DelayCall(transform, 5f, () =>
                        {
                            if (visual.activeInHierarchy)
                            {
                                SpawnARObject();
                            }
                            //else
                            //{
                            //    PlayerControl.Instance.CancelHightLightCreateBtn();
                            //}
                            PlayerControl.Instance.Show();
                        });

                        _firstFindPlacement = true;
                    }
                }
                else
                {
#if !UNITY_EDITOR
                visual.SetActive(false);
#endif
                }
            }
            else
            {
#if !UNITY_EDITOR
                visual.SetActive(false);
#endif
            }
        }

        if (GetMouseButtonDown())
        {
            RaycastHit hit;
            Ray ray = cameraAR.ScreenPointToRay(MousePosition());

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast->ok: " + hit.transform.name);

                ObjectAR arObj = hit.transform.GetComponent<ObjectAR>();
                if (arObj)
                {
                    if (arObj.IsPreview)
                    {
                        SpawnARObject();
                    }
                    else
                    {
                        _arObjectSelected = arObj;
                        SetSelected(arObj);
                        if (!demo)
                            PlayVoiceTouch();
                    }
                }
            }
        }
    }
    private Vector3 _pointLookCam;
    public void SpawnARObject()
    {
        PlayerControl.Instance.Show();

        var arObj = demo ? arObjectDemo : objectSpawnPref;

        /*if (_arObjects.Count >= maxObjectSpawn)
        {
            if (!demo)
                FAQView.Instance.ShowFAQCustom(LabelLanguage.MaxObjectSpawn[idLanguage], true, null, () => FAQView.Instance.ShowChat(false));
            return;
        }*/

        if (visual.activeInHierarchy && arObj != null)
        {
            visual.SetActive(false);
            _onDetectPlacement = false;

            GameObject effect = Instantiate(effectShow);
            effect.transform.position = visual.transform.position + new Vector3(0, 0.02f, 0);
            effect.transform.eulerAngles = visual.transform.eulerAngles;
            Destroy(effect, 2f);
            if (GameAudio.Instance)
            {
                if (flashSound)
                    GameAudio.Instance.PlayClip(SourceType.SOUND_FX, flashSound, false);
                TweenControl.GetInstance().DelayCall(transform, 1.5f, () =>
                {
                    if (yeahSound)
                        GameAudio.Instance.PlayClip(SourceType.SOUND_FX, yeahSound, false);
                });
            }
            TweenControl.GetInstance().DelayCall(transform, 0.3f, () =>
            {
                animTracking.gameObject.SetActive(false);
                ObjectAR obj = Instantiate(arObj, visual.transform.position, visual.transform.rotation);
                _arObjects.Add(obj);
                _arObjectSelected = obj;
                obj.SpawnObject();
                SetSelected(obj);
                objectSpawn = obj.gameObject;

                objectSpawn.transform.localScale = Vector3.one * 1.1f;

                _pointLookCam = new Vector3(cameraAR.transform.position.x, objectSpawn.transform.position.y, cameraAR.transform.position.z);
                objectSpawn.transform.LookAt(_pointLookCam, Vector3.up);

                if (_arObjects.Count < maxObjectSpawn)
                {
                    visual.SetActive(true);
                    if (visual.GetComponent<MeshRenderer>() != null)
                    {
                        visual.GetComponent<MeshRenderer>().enabled = true;
                    }
                    if (visual.transform.childCount > 0)
                    {
                        Destroy(visual.transform.GetChild(0).gameObject);
                    }
                    _onDetectPlacement = true;
                }
            });
            //if (!demo)
            //    PlayVoiceTouch();
        }
        else
        {
            /*if (!demo && _arObjects.Count == 0)
                FAQView.Instance.ShowFAQCustom(LabelLanguage.PlaneNotDetected[idLanguage], true, null, () => FAQView.Instance.ShowChat(false));*/
        }

    }

    private AudioClip _audioTouch;
    public bool isPlayVoice { get; set; }
    private void PlayVoiceTouch()
    {
        if (AudioController.Instance.GetAudioSource(MainApp.AudioType.VOICE).isPlaying) return;
        if (_audioTouch != null)
        {
            isPlayVoice = true;
            GameAudio.Instance.PlayClip(Game.SourceType.VOICE_OVER, _audioTouch, true, () =>
            {
                isPlayVoice = false;
            });
        }
    }

    private void SetSelected(ObjectAR arObj)
    {
        for (int i = 0; i < _arObjects.Count; i++)
        {
            if (ReferenceEquals(_arObjects[i], arObj))
            {
                arObj.SetSelected(true);
                objectSpawn = arObj.gameObject;
            }
            else
            {
                _arObjects[i].SetSelected(false);
            }
        }
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

    private void ClearAllAROject()
    {
        for (int i = _arObjects.Count - 1; i >= 0; i--)
        {
            if (_arObjects[i])
            {
                Destroy(_arObjects[i].gameObject);
            }
        }
    }

    #endregion
}

public enum TypeInit
{
    Vocab,
    Phonic,
    Reward,
    Discovery,
    DiscoveryPhonic,
    Vietnamese,
    FlashCard
}

