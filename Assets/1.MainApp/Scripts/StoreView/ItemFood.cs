using MainApp.VirtualFriend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFood : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] Text txtGold;
    public Button btn;

    public int gold { get; set; }
    FoodData data;
    public void InitItem(FoodData _data)
    {
        img.sprite = _data.imgFood;
        gold = _data.count;
        txtGold.text = gold.ToString();
    }
}
