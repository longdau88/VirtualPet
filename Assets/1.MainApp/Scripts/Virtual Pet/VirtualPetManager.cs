using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Game.Utils;
using MainApp;
using DG.Tweening;
using System.Linq;
using Game;
using Doozy.Engine.UI;
using UnityEngine.Android;

namespace MainApp.VirtualFriend
{
	public class VirtualPetManager : MonoBehaviour
	{
		public static VirtualPetManager Instance { get; private set; }
		[SerializeField] Light light;
		[SerializeField] Light lamp;
		[Header("Screen")]
		[SerializeField] GameObject mainScreen;
		[SerializeField] GameObject bathRoom;
		[SerializeField] GameObject bedRoom;
		[Header("Washing")]
		[SerializeField] ParticleSystem waterShower;
		[SerializeField] ParticleSystem bubblesFx;
		[SerializeField] SoapController soap;
		[Header("Record")]
		[SerializeField] MicInput micInput;
		[SerializeField] AudioSource audioSource;
		[Space]
		[SerializeField] MyPetController myPetInMainScreen;
		[SerializeField] MyPetController myPetInBathRoom;
		[SerializeField] MyPetController myPetInBedRoom;
		[SerializeField] SpriteRenderer bed;
		[Header("UI")]
		[SerializeField] Button btnEat;
		[SerializeField] Button btnGoToilet;
		[SerializeField] Button btnGoToSleep;
		[SerializeField] Button btnLight;
		[Space]
		[SerializeField] UIView panelBtnPlay;
		[SerializeField] Button btnPlayGame;
		[SerializeField] Button btnPlayAR;
		[Space]
		[SerializeField] UIView panelChooseGame;
		[SerializeField] ChooseGameView chooseGameView;
		[Space]
		[SerializeField] UIView panelResultDialog;
		[SerializeField] GameController resultDialog;
		[Space]
		[SerializeField] UIView panelPermissionAR;
		[SerializeField] PopupPermissionAR popupPermissionAR;
		[Space]
		[SerializeField] Image foodIcon;
		[SerializeField] Text txtMinusGold;
		[SerializeField] Image imgGold;
		[SerializeField] Text txtGoldPlayer;
		[Space]
		[SerializeField] Image imgValueHungry;
		[SerializeField] Image imgValueDirty;
		[SerializeField] Image imgValueSleep;
		[Space]
		[SerializeField] Image overlay;
		[Header("Sound")]
		[SerializeField] AudioClip soundWaterShower;
		[SerializeField] AudioClip soundLight;
		[SerializeField] AudioClip soundSleep;
		[Header("Config")]
		[SerializeField] int numGoldToEat;
		[Space]
		[SerializeField] float timeAnimEat;
		[SerializeField] float timeAnimToilet;
		[Space]
		[SerializeField] Color colorWhite;
		[SerializeField] Color colorRed;
		[Space]
		[SerializeField] Color colorLightOff;
		[Space]
		[SerializeField] List<TimeInVirtualPet> lstTimeStart;
		[SerializeField] List<TimeInVirtualPet> lstTimeEnd;

		private PetState petState;

		//MyPetData data;
		Coroutine countDownValue;

		float deltaTimeOnDisable;
		float deltaTimeOnPause;

		bool isDisable = false;
		bool isPauseApp = false;

		float deltaTimeCallUpdateToMinutes;
		bool isPauseUpdate;
		public bool isFreeToAttack { get; private set; }
		public bool isFreeToRepeat { get; private set; }
		bool isRecording;

		TimePetManager TimeManager;
		MyPetController myPet;

		bool isWashing;

		public int countGold;

		private void Awake()
		{
			Instance = this;
		}

		void Start()
		{
			TimeManager = TimePetManager.Instance;

			foodIcon.transform.localScale = Vector3.zero;

			panelBtnPlay.Hide();
			panelChooseGame.Hide();
			panelResultDialog.Hide();
			panelPermissionAR.Hide();

			countGold = 0;

			overlay.enabled = false;

			waterShower.Stop();
			bubblesFx.Stop();

			AddListener();
			InitData();

			chooseGameView.InitPanelChooseGame(DataManager.Instance.lstGame);
		}

