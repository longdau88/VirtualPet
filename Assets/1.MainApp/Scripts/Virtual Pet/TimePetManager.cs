using System.Collections;
using UnityEngine;
using System;
using Game.Utils;
using System.IO;
using System.Collections.Generic;

namespace MainApp.VirtualFriend
{
	public class TimePetManager : MonoBehaviour
	{
		public Action<float, float, float> OnUpdateTime;

		[SerializeField] float deltaTimeToHungry;
		[SerializeField] float deltaTimeToDirty;
		[SerializeField] float deltaTimeToAsleep;
		[SerializeField] float timeSleepToFull;
		[Space]
		[SerializeField] float deltaTimeCallUpdate;

		public MyPetData data { get; set; }

		Coroutine countDownValue;
		bool isPauseUpdate;

		float timeHungry;
		float timeAsleep;
		float timeDirty;

		float deltaTimeCallUpdateToMinutes;

		public float valueHungryRatio { get; set; }
		public float valueAsleepRatio { get; set; }
		public float valueDirtyRatio { get; set; }

		private bool isDisable;
		private float deltaTimeOnDisable;
		private bool isPauseApp;
		private float deltaTimeOnPause;
		public bool IsSleeping { get; private set; }

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		void Start()
		{
			deltaTimeToDirty = deltaTimeToDirty * 60;
			deltaTimeToHungry = deltaTimeToHungry * 60;
			deltaTimeToAsleep = deltaTimeToAsleep * 60;
			timeSleepToFull = timeSleepToFull * 60;

			deltaTimeCallUpdateToMinutes = (float)deltaTimeCallUpdate / 60f;
		}

		public void InitData(bool create)
		{
			if (create)
			{
				data = new MyPetData();

				data.lastTimeEat = DateTime.Now.ToString();
				data.lastTimeToilet = DateTime.Now.ToString();
				data.lastTimeSleep = DateTime.Now.ToString();
				data.lastValueSleep = 1;
				data.gold = 0;
				data.lastStateInt = (int)PetState.Eat;

				data.lstFoodSave = new List<FoodDataSave>(DataManager.Instance.InitFood());
				
				SaveData();
			}
			else
			{
				if (CheckFileDataTxt())
                {
					data = SaveAndLoad<MyPetData>.LoadFileTextNoExpired(StaticConfig.FilePetData);
				}
                else
                {
					data = new MyPetData();

					data.lastTimeEat = DateTime.Now.ToString();
					data.lastTimeToilet = DateTime.Now.ToString();
					data.lastTimeSleep = DateTime.Now.ToString();
					data.lastValueSleep = 1;
					data.gold = 0;
					data.lastStateInt = (int)PetState.Eat;

					data.lstFoodSave = new List<FoodDataSave>(DataManager.Instance.InitFood());
					SaveData();
				}
			}

			GetData();
			countDownValue = StartCoroutine(DecreaseValue());

			VirtualPetManager.Instance.InitData(data, valueHungryRatio, valueAsleepRatio, valueDirtyRatio);
		}

		private void GetData()
		{
			timeHungry = GameUtils.DeltaTimeToMinutes(data.TimeEat);
			timeDirty = GameUtils.DeltaTimeToMinutes(data.TimeToilet);

			//timeAsleep = GameUtils.DeltaTimeToMinutes(data.TimeSleep);

			if (timeHungry >= deltaTimeToHungry)
            {
				valueHungryRatio = 0;
			}
            else
            {
				valueHungryRatio = 1f - (float)timeHungry/(float)deltaTimeToHungry;
			}
			if (timeDirty >= deltaTimeToDirty)
			{
				valueDirtyRatio = 0;
			}
			else
			{
				valueDirtyRatio = 1f - (float)timeDirty / (float)deltaTimeToDirty;
			}

			if (data.lastState == PetState.Sleep)
			{
				SetIsSleeping(true);
				var timeSleep = GameUtils.DeltaTimeToMinutes(data.TimeStartSleep);

				if (timeSleep >= timeSleepToFull)
				{
					timeAsleep = 0;
					valueAsleepRatio = 1;
				}
				else
				{
					valueAsleepRatio = data.lastValueSleep + (float)timeSleep / (float)timeSleepToFull;

					if (valueAsleepRatio >= 1)
					{
						timeAsleep = 0;
						valueAsleepRatio = 1;
					}
					else
					{
						timeAsleep = (1 - valueAsleepRatio) * deltaTimeToAsleep;
					}
				}
			}
			else
			{
				timeAsleep = (1 - data.lastValueSleep) * deltaTimeToAsleep + GameUtils.DeltaTimeToMinutes(data.TimeSleep);

				var value = (float)(deltaTimeToAsleep - timeAsleep) / (float)(deltaTimeToAsleep);

				//Nếu thời gian từ lần ngủ cuối cùng quá thời gian để buồn ngủ
				if (value < 0)
				{
					valueAsleepRatio = 0;
				}
				else
				{
					valueAsleepRatio = data.lastValueSleep - (float)(timeAsleep) / (float)(deltaTimeToAsleep);
					if (valueAsleepRatio < 0)
					{
						valueAsleepRatio = 0;
					}
				}
			}
		}

		public void SaveData()
		{
			SaveAndLoad<MyPetData>.SaveJson(data, StaticConfig.FilePetData, ObjectSaveType.txt, "");
		}

		public void SetIsSleeping(bool isSleeping)
		{
			IsSleeping = isSleeping;
		}

