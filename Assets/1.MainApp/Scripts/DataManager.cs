using MainApp;
using MainApp.VirtualFriend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public bool isTestGame;
    [Space]
    public List<GameInfo> lstGame;
    public List<FoodDataInPage> lstFood;
    [Space]
    [HideInInspector]
    public bool IsShowOtherScene;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private AssetBundle assetScene;
    public void LoadScenes(AssetBundle _assetScene, System.Action onDone = null)
    {
        var async = SceneManager.LoadSceneAsync(_assetScene.GetAllScenePaths()[0], LoadSceneMode.Additive);
        assetScene = _assetScene;

        async.completed += (complete) =>
        {
            IsShowOtherScene = true;
            VirtualPetManager.Instance.ShowMainCanvas(false);
            if (onDone != null) onDone?.Invoke();
        };
    }
    public void LoadScenes(string _assetScene, System.Action onDone = null)
    {
        var async = SceneManager.LoadSceneAsync(_assetScene, LoadSceneMode.Additive);

        async.completed += (complete) =>
        {
            IsShowOtherScene = true;
            VirtualPetManager.Instance.ShowMainCanvas(false);
            if (onDone != null) onDone?.Invoke();
        };
    }
    public void UnloadSceneAr(Action onComplete = null)
    {
        AudioController.Instance.UnPauseAudioBgApp();

        SceneManager.UnloadSceneAsync(StaticConfig.SCENE_AR).completed += (onDone) =>
        {
            Resources.UnloadUnusedAssets();

            VirtualPetManager.Instance.ShowMainApp(true);

            LoadingView.Instance.ShowLoading(1f, () =>
            {
                if (onComplete != null) onComplete?.Invoke();
            });
        };
        VirtualPetManager.Instance.ShowMainCanvas(true);
    }

    public void LoadSceneArAsync(Action assetDownloaded = null)
    {
        AudioController.Instance.PauseAudioBgApp();
        StartCoroutine(IEGetSceneAr(() =>
        {
            IsShowOtherScene = true;
            VirtualPetManager.Instance.ShowMainCanvas(false);
            LoadingView.Instance.ShowLoading(1f, () =>
            {
                if (assetDownloaded != null)
                    assetDownloaded?.Invoke();
            });
        }));
    }
    private IEnumerator IEGetSceneAr(Action assetDownloaded)
    {
        var sceneArLoad = SceneManager.LoadSceneAsync(StaticConfig.SCENE_AR, LoadSceneMode.Additive);

        sceneArLoad.completed += (onDone) =>
        {
            if (assetDownloaded != null)
                assetDownloaded?.Invoke();
        };

        while (!sceneArLoad.isDone)
        {
            yield return null;
        }
    }

    public void UnloadSceneGame(string name, bool hideMainCanvas = false, System.Action onComplete = null, bool UnloadImmediate = false, bool forceToShowCamera = false)
    {
        if (UnloadImmediate) UnLoadSceneGameImmediate(name, hideMainCanvas, onComplete, forceToShowCamera);
        else
        {
            UnLoadSceneGameImmediate(name, hideMainCanvas, onComplete, forceToShowCamera);
        }
    }
    public void UnLoadSceneGameImmediate(string name, bool hideMainCanvas = false, System.Action onComplete = null, bool forceToShowCamera = false)
    {
        TweenControl.GetInstance().KillAll();
        AudioController.Instance.StopAudioBgGame();
        AudioController.Instance.StopAllAudio();

        Debug.Log("Scene name = " + name);
        SceneManager.UnloadSceneAsync(name).completed += (oper) =>
        {
            IsShowOtherScene = false;

            Resources.UnloadUnusedAssets();

            LoadingView.Instance.ShowLoading(1f, () =>
            {
                VirtualPetManager.Instance.ShowMainApp(true);
                if (onComplete != null) onComplete?.Invoke();
            });
        };

        VirtualPetManager.Instance.ShowMainCanvas(true);
    }

}