		private void AddListener()
		{
			btnPlayGame.onClick.RemoveAllListeners();
			btnPlayGame.onClick.AddListener(OnClickPlayGame);

			btnPlayAR.onClick.RemoveAllListeners();
			btnPlayAR.onClick.AddListener(OnClickPlayAR);

			btnEat.onClick.RemoveAllListeners();
			btnEat.onClick.AddListener(OnClickEat);

			btnGoToSleep.onClick.RemoveAllListeners();
			btnGoToSleep.onClick.AddListener(OnClickGotoSleep);

			btnGoToilet.onClick.RemoveAllListeners();
			btnGoToilet.onClick.AddListener(OnClickToilet);

			btnLight.onClick.RemoveAllListeners();
			btnLight.onClick.AddListener(OnClickLight);
			
			soap.onWashDone = OnWashDone;
			soap.onStartWash = OnStartWash;

			micInput.onStartRecord = OnStartRecord;
			micInput.onRecordDone = OnRecordDone;
		}

		private void InitData()
        {
			LoadingView.Instance.ShowLoading(true);

			DOVirtual.Float(0, 1, 3f, value =>
			{
				LoadingView.Instance.SetValue(value);
			});
			TweenControl.GetInstance().DelayCall(this.transform, 3f, () =>
			{
				LoadingView.Instance.HideLoading();

#if UNITY_IOS
					if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
					{
						ShowPopupPermissionAR(() =>
						{
							StartGame();
						});
					}
					else 
						StartGame();
#endif

#if UNITY_ANDROID
				if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
				{
					ShowPopupPermissionAR(() =>
					{
						StartGame();
					});
				}
				else
					StartGame();
#endif
			});
        }

		private void StartGame()
        {
			if (IsCreateGameApp("4"))
			{
				petState = PetState.Normal;
				SaveLastScreenPrefabs((int)petState);

				SaveCreateGameApp("4");
				SaveGoldPlayer(countGold);

				TimeManager.InitData(true);

				TimeManager.OnUpdateTime = UpdateValueTime;
			}
			else
			{
				GetGoldPlayer();

				petState = GetLastScreenPrefabs();

				TimeManager.InitData(false);

				TimeManager.OnUpdateTime = UpdateValueTime;
			}
		}

		#region UI SavePlayerPrefs
		public bool IsCreateGameApp(string s)
		{
			return !PlayerPrefs.HasKey(StaticConfig.CREATE_APP + s);
		}
		public void SaveCreateGameApp(string s)
		{
			PlayerPrefs.SetString(StaticConfig.CREATE_APP + s, DateTime.Now.ToString());
			PlayerPrefs.Save();
		}
		public string GetCreateGameApp(string s)
		{
			return PlayerPrefs.GetString(StaticConfig.CREATE_APP + s, string.Empty);
		}

		public bool IsCreateGameApp()
		{
			return !PlayerPrefs.HasKey(StaticConfig.CREATE_APP);
		}
		public void SaveCreateGameApp()
		{
			PlayerPrefs.SetString(StaticConfig.CREATE_APP, DateTime.Now.ToString());
			PlayerPrefs.Save();
		}
		public string GetCreateGameApp()
		{
			return PlayerPrefs.GetString(StaticConfig.CREATE_APP, string.Empty);
		}

		public void SaveGoldPlayer(int value)
		{
			countGold = value;
			txtGoldPlayer.text = countGold.ToString();
			PlayerPrefs.SetInt(StaticConfig.GOLD_PLAYER_APP, countGold);
			PlayerPrefs.Save();
		}
		public int GetGoldPlayer()
		{
			if (PlayerPrefs.HasKey(StaticConfig.GOLD_PLAYER_APP))
			{
				countGold = PlayerPrefs.GetInt(StaticConfig.GOLD_PLAYER_APP);
			}
            else
            {
				SaveGoldPlayer(0);
			}
			txtGoldPlayer.text = countGold.ToString();
			return countGold;
		}

		public void SaveLastScreenPrefabs(int value)
        {
			PlayerPrefs.SetInt(StaticConfig.LAST_SCREEN, value);
			PlayerPrefs.Save();
		}
		public PetState GetLastScreenPrefabs()
		{
			var stage = PlayerPrefs.GetInt(StaticConfig.LAST_SCREEN);
            switch (stage)
            {
				case (int)PetState.Eat:
					return PetState.Eat;
					break;
				case (int)PetState.InToilet:
					return PetState.InToilet;
					break;
				case (int)PetState.Sleep:
					return PetState.Sleep;
					break;
				case (int)PetState.ReadyToSleep:
					return PetState.ReadyToSleep;
					break;
				default:
					return PetState.Normal;
					break;
			}
		}

