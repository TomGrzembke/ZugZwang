using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTimer : MonoBehaviour
{
    [Separator("Timer Settings")]
    [SerializeField] private float durationInSeconds;
    [SerializeField] private bool useUnscaledTime;
    [SerializeField] private bool startOnAwake;
    [SerializeField] private bool loop;
    

    [Separator("Events")]
    public UnityEvent OnTimerCompleted;

    public UnityAction<float> OnTimeLeft;

    private Coroutine timerCoroutine;
    public float timeLeft { get; private set; }
    public bool isRunning { get; private set; }

    private void Start()
    {
        if (startOnAwake)
            StartTimer();
    }

    public void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        isRunning = false;
        timeLeft = 0f;
    }

    private IEnumerator TimerRoutine()
    {
        timeLeft = durationInSeconds;
        isRunning = true;

        while (timeLeft > 0f)
        {
            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            timeLeft -= delta;
            OnTimeLeft?.Invoke(timeLeft);
            yield return null;
        }

        isRunning = false;
        OnTimerCompleted?.Invoke();
        
        if(loop) StartTimer();
    }
}