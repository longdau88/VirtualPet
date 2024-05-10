using MainApp.VirtualFriend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFoodEat : MonoBehaviour
{
    [SerializeField] Image imgFood;
    [SerializeField] Text txtCount;
    [SerializeField] Button btn;

    string pos;
    int count;
    FoodData data;

    public void InitFoodEat(string pos, FoodData data, int count)
    {
        imgFood.sprite = data.imgFood;
        imgFood.SetNativeSize();
        txtCount.text = count.ToString();

        this.pos = pos;
        this.count = count;
        this.data = data;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            OnClickFoodEat();
        });
    }
    public void OnClickFoodEat()
    {
        if (count <= 0) return;

        if (VirtualPetManager.Instance.GetValueAmountHungry() >= 0.97f)
        {
            VirtualPetManager.Instance.myPet.SetRefuse();
            return;
        }

        count--;
        txtCount.text = count.ToString();

        for (int i = 0; i < VirtualPetManager.Instance.TimeManager.data.lstFoodSave.Count; i++)
        {
            if (pos.Equals(VirtualPetManager.Instance.TimeManager.data.lstFoodSave[i].pos))
            {
                VirtualPetManager.Instance.TimeManager.data.lstFoodSave[i].count = count;
                VirtualPetManager.Instance.TimeManager.SaveData();
            }
        }

        float fill = (float)DataManager.Instance.GetGoldFood(pos) / (float)DataManager.Instance.GetMaxGold();
        VirtualPetManager.Instance.OnClickFood(gameObject.GetComponent<RectTransform>(), imgFood.sprite, () =>
        {
            VirtualPetManager.Instance.myPet.SetEat();
            TweenControl.GetInstance().DelayCall(transform, 2f, () =>
            {
                var x = VirtualPetManager.Instance.GetValueAmountHungry() + 0.5f * fill;
                if (x >= 1f)
                {
                    x = 1f;
                }
                VirtualPetManager.Instance.TimeManager.SetTimeHungry(x);
                VirtualPetManager.Instance.SetFillImgValueHungry(x, true);

                VirtualPetManager.Instance.ShowOverLay(false);
            });
        });
    }
}
