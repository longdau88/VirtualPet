using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class CameraControl : MonoBehaviour
{

    #region public property

    public UPFTCameraMode cameraMode
    {
        get { return _cameraMode; }
        set
        {
            if (_cameraMode != value)
            {
                SetCameraMode(value);
            }
        }
    }

    public Camera currentCamera
    {
        get;
        private set;
    }

    public GameObject single;
    public GameObject dual;

    [Header("Config")]
    public float yaw = 0.0f;
    public float pitch = 0.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public Action<bool> OnChangeCam;

    #endregion

    #region inspector variable

    [SerializeField]
    private UPFTCameraMode _cameraMode = UPFTCameraMode.Stereoscopic;

    public UPFTCameraConfig cameraConfig;

    #endregion

    #region private variable

    private UPFTBaseCameraManager[] _cameraManagers = null;

    #endregion

    float rotationZ;

    #region unity event

    void Awake()
    {
        single = transform.GetChild(0).gameObject;
        dual = transform.GetChild(1).gameObject;


    }

    void Start()
    {
        _cameraManagers = GetComponentsInChildren<UPFTBaseCameraManager>();
        SetCameraConfig(cameraConfig);
        SetCameraMode(cameraMode);

        rotationZ = transform.eulerAngles.z;
        transform.localEulerAngles = new Vector3(0, 90, 0);

        if (SystemInfo.supportsGyroscope)
        {
            var gyro = Input.gyro;
            gyro.enabled = true;
        }
        else
        {
            cameraMode = UPFTCameraMode.Normal;

            single.SetActive(true);
            currentCamera = single.transform.GetChild(0).GetComponent<Camera>();
            dual.SetActive(false);

            //currentCamera.GetComponent<YT_RotateCamera>().enabled = true;
        }
    }

    public void OnClickBack()
    {
        //UINavigation.GetInstance().Show();
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.UnloadSceneAsync(1);
    }


    //	void Update()
    //	{
    //#if UNITY_ANDROID || UNITY_IPHONE

    //		yaw -= speedH * Input.gyro.rotationRateUnbiased.y;
    //		pitch -= speedV * Input.gyro.rotationRateUnbiased.x;
    //		transform.eulerAngles = new Vector3(pitch, yaw, rotationZ);
    //#endif
    //	}

    #endregion

    #region public method
    public void ToggleCameraMode()
    {
        Debug.Log("click");

        switch (cameraMode)
        {
            case UPFTCameraMode.Normal:
                cameraMode = UPFTCameraMode.Stereoscopic;

                dual.SetActive(true);
                single.SetActive(false);
                for (int i = 0; i < dual.transform.childCount; i++)
                {
                    var camera = dual.transform.GetChild(0);
                    if (camera.gameObject.activeSelf && camera.gameObject.CompareTag("MainCamera"))
                    {
                        currentCamera = camera.gameObject.GetComponent<Camera>();
                        break;
                    }
                }
                OnChangeCam?.Invoke(true);
                break;
            case UPFTCameraMode.Stereoscopic:
                cameraMode = UPFTCameraMode.Normal;

                single.SetActive(true);
                currentCamera = single.transform.GetChild(0).GetComponent<Camera>();
                dual.SetActive(false);
                OnChangeCam?.Invoke(false);
                break;
        }
    }

    #endregion


    #region private method

    private void SetCameraMode(UPFTCameraMode mode)
    {
        _cameraMode = mode;

        foreach (UPFTBaseCameraManager manager in _cameraManagers)
        {
            manager.SetCameraMode(mode);

            UPFTCamera[] cameras = manager.cameras;
            foreach (UPFTCamera camera in cameras)
            {
                if (camera.gameObject.activeSelf && camera.gameObject.CompareTag("MainCamera"))
                {
                    currentCamera = camera.gameObject.GetComponent<Camera>();
                    break;
                }
            }
        }
    }

    private void SetCameraConfig(UPFTCameraConfig config)
    {
        foreach (UPFTBaseCameraManager manager in _cameraManagers)
        {
            manager.SetCameraConfig(config);
        }
    }

    #endregion
}
