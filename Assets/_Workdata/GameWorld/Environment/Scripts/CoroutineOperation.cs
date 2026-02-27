using System;
using System.Collections;
using UnityEngine;

public class CoroutineOperation
{
    private readonly MonoBehaviour caller;
    public Coroutine coroutine;
    private Coroutine timeCoroutine;

    public CoroutineOperation(MonoBehaviour caller, IEnumerator coroutine, float time)
    {
        this.caller = caller;
        this.coroutine = caller.StartCoroutine(coroutine);
        timeCoroutine = caller.StartCoroutine(TimeTrackerCor(time));
    }

    public CoroutineOperation(MonoBehaviour caller, Coroutine coroutine, float time)
    {
        this.caller = caller;
        this.coroutine = coroutine;
        timeCoroutine = caller.StartCoroutine(TimeTrackerCor(time));
    }

    public CoroutineOperation(MonoBehaviour caller, float time)
    {
        this.caller = caller;
        timeCoroutine = caller.StartCoroutine(TimeTrackerCor(time));
    }

    public bool IsFinished { get; private set; }
    public bool IsRunning { get; private set; }


    /// <summary> true = finished normally, false = stop "finish" </summary>
    public event Action<bool> OnFinished;

    private IEnumerator TimeTrackerCor(float time)
    {
        IsFinished = false;
        IsRunning = true;
        yield return new WaitForSecondsRealtime(time);
        OnFinished?.Invoke(true);
        coroutine = null;
        timeCoroutine = null;
        IsFinished = true;
    }

    public void StopCoroutine()
    {
        if (coroutine != null) caller.StopCoroutine(coroutine);

        if (timeCoroutine != null) caller.StopCoroutine(timeCoroutine);

        coroutine = null;
        timeCoroutine = null;
        OnFinished?.Invoke(false);
        IsFinished = true;
    }
}