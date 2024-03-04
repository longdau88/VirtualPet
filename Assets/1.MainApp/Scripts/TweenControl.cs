using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.Events;
using DG.Tweening.Core;

public enum EaseType
{
    Linear,
    OutBack,
    InBack,
    InOutBack,
    InOutQuad,
    InQuad,
    OutQuad,
	Ease,
}

public enum NameAxis
{
    numX,
    numY,
    numZ
}

public enum ShakeType
{
    ShakeTypePosition,
    ShakeTypeRotate,
    ShakeTypeScale
}

public class TweenControl : MonoBehaviour
{
    private static TweenControl _instance;

    private const string NAME = "Tween Control";

    private static Dictionary<Transform, List<Tweener>> tweenerDic;
    private static Dictionary<Transform, List<Tween>> tweenDic;

	private Tween tweenNew;
	private Tween tweenFaq;

    public static TweenControl GetInstance()
    {

        if (_instance == null)
            return Init();
        else
            return _instance;
    }

    public void ClearDic()
    {
        tweenDic.Clear();
        tweenerDic.Clear();
    }

    static TweenControl Init()
    {
        var instance = new GameObject(NAME);
        _instance = instance.AddComponent<TweenControl>();
        tweenerDic = new Dictionary<Transform, List<Tweener>>();
        tweenDic = new Dictionary<Transform, List<Tween>>();
        DontDestroyOnLoad(instance);
        DOTween.Init();
        return _instance;
    }


    public List<Tween> GetTween(Transform trans)
    {
        if (tweenDic.ContainsKey(trans)) return tweenDic[trans];

        return new List<Tween>();
    }
    public List<Tweener> GetTweener(Transform trans)
    {
        if (tweenerDic.ContainsKey(trans)) return tweenerDic[trans];

        return new List<Tweener>();
    }

    void AddTweener(Transform tr, Tweener tweener)
    {
        if (tweenerDic.ContainsKey(tr))
        {
            tweenerDic[tr].Add(tweener);
        }
        else
        {
            var tweeners = new List<Tweener>();
            tweeners.Add(tweener);
            tweenerDic.Add(tr, tweeners);
        }
    }

    public void KillTweener(Transform trans, bool complete = false)
    {
        if (!tweenerDic.ContainsKey(trans))
            return;
        trans.DOKill(complete);
        DOTween.Kill(trans, complete);
        tweenerDic.Remove(trans);
    }

    public void KillDelayCall(Transform trans, bool complete = false)
    {
        if (tweenDic.ContainsKey(trans))
        {
            var listTween = tweenDic[trans];

            for (int i = 0; i < listTween.Count; i++)
            {
                var curTween = listTween[i];

                curTween.Kill(complete);
            }
        }
    }

    public void KillFadeAnfa(Transform trans, bool complete = false)
    {
        if (tweenerDic.ContainsKey(trans))
        {
            var listTween = tweenerDic[trans];

            for (int i = 0; i < listTween.Count; i++)
            {
                listTween[i].Kill(complete);
            }
        }
    }

    public void KillSequence(Transform trans)
    {
        if (!tweenerDic.ContainsKey(trans))
            return;
        trans.DOKill(false);
        tweenDic.Remove(trans);
    }

    void AddTween(Transform tr, Tween tween)
    {
        if (tweenDic.ContainsKey(tr.transform))
        {
            tweenDic[tr.transform].Add(tween);
        }
        else
        {
            var tweens = new List<Tween>();
            tweens.Add(tween);
            tweenDic.Add(tr.transform, tweens);
        }
    }

    void AddSequence(Transform tr, Sequence sequen)
    {
        if (tweenDic.ContainsKey(tr.transform))
        {
            tweenDic[tr.transform].Add(sequen);
        }
        else
        {
            var tweens = new List<Tween>();
            tweens.Add(sequen);
            tweenDic.Add(tr.transform, tweens);
        }
    }

    public void KillAllTweenTrans(Transform trans, bool complete = false)
    {
        KillTween(trans, complete);
        KillTweener(trans, complete);
        KillDelayCall(trans, complete);
    }

    public void KillTween(Transform trans, bool complete = false)
    {
        if (!tweenDic.ContainsKey(trans))
            return;
        trans.DOKill(complete);
        DOTween.Kill(trans, complete);
        tweenDic.Remove(trans);

    }

