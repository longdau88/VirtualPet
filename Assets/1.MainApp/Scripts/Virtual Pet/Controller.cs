using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainApp.VirtualFriend
{
	public class Controller : MonoBehaviour
	{
		[SerializeField] AudioSource audioSource;
		[SerializeField] MicInput micInput;

		// Start is called before the first frame update
		void Start()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			micInput.onRecordDone = OnRecordDone;
		}

		private void OnRecordDone(AudioClip clip, float time)
		{
			StartCoroutine(DelayCall(1, () =>
			{
				try
				{
					audioSource.clip = clip;
					audioSource.Play();
					var length = (float)time / audioSource.pitch;

					StartCoroutine(DelayCall(length, () =>
					{
						micInput.InitMic();
						micInput._isInitialized = true;
					}));
				}
				catch (System.Exception e)
				{
					Debug.Log(e.Message);
				}

			}));
		}

		IEnumerator DelayCall(float time, System.Action onDone)
		{
			yield return new WaitForSeconds(time);
			onDone?.Invoke();
		}
	}
}