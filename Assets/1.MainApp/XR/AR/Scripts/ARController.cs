using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARController : MonoBehaviour
{
    public static ARController Instance;


    [Header("UI")]
    [SerializeField] Button btnBack;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    void Start()
    {
        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnClickButtonBack);

        /*if (SystemConfig.Instance)
            SystemConfig.Instance.SetTargetFrameRate();
        InitStart();*/
    }
    private void OnClickButtonBack()
    {
        TweenControl.GetInstance().KillDelayCallNew(this.transform);
        DataManager.Instance.UnloadSceneAr();
    }
}
