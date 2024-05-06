using DG.Tweening;
using Doozy.Engine.UI;
using Game;
using MainApp;
using MainApp.VirtualFriend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] Button btnClose;

	[SerializeField] Image imgTitle;
	[SerializeField] List<Sprite> lstImgTitle;
	[Space]
	[SerializeField] Text txtScore;
	[Space]
	[SerializeField] GameObject coinPrefabs;
	[SerializeField] RectTransform targetCoin;
	[SerializeField] RectTransform parentCoin;
	[Space]
	[SerializeField] AudioClip auc_Yeah;
	[SerializeField] AudioClip auc_Ui;
	[SerializeField] AudioClip auc_addCoin;


	bool isShowing;



	// Start is called before the first frame update
	void Start()
    {
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(OnClickClose);
		isShowing = false;
	}

	public void ShowResult(int score)
	{
		if (isShowing) return;

		if (score > 0)
		{
			if (auc_Yeah != null)
			{
				GameAudio.Instance.PlayClip(SourceType.SOUND_FX, auc_Yeah, false);
			}
		}
        else
        {
			if (auc_Ui != null)
			{
				GameAudio.Instance.PlayClip(SourceType.SOUND_FX, auc_Ui, false);
			}
		}

		isShowing = true;
		imgTitle.sprite = lstImgTitle[1];

		txtScore.text = score.ToString();

		TweenControl.GetInstance().DelayCall(transform, 1f, () =>
		{
			VirtualPetManager.Instance.HidePanelResultDialog(() =>
			{
				isShowing = false;
				PrepareCoins(score);
			});
		});
	}

	private void PrepareCoins(int coin)
	{
		for (int i = 0; i < coin; i++)
        {
			GameObject item = Instantiate(coinPrefabs, parentCoin);
			item.transform.localPosition = Vector3.zero;
			var temp = i;
			TweenControl.GetInstance().DelayCall(transform, 0.1f * temp, () =>
			{
				item.transform.SetParent(targetCoin);
				item.transform.localScale = Vector3.one;
				if (auc_addCoin != null)
				{
					GameAudio.Instance.PlayClip(SourceType.SOUND_FX, auc_addCoin, false);
				}
				TweenControl.GetInstance().MoveRect(item.GetComponent<RectTransform>(), Vector3.zero, 0.5f, () =>
				{
					Destroy(item);
					VirtualPetManager.Instance.countGold++;
					VirtualPetManager.Instance.SaveGoldPlayer(VirtualPetManager.Instance.countGold);
				});
			});
		}
    }


	private void OnClickClose()
    {
        VirtualPetManager.Instance.HidePanelResultDialog();
    }
}
