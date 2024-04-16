using MainApp;
using MainApp.VirtualFriend;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TheRunner2
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; set; }
		[SerializeField] string sceneName;
		[SerializeField] int timePlay = 180;
		[SerializeField] int maxQues = 7;
		[SerializeField] Button btnBack;
		[SerializeField] Image overlay;
		[SerializeField] AudioClip bgMusic;
		[SerializeField] GameObject objClock;
		[Space]
		[SerializeField] Canvas canvasMain;
		[Space]
		[SerializeField] GameObject panelTitle;
		[Space]
		[SerializeField] GameObject panelGamePlay;
		[Space]
		public Player player;
		public float speedMove;
		[Space]
		[SerializeField] RectTransform BGContainer;
		[SerializeField] BackgroundController BGFirstPrefabs;
		[SerializeField] List<BackgroundController> lstBGLastPrefabs;
		[Space]
		[SerializeField] GameObject objHeart;
		[SerializeField] List<Image> lstHeart;
		[SerializeField] Sprite imgRed;
		[SerializeField] Sprite imgGray;
		[Space]
		[SerializeField] GameObject objCoin;
		[SerializeField] Text txtCoin;
		[Space]
		[SerializeField] GameObject effectCollectItem;
		[Space]
		[SerializeField] GameObject panelEndGame;

		private Vector2 _sizeBG = new Vector2(3840, 1080);
		private AudioClip voice_ChoosePlayer;
		int coin;

		private void Awake()
		{
			Instance = this;
		}
		public void ShowOverlay(bool isShow)
		{
			overlay.raycastTarget = isShow;
		}
		void Start()
		{
			AddListener();
			InitGame();
		}
		private void AddListener()
		{
			btnBack.onClick.RemoveAllListeners();
			btnBack.onClick.AddListener(OnClickBack);

			if (bgMusic != null)
				AudioController.Instance.PlaySoundBgGame(bgMusic);
		}

		public void SetHeart(bool isShow = false)
        {
			if (!isShow)
				objHeart.transform.localScale = Vector3.zero;
			else
			objHeart.transform.localScale = Vector3.one;

			for (int i = 0; i < lstHeart.Count; i++)
            {
				if (i < m_heart)
					lstHeart[i].sprite = imgRed;
				else lstHeart[i].sprite = imgGray;
			}
        }

		private void InitGame()
		{
			ShowOverlay(true);

			if (canvasMain != null)
			{
				_sizeBG = new Vector2(3840, canvasMain.GetComponent<RectTransform>().sizeDelta.y);
			}
			else
			{
				_sizeBG = new Vector2(3840, 1080);
			}

			panelTitle.transform.localScale = Vector3.one;
			panelGamePlay.SetActive(false);
			objHeart.transform.localScale = Vector3.zero;
			objCoin.transform.localScale = Vector3.zero;
			panelEndGame.transform.localScale = Vector3.zero;

			EndGame = true;

			VirtualPetManager.Instance.SetTargetFrameRate();
			TweenControl.GetInstance().DelayCall(transform, 3f, () =>
			{
				TweenControl.GetInstance().Scale(panelTitle, Vector3.zero, 0.1f, () =>
				{
					panelGamePlay.SetActive(true);
					GetQuestion();
				});
			});
		}
		private List<BackgroundController> _backgrounds = new List<BackgroundController>();

		public bool EndGame { get; set; }
		public int m_heart { get; set; }

		private void InitBG()
        {
			_backgrounds = new List<BackgroundController>(BGContainer.GetComponentsInChildren<BackgroundController>(true).Where(obj => obj.transform != BGContainer.transform));

			for (int i = 0; i < _backgrounds.Count; i++) 
            {
				_backgrounds[i].Init(this, new Vector2(3840 * i, 0), _sizeBG, false);
			}
		}
		public void MoveBG()
		{
			foreach (var x in _backgrounds)
            {
				x.Move();
            }
			player.Run();
		}
		public void StopBG(bool isPlay)
		{
			if (isPlay)
			{
				player.Idle();
				objClock.SetActive(false);
				ClockController.Instance.ClockPause(true);
			}
			else player.Run();
			foreach (var x in _backgrounds)
			{
				x.SetPause(isPlay);
			}
		}
		public void DestroyBackground(BackgroundController background)
		{
			_backgrounds.Remove(background);
			Destroy(background.gameObject);

			var bg = Instantiate(lstBGLastPrefabs[Random.Range(0, lstBGLastPrefabs.Count)], BGContainer);
			bool hasHeart = false;
			if (Random.Range(100, 1000) % 2 == 0)
			{
				hasHeart = true;
			}
			bg.Init(this, new Vector2(3840, 0), _sizeBG, hasHeart);
			bg.Move();
			_backgrounds.Add(bg);

		}
		public void GetQuestion()
		{
			_backgrounds = new List<BackgroundController>();

			objHeart.transform.localScale = Vector3.zero;

			player.Init(this);

			m_heart = 3;
			coin = 0;
			txtCoin.text = coin.ToString();

			objCoin.transform.localScale = Vector3.one;

			InitBG();

			SetHeart(true);

			MoveBG();

			EndGame = false;

			objClock.SetActive(true);

			ClockController.Instance.ClockRun(timePlay, 1, OnEndGame);
		}

		public void CheckItemBubble(RectTransform pos)
        {
			GameObject fx = Instantiate(effectCollectItem, pos);
			fx.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			fx.transform.SetParent(txtCoin.gameObject.GetComponent<RectTransform>());
			TweenControl.GetInstance().MoveRect(fx.GetComponent<RectTransform>(), Vector2.zero, 0.3f, () =>
			{
				Destroy(fx);
				coin++;
				txtCoin.text = coin.ToString();
			});
		}

		private void OnClickBack()
		{
			DataManager.Instance.UnloadSceneGame(sceneName);
		}
		public void OnEndGame()
        {
			objClock.SetActive(false);
			objHeart.SetActive(false);
			objCoin.SetActive(false);
			EndGame = true;

			VirtualPetManager.Instance.ShowPanelResultDialog(coin, sceneName);
		}
	}
}