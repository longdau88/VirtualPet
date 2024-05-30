﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// 端末傾き算出用クラス.
[System.Serializable]
public class UPFTHeadTracker : MonoBehaviour {

#region static readonly variable

	#if UNITY_ANDROID
	private static readonly Vector3 INIT_FORWARD_VECTOR = Vector3.forward;
	#elif UNITY_IPHONE
	private static readonly Vector3 INIT_FORWARD_VECTOR = Vector3.right;
	private static readonly Quaternion CORRECT_ROTATION_X = Quaternion.Euler(90, 0, 0);
	#endif

	#if UNITY_ANDROID || UNITY_IPHONE
	private static readonly Vector3 EXPECT_INIT_FORWARD_VECTOR = Vector3.forward;
#endif

	

	#endregion

	#region public property

	// カメラモード.
	public UPFTCameraMode cameraMode {
		get { return _cameraMode; }
		set {
			if (_cameraMode != value) {
				SetCameraMode(value);
			}
		}
	}

	// カメラオブジェクト取得用.
	// Stereoscopicモードの場合は右側のカメラを参照する.
	public Camera currentCamera {
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

	#endregion

	#region inspector variable

	[SerializeField]
	private UPFTCameraMode _cameraMode = UPFTCameraMode.Stereoscopic;

	// カメラ設定用パラメーター.
	public UPFTCameraConfig cameraConfig;

#endregion

#region private variable

	// カメラ管理オブジェクト.
	private UPFTBaseCameraManager[] _cameraManagers = null;

	#if UNITY_ANDROID
	private static AndroidJavaObject _plugin = null;
	#endif

	#if  UNITY_ANDROID || UNITY_IPHONE
	private Quaternion correctRotationY;
#endif

	#endregion

	//float rotationZ;

#region unity event

	void Awake()
	{
		single = transform.GetChild(0).gameObject;
		dual = transform.GetChild(1).gameObject;

//#if UNITY_ANDROID && !UNITY_EDITOR
//		_plugin = new AndroidJavaObject("com.upft.vr.cardboardbridge.CardboardBridge");
//		if (_plugin != null) {
//			_plugin.Call("init");
//		}

//		correctRotationY = Quaternion.identity;
//#elif UNITY_IPHONE
//		correctRotationY = Quaternion.FromToRotation(INIT_FORWARD_VECTOR, EXPECT_INIT_FORWARD_VECTOR);
//#endif
	}


	// Use this for initialization
	void Start () {
		_cameraManagers = GetComponentsInChildren<UPFTBaseCameraManager>();
		SetCameraConfig(cameraConfig);
		SetCameraMode(cameraMode);


		StartTracking();

		//rotationZ = transform.eulerAngles.z;
		//transform.localEulerAngles = new Vector3(0, 90, 0);

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

	
	// Update is called once per frame
//	void Update () {

//#if UNITY_ANDROID || UNITY_IPHONE
//        //transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, 0);
//        //transform.localRotation = UpdateTracking();

//        if (Input.touchCount > 0 || Input.GetMouseButton(0))
//        {
//            yaw -= speedH * Input.GetAxis("Mouse X");
//            pitch -= speedV * Input.GetAxis("Mouse Y");
//        }
//        else
//        {
//            yaw -= speedH * Input.gyro.rotationRateUnbiased.y;
//            pitch -= speedV * Input.gyro.rotationRateUnbiased.x;
//        }
//        transform.eulerAngles = new Vector3(pitch, yaw, rotationZ);

//#endif
//        //transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, -Input.gyro.rotationRateUnbiased.z);

//    }

	private static Quaternion ConvertRotation(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}

	private Quaternion GetRotFix()
	{
		return Quaternion.identity;
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus) {
			PauseTracking();
		} else {
			RestartTracking();
		}
	}

	void OnDestroy()
	{
		StopTracking();
	}

	void OnApplicationQuit()
	{
		StopTracking();
	}


#endregion

#region public method

	// カメラモードの変更処理.
	// Normalだった場合はStereoscopicに、Stereoscopicだった場合はNormalに変更.
	public void ToggleCameraMode()
	{
		Debug.Log("click");
		//switch (_cameraMode) {
		//case UPFTCameraMode.Normal:
		//		if (btnBack)
		//		{
		//			btnBack.transform.GetChild(0).gameObject.SetActive(false);
		//		}
		//		SetCameraMode(UPFTCameraMode.Stereoscopic);

		//	break;
		//case UPFTCameraMode.Stereoscopic:
		//		if (btnBack)
		//		{
		//			btnBack.transform.GetChild(0).gameObject.SetActive(true);
		//		}
		//SetCameraMode(UPFTCameraMode.Normal);

		//		break;
		//}

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

				break;
			case UPFTCameraMode.Stereoscopic:
				cameraMode = UPFTCameraMode.Normal;

				single.SetActive(true);
				currentCamera = single.transform.GetChild(0).GetComponent<Camera>();
				dual.SetActive(false);
				break;
		}
	}

