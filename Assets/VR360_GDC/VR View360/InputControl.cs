using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputControl : MonoBehaviour
{
	public static InputControl Instance { get; private set; }
	public GameObject cameraOrbit;

	public float rotateSpeed = 8f;
	public Transform target;

	[Space]
	[SerializeField] float zoomOutMax = 2;
	[SerializeField] float zoomOutMin = 0.5f;

	[Space]
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip bgMusic;

	bool IsDragging;
	Vector2 OldDragPosition;

	PlayerController playerControl;

	Vector3 maxScale;
	Vector3 minScale;
	Vector3 defaultScale;

	Image imgInput;

	private void Awake()
	{
		if (Instance == null) Instance = this;
	}

	private void Start()
	{
		imgInput = GetComponent<Image>();
		var player = target.GetChild(0);
		if (player.GetComponent<PlayerController>() == null)
		{
			playerControl = player.gameObject.AddComponent<PlayerController>();
		}
		else
		{
			playerControl = player.GetComponent<PlayerController>();
		}

		defaultScale = playerControl.transform.localScale;

		maxScale = new Vector3(defaultScale.x * zoomOutMax, defaultScale.y * zoomOutMax, defaultScale.z * zoomOutMax);
		minScale = new Vector3(defaultScale.x * zoomOutMin, defaultScale.y * zoomOutMin, defaultScale.z * zoomOutMin);

		if (playerControl.GetComponent<Animation>() != null) playerControl.GetComponent<Animation>().wrapMode = WrapMode.Loop;

		//if (MainView.Instance.IsUnityPC)
		//{
		//	rotateSpeed = rotateSpeed * 2;
		//}
	}

	public void HideVr()
	{
		cameraOrbit.SetActive(false);
		playerControl.gameObject.SetActive(false);
	}

	public void ShowVr()
	{
		cameraOrbit.SetActive(true);
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			CheckToZoomObject();

#if !UNITY_EDITOR && !UNITY_STANDALONE
			if (Input.touchCount != 1) return;
			if (Input.GetTouch(0).phase != TouchPhase.Moved) return;
#endif
			//if (isFirstDrag)
			//{
			//	isFirstDrag = false;
			//}

			float h = rotateSpeed * Input.GetAxisRaw("Mouse X");
			float v = rotateSpeed * Input.GetAxisRaw("Mouse Y");

			if (cameraOrbit.transform.eulerAngles.z + v <= 0.1f || cameraOrbit.transform.eulerAngles.z + v >= 179.9f)
				v = 0;

			cameraOrbit.transform.eulerAngles = new Vector3(cameraOrbit.transform.eulerAngles.x,
				cameraOrbit.transform.eulerAngles.y + h, cameraOrbit.transform.eulerAngles.z + v);
		}

#if UNITY_EDITOR || UNITY_STANDALONE
		float scrollFactor = Input.GetAxis("Mouse ScrollWheel");

		if (scrollFactor != 0)
		{
			cameraOrbit.transform.localScale = new Vector3(Mathf.Clamp(cameraOrbit.transform.localScale.x * (1f - scrollFactor), 0.8f, zoomOutMax),
				Mathf.Clamp(cameraOrbit.transform.localScale.y * (1f - scrollFactor), 0.8f, zoomOutMax),
				Mathf.Clamp(cameraOrbit.transform.localScale.z * (1f - scrollFactor), 0.8f, zoomOutMax));
		}
#endif

	}

	private void ZoomObject(float increment)
	{
		playerControl.transform.localScale = new Vector3(Mathf.Clamp(playerControl.transform.localScale.x + (increment * defaultScale.x), minScale.x, maxScale.x),
			Mathf.Clamp(playerControl.transform.localScale.y + (increment * defaultScale.y), minScale.y, maxScale.y),
			Mathf.Clamp(playerControl.transform.localScale.z + (increment * defaultScale.z), minScale.z, maxScale.z));
	}

	private void CheckToZoomObject()
	{
		if (playerControl == null) return;
		if (Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			if (touchZero.phase != TouchPhase.Moved || touchOne.phase != TouchPhase.Moved) return;

			var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevManitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

			float offset = currentMagnitude - prevManitude;

			ZoomObject(offset * 0.001f);
		}
	}


}
