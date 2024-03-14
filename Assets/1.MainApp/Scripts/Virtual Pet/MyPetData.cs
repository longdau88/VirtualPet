using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Utils;

namespace MainApp.VirtualFriend
{
	[Serializable]
	public class MyPetData
	{
		public string lastTimeEat;
		public string lastTimeSleep;
		public string lastTimeToilet;
		public string timeStartSleep;

		public float lastValueSleep;

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

	public enum PetState
	{
		Normal,
		Eat,
		Sleep,
		ReadyToSleep,
		InToilet
	}
}