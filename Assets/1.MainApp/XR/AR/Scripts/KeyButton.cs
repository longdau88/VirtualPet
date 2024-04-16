using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class KeyButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public PlayerActionName playerActionName;
    public PlayerActionType playerActionType;

    private bool _enable;

    public void SetEnable(bool enable)
    {
        _enable = enable;
    }

    public void Init(bool active)
    {
        gameObject.SetActive(active);
        _enable = active;
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (_enable && playerActionType == PlayerActionType.CLICK)
        {
            PlayerControl.Instance.CallKeyOnClickEvent(playerActionName);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (_enable && playerActionType == PlayerActionType.HOLD)
        {
            PlayerControl.Instance.CallKeyOnHoldDownEvent(playerActionName);
        }

    }

    public void OnPointerUp(PointerEventData data)
    {
        PlayerControl.Instance.CallKeyOnHoldUpEvent(playerActionName);
    }
}
public enum PlayerActionName
{
    IDLE,
    WALK,
    RUN,
    DASH,
    JUMP,
    CRAWL,
    CLIMB,
    FLY,
    POSE,
    ATTACK,
    HIT,
    SHOT,
    KICK,
    THROW,
    SKILL,
    BOW,
    CLING,
    SIT
}
public enum PlayerActionType
{
    CLICK,
    HOLD
}