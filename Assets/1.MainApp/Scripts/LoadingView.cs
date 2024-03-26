using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using DG.Tweening;
using System;

namespace MainApp
{
	public class LoadingView : MonoBehaviour
	{
		public static LoadingView Instance { get; private set; }
		[SerializeField] UIView loadingView;
		[SerializeField] Slider loadingBar;
		[SerializeField] Image loadingBarImg;
		[Space]
		[SerializeField] GameObject loadingCircle;

		float countTimeShow;
		public bool IsShowLoadingCircle { get; private set; }
		public bool IsShowLoadingBar { get; private set; }

		private void Awake()
		{
			if (Instance == null)
				Instance = this;

			loadingView.gameObject.SetActive(false);
		}

		public void ShowLoading(float time, Action onDone = null)
        {
			ShowLoading(true);

			DOVirtual.Float(0, 1, time, value =>
			{
				SetValue(value);
			});
			TweenControl.GetInstance().DelayCall(this.transform, time, () =>
			{
				HideLoading();
				if (onDone != null)
					onDone?.Invoke();
			});
		}
		public void ShowLoading(bool isLoadingAR = false)
		{
			if (loadingView.IsShowing) return;

			IsShowLoadingBar = true;
			loadingView.Show();
			loadingBarImg.fillAmount = 0;

			if (CheckTimeOut != null) StopCoroutine(CheckTimeOut);
			CheckTimeOut = StartCoroutine(ICheckTimeOut(LoadingType.ProgressBar));
		}

		public void HideLoading()
		{
			if (!loadingView.gameObject.activeInHierarchy) return;

			IsShowLoadingBar = false;

			loadingView.Hide();

			StopCoroutine(CheckTimeOut);
		}

		public void CheckToHideLoadingBar(System.Action onHideDone)
		{
			if (!loadingView.gameObject.activeInHierarchy) return;
			HideLoading();
			TweenControl.GetInstance().DelayCall(this.transform, 0.3f, () =>
			{
				onHideDone?.Invoke();
			});
		}

		public void SetValue(float value, float maxValue = 1)
		{
			var duration = 0.12f;
			if ((float)value / (float)maxValue - loadingBarImg.fillAmount > 0.3f) duration = 0.2f;

			loadingBarImg.DOFillAmount((float)value / (float)maxValue, duration);
		}
		

		public void ShowLoadingCirlce(bool AutoHideOnTimeOut = true)
		{
			if (loadingCircle.activeInHierarchy) return;

			IsShowLoadingCircle = true;
			loadingCircle.SetActive(true);
			countTimeShow = Time.realtimeSinceStartup;

			if (AutoHideOnTimeOut)
			{
				if (CheckTimeOut != null) StopCoroutine(CheckTimeOut);
				CheckTimeOut = StartCoroutine(ICheckTimeOut(LoadingType.Circle));
			}
		}

		public void HideLoadingCircle(System.Action onHideDone = null)
		{
			if (!loadingCircle.activeInHierarchy)
			{
				onHideDone?.Invoke();
				return;
			}
			var deltaTime = Time.realtimeSinceStartup - countTimeShow;

			if (deltaTime < 1)
			{
				TweenControl.GetInstance().DelayCall(this.transform, 1 - deltaTime, () =>
				{
					IsShowLoadingCircle = false;
					loadingCircle.SetActive(false);
					  onHideDone?.Invoke();
				});
			}
			else
			{
				IsShowLoadingCircle = false;
				loadingCircle.SetActive(false);
				onHideDone?.Invoke();
			}
		}

		public void CheckToHideLoading()
		{
			if (loadingView.gameObject.activeInHierarchy)
			{
				HideLoading();
			}
			else if (loadingCircle.activeInHierarchy)
			{
				HideLoadingCircle(()=>
				{
					IsShowLoadingCircle = false;
				});
			}
		}

		Coroutine CheckTimeOut;
		IEnumerator ICheckTimeOut(LoadingType loadingType)
		{
			yield return new WaitForSeconds(60);
			switch (loadingType)
			{
				case LoadingType.ProgressBar:
					if (!loadingView.gameObject.activeInHierarchy) yield break;

					HideLoading();
					break;
				case LoadingType.Circle:
					if (!loadingCircle.activeInHierarchy) yield break;
					HideLoadingCircle(null);
					break;
			}
		}
	}

	public enum LoadingType
	{
		ProgressBar,
		Circle
	}
}
