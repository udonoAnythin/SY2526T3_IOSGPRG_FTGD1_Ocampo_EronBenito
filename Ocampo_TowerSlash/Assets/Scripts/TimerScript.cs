using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimerScript : Singleton<TimerScript>
{
    public IEnumerator CO_ExecuteInSecondIntervals(UnityAction action, float executeTime)
    {
        
        float currentTime = 0;

        while (true)
        {
            yield return null;

            currentTime += Time.deltaTime;

            if (currentTime >= executeTime)
            {
                action.Invoke();

                currentTime = 0;

            }

        }

    }

    public IEnumerator CO_ExecuteInRandomIntervals(UnityAction action, float minTime, float maxTime)
    {

        float currentTime = 0;
        float timer = Random.Range(minTime, maxTime);

        while (true)
        {
            yield return null;

            currentTime += Time.deltaTime;

            if (currentTime >= timer)
            {
                action.Invoke();

                currentTime = 0;
                timer = Random.Range(minTime, maxTime);

            }

        }

    }

    public IEnumerator CO_ExecuteInCountdown(UnityAction action, float startTime)
    {

        float currentTime = 0;

        while (currentTime <= startTime)
        {
            currentTime += Time.deltaTime;

            yield return null;

        }

        if (currentTime >= startTime)
        {
            action.Invoke();

            yield return null;
        }

    }

    public IEnumerator CO_ExecuteDuringTimer(UnityAction action, float duration, UnityAction actionWhenCompleted = null)
    {
        float currentTime = 0;

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;

            action.Invoke();

            yield return null;

        }

        if (actionWhenCompleted != null)
        {
            actionWhenCompleted.Invoke();

            yield return null;
        }
    }

}