    public void KillAll(bool onComplete = false)
    {
        DOTween.KillAll(onComplete);
        tweenerDic.Clear();
        tweenDic.Clear();
    }
    public void PauseAll()
    {
        DOTween.PauseAll();
    }
    public void PlayAll()
    {
        DOTween.PlayAll();
    }




    public void PlayTween(Transform trans)
    {
        if (!tweenDic.ContainsKey(trans))
            return;
        var tweens = tweenDic[trans];
        foreach (var tween in tweens)
        {
            tween.Play();
        }
    }

	public void MoveRectXY(RectTransform rect, Vector2 targetPoi, float time, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
	{
		var tweener = rect.DOAnchorPos(targetPoi, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(rect.transform)).SetDelay(delay);
		AddTweener(rect.gameObject.transform, tweener);
	}

	public void MoveRectX2(RectTransform gameObj, float X, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
	{
		var tweener = gameObj.DOAnchorPosX(X, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
		AddTweener(gameObj.transform, tweener);
	}


	public void PauseTween(Transform trans)
    {
        if (!tweenDic.ContainsKey(trans))
            return;
        var tweens = tweenDic[trans];
        foreach (var tween in tweens)
        {
            tween.Pause();
        }
    }
    public void PauseTweener(Transform trans)
    {
        if (!tweenerDic.ContainsKey(trans))
            return;
        var tweens = tweenerDic[trans];
        foreach (var tween in tweens)
        {
            tween.Pause();
        }
    }
    public void PlayTweener(Transform trans, TweenCallback onComplete = null)
    {
        if (!tweenerDic.ContainsKey(trans))
            return;
        var tweens = tweenerDic[trans];
        foreach (var tween in tweens)
        {
            if (onComplete != null)
            {
                tween.Play().OnComplete(onComplete);
            }
            else
            {
                tween.Play();
            }

        }
    }

    public Tweener Shake(GameObject target, float time, Vector3 strength, int vibrato, ShakeType shakeType, float randomness = 180, bool snapping = false, bool fadeOut = true, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        switch (shakeType)
        {
            case ShakeType.ShakeTypePosition:
                var tweener = target.transform.DOShakePosition(time, strength, vibrato, randomness, snapping, fadeOut).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(target.transform)).SetDelay(delay);
                AddTweener(target.gameObject.transform, tweener);
                return tweener;
            
            case ShakeType.ShakeTypeRotate:
                var tweenerRotate = target.transform.DOShakeRotation(time, strength, vibrato, randomness, fadeOut).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(target.transform)).SetDelay(delay);
                AddTweener(target.gameObject.transform, tweenerRotate);
                return tweenerRotate;
            case ShakeType.ShakeTypeScale:
                var tweenerScale = target.transform.DOShakeScale(time, strength, vibrato, randomness, fadeOut).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(target.transform)).SetDelay(delay);
                AddTweener(target.gameObject.transform, tweenerScale);
                return tweenerScale;
        }
        return null;
    }

    public Tween Flip(GameObject objectTarget, NameAxis temp, float numTurm, float time, bool loop = true, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        float valueX = 0, valueY = 0, valueZ = 0;
        switch (temp)
        {
            case NameAxis.numX:
                valueX = 360 * numTurm;
                break;
            case NameAxis.numY:
                valueY = 360 * numTurm;
                break;
            case NameAxis.numZ:
                valueZ = 360 * numTurm;
                break;
        }
        Vector3 pos = new Vector3(valueX, valueY, valueZ);
        Tweener tweener;
        if (loop == true)
        {
            tweener = objectTarget.transform.DORotate(pos, time, RotateMode.LocalAxisAdd).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(() =>
             {
                 Flip(objectTarget, temp, numTurm, time, loop, onComplete, easeType, delay);
             }).OnKill(() => OnKillFuntion(objectTarget.transform)).SetDelay(delay);
        }
        else
        {
            tweener = objectTarget.transform.DORotate(pos, time, RotateMode.FastBeyond360).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnKill(() => OnKillFuntion(objectTarget.transform)).SetDelay(delay);
            if (onComplete != null)
                tweener.OnComplete(() => onComplete.Invoke());
        }
        AddTweener(objectTarget.transform, tweener);

        return tweener;
    }

    public Sequence Jump(Transform trans, Vector3 endValue, float jumpPower, int numJump, float time, bool snapping = false, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = trans.DOJump(endValue, jumpPower, numJump, time, snapping).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(trans)).SetDelay(delay);
        AddSequence(trans, tweener);

