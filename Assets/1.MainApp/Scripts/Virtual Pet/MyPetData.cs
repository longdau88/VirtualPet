using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Utils;
using System.Linq;

namespace MainApp.VirtualFriend
{
	[Serializable]
	public class MyPetData
	{
		public int gold;

		public string lastTimeEat;
		public string lastTimeSleep;
		public string lastTimeToilet;
		public string timeStartSleep;

		public float lastValueSleep;

		public List<FoodDataSave> lstFoodSave;

		public PetState lastState { get { return (PetState)lastStateInt; } }
		public int lastStateInt;

		public DateTime TimeEat
		{
			get { return DateTime.Parse(lastTimeEat); }
		}

		public DateTime TimeSleep
		{
			get { return DateTime.Parse(lastTimeSleep); }
		}

		public DateTime TimeToilet
		{
			get { return DateTime.Parse(lastTimeToilet); }
		}

		public DateTime TimeStartSleep
		{
			get { return DateTime.Parse(timeStartSleep); }
		}
	}
	[System.Serializable]
	public class FoodData
	{
		public int id;
		public Sprite imgFood;
		public int count;
	}
	[System.Serializable]
	public class FoodDataInPage
	{
		public List<FoodData> lstFood;
	}

	[System.Serializable]
	public class FoodDataSave
	{
		public string pos;
		public int count;

		public void Init(string _pos)
        {
			pos = _pos;
			GetPos();
			count = 0;
		}

		public int indexPage { get; set; }
		public int indexInPage { get; set; }

		public void GetPos()
        {
			string[] parts = pos.Split('|');
			List<int> numbers = parts.Select(int.Parse).ToList();
			indexPage = numbers[0];
			indexInPage = numbers[1];
		}
	}

	public enum PetState
	{
		Normal,
		Eat,
		Kitchen,
		Sleep,
		ReadyToSleep,
		InToilet
	}
}