		public void SetTimeHungry(float x)
        {
			var discount = x - valueHungryRatio;
			discount *= deltaTimeToHungry;

			var discountTime = GameUtils.MinutesToDateTime(PetState.Eat, data,discount);

			data.lastTimeEat = discountTime.ToString();
			timeHungry = GameUtils.DeltaTimeToMinutes(data.TimeEat);

			valueHungryRatio = (float)(deltaTimeToHungry - timeHungry) / (float)(deltaTimeToHungry);

			SaveData();
		}

		IEnumerator DecreaseValue()
		{
			while (isPauseUpdate)
			{
				yield return new WaitForSeconds(1f);
			}

			if (timeHungry < deltaTimeToHungry)
			{
				timeHungry += deltaTimeCallUpdateToMinutes;
				valueHungryRatio = (float)(deltaTimeToHungry - timeHungry) / (float)(deltaTimeToHungry);
			}

			if (timeAsleep < deltaTimeToAsleep)
			{
				if (!IsSleeping)
				{
					UpdateTimeSleepOnSleep(deltaTimeCallUpdateToMinutes);
					valueAsleepRatio = (float)(deltaTimeToAsleep - timeAsleep) / (float)(deltaTimeToAsleep);
				}
				else
				{
					UpdateTimeSleepOnSleep(deltaTimeCallUpdateToMinutes);

					if (timeAsleep <= 0)
					{
						timeAsleep = 0;

						//data.lastValueSleep = 1;
						//UpdateTime(PetState.Sleep);

						SaveData();
					}
					valueAsleepRatio = valueAsleepRatio + ((float)deltaTimeCallUpdateToMinutes / (float)timeSleepToFull);
				}
			}

			if (timeDirty < deltaTimeToDirty)
			{
				timeDirty += deltaTimeCallUpdateToMinutes;
				valueDirtyRatio = (float)(deltaTimeToDirty - timeDirty) / (float)(deltaTimeToDirty);
			}

			//CheckStateMyPet();

			OnUpdateTime?.Invoke(valueHungryRatio, valueAsleepRatio, valueDirtyRatio);

			yield return new WaitForSeconds(deltaTimeCallUpdate);

			StopCoroutine(countDownValue);
			countDownValue = StartCoroutine(DecreaseValue());
		}

		public bool IsExpired(PetState state)
		{
			var value = 0f;
			switch (state)
			{
				case PetState.Eat:
					value = valueHungryRatio;
					break;
				case PetState.Sleep:
					value = valueAsleepRatio;
					break;
				case PetState.InToilet:
					value = valueDirtyRatio;
					break;
			}
			return !(value > 0.4f);
		}

		public void UpdateTime(PetState state)
		{
			switch (state)
			{
				case PetState.Eat:
					data.lastTimeEat = DateTime.Now.ToString();
					timeHungry = GameUtils.DeltaTimeToMinutes(data.TimeEat);

					valueHungryRatio = (float)(deltaTimeToHungry - timeHungry) / (float)(deltaTimeToHungry);
					break;
				case PetState.Sleep:
					data.lastTimeSleep = DateTime.Now.ToString();
					timeAsleep = (1 - data.lastValueSleep) * deltaTimeToAsleep + GameUtils.DeltaTimeToMinutes(data.TimeSleep);

					valueAsleepRatio = /*data.lastValueSleep -*/ (float)(deltaTimeToAsleep - timeAsleep) / (float)(deltaTimeToAsleep);
					if (valueAsleepRatio < 0) valueAsleepRatio = 0;
					if (valueAsleepRatio > 1) valueAsleepRatio = 1;
					break;
				case PetState.InToilet:
					data.lastTimeToilet = DateTime.Now.ToString();
					timeDirty = GameUtils.DeltaTimeToMinutes(data.TimeToilet);

					valueDirtyRatio = (float)(deltaTimeToDirty - timeDirty) / (float)(deltaTimeToDirty);
					break;
			}
		}


		#region On Pause

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				isPauseApp = true;
				deltaTimeOnPause = Time.realtimeSinceStartup;
				StopCoroutine(countDownValue);
			}
			else
			{
				if (isDisable) return;
				if (!isPauseApp) return;

				isPauseApp = false;

				deltaTimeOnPause = Time.realtimeSinceStartup - deltaTimeOnPause;
				var timeToMinutes = (float)deltaTimeOnPause / 60f;

				timeHungry += timeToMinutes;
				timeDirty += timeToMinutes;

				UpdateTimeSleepOnSleep(timeToMinutes);

				countDownValue = StartCoroutine(DecreaseValue());
			}
		}

		private void OnApplicationQuit()
		{
			if (!IsSleeping) return;
			data.lastValueSleep = valueAsleepRatio < 1 ? valueAsleepRatio : 1;
			data.lastTimeSleep = DateTime.Now.ToString();

			SaveData();
		}
		#endregion

		private void UpdateTimeSleepOnSleep(float time)
		{
			if (IsSleeping)
			{
				timeAsleep = timeAsleep - (time * ((float)deltaTimeToAsleep / (float)timeSleepToFull));
			}
			else
			{
				timeAsleep += time;
			}
		}

		public void CreateFolderData()
		{
			var pathData = Path.Combine(Application.temporaryCachePath, "data");
			if (!Directory.Exists(pathData))
			{
				DirectoryInfo directory = new DirectoryInfo(pathData);
				directory = Directory.CreateDirectory(pathData);

				Debug.Log("Create Folder Data");
			}
		}

		private bool CheckFileDataTxt()
        {
			var path = Path.Combine(PathDataNoExpired, StaticConfig.FilePetData + ".txt");
			Debug.Log(path);
			return File.Exists(path);
		}

		public static string PathDataNoExpired
		{
			get { return Path.Combine(Application.temporaryCachePath, "data"); }
		}
	}
}