		public bool IsLoadGame(string s)
		{
			return !PlayerPrefs.HasKey(s);
		}
		public void SaveLoadGame(string s, float value)
		{
			PlayerPrefs.SetFloat(s, value);
			PlayerPrefs.Save();
		}
		public float SaveLoadGame(string s)
		{
			return PlayerPrefs.GetFloat(s);
		}
		public void DeleteLoadGame(string s)
		{
			PlayerPrefs.DeleteKey(s);
		}

		#endregion

		public void InitData(MyPetData data, float valueHungry, float valueAsleep, float valueDirty)
		{
			//this.data = data;

			GetData(valueHungry, valueAsleep, valueDirty);
			GetLastScreen();

			CheckStateToShowAnim();

			SetFreeToRepeat(true);
			SetFreeToAttack(true);
		}

		private void GetLastScreen()
		{
			switch (TimeManager.data.lastState)
			{
				case PetState.Normal:
					mainScreen.SetActive(true);
					bathRoom.SetActive(false);
					bedRoom.SetActive(false);

					myPet = myPetInMainScreen;
					break;

				case PetState.Eat:
					mainScreen.SetActive(true);
					bathRoom.SetActive(false);
					bedRoom.SetActive(false);

					myPet = myPetInMainScreen;
					break;

				case PetState.Sleep:
					mainScreen.SetActive(false);
					bathRoom.SetActive(false);
					bedRoom.SetActive(true);

					SetSleep();

					TurnOffLight(true);
					myPet = myPetInBedRoom;

					Debug.Log("value = " + imgValueSleep.fillAmount);
					if (imgValueSleep.fillAmount <= 0.98) myPet.SetGotoSleep();

					break;

				case PetState.ReadyToSleep:
					mainScreen.SetActive(false);
					bathRoom.SetActive(false);
					bedRoom.SetActive(true);

					TurnOffLight(false);

					myPet = myPetInBedRoom;
					break;

				case PetState.InToilet:
					mainScreen.SetActive(false);
					bathRoom.SetActive(true);
					bedRoom.SetActive(false);

					myPet = myPetInBathRoom;
					break;
			}
		}

		private void GetData(float valueHungry, float valueAsleep, float valueDirty)
		{
			ShowValueHungry(valueHungry);
			ShowValueDirty(valueDirty);
			ShowValueAsleep(valueAsleep);
		}

		private void CheckStateToShowAnim()
		{
			if (isWashing) return;
			if (!myPet) return;

			if (TimeManager.IsExpired(PetState.InToilet))
			{
				myPet.SetDirty(true);
			}
			else if (TimeManager.IsExpired(PetState.Eat))
			{
				myPet.SetHungry(true);
			}
			else if (TimeManager.IsExpired(PetState.Sleep))
			{
				myPet.SetAsleep(true);
			}
			else
			{
				myPet.SetIdle();
			}
		}

		public void SetFreeToRepeat(bool isFree)
		{
			if (TimeManager.data.lastState != PetState.Eat && TimeManager.data.lastState != PetState.Normal)
			{
				isFreeToRepeat = false;
				micInput.OnNotRecord();
				return;
			}
			isFreeToRepeat = isFree;
			if (!isFree) micInput.OnNotRecord();
		}

		public void SetFreeToAttack(bool isFree)
		{
			if (TimeManager.data.lastState != PetState.Eat && TimeManager.data.lastState != PetState.Normal)
			{
				isFreeToAttack = false;
				return;
			}
			isFreeToAttack = isFree;
		}

		private void OnStartWash()
		{
			bubblesFx.loop = true;
			bubblesFx.Play();
			SetFreeToAttack(false);
			SetFreeToRepeat(false);
			isRecording = false;

			isWashing = true;
			myPet.SetAsleep(false);
			myPet.SetHungry(false);
			myPet.SetDirty(false);
			myPet.SetRelax();
		}

		private void OnWashDone()
		{
			isWashing = false;
			SetFreeToRepeat(false);
			GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundWaterShower, false, () =>
			{
				SetFreeToAttack(true);
				SetFreeToRepeat(true);
			});
			waterShower.Play();


