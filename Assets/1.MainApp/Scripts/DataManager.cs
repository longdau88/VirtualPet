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
    public List<GameInfo> lstGame;

    private void Awake()
    {
        Instance = this;
    }

    private AssetBundle assetScene;
    public void LoadScenes(AssetBundle _assetScene, System.Action onDone = null)
    {
        var async = SceneManager.LoadSceneAsync(_assetScene.GetAllScenePaths()[0], LoadSceneMode.Additive);
        assetScene = _assetScene;

        async.completed += (complete) =>
        {
            Debug.Log("Load asset from local");
            if (onDone != null)
                onDone.Invoke();
        };
    }

    public void UnloadSceneAr(Action onComplete = null)
    {
        AudioController.Instance.UnPauseAudioBgApp();

        SceneManager.UnloadSceneAsync(StaticConfig.SCENE_AR).completed += (onDone) =>
        {
            Resources.UnloadUnusedAssets();

            if (onComplete != null)
                onComplete?.Invoke();
        };
    }

    public void LoadSceneArAsync(Action assetDownloaded = null)
    {
        AudioController.Instance.PauseAudioBgApp();
        LoadingView.Instance.ShowLoading(true);
        StartCoroutine(IEGetSceneAr(() =>
        {
            if (assetDownloaded != null)
                assetDownloaded?.Invoke();
        }));
    }
    private IEnumerator IEGetSceneAr(Action assetDownloaded)
    {
        var sceneArLoad = SceneManager.LoadSceneAsync(StaticConfig.SCENE_AR, LoadSceneMode.Additive);

        sceneArLoad.completed += (onDone) =>
        {
            LoadingView.Instance.SetValue(0.95f);

            LoadingView.Instance.HideLoading();

            if (assetDownloaded != null)
                assetDownloaded?.Invoke();
        };

        while (!sceneArLoad.isDone)
        {
            if (sceneArLoad.progress <= 0.9f)
                LoadingView.Instance.SetValue(sceneArLoad.progress);
            yield return null;
        }
    }
}
