using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClockController : MonoBehaviour
{

    public static ClockController Instance;
    private int timeCountDown;
    private int deltaTime;
    [HideInInspector]
    public int currentTimeCountDown;
    private bool stopCountDown = false;
    private bool pauseCountDown = false;
    public System.Action onCountDownDone, onUpDateCountDown;
    public System.Action waringCountDown;

    public AudioSource audioWaring;
    public int timeWaring = 10;
    public bool activeWaring;
    public GameObject dynamic;
    public float dynamicAngle = 360;

    public bool HideAtStart = false;

    private Coroutine currentCoroutine;
    private bool stateBegin;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //add code Tuoc
        if (HideAtStart)
        {
            HideClock();
        }
    }
    public void HideClock()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public void ShowClock()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
    public void ClockPause(bool setPause)
    {
        if (setPause)
        {
            isCountDown = false;
            pauseCountDown = true;
        }
        else
        {
            isCountDown = true;
            pauseCountDown = false;
        }
    }
    public void StopCountDown()
    {
        onCountDownDone = null;
        currentTimeCountDown = 0;
        stopCountDown = true;
        try
        {
            StopCoroutine(StartCountDownIE(currentTimeCountDown));
        }
        catch { }
    }

    public void SetStop(bool isStop)
    {
        stopCountDown = isStop;
    }

    public void StopCountDownNoRsTime()
    {
        onCountDownDone = null;
        stopCountDown = true;
    }

    public void ClockRun(int time, int deltaTime = 1, System.Action endFuntion = null, System.Action upDateFuntion = null)
    {
        ShowClock();
        stateBegin = false;
        ClockInit(time);
        StartMove();
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        stopCountDown = false;
        pauseCountDown = false;
        onCountDownDone = null;
        onCountDownDone = endFuntion;
        onUpDateCountDown = null;
        onUpDateCountDown = upDateFuntion;
        currentTimeCountDown = time;
        currentCoroutine = StartCoroutine(StartCountDownIE(deltaTime));
    }

    public void ClockInit(int timeMax, int deltaTime = 1)
    {
        stateBegin = true;
        stopCountDown = true;
        ResetValue();
        this.timeCountDown = maxTime;
        this.deltaTime = deltaTime;
        this.maxTime = timeMax;
        imageFill.fillAmount = 1;

        TextTime.text = timeMax.ToString();
    }
    IEnumerator StartCountDownIE(int deltaTime)
    {
        while (currentTimeCountDown > 0 && !stopCountDown)
        {
            if (pauseCountDown)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(deltaTime);
                if (!stopCountDown && !pauseCountDown)
                {
                    currentTimeCountDown -= deltaTime;
                    TextTime.text = currentTimeCountDown.ToString();
                    if (currentTimeCountDown < timeWaring && audioWaring != null && activeWaring)
                    {
                        audioWaring.Play();
                    }
                    if (onUpDateCountDown != null)
                    {

                        onUpDateCountDown();
                    }
                }
            }
        }
        if (onCountDownDone != null && !stopCountDown)
        {
            imageFill.fillAmount = 0;
            onCountDownDone();
        }
    }

    public void AddClockTime(int addTime)
    {
        currentTimeCountDown += addTime;
        if (currentTimeCountDown <= 0)
            currentTimeCountDown = 0;
        TextTime.text = currentTimeCountDown.ToString();
    }

    public Image imageFill;
    public Text TextTime;
    private int maxTime;
    private bool isCountDown;
    private float currentFillAmount = 1;
    private float currentAngleZ = 0;

    public bool GetCountDownStage()
    {
        return isCountDown;
    }

    public void ResetValue()
    {
        isCountDown = false;
        currentFillAmount = 1;
        imageFill.fillAmount = currentFillAmount;
        currentAngleZ = 0;
        if (dynamic == null) return;
        dynamic.transform.eulerAngles = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (isCountDown)
        {
            currentFillAmount -= Time.fixedDeltaTime / maxTime;
            currentAngleZ -= Time.fixedDeltaTime * dynamicAngle / maxTime;
            if (currentAngleZ > -1 * dynamicAngle)
            {
                Vector3 vectorAngle = new Vector3(0, 0, currentAngleZ);
                imageFill.fillAmount = currentFillAmount;
                if (dynamic == null) return;
                dynamic.transform.eulerAngles = vectorAngle;
            }
        }
        else if (!stateBegin)
        {

            var currentFillAmount = 1 - (float)(maxTime - currentTimeCountDown) / (float)maxTime;
            var currentAngleZ = -(180 - currentFillAmount * dynamicAngle);
            if (currentAngleZ > -1 * dynamicAngle)
            {
                Vector3 vectorAngle = new Vector3(0, 0, currentAngleZ);
                imageFill.fillAmount = currentFillAmount;
                if (dynamic == null) return;
                dynamic.transform.eulerAngles = vectorAngle;
            }
        }
    }
    private void StartMove()
    {
        isCountDown = true;
        currentFillAmount = 1;
        currentAngleZ = 0;
        imageFill.fillAmount = currentFillAmount;
        imageFill.fillAmount = currentFillAmount;
        if (dynamic == null) return;
        dynamic.transform.eulerAngles = Vector3.zero;
    }

    public void ResetText()
	{
        TextTime.text = "0";
	}
}
