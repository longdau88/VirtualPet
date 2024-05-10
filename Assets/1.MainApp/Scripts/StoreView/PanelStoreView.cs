using DG.Tweening;
using Game;
using MainApp.VirtualFriend;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelStoreView : MonoBehaviour
{
    [Header("LstFood")]
    [SerializeField] GameObject objLstFood;
    [SerializeField] RectTransform content;
    [SerializeField] ItemFood itemPrefabs;
    [SerializeField] Button btnClosePanel;
    [SerializeField] Scrollbar scroll;
    [Header("PanelBuy")]
    [SerializeField] GameObject objBuyFood;
    [SerializeField] Button btnCloseBuy;
    [SerializeField] Image img;
    [SerializeField] Button btnMinus;
    [SerializeField] Button btnPlus;
    [SerializeField] Button btnBuy;
    [SerializeField] Text txtCount;
    [Header("PopupNotification")]
    [SerializeField] GameObject objNotification;
    [SerializeField] Button btnCloseNotification;
    [Space]
    [SerializeField] Text txtMinusGold;
    [SerializeField] Image imgGold;
    [SerializeField] AudioClip aucMinusGold;


    List<ItemFood> lstFoodData;

    void Start()
    {
        btnClosePanel.onClick.RemoveAllListeners();
        btnClosePanel.onClick.AddListener(() =>
        {
            VirtualPetManager.Instance.PlayAucClickBtn();

            VirtualPetManager.Instance.HideStoreView();
        });

        btnCloseNotification.onClick.RemoveAllListeners();
        btnCloseNotification.onClick.AddListener(() => HidePopupNotification());

        btnCloseBuy.onClick.RemoveAllListeners();
        btnCloseBuy.onClick.AddListener(() => HidePanelBuy());

        btnMinus.onClick.RemoveAllListeners();
        btnMinus.onClick.AddListener(() => OnClickMinus());

        btnPlus.onClick.RemoveAllListeners();
        btnPlus.onClick.AddListener(() => OnClickPlus());

        btnBuy.onClick.RemoveAllListeners();
        btnBuy.onClick.AddListener(() => OnClickBuy());
    }

    public void Init(List<FoodDataInPage> data)
    {
        objLstFood.SetActive(true);
        objBuyFood.SetActive(false);
        imgGold.transform.localScale = Vector3.zero;

        FunctionCommon.DeleteAllChild(content);
        lstFoodData = new List<ItemFood>();

        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < data[i].lstFood.Count; j++)
            {
                var item = Instantiate(itemPrefabs, content);
                string pos = i.ToString() + "|" + j.ToString();
                item.InitItem(data[i].lstFood[j]);
                item.btn.onClick.RemoveAllListeners();
                item.btn.onClick.AddListener(() => ShowPanelBuy(pos));
                lstFoodData.Add(item);
            }
        }

        TweenControl.GetInstance().DelayCall(transform, 0.3f, () =>
        {
            scroll.value = 1;
        });
    }

    int count;
    int countCoin;
    int buyCoin;
    string posItem;
    public void ShowPanelBuy(string pos)
    {
        string[] parts = pos.Split('|');
        List<int> numbers = parts.Select(int.Parse).ToList();
        objBuyFood.SetActive(true);

        btnClosePanel.gameObject.SetActive(false);
        imgGold.transform.localScale = Vector3.zero;

        count = 1;
        img.sprite = DataManager.Instance.lstFood[numbers[0]].lstFood[numbers[1]].imgFood;
        countCoin = DataManager.Instance.lstFood[numbers[0]].lstFood[numbers[1]].count;
        buyCoin = countCoin * count;
        txtCount.text = count.ToString();

        posItem = pos;
    }

    public void HidePanelBuy()
    {
        VirtualPetManager.Instance.PlayAucClickBtn();
        objBuyFood.SetActive(false);
        btnClosePanel.gameObject.SetActive(true);
        buyCoin = 0;
        countCoin = 0;
        count = 0;
        posItem = string.Empty;
    }

    public void OnClickMinus()
    {
        VirtualPetManager.Instance.PlayAucClickBtn();
        count--;
        if (count <= 1) count = 1;
        buyCoin = countCoin * count;
        txtCount.text = count.ToString();
    }
    public void OnClickPlus()
    {
        VirtualPetManager.Instance.PlayAucClickBtn();
        count++;
        buyCoin = countCoin * count;
        txtCount.text = count.ToString();
    }
    public void OnClickBuy()
    {
        VirtualPetManager.Instance.PlayAucClickBtn();

        if (string.IsNullOrEmpty(posItem)) return;
        
        if (buyCoin > VirtualPetManager.Instance.GetGoldPlayer())
        {
            ShowPopupNotification();
            return;
        }

        for (int i = 0; i < VirtualPetManager.Instance.TimeManager.data.lstFoodSave.Count; i++)
        {
            if (posItem.Equals(VirtualPetManager.Instance.TimeManager.data.lstFoodSave[i].pos))
            {
                VirtualPetManager.Instance.TimeManager.data.lstFoodSave[i].count += count;
                VirtualPetManager.Instance.countGold -= buyCoin;
                VirtualPetManager.Instance.SaveGoldPlayer(VirtualPetManager.Instance.countGold);
                PlayAnimFood();
            }
        }
    }

    public void ShowPopupNotification()
    {
        TweenControl.GetInstance().Scale(objNotification, Vector3.one, 0.2f, () =>
        {
            btnCloseNotification.onClick.AddListener(() => HidePopupNotification());
        });
    }
    
    public void HidePopupNotification()
    {
        VirtualPetManager.Instance.PlayAucClickBtn();
        TweenControl.GetInstance().Scale(objNotification, Vector3.zero, 0.2f, () =>
        {
            btnCloseNotification.onClick.RemoveAllListeners();
        });
    }

    private void PlayAnimFood()
    {
        var curPos = imgGold.rectTransform.localPosition;

        imgGold.transform.localScale = Vector3.one;
        txtMinusGold.text = "-" + buyCoin;

        if (aucMinusGold)
        {
            GameAudio.Instance.PlayClip(SourceType.SOUND_FX, aucMinusGold, false);
        }
        TweenControl.GetInstance().MoveRect(imgGold.rectTransform, new Vector2(curPos.x, curPos.y + 500), 3, () =>
        {
            imgGold.rectTransform.localPosition = curPos;
            imgGold.transform.localScale = Vector3.zero;

            txtMinusGold.color = Color.white;
            txtMinusGold.GetComponent<Outline>().effectColor = Color.black;
            imgGold.color = Color.white;
        });

        TweenControl.GetInstance().FadeAnfaText(txtMinusGold, 0, 2, null, Ease.Linear, 1);
        TweenControl.GetInstance().FadeAnfa(imgGold, 0, 2, null, Ease.Linear, 1);
        txtMinusGold.GetComponent<Outline>().DOFade(0, 2).SetDelay(1);
    }
}
