using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    public VariableJoystick variableJoystick;

    public Button createObjButton;

    public List<KeyButton> keyButtons;

    public delegate void KeyAction(PlayerActionName playerActionName);
    public event KeyAction keyOnClickEvent;
    public event KeyAction KeyOnHoldEvent;
    public event KeyAction KeyOnHoldUpEvent;
    public event KeyAction KeyOnHoldDownEvent;

    private void Awake()
    {
        Instance = this;
        variableJoystick.gameObject.SetActive(false);
        createObjButton.gameObject.SetActive(false);
        InitKeyButton(new List<PlayerActionName>());
    }

    private bool _init = false;
    public void Init()
    {
        if (_init) return;
        createObjButton.onClick.RemoveAllListeners();
        createObjButton.onClick.AddListener(() =>
        {
            ARController.Instance.SpawnARObject();
        });
        _init = true;
    }

    public void Show()
    {
        variableJoystick.gameObject.SetActive(true);
        createObjButton.gameObject.SetActive(true);
        InitKeyButton(ARController.Instance.demo ? ARController.Instance.playerActionNameDemo : ARController.Instance.PlayerActionName);
    }

    private void InitKeyButton(List<PlayerActionName> keyButtonType)
    {
        for (int i = 0; i < keyButtons.Count; i++)
        {
            if (keyButtonType.Contains(keyButtons[i].playerActionName))
            {
                keyButtons[i].Init(true);
            }
            else
            {
                keyButtons[i].Init(false);
            }

        }
    }

    public void CallKeyOnClickEvent(PlayerActionName playerActionName)
    {
        keyOnClickEvent?.Invoke(playerActionName);
    }
    public void CallKeyOnHoldEvent(PlayerActionName playerActionName)
    {
        KeyOnHoldEvent?.Invoke(playerActionName);
    }

    public void CallKeyOnHoldUpEvent(PlayerActionName playerActionName)
    {
        KeyOnHoldUpEvent?.Invoke(playerActionName);
    }

    public void CallKeyOnHoldDownEvent(PlayerActionName playerActionName)
    {
        KeyOnHoldDownEvent?.Invoke(playerActionName);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            keyOnClickEvent?.Invoke(PlayerActionName.JUMP);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            KeyOnHoldDownEvent?.Invoke(PlayerActionName.RUN);
        }
        if (Input.GetKey(KeyCode.F))
        {
            KeyOnHoldEvent?.Invoke(PlayerActionName.RUN);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            KeyOnHoldUpEvent?.Invoke(PlayerActionName.RUN);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            keyOnClickEvent?.Invoke(PlayerActionName.SIT);
        }
    }
#endif

}
