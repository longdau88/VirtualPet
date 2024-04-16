using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FlappyEddie
{
	public class ItemTrigger : MonoBehaviour
	{
		[SerializeField] ItemType itemType;

		public ItemType ItemType
		{
			get { return itemType; }
		}
	}

	public enum ItemType
	{
		Coin,
		Dead
	}
}