using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimerScript : Singleton<TimerScript>
{

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public IEnumerator CO_ExecuteInSecondIntervals(UnityAction action, float executeTime)
    {

        float currentTime = 0;

        while (true)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= executeTime)
            {
                action.Invoke();

                currentTime = 0;

            }

            yield return new WaitForEndOfFrame();

        }

    }

    public IEnumerator CO_ExecuteInCountdown(UnityAction action, float startTime)
    {

        float currentTime = 0;

        while (currentTime <= startTime)
        {
            currentTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();

        }

        if (currentTime >= startTime)
        {
            action.Invoke();

            yield return null;
        }

    }

}
