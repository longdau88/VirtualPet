using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TheRunner2
{
    public enum TypeItem
    {
        HEART,
        BUBBLE,
        OBSTACLE,
    }
    public class ItemController : MonoBehaviour
    {
        public TypeItem typeItem;
    }
}