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
		[Space]
		public TimePetManager TimeManager;
		[Header("Screen")]
		[SerializeField] GameObject UI;
		[SerializeField] GameObject ScreenMainApp;
		[SerializeField] GameObject mainScreen;
		[SerializeField] GameObject kitchenScreen;
		[SerializeField] GameObject bathRoom;
		[SerializeField] GameObject bedRoom;
		[Space]
		[SerializeField] Camera mainCamera;
		[Header("Washing")]
		[SerializeField] ParticleSystem waterShower;
		[SerializeField] ParticleSystem bubblesFx;
		[SerializeField] SoapController soap;
		[Header("Record")]
		[SerializeField] MicInput micInput;
		[SerializeField] AudioSource audioSource;
		[Space]
		[SerializeField] MyPetController myPetInMainScreen;
		[SerializeField] MyPetController myPetInkitchenScreen;
		[SerializeField] MyPetController myPetInBathRoom;
		[SerializeField] MyPetController myPetInBedRoom;
		[SerializeField] SpriteRenderer bed;
		[Header("UI")]
		[SerializeField] GameObject objBtn;
		[SerializeField] Button btnEat;
		[SerializeField] Button btnGoToilet;
		[SerializeField] Button btnGoToSleep;
		[SerializeField] Button btnLight;
		[Space]
		[SerializeField] GameObject objFood;
		[SerializeField] Button btnLeft;
		[SerializeField] Button btnRight;
		[SerializeField] List<ItemFoodEat> lstFood;
		[SerializeField] Image imgFoodPrefabs;
		[SerializeField] RectTransform posEndFood;
		[Space]
		[SerializeField] Button btnStore;
		[SerializeField] GameObject panelStoreView;
		[SerializeField] PanelStoreView StoreView;
		[Space]
		public UIView panelBtnPlay;
		[SerializeField] Button btnPlayGame;
		[SerializeField] Button btnPlayAR;
		[Space]
		[SerializeField] UIView panelChooseGame;
		[SerializeField] ChooseGameView chooseGameView;
		[Space]
		[SerializeField] GameController resultDialog;
		[Space]
		[SerializeField] PopupPermissionAR popupPermissionAR;
		[Space]
		[SerializeField] Text txtGoldPlayer;
		[SerializeField] Button btnAddMoreGold;
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
		[SerializeField] AudioClip aucClick;
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
		[Space]
		[SerializeField] GameObject ARPrefab;

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

		public bool isPlayGame { get; set; }

		bool isRecording;

		public MyPetController myPet { get; set; }

		bool isWashing;

		public int countGold;

		Canvas ThisCanvas;
		AudioListener audioListener;

		bool isInit;


		public void ShowOverLay(bool kt)
        {
			overlay.enabled = kt;
		}
		public void PlayAucClickBtn()
        {
			if (aucClick)
			{
				GameAudio.Instance.PlayClip(SourceType.SOUND_FX, aucClick, false);
			}
		}
		public void ShowMainCanvas(bool isDisable)
		{
			ThisCanvas.enabled = isDisable;
			mainCamera.enabled = isDisable;
			audioListener.enabled = isDisable;
		}

		private void Awake()
		{
			Instance = this;

			ThisCanvas = GetComponent<Canvas>();
			audioListener = mainCamera.GetComponent<AudioListener>();
			DontDestroyOnLoad(gameObject);
			SetSystemConfig();
		}

		void Start()
		{
			isInit = false;
			isPlayGame = false;

			ShowMainApp(true);

			objFood.SetActive(false);

			panelBtnPlay.Hide();
			panelChooseGame.Hide();
			resultDialog.gameObject.transform.localScale = Vector3.zero;
			popupPermissionAR.gameObject.transform.localScale = Vector3.zero;
			panelStoreView.gameObject.transform.localScale = Vector3.zero;

			countGold = 0;

			overlay.enabled = false;

			waterShower.Stop();
			bubblesFx.Stop();

			AddListener();
			InitData();

			chooseGameView.InitPanelChooseGame(DataManager.Instance.lstGame);
		}

		private Vector2 startPos;
		private bool isSwiping = false;
		private const float minSwipeDistance = 20f;

		private Vector2 fingerDownPosition;
		private Vector2 fingerUpPosition;
		private bool detectSwipeOnlyAfterRelease = true;
		private float minDistanceForSwipe = 20f;

		void Update()
        {
			if (!isInit) return;
			if (isPlayGame) return;
			if (isShowPopup) return;
			if (TimeManager.data.lastState != PetState.Eat && TimeManager.data.lastState != PetState.Kitchen) return;

#if UNITY_EDITOR
			if (Input.GetMouseButtonDown(0)) // Bắt đầu vuốt
			{
				startPos = Input.mousePosition;
				isSwiping = true;
			}
			else if (Input.GetMouseButtonUp(0)) // Kết thúc vuốt
			{
				isSwiping = false;
				Vector2 endPos = Input.mousePosition;
				Vector2 swipeDirection = endPos - startPos;

				if (swipeDirection.magnitude >= minSwipeDistance)
				{
					swipeDirection.Normalize(); // Chuẩn hóa vector hướng

					float angle = Vector2.SignedAngle(Vector2.right, swipeDirection);

					if (angle > -45 && angle <= 45) // Vuốt sang phải
					{
						if (TimeManager.data.lastState == PetState.Kitchen) return;
						ChangeScreen(PetState.Kitchen, () => { });					
					}
					else if (angle > 135 || angle <= -135) // Vuốt sang trái
					{
						if (TimeManager.data.lastState == PetState.Eat) return;
						ChangeScreen(PetState.Eat, () =>
						{
							//SetEat();
						});
					}
				}
			}
#elif UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    fingerDownPosition = touch.position;
                    fingerUpPosition = touch.position;
                }

                if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
                {
                    fingerUpPosition = touch.position;
                    DetectSwipe();
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    fingerUpPosition = touch.position;
                    DetectSwipe();
                }
            }