			isPauseUpdate = true;
			TimeManager.data.lastTimeToilet = DateTime.Now.ToString();

			TimeManager.UpdateTime(PetState.InToilet);

			imgValueDirty.DOFillAmount(1, 1).onComplete = () =>
			{
				isPauseUpdate = false;
				bubblesFx.Stop();

				myPet.SetDirty(false);
				CheckStateToShowAnim();
			};

			TimeManager.SaveData();
		}

		#region Sleep
		private void TurnOffLight(bool isOff)
		{
			if (isOff)
			{
				lamp.gameObject.SetActive(true);
				light.intensity = 0;
				bed.color = colorLightOff;
			}
			else
			{
				lamp.gameObject.SetActive(false);
				light.intensity = 1;
				bed.color = Color.white;
			}
		}

		private void SetSleep()
		{
			TimeManager.SetIsSleeping(true);
			SetFreeToRepeat(false);
			SetFreeToAttack(false);
		}

		private void StopSleep()
		{
			if (!TimeManager.IsSleeping) return;

			AudioController.Instance.StopAudio(soundSleep);
			SetFreeToRepeat(true);
			SetFreeToAttack(true);
			TimeManager.SetIsSleeping(false);
			TimeManager.data.lastValueSleep = imgValueSleep.fillAmount < 1 ? imgValueSleep.fillAmount : 1;
			TimeManager.UpdateTime(PetState.Sleep);

			TimeManager.SaveData();
		}
		#endregion

		#region OnClick Handler
		private void OnClickEat()
		{
			if (TimeManager.data.lastState == PetState.Eat)
			{
				if (panelBtnPlay.IsVisible)
                {
					panelBtnPlay.Hide();
				}
                else
				{
					panelBtnPlay.Show();
				}
			}
			else
			{
				ChangeScreen(PetState.Eat, () =>
				{
					SetEat();
				});
			}
		}

		private void SetEat()
		{
			if (imgValueHungry.fillAmount > 0.5f)
			{
				myPet.SetRefuse();
			}
			else
			{
                if (numGoldToEat <= countGold)
                {
                    SetFreeToRepeat(false);
                    isPauseUpdate = true;
                    myPet.SetEat();

                    TweenControl.GetInstance().DelayCall(this.transform, 2f, () =>
                    {
                        PlayAnimFood();
                    });

					SaveGoldPlayer(countGold - numGoldToEat);

					TweenControl.GetInstance().DelayCall(transform, 0.5f, () =>
                    {
                        TimeManager.UpdateTime(PetState.Eat);

                        btnEat.image.color = colorWhite;
                        imgValueHungry.DOFillAmount(1, timeAnimEat).onComplete = () =>
                        {
                            myPet.SetHungry(false);
                            isPauseUpdate = false;
                            SetFreeToRepeat(true);

                            CheckStateToShowAnim();
                        };

                        TimeManager.SaveData();
                    }, () =>
                    {
                        //MainView.Instance.ShowPopupSystem(LabelLanguage.OtherError[LanguageController.Instance.GetIdLanguage()]);
                    });
                }
                else
                {
                    //MainView.Instance.ShowPopupSystem(LabelLanguage.NotEnoughGold[LanguageController.Instance.GetIdLanguage()]);
                }
            }
		}

		private void PlayAnimFood()
		{
			var curPos = foodIcon.rectTransform.localPosition;

			foodIcon.transform.localScale = Vector3.one;
			txtMinusGold.text = "-" + numGoldToEat;

			TweenControl.GetInstance().MoveRect(foodIcon.rectTransform, new Vector2(curPos.x, curPos.y + 500), 3, () =>
			{
				foodIcon.rectTransform.localPosition = curPos;
				foodIcon.color = Color.white;
				foodIcon.transform.localScale = Vector3.zero;

				txtMinusGold.color = Color.white;
				txtMinusGold.GetComponent<Outline>().effectColor = Color.black;
				imgGold.color = Color.white;
			});

			TweenControl.GetInstance().FadeAnfa(foodIcon, 0, 2, null, Ease.Linear, 1);
			TweenControl.GetInstance().FadeAnfaText(txtMinusGold, 0, 2, null, Ease.Linear, 1);
			TweenControl.GetInstance().FadeAnfa(imgGold, 0, 2, null, Ease.Linear, 1);
			txtMinusGold.GetComponent<Outline>().DOFade(0, 2).SetDelay(1);
		}

