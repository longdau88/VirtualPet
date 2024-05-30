using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRController : MonoBehaviour
{
	public static VRController Instance { get; private set; }
	[SerializeField] string sceneName = "VR 360";
	[Space]
	[SerializeField] MeshRenderer mesh;
	[SerializeField] Button btnSwitch;
	[SerializeField] Button btnBack;

	Texture imageVR;
	Sprite spriteOutput;
	TypeViewVR typeView;
	InputControl oldInput;

	InputControl controller;

	private void Awake()
	{
		if (Instance == null) Instance = this;
	}

	void Start()
    {
		if (btnSwitch)
		{
			btnSwitch.onClick.RemoveAllListeners();
			btnSwitch.onClick.AddListener(OnClickSwitch);
		}

		btnBack.onClick.AddListener(OnClickBack);

		InitVRController();
	}

	public void InitVRController()
	{
		controller = InputControl.Instance;
	}

	private void OnClickBack()
	{
		SceneManager.UnloadSceneAsync(sceneName);
	}

	private void OnClickSwitch()
	{
		//if (!Input.gyro.enabled)
		//{
		//	Input.gyro.enabled = true;
		//}
		//try
		//{
		//	control.ToggleCameraMode();
		//}
		//catch (System.Exception e)
		//{
		//	var stringDebug = control == null ? "null" : "not null";
		//	var stringGyro = SystemInfo.supportsGyroscope ? " has gyro" : " no gyro";

		//	btnSwitch.GetComponentInChildren<Text>().text = e.Message + " .... " + stringDebug + stringGyro;
		//}
	}

	T CopyComponent<T>(T original, GameObject destination) where T : Component
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy as T;
	}
}

public enum TypeViewVR
{
	Image = 0,
	VideoLink = 1,
	Model = 2,
	Discovery
}