#elif UNITY_IOS
			if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    fingerDownPosition = touch.position;
                    fingerUpPosition = touch.position;
                }

                if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
                {
                    fingerUpPosition = touch.position;
                    DetectSwipe();
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    fingerUpPosition = touch.position;
                    DetectSwipe();
                }
            }
#endif
		}

		private bool SwipeDistanceCheckMet()
		{
			return Vector3.Distance(fingerDownPosition, fingerUpPosition) > minDistanceForSwipe;
		}

		private void DetectSwipe()
		{
			if (SwipeDistanceCheckMet())
			{
				Vector2 direction = fingerUpPosition - fingerDownPosition;

				if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
				{
					if (direction.x > 0)
					{
						if (TimeManager.data.lastState == PetState.Kitchen) return;
						ChangeScreen(PetState.Kitchen, () => { });
					}
					else
					{
						if (TimeManager.data.lastState == PetState.Eat) return;
						ChangeScreen(PetState.Eat, () =>
						{
							//SetEat();
						});
					}
				}

				fingerUpPosition = fingerDownPosition;
			}
		}

		int targetFrameRate;
		private void SetSystemConfig()
		{
			var valueGraphicMemory = (float)SystemInfo.graphicsMemorySize / 1024f;
			var valueSystemMemory = (float)SystemInfo.systemMemorySize / 1024f;

			valueGraphicMemory = Mathf.RoundToInt(valueGraphicMemory);
			valueSystemMemory = Mathf.RoundToInt(valueSystemMemory);

#if UNITY_EDITOR
			Debug.Log("Card: " + valueGraphicMemory + " --- RAM: " + valueSystemMemory);
#endif

#if UNITY_ANDROID
			if (valueSystemMemory >= 6)
			{
				targetFrameRate = 60;
			}
			else if (valueSystemMemory >= 4)
			{
				targetFrameRate = 55;
				//MainView.youtubeControl.videoQuality = YoutubeSettings.YoutubeVideoQuality.STANDARD;
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
				}
				else if (valueSystemMemory > 1)
				{
					targetFrameRate = 50;
				}
				else
				{
					targetFrameRate = 45;
				}
#endif

			SetTargetFrameRate();
		}

		public void SetTargetFrameRate()
		{
			Application.targetFrameRate = targetFrameRate;
		}

		private void AddListener()
		{
			btnPlayGame.onClick.RemoveAllListeners();
			btnPlayGame.onClick.AddListener(OnClickPlayGame);

			btnAddMoreGold.onClick.RemoveAllListeners();
			btnAddMoreGold.onClick.AddListener(OnClickPlayGame);

			btnStore.onClick.RemoveAllListeners();
			btnStore.onClick.AddListener(OnClickStore);

			btnLeft.onClick.RemoveAllListeners();
			btnLeft.onClick.AddListener(OnClickLeftFood);

			btnRight.onClick.RemoveAllListeners();
			btnRight.onClick.AddListener(OnClickRightFood);

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
			LoadingView.Instance.ShowLoading(3f, () =>
			{
#if UNITY_EDITOR
                StartGame();
#elif UNITY_ANDROID
								if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
								{
									ShowPopupPermissionAR(() =>
									{
										StartGame();
									});
								}
								else
									StartGame();
#elif UNITY_IOS
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
            });
        }

		private void StartGame()
        {
			if (IsCreateGameApp("11"))
			{
				petState = PetState.Normal;
				SaveLastScreenPrefabs((int)petState);

				SaveCreateGameApp("11");
				SaveGoldPlayer(countGold);

				TimeManager.InitData(true);

				TimeManager.OnUpdateTime = UpdateValueTime;
			}
			else
			{
				TimeManager.InitData(false);

				GetGoldPlayer();

				petState = GetLastScreenPrefabs();

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
			TimeManager.data.gold = value;
			TimeManager.SaveData();
		}
		public int GetGoldPlayer()
		{
			countGold = TimeManager.data.gold;
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

			CheckStateToShowAnim(TimeManager.data.lastState);

			SetFreeToRepeat(true);
			SetFreeToAttack(true);


			isInit = true;
		}

		private void GetLastScreen()
		{
			switch (TimeManager.data.lastState)
			{
				case PetState.Normal:
					mainScreen.SetActive(true);
					bathRoom.SetActive(false);
					bedRoom.SetActive(false);
					kitchenScreen.SetActive(false);

					myPet = myPetInMainScreen;
					break;

				case PetState.Eat:
					mainScreen.SetActive(true);
					bathRoom.SetActive(false);
					bedRoom.SetActive(false);
					kitchenScreen.SetActive(false);

					myPet = myPetInMainScreen;
					break;

				case PetState.Kitchen:
					mainScreen.SetActive(false);
					bathRoom.SetActive(false);
					bedRoom.SetActive(false);
					kitchenScreen.SetActive(true);

					myPet = myPetInkitchenScreen;

					objBtn.SetActive(false);
					InitFood();
					break;

				case PetState.Sleep:
					mainScreen.SetActive(false);
					bathRoom.SetActive(false);
					bedRoom.SetActive(true);
					kitchenScreen.SetActive(false);

					SetSleep();

					TurnOffLight(true);
					myPet = myPetInBedRoom;

					Debug.Log("value = " + imgValueSleep.fillAmount);
					if (imgValueSleep.fillAmount <= 0.98) myPet.SetGotoSleep();

					break;

				case PetState.ReadyToSleep:
					mainScreen.SetActive(false);
					kitchenScreen.SetActive(false);
					bathRoom.SetActive(false);
					bedRoom.SetActive(true);

					TurnOffLight(false);

					myPet = myPetInBedRoom;
					break;

				case PetState.InToilet:
					mainScreen.SetActive(false);
					kitchenScreen.SetActive(false);
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

		private void CheckStateToShowAnim(PetState screen)
		{
			if (isWashing) return;
			if (!myPet) return;

			switch (screen)
            {
                case PetState.Eat:
					if (TimeManager.IsExpired(PetState.Eat))
					{
						myPet.SetHungry(true);
					}
					break;
				case PetState.InToilet:
					if (TimeManager.IsExpired(PetState.InToilet))
					{
						myPet.SetDirty(true);
					}
					break;
				case PetState.ReadyToSleep:
					if (TimeManager.IsExpired(PetState.Sleep))
					{
						myPet.SetAsleep(true);
					}
					break;
				default:
					myPet.SetIdle();
					break;
			};
		}

		public void SetFreeToRepeat(bool isFree)
		{
			if (TimeManager.data.lastState != PetState.Eat)
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
			if (TimeManager.data.lastState != PetState.Eat)
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
				CheckStateToShowAnim(TimeManager.data.lastState);
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
			PlayAucClickBtn();
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
					//SetEat();
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

							CheckStateToShowAnim(TimeManager.data.lastState);
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

        public float GetValueAmountHungry()
        {
			return imgValueHungry.fillAmount;

		}
		public void SetFillImgValueHungry(float value, bool kt = false)
        {
			imgValueHungry.DOFillAmount(value, 1f).onComplete = () =>
			{
				if (!kt)
					myPet.SetRelax();
				CheckStateToShowAnim(TimeManager.data.lastState);
			};
		}

		private void PlayAnimFood()
		{
			/*var curPos = foodIcon.rectTransform.localPosition;

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
			txtMinusGold.GetComponent<Outline>().DOFade(0, 2).SetDelay(1);*/
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
			PlayAucClickBtn();
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
			PlayAucClickBtn();
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
				CheckStateToShowAnim(TimeManager.data.lastState);
			};

			TimeManager.data.lastStateInt = (int)PetState.InToilet;
			TimeManager.SaveData();
		}

		public bool isShowPopup { get; set; }

		private void OnClickPlayGame()
        {
			PlayAucClickBtn();
			ShowPanelChooseGame();
		}
		private void OnClickPlayAR()
		{
			if (panelBtnPlay.IsVisible)
			{
				panelBtnPlay.Hide();
			}

			PlayAucClickBtn();
			DataManager.Instance.LoadSceneArAsync(() =>
			{
				ShowMainApp(false);

				isPlayGame = true;

				ARController.Instance.InitARViewVocab(ARPrefab);
			});
		}

		public void ShowMainApp(bool iShow)
        {
			ScreenMainApp.SetActive(iShow);
			UI.SetActive(iShow);
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

		public void ShowPanelResultDialog(int score, string sceneName)
		{
			isShowPopup = true;

			DataManager.Instance.UnloadSceneGame(sceneName, true, () =>
			{
				TweenControl.GetInstance().Scale(resultDialog.gameObject, Vector3.one, 0.2f, () =>
				{
					resultDialog.ShowResult(score);
				});
			}, true, true);
		}
		public void ShowPanelResultDialog(int score)
		{
			isShowPopup = true;

			TweenControl.GetInstance().Scale(resultDialog.gameObject, Vector3.one, 0.2f, () =>
			{
				resultDialog.ShowResult(score);
			});
		}
		public void HidePanelResultDialog(Action onConfirm = null)
		{
			isShowPopup = false;
			TweenControl.GetInstance().Scale(resultDialog.gameObject, Vector3.zero, 0.2f, () =>
			{
				isPlayGame = false;

				if (imgValueHungry.fillAmount < 0.5f)
                {
					var x = imgValueHungry.fillAmount + 0.5f / (DataManager.Instance.lstGame.Count + 1);
					if (x > 0.5f)
                    {
						x = 0.5f;
                    }

					TimeManager.SetTimeHungry(x);

					SetFillImgValueHungry(x);
                }

				if (onConfirm != null) onConfirm?.Invoke();
			});
		}

		public void ShowPopupPermissionAR(Action onConfirm)
		{
			isShowPopup = true;
			TweenControl.GetInstance().Scale(popupPermissionAR.gameObject, Vector3.one, 0.2f, () =>
			{
				popupPermissionAR.InitPopup(onConfirm);
			});
		}
		public void HidePopupPermissionAR()
		{
			isShowPopup = false;
			TweenControl.GetInstance().Scale(popupPermissionAR.gameObject, Vector3.zero, 0.2f, () =>
			{

			});
		}
		public void UpdateValueTime(float timeHungry, float timeAsleep, float timeDirty)
		{
			Debug.Log("Update value  " + timeHungry + "  " + timeAsleep + "   " + timeDirty);
			ShowValueHungry(timeHungry);

			ShowValueDirty(timeDirty);

			ShowValueAsleep(timeAsleep);

			CheckStateToShowAnim(TimeManager.data.lastState);

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
				case PetState.Kitchen:
					kitchenScreen.SetActive(false);
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
					objBtn.SetActive(true);
					objFood.SetActive(false);
					break;
				case PetState.Kitchen:
					myPet = myPetInkitchenScreen;
					kitchenScreen.SetActive(true);
					SetFreeToAttack(false);
					objBtn.SetActive(false);
					InitFood();
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
		int indexLstFood;
		public string GetCountFood(int i, int j)
        {
			string pos = i.ToString() + "|" + j.ToString();

			int count = 0;
			var foundElement = TimeManager.data.lstFoodSave.FirstOrDefault(food => food.pos.Equals(pos));
			if (foundElement != null)
            {
				count = foundElement.count;
			}
            else
            {
				count = 0;
			}

			return count.ToString();
        }
		public int GetCountFoodInt(string s)
		{
			int count = 0;
			var foundElement = TimeManager.data.lstFoodSave.FirstOrDefault(food => food.pos.Equals(s));
			if (foundElement != null)
			{
				count = foundElement.count;
			}
			else
			{
				count = 0;
			}

			return count;
		}
		private void InitFood()
        {
			objFood.SetActive(true);
			indexLstFood = 0;
			var page = DataManager.Instance.lstFood[indexLstFood];
			var count = page.lstFood.Count;

			btnLeft.gameObject.SetActive(false);
			if (DataManager.Instance.lstFood.Count <= 1)
			{
				btnRight.gameObject.SetActive(false);
			}
            else
            {
				btnRight.gameObject.SetActive(true);
			}

			for (int i = 0; i < lstFood.Count; i++)
            {
				var temp = i;
				if (temp < count)
				{
					string s = indexLstFood.ToString() + "|" + temp.ToString();
					var x = GetCountFoodInt(s);
					lstFood[temp].InitFoodEat(s , page.lstFood[temp], x);
					lstFood[temp].gameObject.SetActive(true);
				}
                else
                {
					lstFood[temp].gameObject.SetActive(false);
				}
			}
		}
		private void LoadFoodInPage()
        {
			objFood.SetActive(true);
			var page = DataManager.Instance.lstFood[indexLstFood];
			var count = page.lstFood.Count;

			if (indexLstFood == 0)
			{
				btnLeft.gameObject.SetActive(false);
			}
			else
			{
				btnLeft.gameObject.SetActive(true);
			}
			if (DataManager.Instance.lstFood.Count - 1 == indexLstFood)
			{
				btnRight.gameObject.SetActive(false);
			}
            else
            {
				btnRight.gameObject.SetActive(true);
			}

			for (int i = 0; i < lstFood.Count; i++)
			{
				var temp = i;
				if (temp < count)
				{
					string s = indexLstFood.ToString() + "|" + temp.ToString();
					var x = GetCountFoodInt(s);
					lstFood[temp].InitFoodEat(s, page.lstFood[temp], x);
					lstFood[temp].gameObject.SetActive(true);
				}
				else
				{
					lstFood[temp].gameObject.SetActive(false);
				}
			}
		}

		public void OnClickLeftFood()
        {
			indexLstFood--;
			if (indexLstFood <= 0)
            {
				btnLeft.gameObject.SetActive(false);
				indexLstFood = 0;
			}
            else
            {
				btnLeft.gameObject.SetActive(true);
			}
			var page = DataManager.Instance.lstFood[indexLstFood];
			var count = page.lstFood.Count;

			btnRight.gameObject.SetActive(true);

			for (int i = 0; i < lstFood.Count; i++)
			{
				var temp = i;
				if (temp < count)
				{
					string s = indexLstFood.ToString() + "|" + temp.ToString();
					var x = GetCountFoodInt(s);
					lstFood[temp].InitFoodEat(s, page.lstFood[temp], x);
					lstFood[temp].gameObject.SetActive(true);
				}
				else
				{
					lstFood[temp].gameObject.SetActive(false);
				}
			}
		}
		public void OnClickRightFood()
		{
			indexLstFood++;
			btnLeft.gameObject.SetActive(true);
			if (indexLstFood >= DataManager.Instance.lstFood.Count - 1)
            {
				indexLstFood = DataManager.Instance.lstFood.Count - 1;
				btnRight.gameObject.SetActive(false);
			}

			var page = DataManager.Instance.lstFood[indexLstFood];
			var count = page.lstFood.Count;

			for (int i = 0; i < lstFood.Count; i++)
			{
				var temp = i;
				if (temp < count)
				{
					string s = indexLstFood.ToString() + "|" + temp.ToString();
					var x = GetCountFoodInt(s);
					lstFood[temp].InitFoodEat(s, page.lstFood[temp], x);
					lstFood[temp].gameObject.SetActive(true);
				}
				else
				{
					lstFood[temp].gameObject.SetActive(false);
				}
			}
		}

		public void OnClickFood(RectTransform posStart, Sprite _img, System.Action onDone = null)
        {
			overlay.enabled = true;
			overlay.color = new Color(0, 0, 0, 0);
			var img = Instantiate(imgFoodPrefabs, posStart);
			img.sprite = _img;
			img.SetNativeSize();
			img.gameObject.transform.localScale = Vector3.one * 2;
			var rect = img.gameObject.GetComponent<RectTransform>();
			rect.anchoredPosition = Vector2.zero;
			img.gameObject.transform.SetParent(posEndFood);

			TweenControl.GetInstance().Scale(img.gameObject, Vector3.zero, 0.3f);
			TweenControl.GetInstance().MoveRect(rect, Vector2.zero, 0.3f, () =>
			{
				Destroy(img.gameObject);
				if (onDone != null) onDone?.Invoke();
			});
		}

		public void OnClickStore()
        {
			overlay.enabled = true;
			overlay.color = new Color(0, 0, 0, 0);
			PlayAucClickBtn();
			TweenControl.GetInstance().Scale(panelStoreView, Vector3.one, 0.2f, () =>
			{
				isShowPopup = true;
				StoreView.Init(DataManager.Instance.lstFood);
			});
        }

		public void HideStoreView()
        {
			TweenControl.GetInstance().Scale(panelStoreView, Vector3.zero, 0.2f, () =>
			{
				isShowPopup = false;
				overlay.enabled = false;

				LoadFoodInPage();
			});
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
							CheckStateToShowAnim(TimeManager.data.lastState);
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