	public void ResetOrientation() {

		//#if UNITY_ANDROID && !UNITY_EDITOR
		//Vector3 f = transform.forward;
		//correctRotationY = Quaternion.FromToRotation(new Vector3(f.x, 0, f.z).normalized, INIT_FORWARD_VECTOR) * correctRotationY;
		//#elif UNITY_IPHONE
		//Vector3 f = transform.forward;
		//correctRotationY = Quaternion.FromToRotation(new Vector3(f.x, 0, f.z).normalized, EXPECT_INIT_FORWARD_VECTOR) * correctRotationY;
		//#endif
	}

#endregion


#region private method

	private void StartTracking() {

//#if UNITY_ANDROID && !UNITY_EDITOR
//		if (_plugin != null) {
//			_plugin.Call("startTracking");
//		}
//#elif UNITY_IPHONE
//		Input.gyro.enabled = true;
//#endif
	}

	private void StopTracking() {
//#if UNITY_ANDROID && !UNITY_EDITOR
//		if (_plugin != null) {
//			_plugin.Call("stopTracking");
//		}
//#elif UNITY_IPHONE
//		Input.gyro.enabled = false;
//#endif
	}

	private void PauseTracking() {
		//#if UNITY_ANDROID && !UNITY_EDITOR
		//		if (_plugin != null) {
		//			_plugin.Call("stopTracking");
		//		}
		//		Vector3 f = transform.forward;
		//		correctRotationY = Quaternion.FromToRotation(INIT_FORWARD_VECTOR, new Vector3(f.x, 0, f.z).normalized);
		//#endif
	}

	private void RestartTracking()
		{
//#if UNITY_ANDROID && !UNITY_EDITOR
//		if (_plugin != null) {
//			_plugin.Call("startTracking");
//		}
//#endif
		}

		private Quaternion UpdateTracking() {

		//#if UNITY_EDITOR
		//		return Quaternion.identity;
		//#elif UNITY_ANDROID
		//		if (_plugin != null) {
		//			float[] q = _plugin.Call<float[]>("getQuaternion");
		//			return correctRotationY *  new Quaternion(q[0], q[1], q[2], q[3]);
		//		}

		//		return Quaternion.identity;

		//#elif UNITY_IPHONE
		//		Quaternion q = Input.gyro.attitude;
		//		Quaternion qq = new Quaternion(-q.x, -q.z, -q.y, q.w);
		//		return correctRotationY * qq * CORRECT_ROTATION_X;
		//#endif
		return Quaternion.identity;
	}

	// カメラモード設定処理.
	private void SetCameraMode(UPFTCameraMode mode)
	{
		_cameraMode = mode;
		
		foreach (UPFTBaseCameraManager manager in _cameraManagers) {
			manager.SetCameraMode(mode);
			
			UPFTCamera[] cameras = manager.cameras;
			foreach (UPFTCamera camera in cameras) {
				if (camera.gameObject.activeSelf && camera.gameObject.CompareTag("MainCamera")) {
					currentCamera = camera.gameObject.GetComponent<Camera>();
					break;
				}
			}
		}
	}
	
	// カメラパラメータ設定処理.
	// 初回起動時にしか設定されません.
	private void SetCameraConfig(UPFTCameraConfig config)
	{
		foreach (UPFTBaseCameraManager manager in _cameraManagers) {
			manager.SetCameraConfig(config);
		}
	}

#endregion
}