        return tweener;
    }

    public Sequence JumpRect(RectTransform trans, Vector3 endValue, float jumpPower = 200, int numJump = 1, float time = 1, bool snapping = false, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = trans.DOJumpAnchorPos(endValue, jumpPower, numJump, time, snapping).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(trans)).SetDelay(delay);
        AddSequence(trans, tweener);

        return tweener;
    }

    public void MoveRect(RectTransform rect, Vector3 targetPoi, float time, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = rect.DOAnchorPos(new Vector2(targetPoi.x, targetPoi.y), time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(rect.transform)).SetDelay(delay);
        AddTweener(rect.gameObject.transform, tweener);
    }
    public void MoveRectLoop(RectTransform rect, Vector3 targetPoi, float time, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = rect.DOAnchorPos(new Vector2(targetPoi.x, targetPoi.y), time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(rect.transform)).SetDelay(delay);
        AddTweener(rect.gameObject.transform, tweener);
    }
    public void Move(Transform trans, Vector3 targetPoi, float time, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = trans.DOMove(targetPoi, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(trans)).SetDelay(delay);

        AddTweener(trans, tweener);
    }
    public void MoveLocal(Transform trans, Vector3 targetPoi, float time, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = trans.DOLocalMove(targetPoi, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(trans)).SetDelay(delay);

        AddTweener(trans, tweener);
    }

    public void ScaleFromZero(GameObject gameObj, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.OutBack, float delay = 0)
    {

        gameObj.transform.localScale = Vector3.zero;
        gameObj.SetActive(true);
        var tweener = gameObj.transform.DOScale(Vector3.one, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
        AddTweener(gameObj.transform, tweener);
    }

    public void ScaleFromZero(GameObject gameObj, Vector3 scaleTo, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.OutBack, float delay = 0)
    {
        gameObj.transform.localScale = Vector3.zero;
        gameObj.SetActive(true);
        var tweener = gameObj.transform.DOScale(Vector3.one, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
        AddTweener(gameObj.transform, tweener);
    }

    public void ScaleFromOne(GameObject gameObj, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.InBack, float delay = 0)
    {

        gameObj.transform.localScale = Vector3.one;
        var tweener = gameObj.transform.DOScale(Vector3.zero, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
        AddTweener(gameObj.transform, tweener);
    }

    public void HightLight(GameObject go, float scaleBegin = 1f, float scaleEnd = 1.05f, float timePerScale = 0.25f, bool hasTimeHightLight = false, float timeHightLight = 0, float scaleAfterHighLight = 1.05f, System.Action callback = null)
    {
        go.transform.localScale = scaleBegin * Vector3.one;

        var tweener = go.transform.DOScale(Vector3.one * scaleEnd, timePerScale).SetLoops(-1, LoopType.Yoyo);
        AddTweener(go.transform, tweener);
        if (hasTimeHightLight)
        {
            var tweener2 = go.transform.DOScale(Vector3.one * scaleEnd, timePerScale).SetLoops((int)(timeHightLight / timePerScale), LoopType.Yoyo).OnComplete(
                () =>
                {
                    TweenControl.GetInstance().KillTweener(go.transform);
                    go.transform.localScale = scaleAfterHighLight * Vector3.one;
                    callback?.Invoke();
                });
        }
    }

    public void Scale(GameObject gameObj, Vector3 ScaleTo, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = gameObj.transform.DOScale(ScaleTo, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
        AddTweener(gameObj.transform, tweener);
    }

    public void MoveRectX(RectTransform gameObj, float X, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = gameObj.DOAnchorPosX(X, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
        AddTweener(gameObj.transform, tweener);
    }

    public void MoveRectY(RectTransform gameObj, float Y, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = gameObj.DOAnchorPosY(Y, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
        AddTweener(gameObj.transform, tweener);
    }

    public void MoveRectY2(RectTransform gameObj, float Y, float time = 1.0f, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = gameObj.DOAnchorPosY(Y, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(gameObj.transform)).SetDelay(delay);
        AddTweener(gameObj.transform, tweener);
    }

    public void FadeAnfa(Image image, float endColor, float duration, TweenCallback onComplete = null, Ease easeType = Ease.Linear, float delay = 0)
    {
        var tweener = image.DOFade(endColor, duration).SetEase(easeType).OnComplete(onComplete).OnKill(() => OnKillFuntion(image.transform)).SetDelay(delay);
        AddTweener(image.gameObject.transform, tweener);
    }

    public void FadeAnfaSkeleton(SkeletonGraphic skeleton, float endColor, float duration, TweenCallback onComplete = null, Ease easeType = Ease.Linear, float delay = 0)
    {
        var tweener = skeleton.DOFade(endColor, duration).SetEase(easeType).OnComplete(onComplete).OnKill(() => OnKillFuntion(skeleton.transform)).SetDelay(delay);
        AddTweener(skeleton.gameObject.transform, tweener);
    }

    public void FadeAnfaText(Text txt, float endColor, float duration, TweenCallback onComplete = null, Ease easeType = Ease.Linear, float delay = 0)
    {
        var tweener = txt.DOFade(endColor, duration).SetEase(easeType).OnComplete(onComplete).OnKill(() => OnKillFuntion(txt.transform)).SetDelay(delay);
        AddTweener(txt.gameObject.transform, tweener);
    }

    public void FadeAnfaTexDraw(Text txt, float endColor, float duration, TweenCallback onComplete = null, Ease easeType = Ease.Linear, float delay = 0)
    {
        var tweener = txt.DOFade(endColor, duration).SetEase(easeType).OnComplete(onComplete).OnKill(() => OnKillFuntion(txt.transform)).SetDelay(delay);
        AddTweener(txt.gameObject.transform, tweener);
    }

    public void FadeAnfa(SkeletonGraphic image, float endColor, float duration, TweenCallback onComplete = null, Ease easeType = Ease.Linear, float delay = 0)
    {
        var tweener = image.DOFade(endColor, duration).SetEase(easeType).OnComplete(onComplete).OnKill(() => OnKillFuntion(image.transform)).SetDelay(delay);
        AddTweener(image.gameObject.transform, tweener);
    }

    public void FadeAnfa(MaskableGraphic graphic, float endColor, float duration, TweenCallback onComplete = null, Ease easeType = Ease.Linear, float delay = 0)
    {
        var tweener = graphic.DOFade(endColor, duration).SetEase(easeType).OnComplete(onComplete).OnKill(() => OnKillFuntion(graphic.transform)).SetDelay(delay);
        AddTweener(graphic.gameObject.transform, tweener);
    }
    public void ColorTo(MaskableGraphic obj, Color32 endColor, float duration, TweenCallback onComplete = null, Ease easeType = Ease.Linear, float delay = 0)
    {
        var tweener = obj.DOColor(endColor, duration).SetEase(easeType).OnComplete(onComplete).OnKill(() => OnKillFuntion(obj.transform)).SetDelay(delay);
        AddTweener(obj.gameObject.transform, tweener);
    }

    public void LocalRotateLoop(Transform trans, float cycle, float duration, float dir = 1, Ease easeType = Ease.Linear, float delay = 0)
    {
        var startRotation = transform.localEulerAngles;
        var endValue = startRotation;
        endValue.z += dir * cycle;
        var tweener = trans.DORotate(endValue, duration, RotateMode.LocalAxisAdd).SetEase(easeType).OnComplete(() =>
     {
         LocalRotateLoop(trans, cycle, duration, dir, easeType, delay);
     }).OnKill(() => OnKillFuntion(trans)).SetDelay(delay);
        AddTweener(trans, tweener);
    }

    public void LocalRotateLoopY(Transform trans, float cycle, float duration, float dir = 1, Ease easeType = Ease.Linear, float delay = 0)
    {
        var startRotation = transform.localEulerAngles;
        var endValue = startRotation;
        endValue.y += dir * cycle;
        var tweener = trans.DORotate(endValue, duration, RotateMode.LocalAxisAdd).SetEase(easeType).OnComplete(() =>
        {
            LocalRotateLoopY(trans, cycle, duration, dir, easeType, delay);
        }).OnKill(() => OnKillFuntion(trans)).SetDelay(delay);
        AddTweener(trans, tweener);
    }

    public void LocalRotateLoopZ(Transform trans, float cycle, float duration, float dir = 1, Ease easeType = Ease.Linear, float delay = 0)
    {
        var startRotation = transform.localEulerAngles;
        var endValue = startRotation;
        endValue.z += dir * cycle;
        var tweener = trans.DORotate(endValue, duration, RotateMode.LocalAxisAdd).SetEase(easeType).OnComplete(() =>
        {
            LocalRotateLoopZ(trans, cycle, duration, dir, easeType, delay);
        }).OnKill(() => OnKillFuntion(trans)).SetDelay(delay);
        AddTweener(trans, tweener);
    }

    public void LocalRotate(Transform trans, Vector3 angle, float duration, UnityAction onComplete = null, EaseType easeType = EaseType.Linear)
    {
        var type = (Ease)Enum.Parse(typeof(Ease), easeType.ToString());

        var tweener = trans.DOLocalRotate(angle, duration, RotateMode.FastBeyond360).SetEase(type).OnKill(() => OnKillFuntion(trans));
        if (onComplete != null)
            tweener.OnComplete(() => onComplete.Invoke());
        AddTweener(trans, tweener);
    }

    public void LocalRotateFast(Transform trans, Vector3 angle, float duration, UnityAction onComplete = null, EaseType easeType = EaseType.Linear)
    {
        var type = (Ease)Enum.Parse(typeof(Ease), easeType.ToString());

        var tweener = trans.DORotate(angle, duration, RotateMode.Fast).SetEase(type).OnKill(() => OnKillFuntion(trans));
        if (onComplete != null)
            tweener.OnComplete(() => onComplete.Invoke());
        AddTweener(trans, tweener);
    }

    public void DelayCall(Transform trans, float delay, UnityAction action, TweenCallback onComplete = null, bool ignoreTimeScale = false)
    {
        Tween tween;
        if (onComplete != null)

            tween = DOVirtual.DelayedCall(delay, () => action.Invoke(), ignoreTimeScale).OnComplete(onComplete).OnKill(() => OnKillFuntion(trans));
        else
            tween = DOVirtual.DelayedCall(delay, () => action.Invoke(), ignoreTimeScale).OnKill(() => OnKillFuntion(trans));
        AddTween(trans, tween);
    }

	public void DelayCallNew(Transform trans, float delay, UnityAction action, TweenCallback onComplete = null, bool ignoreTimeScale = false)
	{
		if (onComplete != null)

			tweenNew = DOVirtual.DelayedCall(delay, () => action.Invoke(), ignoreTimeScale).OnComplete(onComplete).OnKill(() => OnKillFuntion(trans));
		else
			tweenNew = DOVirtual.DelayedCall(delay, () => action.Invoke(), ignoreTimeScale).OnKill(() => OnKillFuntion(trans));
		AddTween(trans, tweenNew);
	}

	public void KillDelayCallNew(Transform trans, bool complete = false)
	{
		if (tweenNew != null) tweenNew.Kill(complete);
	}

	public void DelayCallFAQ(Transform trans, float delay, UnityAction action, TweenCallback onComplete = null, bool ignoreTimeScale = false)
	{
		if (onComplete != null)

			tweenFaq = DOVirtual.DelayedCall(delay, () => action.Invoke(), ignoreTimeScale).OnComplete(onComplete).OnKill(() => OnKillFuntion(trans));
		else
			tweenFaq = DOVirtual.DelayedCall(delay, () => action.Invoke(), ignoreTimeScale).OnKill(() => OnKillFuntion(trans));
	}

	public void KilDelayCallFAQ(Transform trans, bool complete = false)
	{
		if (tweenFaq != null) tweenFaq.Kill(complete);
	}

    public void ValueTo(Transform callTrans, DOGetter<float> targetTween, DOSetter<float> setter, float endValue, float duration, UnityAction updateAction = null, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear)
    {
        var type = (Ease)Enum.Parse(typeof(Ease), easeType.ToString());

        var tween = DOTween.To(targetTween, setter, endValue, duration).OnComplete(onComplete).OnUpdate(() =>
            {
                object[] param = new object[] { targetTween };
                updateAction.DynamicInvoke(param);
            }
            ).SetEase(type).OnKill(() => OnKillFuntionTween(callTrans));

        print(targetTween);
        AddTween(callTrans, tween);
    }

    public Tweener ValueTo(Transform callTrans, DOSetter<float> setter, float start, float end, float duration, Action onComplete = null, EaseType easeType = EaseType.Linear)
    {
        var tween = DOTween.To(setter, start, end, duration).OnComplete(() =>
        {
            if (onComplete != null)
                onComplete();

        }).OnKill(() => OnKillFuntionTween(callTrans)).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString()));
        AddTween(callTrans, tween);
        return tween;
    }

    public void PunchPosition(Transform trans, Vector3 target, float duration)
    {
        var currPos = trans.position;
        var tweener = trans.DOMove(target, duration).OnComplete(() =>
      {
          trans.DOMove(currPos, duration).OnComplete(() =>
          {
              PunchPosition(trans, target, duration);
          });
      }).OnKill(() => OnKillFuntionTween(trans));
        AddTweener(transform, tweener);
    }

    public Tween PunchScale(Transform trans, Vector3 punch, float duration, int vibrato = 1, float elasticity = 1, int loop = 0, LoopType loopType = LoopType.Restart, Action onComplete = null)
    {
        var currPos = trans.position;
        var tweener = trans.DOPunchScale(punch, duration, vibrato, elasticity).OnComplete(() =>
        {
            if (onComplete != null)
                onComplete();
        }).SetLoops(loop, loopType).OnKill(() => OnKillFuntionTween(trans));

        AddTweener(transform, tweener);
        return tweener;
    }

    void OnKillFuntion(Transform tr)
    {
        if (!tweenerDic.ContainsKey(tr))
            return;
        var tweenersList = tweenerDic[tr];
        List<int> listIndex = new List<int>();
        for (int i = 0; i < tweenersList.Count - 1; i++)
        {
            if (tweenersList[i] == null)
            {
                listIndex.Add(i);
            }
        }
        for (int i = listIndex.Count - 1; i >= 0; i--)
        {
            tweenersList.Remove(tweenersList[i]);
        }
    }
    void OnKillFuntionTween(Transform tr)
    {
        if (!tweenDic.ContainsKey(tr))
            return;
        var tweensList = tweenDic[tr];
        List<int> listIndex = new List<int>();
        for (int i = 0; i < tweensList.Count - 1; i++)
        {
            if (tweensList[i] == null)
            {
                listIndex.Add(i);
            }
        }
        for (int i = listIndex.Count - 1; i >= 0; i--)
        {
            tweensList.Remove(tweensList[i]);
        }
    }

    public void MoveRect(RectTransform rect, Vector2 targetPoi, float time, TweenCallback onComplete = null, EaseType easeType = EaseType.Linear, float delay = 0)
    {
        var tweener = rect.DOAnchorPos(targetPoi, time).SetEase((Ease)Enum.Parse(typeof(Ease), easeType.ToString())).OnComplete(onComplete).OnKill(() => OnKillFuntion(rect.transform)).SetDelay(delay);
        AddTweener(rect.gameObject.transform, tweener);
    }

    // Di chuyển GameObject theo đường cong có điểm đỉnh.
    public void MoveFollowBenzier(GameObject go, Vector3 mounthPos, Vector3 endPos, float timeMoveUp = 2.5f, float timeMoveDown = 1.5f, System.Action callback = null)
    {
        TweenControl.GetInstance().MoveRectX(go.GetComponent<RectTransform>(), endPos.x, timeMoveUp + timeMoveDown, null, EaseType.Linear);

        TweenControl.GetInstance().MoveRectY(go.GetComponent<RectTransform>(), mounthPos.y, timeMoveUp, () =>
        {
            TweenControl.GetInstance().MoveRectY(go.GetComponent<RectTransform>(), endPos.y, timeMoveDown, () =>
              {
                  callback?.Invoke();
              }, EaseType.InQuad);
        }, EaseType.OutQuad);
    }
    // di chuyển cong theo đường cong ngang
    public void MoveFollowBenzierHorizontal(GameObject go, Vector3 mounthPos, Vector3 endPos, float timeMoveUp = 2.5f, float timeMoveDown = 1.5f, System.Action callback = null)
    {
        TweenControl.GetInstance().MoveRectY(go.GetComponent<RectTransform>(), endPos.y, timeMoveUp + timeMoveDown, null, EaseType.Linear);

        TweenControl.GetInstance().MoveRectX(go.GetComponent<RectTransform>(), mounthPos.x, timeMoveUp, () =>
        {
            TweenControl.GetInstance().MoveRectX(go.GetComponent<RectTransform>(), endPos.x, timeMoveDown, () =>
            {
                callback?.Invoke();
            }, EaseType.InQuad);
        }, EaseType.OutQuad);
    }
}

