using Doozy.Engine.UI;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainApp.VirtualFriend
{
    [System.Serializable]
    public class GameInfo
    {
        public int id;
        public AssetBundle game_ios;
        public AssetBundle game_android;
        public string sceneName;
        public Sprite game_avatar;
    }

    public class ChooseGameView : MonoBehaviour
    {
        [SerializeField] Button btnClose;
        [SerializeField] ScrollRect scroll;
        [SerializeField] GameObject arrow;
        [SerializeField] Image imgTitle;
        [SerializeField] List<Button> lstButtonGame;
        [SerializeField] List<Image> lstAvatarGame;
        [SerializeField] List<GameObject> listRowItem;
        [Space]
        [SerializeField] Sprite defaultAvatar;
        [SerializeField] List<Sprite> spriteTitle;

        bool showArrow;
        List<GameInfo> lstGame;

        public void InitPanelChooseGame(List<GameInfo> lstGame)
        {
            this.lstGame = lstGame;

            var totalGame = lstGame.Count;
            var numRow = totalGame / 2;
            if (totalGame % 2 != 0)
            {
                numRow += 1;
            }

            for (int i = 0; i < listRowItem.Count; i++)
            {
                var temp = i;
                if (temp >= numRow) listRowItem[temp].SetActive(false);
                else
                    listRowItem[temp].SetActive(true);
            }

            showArrow = numRow > 2;
            arrow.SetActive(showArrow);

            for (int i = 0; i < lstButtonGame.Count; i++)
            {
                var temp = i;
                if (temp < lstGame.Count)
                {
                    lstButtonGame[temp].transform.localScale = Vector2.one;
                    lstButtonGame[temp].gameObject.SetActive(true);

                    if (lstGame[temp].game_avatar != null)
                    {
                        lstAvatarGame[temp].sprite = lstGame[temp].game_avatar;
                        lstAvatarGame[temp].preserveAspect = true;
                    }
                    else
                    {
                        lstAvatarGame[temp].sprite = defaultAvatar;
                        lstAvatarGame[temp].preserveAspect = true;
                    }
                }
                else
                {
                    lstButtonGame[temp].gameObject.SetActive(false);
                }
            }

            imgTitle.sprite = spriteTitle[LanguageController.Instance.GetIdLanguage()];

            TweenControl.GetInstance().DelayCall(scroll.transform, 1f, () =>
            {
                scroll.verticalNormalizedPosition = 1;
            });
        }
        public void OnClickClose()
        {
            VirtualPetManager.Instance.PlayAucClickBtn();
            VirtualPetManager.Instance.HidePanelChooseGame();
        }

        void Start()
        {
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(OnClickClose);

            for (int i = 0; i < lstButtonGame.Count; i++)
            {
                var temp = i;
                lstButtonGame[temp].onClick.AddListener(() =>
                {
                    OnClickGame(temp);
                });
            }
            scroll.onValueChanged.AddListener(OnScroll);
        }

        private void OnScroll(Vector2 pos)
        {
            if (scroll.verticalNormalizedPosition > 0.9f)
            {
                if (showArrow) arrow.SetActive(true);
            }
            else if (scroll.verticalNormalizedPosition < 0.3f)
            {
                if (showArrow) arrow.SetActive(false);
            }
        }
        private AssetBundle assetScene;
        private void OnClickGame(int index)
        {
            GameAudio.Instance.PlaySoundClickButton();
            VirtualPetManager.Instance.panelBtnPlay.Hide();

            VirtualPetManager.Instance.isPlayGame = true;

            if (DataManager.Instance.isTestGame)
            {
                LoadingView.Instance.ShowLoading(1f, () =>
                {
                    var async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
                    async.completed += (obj) =>
                    {
                        DataManager.Instance.IsShowOtherScene = true;
                        VirtualPetManager.Instance.ShowMainApp(false);
                        gameObject.GetComponent<UIView>().Hide();
                    };
                });
            }
            else
            {
                assetScene = null;

#if UNITY_IOS
		assetScene = lstGame[index].ios;
#endif
#if UNITY_ANDROID
                assetScene = lstGame[index].game_android;
#endif

                LoadingView.Instance.ShowLoading(1f, () =>
                {
                    if (assetScene != null)
                    {
                        DataManager.Instance.LoadScenes(assetScene, () =>
                        {
                            VirtualPetManager.Instance.ShowMainApp(false);
                            gameObject.GetComponent<UIView>().Hide();
                        });
                    }
                    else
                    {
                        DataManager.Instance.LoadScenes(lstGame[index].sceneName, () =>
                        {
                            VirtualPetManager.Instance.ShowMainApp(false);
                            gameObject.GetComponent<UIView>().Hide();
                        });
                    }
                });
            }
        }
    }
}