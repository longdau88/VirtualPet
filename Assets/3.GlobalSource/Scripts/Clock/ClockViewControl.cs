using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClockViewControl : MonoBehaviour
{

    public RectTransform dynamicHand;
    public Image imageFill;


    public int maxTime;
    public bool isCountDown;

    public Vector3 targetRotation;
    public Vector3 defaulRotation;

    private float currentFillAmount = 1;
    private float currentAngleZ=0;

    public void Init(int maxTime)
    {
        this.maxTime = maxTime;
        imageFill.fillAmount = 1;
    }

    private void Start()
    {
        //  StartMove();
    }
    public void ReStartValue()
    {
        isCountDown = false;
        currentFillAmount = 1;
        imageFill.fillAmount = currentFillAmount;
        imageFill.fillAmount = currentFillAmount;
        currentAngleZ = 0;
        dynamicHand.transform.eulerAngles = Vector3.zero;
    }
    public void PauseClock()
    {
        isCountDown = false;
    }
    private void FixedUpdate()
    {
        if (isCountDown)
        {
            currentFillAmount -= Time.fixedDeltaTime/maxTime;
            currentAngleZ -= Time.fixedDeltaTime*180/ maxTime;
            if (currentAngleZ > -180)
            {
                Vector3 vectorAngle = new Vector3(0, 0, currentAngleZ);
                imageFill.fillAmount = currentFillAmount;
                dynamicHand.transform.eulerAngles = vectorAngle;
            }
            
        }
    }
public void StartMove()
    {
        isCountDown = true;
        currentFillAmount = 1;
        currentAngleZ = 0;
        imageFill.fillAmount = currentFillAmount;
        imageFill.fillAmount = currentFillAmount;
        dynamicHand.transform.eulerAngles = Vector3.zero;
    }
}
public class ReturnValue
{
    public float value;
}
