using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerScript : MonoBehaviour
{

    public static TimerScript instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator CO_ExecuteInRealTime(float duration, UnityAction actionBefore = null, UnityAction actionAfter = null)
    {
        if (actionBefore != null)
        {
            actionBefore.Invoke();
        }

        yield return new WaitForSecondsRealtime(duration);

        if (actionAfter != null)
        {
            actionAfter.Invoke();
        }
    }

    public IEnumerator CO_ExecuteDuringTimer(float duration, UnityAction actionDuring, UnityAction actionAfter = null)
    {
        float currentTime = 0;

        do
        {
            currentTime += Time.deltaTime;

            actionDuring.Invoke();

            yield return null;

        } while (currentTime <= duration);

        if (actionAfter != null)
        {
            actionAfter.Invoke();

            yield return null;
        }
    }

    public IEnumerator CO_ExecuteAfterTimer(float duration, UnityAction action)
    {
        float currentTime = 0;

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;

            yield return null;
        }

        if (currentTime >= duration)
        {
            action.Invoke();

            yield return null;
        }
    }



}
