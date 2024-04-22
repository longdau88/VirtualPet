using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MainApp.VirtualFriend;

namespace Game.FlappyEddie
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] string sceneName;
		public int numQues;
		[SerializeField] Text healText;
		[SerializeField] Button btnBack;
		[Space]
		[SerializeField] AudioClip bgMusic;
		[Space]
		[SerializeField] RectTransform canvas;
		[SerializeField] MoveBG background;
		[SerializeField] CharacterController character;
		[SerializeField] RectTransform boxContain;
		[SerializeField] List<BoxController> lstBoxPrefab;
		[Space]
		[SerializeField] Text txtCoin;
		[SerializeField] Text txtSecond;

		[Header("Config")]
		[SerializeField] int maxBoxInit;
		[SerializeField] float minLoudnessToJump = 0.5f;

		int deltaTimeMoveBox = 4;
		public float speedMoveBox = 300;

		List<BoxController> lstBox;
		bool isPauseGame;

		int countQuesRight;
		int countQues;
		int countCoin;

		public void UpdateTextHeal()
		{
			healText.text = "x " + numQues.ToString();
		}

		void Start()
		{
			VirtualPetManager.Instance.SetTargetFrameRate();

			InitGame();
			AddListener();
			InitBox();
		}

		private void InitGame()
		{
			MainApp.AudioController.Instance.PlaySoundBgGame(bgMusic);

			isPauseGame = true;
			background.SetPause(true);
			character.SetPhysics(false);

			TweenControl.GetInstance().DelayCall(this.transform, 0.5f, () =>
			{
				CountDownGame();
			});
		}

		private void AddListener()
		{
			character.OnDead = OnDead;
			character.OnCollect = OnCollect;

			btnBack.onClick.RemoveAllListeners();
			btnBack.onClick.AddListener(OnClickBack);
		}

		private void InitBox()
		{
			lstBox = new List<BoxController>();

			var startPos = new Vector2((canvas.sizeDelta.x / 2) + 500, 0);
			var endPos = new Vector2(-(canvas.sizeDelta.x / 2) - 500, 0);

			for (int i = 0; i < maxBoxInit; i++)
			{
				var boxPref = RandomSingleObject<BoxController>.GetRandom(lstBoxPrefab);
				var box = Instantiate(boxPref, boxContain);

				box.InitBox(startPos, endPos);

				lstBox.Add(box);
			}
			
		}
		private void CountDownGame()
		{
			StartCoroutine(ICountDownGame());
		}

		private IEnumerator ICountDownGame()
		{
			var currentTimeCountdown = 3;

			txtSecond.text = currentTimeCountdown.ToString();

			TweenControl.GetInstance().ScaleFromZero(txtSecond.gameObject, 0.2f);
			txtSecond.enabled = true;

			while (currentTimeCountdown > 0)
			{
				yield return new WaitForSeconds(1);
				currentTimeCountdown -= 1;
				txtSecond.text = currentTimeCountdown.ToString();

				TweenControl.GetInstance().ScaleFromZero(txtSecond.gameObject, 0.2f);
			}

			isPauseGame = false;
			background.SetPause(false);
			character.SetPhysics(true);
			MoveBox();

			UpdateTextHeal();

			txtSecond.enabled = false;
		}

		#region Controller

		private void MoveBox()
		{
			StartCoroutine(IMoveBox());
		}

		private IEnumerator IMoveBox()
		{
			if (isPauseGame)
			{
				yield break;
			}

			var lstBoxRandom = lstBox.Where(it => !it.isMoving).ToList();
			var boxMove = RandomSingleObject<BoxController>.GetRandom(lstBoxRandom);

			boxMove.Move(speedMoveBox);

			yield return new WaitForSeconds(deltaTimeMoveBox);
			StartCoroutine(IMoveBox());
		}

		private void OnDead()
		{
			isPauseGame = true;

			background.SetPause(true);
			TweenControl.GetInstance().PauseAll();

			TweenControl.GetInstance().DelayCall(this.transform, 0.3f, () =>
			{
				character.PlaySoundDead(() =>
				{
					ShowPopup();
				});
			});
			
		}

		private void OnCollect()
		{
			countCoin++;
			txtCoin.text = countCoin.ToString();
		}

		private void RestartGame()
		{
			isPauseGame = false;
			background.SetPause(false);

			character.ResetCharacter();

			MoveBox();
		}

		private void ResetGame()
		{
			for (int i = 0; i < lstBox.Count; i++)
			{
				lstBox[i].ResetBox();
			}
		}


		private void OnNextQues()
		{
			RestartGame();
		}

		private void OnEndGame()
		{
			TweenControl.GetInstance().PauseAll();
			isPauseGame = false;
			character.SetPhysics(false);
			background.SetPause(true);

			VirtualPetManager.Instance.ShowPanelResultDialog(countCoin, sceneName);
		}

		private void OnClickBack()
		{
			TweenControl.GetInstance().PauseAll();
			isPauseGame = false;
			background.SetPause(true);

			DataManager.Instance.UnloadSceneGame(sceneName);
		}
		#endregion


		#region Popup
		private void ShowPopup()
		{
			numQues--;
			if (numQues <= 0)
			{
				numQues = 0;
				UpdateTextHeal();

				OnEndGame();
			}
            else
            {
				UpdateTextHeal();
				TweenControl.GetInstance().DelayCall(this.transform, 1, () =>
				{
					ResetGame();
					OnNextQues();
				});
			}
		}

		#endregion

		private void OnMouseDown()
		{
			if (isPauseGame) return;

			character.SetJump();
		}
	}
}