		private void OnClickLight()
		{
			GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundLight, false);
			TurnOffLight(light.intensity == 1);

			if (TimeManager.IsSleeping)
			{
				if (imgValueSleep.fillAmount < 0.4f)
				{
					myPet.SetRefuse();
				}
				else
				{
					myPet.SetRelax();

				}
				TimeManager.data.lastValueSleep = imgValueSleep.fillAmount < 1 ? imgValueSleep.fillAmount : 1;
				StopSleep();

				TimeManager.data.lastStateInt = (int)PetState.ReadyToSleep;
			}
			else
			{
				if (imgValueSleep.fillAmount > 0.6f)
				{
					myPet.SetRefuse();
				}
				else
				{
					myPet.SetGotoSleep();
					TimeManager.data.lastValueSleep = imgValueSleep.fillAmount < 1 ? imgValueSleep.fillAmount : 1;
					TimeManager.data.timeStartSleep = DateTime.Now.ToString();

					TimeManager.data.lastStateInt = (int)PetState.Sleep;
					SetSleep();
				}
			}

			TimeManager.SaveData();
		}

		private void OnClickGotoSleep()
		{
			panelBtnPlay.Hide();
			if (TimeManager.data.lastState == PetState.ReadyToSleep)
			{
				return;
			}
			else
			{
				ChangeScreen(PetState.ReadyToSleep, () =>
				{
					SetReadySleep();
				});
			}
		}

		private void SetReadySleep()
		{
			TimeManager.data.lastStateInt = (int)PetState.ReadyToSleep;
			TimeManager.SaveData();
		}

		private void OnClickToilet()
		{
			panelBtnPlay.Hide();
			if (TimeManager.data.lastState == PetState.InToilet)
			{
				//SetBreakteeth();
				return;
			}
			else
			{
				ChangeScreen(PetState.InToilet, () =>
				{
					if (IsTimeToBreakTeeth())
						SetBreakteeth();
					else
					{
						TimeManager.data.lastStateInt = (int)PetState.InToilet;
						TimeManager.SaveData();
					}
				});
			}
		}

		private void SetBreakteeth()
		{
			SetFreeToRepeat(false);
			myPet.SetBreakTeeth();

			TimeManager.data.lastTimeToilet = DateTime.Now.ToString();

			//Goi update time dirty
			TimeManager.UpdateTime(PetState.InToilet);

			btnGoToilet.image.color = colorWhite;

			isPauseUpdate = true;
			imgValueDirty.DOFillAmount(1, timeAnimToilet).onComplete = () =>
			{
				isPauseUpdate = false;
				SetFreeToRepeat(true);

				myPet.SetDirty(false);
				CheckStateToShowAnim();
			};

			TimeManager.data.lastStateInt = (int)PetState.InToilet;
			TimeManager.SaveData();
		}

		public bool isShowPopup { get; set; }

		private void OnClickPlayGame()
        {
			ShowPanelChooseGame();
		}
		private void OnClickPlayAR()
		{
			DataManager.Instance.LoadSceneArAsync();
		}
		#endregion

		public void ShowPanelChooseGame()
        {
			isShowPopup = true;
			panelChooseGame.Show();
		}
		public void HidePanelChooseGame()
		{
			isShowPopup = false;
			panelChooseGame.Hide();
		}

		public void ShowPanelResultDialog()
		{
			isShowPopup = true;
			panelResultDialog.Show();
		}
		public void HidePanelResultDialog()
		{
			isShowPopup = false;
			panelResultDialog.Hide();
		}

		public void ShowPopupPermissionAR(Action onConfirm)
		{
			isShowPopup = true;
			panelPermissionAR.Show();

			popupPermissionAR.InitPopup(onConfirm);
		}
		public void HidePopupPermissionAR()
		{
			isShowPopup = false;
			panelPermissionAR.Hide();
		}

		public void UpdateValueTime(float timeHungry, float timeAsleep, float timeDirty)
		{
			Debug.Log("Update value  " + timeHungry + "  " + timeAsleep + "   " + timeDirty);
			ShowValueHungry(timeHungry);

			ShowValueDirty(timeDirty);

			ShowValueAsleep(timeAsleep);

			CheckStateToShowAnim();

			if (TimeManager.IsSleeping) myPet.PlaySoundSleep();
		}

		bool IsTimeToBreakTeeth()
		{
			var curHour = DateTime.Now.Hour;
			var curMinute = DateTime.Now.Minute;

			if (lstTimeStart.Any(it => it.hour == curHour && curMinute >= it.minute))
			{
				if (lstTimeEnd.Any(it => it.hour == curHour))
				{
					return lstTimeEnd.Any(it => it.hour == curHour && curMinute <= it.minute);
				}
				return true;
			}
			else if (lstTimeStart.Any(it => it.hour == curHour && curMinute <= it.minute))
				return true;

			return false;
		}


		#region UI Handler
		private void ShowValueHungry(float value)
		{
			if (imgValueHungry == null) return;

			imgValueHungry.fillAmount = value;
			btnEat.image.color = value < 0.3f ? colorRed : colorWhite;
		}

		private void ShowValueDirty(float value)
		{
			imgValueDirty.fillAmount = value;
			btnGoToilet.image.color = value < 0.3f ? colorRed : colorWhite;
		}

		private void ShowValueAsleep(float value)
		{
			Debug.Log("value0 = " + value);
			imgValueSleep.fillAmount = value;
			btnGoToSleep.image.color = value < 0.3f ? colorRed : colorWhite;
		}

		private void ShowAndHideScreen(PetState stateScreen)
		{
			switch (TimeManager.data.lastState)
			{
				case PetState.Normal:
					mainScreen.SetActive(false);
					break;
				case PetState.Eat:
					mainScreen.SetActive(false);
					break;
				case PetState.Sleep:
					bedRoom.SetActive(false);
					break;
				case PetState.ReadyToSleep:
					bedRoom.SetActive(false);
					break;
				case PetState.InToilet:
					bathRoom.SetActive(false);
					break;
			}

			switch (stateScreen)
			{
				case PetState.Eat:
					myPet = myPetInMainScreen;
					mainScreen.SetActive(true);
					SetFreeToAttack(true);
					break;
				case PetState.ReadyToSleep:
					myPet = myPetInBedRoom;
					bedRoom.SetActive(true);
					SetFreeToAttack(false);
					break;
				case PetState.InToilet:
					myPet = myPetInBathRoom;
					bathRoom.SetActive(true);
					SetFreeToAttack(true);
					break;
			}
		}

		private void ChangeScreen(PetState stateScreen, Action onComplete)
		{
			overlay.enabled = true;
			overlay.color = new Color(0, 0, 0, 0);
			TweenControl.GetInstance().FadeAnfa(overlay, 1, 0.25f, () =>
			{
				TurnOffLight(false);
				StopSleep();

				ShowAndHideScreen(stateScreen);

				TweenControl.GetInstance().DelayCall(this.transform, 0.05f, () =>
				{

					TimeManager.data.lastStateInt = (int)stateScreen;

					SetFreeToAttack(true);
					SetFreeToRepeat(true);

					TweenControl.GetInstance().FadeAnfa(overlay, 0, 0.25f, () =>
					{
						TimeManager.SaveData();
						overlay.enabled = false;
						onComplete?.Invoke();
					});
				});
			});
		}
		#endregion

		#region Record control
		private void OnStartRecord()
		{
			if (isFreeToRepeat)
			{
				isRecording = true;
				myPet.SetAnimHearing();
				micInput.OnStartRecord();
			}
		}

		private void OnRecordDone(AudioClip clip, float time)
		{
			if (isFreeToRepeat && isRecording)
			{
				isRecording = false;
				TweenControl.GetInstance().DelayCall(this.transform, 1, () =>
				{
					myPet.SetAnimTalk();
					try
					{
						audioSource.clip = clip;
						audioSource.Play();
						var length = (float)time / audioSource.pitch;

						TweenControl.GetInstance().DelayCall(this.transform, length, () =>
						{
							CheckStateToShowAnim();
							micInput.OnNotRecord();
							micInput.InitMic();
							micInput._isInitialized = true;
						});
					}
					catch (System.Exception e)
					{
						Debug.Log("Fail in play voice record");
					}
				});
			}
		}

		#endregion

		private void OnDestroy()
		{
			TimeManager.OnUpdateTime = null;
		}
	}

	[Serializable]
	public class TimeInVirtualPet
	{
		public int hour;
		public int minute;
	}
}