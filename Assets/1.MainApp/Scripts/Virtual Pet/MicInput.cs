using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace MainApp.VirtualFriend
{
	public class MicInput : MonoBehaviour
	{
		#region SingleTon

		public static MicInput Inctance { set; private get; }

		#endregion
		public Action<AudioClip, float> onRecordDone;
		public Action onStartRecord;

		private void Awake()
		{
			if (Inctance == null) Inctance = this;
		}

		public static float MicLoudness;
		private string _device;

		bool isRecording;
		[SerializeField] bool isTest;
		[SerializeField] float valueDemo;
		[SerializeField] float minLoudnessToEnd;
		[SerializeField] float minLoudnessToRecord;
		[SerializeField] float deltaTime;
		private float currentTimeRecord;

		VirtualPetManager manager;

		float deltaTimeToCallUpdate = 0.1f;
		float timeCallUpdate;

		int limitedTimeCreateSound = 180;

		//mic initialization
		public void InitMic()
		{
			try
			{
				if (_device == null) _device = Microphone.devices[0];

				StartCoroutine(ICheckLimitTimeCreateSound());
				_clipRecord = Microphone.Start(_device, true, limitedTimeCreateSound, 44100);

#if UNITY_IOS
				iPhoneSpeaker.ForceToSpeaker();
#endif
			}
			catch (Exception e)
			{
				Debug.Log("fail to init mic input = " + e.Message);
			}

		}

		Coroutine CheckLimitTimeCreateSound;
		IEnumerator ICheckLimitTimeCreateSound()
		{
			yield return new WaitForSeconds(limitedTimeCreateSound);
			StopMicrophone();
			InitMic();
			Debug.Log("Stop and re-init mic");
		}

		void StopMicrophone()
		{
			Microphone.End(_device);
			if (CheckLimitTimeCreateSound != null) StopCoroutine(CheckLimitTimeCreateSound);
		}

		AudioClip _clipRecord;
		int _sampleWindow = 128;
		Coroutine ICheckingLoudness;
		
		float LevelMax()
		{
			float levelMax = 0;
			float[] waveData = new float[_sampleWindow];
			int micPosition = Microphone.GetPosition(_device) - (_sampleWindow + 1); // null means the first microphone
			if (micPosition < 0) return 0;
			_clipRecord.GetData(waveData, micPosition);
			// Getting a peak on the last 128 samples
			for (int i = 0; i < _sampleWindow; i++)
			{
				float wavePeak = waveData[i] * waveData[i];
				if (levelMax < wavePeak)
				{
					levelMax = wavePeak;
				}
			}
			return levelMax;
		}

		private void Start()
		{
			manager = VirtualPetManager.Instance;	
		}

		void Update()
		{
			if (!manager.isFreeToRepeat) return;

			if (isRecording) return;

			if (timeCallUpdate <= 0)
			{
				if (!isTest)
				{
					MicLoudness = LevelMax();
				}
				else
				{
					MicLoudness = valueDemo;
				}

				if (MicLoudness >= minLoudnessToRecord)
				{
					StopMicrophone();
					InitMic();
					currentTimeRecord = Time.realtimeSinceStartup;
					onStartRecord?.Invoke();
					ICheckingLoudness = StartCoroutine(ICheckingLoudnessMic());
				}

				timeCallUpdate = deltaTimeToCallUpdate;
			}
			else
			{
				timeCallUpdate -= Time.deltaTime;
			}
			
		}

		//private void LateUpdate()
		//{
		//	if (!isRecording && MicLoudness >= minLoudnessToRecord)
		//	{
		//		StopMicrophone();
		//		InitMic();
		//		currentTimeRecord = Time.realtimeSinceStartup;
		//		onStartRecord?.Invoke();
		//		ICheckingLoudness = StartCoroutine(ICheckingLoudnessMic());
		//	}
		//}

		IEnumerator ICheckingLoudnessMic()
		{
			yield return new WaitForSeconds(deltaTime);
			
			if (!isTest)
			{
				MicLoudness = LevelMax();
			}
			else
			{
				MicLoudness = valueDemo;
			}

			if (isRecording && MicLoudness < minLoudnessToEnd)
			{
				StopMicrophone();
				_isInitialized = false;

				onRecordDone?.Invoke(_clipRecord, Time.realtimeSinceStartup - currentTimeRecord);
				StopCoroutine(ICheckingLoudness);
			}
			else if (isRecording)
			{
				StopCoroutine(ICheckingLoudness);
				ICheckingLoudness = StartCoroutine(ICheckingLoudnessMic());
			}
			else
			{
				StopCoroutine(ICheckingLoudness);
			}
		}

		public void OnStartRecord()
		{
			isRecording = true;
		}

		public void OnNotRecord()
		{
			isRecording = false;
		}

		[HideInInspector]
		public bool _isInitialized;
		// start mic when scene starts
		void OnEnable()
		{
			InitMic();
			_isInitialized = true;
		}

		//stop mic when loading a new level or quit application
		void OnDisable()
		{
			isRecording = false;
			StopMicrophone();
		}

		void OnDestroy()
		{
			isRecording = false;
			StopMicrophone();
		}

		// make sure the mic gets started & stopped when application gets focused
		void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				if (!_isInitialized)
				{
					InitMic();
					_isInitialized = true;
				}
			}
			if (!focus)
			{
				isRecording = false;
				StopMicrophone();
				_isInitialized = false;

			}
		}
	}
}