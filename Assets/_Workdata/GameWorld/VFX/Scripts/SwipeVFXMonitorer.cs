using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeVFXMonitorer : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particleSystemsToLoop;
    [SerializeField] private List<ParticleSystem> particleSystemsToOneShot;
    [SerializeField] private float fadeDuration = 1.0f;

    [SerializeField] private float clearSeconds = 3;

    private ObjectPooling objectPooling;

    private bool isFading = false;
    private MoveRow _targetMoveRow;
    private List<Gradient> originalGradients = new();

    private void Start()
    {
        foreach (ParticleSystem ps in particleSystemsToLoop)
        {
            if (ps != null)
            {
                var colorOverLifetime = ps.colorOverLifetime;
                if (colorOverLifetime.enabled)
                {
                    originalGradients.Add(colorOverLifetime.color.gradient);
                }
            }
        }
    }

    // Is called by SwipableVFX to start lifecycle
    public void StartVFXLifecycle(ObjectPooling pooling)
    {
        objectPooling = pooling;
        StartLooping();
        Invoke(nameof(ClearObject), clearSeconds);
    }


    public void SetTargetMoveRow(MoveRow moveRow)
    {
        if (_targetMoveRow != null && _targetMoveRow != moveRow)
        {
            _targetMoveRow.OnMovementFinished -= OnTargetMovementFinished;
        }

        _targetMoveRow = moveRow;

        if (_targetMoveRow != null)
        {
            _targetMoveRow.OnMovementFinished += OnTargetMovementFinished;
        }
    }

    private void OnTargetMovementFinished()
    {
        TriggerOneShotParticles();
        StartFadeOut();


        if (_targetMoveRow != null)
        {
            _targetMoveRow.OnMovementFinished -= OnTargetMovementFinished;
        }
    }

    public void StartVFXMovement(Vector2 swipeDir, float moveTime, AnimationCurve moveCurve)
    {
        float fieldZMultiplier = 1f;
        Vector3 moveAmount = new Vector3(0, 0, swipeDir.x * fieldZMultiplier);

        StartCoroutine(MoveVFXRoutine(moveAmount, moveTime, moveCurve));
    }

    private IEnumerator MoveVFXRoutine(Vector3 moveAmount, float duration, AnimationCurve curve)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + moveAmount;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            float curveValue = curve.Evaluate(t);

            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private void StartLooping()
    {
        foreach (ParticleSystem ps in particleSystemsToLoop)
        {
            if (ps != null)
            {
                var main = ps.main;
                main.loop = true;

                ps.Play();
            }
        }
    }


    private void TriggerOneShotParticles()
    {
        foreach (ParticleSystem ps in particleSystemsToOneShot)
        {
            if (ps != null)
            {
                ps.Play();
            }
        }
    }


    public void StartFadeOut()
    {
        if (isFading) return;
        StartCoroutine(FadeOutAndStopRoutine());
    }

    private IEnumerator FadeOutAndStopRoutine()
    {
        isFading = true;
        float timer = 0f;

        foreach (ParticleSystem ps in particleSystemsToLoop)
        {
            if (ps != null)
            {
                ps.Stop();
            }
        }

        List<ParticleSystem.ColorOverLifetimeModule> colorOverLifetimeModules =
            new List<ParticleSystem.ColorOverLifetimeModule>();

        foreach (ParticleSystem ps in particleSystemsToLoop)
        {
            if (ps != null)
            {
                var colorOverLifetime = ps.colorOverLifetime;
                if (colorOverLifetime.enabled)
                {
                    colorOverLifetimeModules.Add(colorOverLifetime);
                }
            }
        }

        while (timer < fadeDuration)
        {
            float alpha = 1.0f - (timer / fadeDuration);


            for (int i = 0; i < colorOverLifetimeModules.Count; i++)
            {
                ParticleSystem.ColorOverLifetimeModule module = colorOverLifetimeModules[i];

                Gradient newGradient = new Gradient();

                GradientColorKey[] colors = originalGradients[i].colorKeys;
                GradientAlphaKey[] alphas = originalGradients[i].alphaKeys;

                for (int j = 0; j < alphas.Length; j++)
                {
                    alphas[j].alpha = originalGradients[i].alphaKeys[j].alpha * alpha;
                }

                newGradient.SetKeys(colors, alphas);
                module.color = new ParticleSystem.MinMaxGradient(newGradient);
            }

            timer += Time.deltaTime;
            yield return null;
        }


        bool allParticlesDead = false;
        while (!allParticlesDead)
        {
            allParticlesDead = true;
            foreach (ParticleSystem ps in particleSystemsToLoop)
            {
                if (ps != null && ps.IsAlive(true))
                {
                    allParticlesDead = false;
                    break;
                }
            }

            if (allParticlesDead)
            {
                foreach (ParticleSystem ps in particleSystemsToOneShot)
                {
                    if (ps != null && ps.IsAlive(true))
                    {
                        allParticlesDead = false;
                        break;
                    }
                }
            }

            if (!allParticlesDead)
            {
                yield return null;
            }
        }

        
     


        ClearObject();
    }

    private void ClearObject()
    {
        List<ParticleSystem.ColorOverLifetimeModule> colorOverLifetimeModules =
            new List<ParticleSystem.ColorOverLifetimeModule>();

        foreach (ParticleSystem ps in particleSystemsToLoop)
        {
            if (ps != null)
            {
                var colorOverLifetime = ps.colorOverLifetime;
                if (colorOverLifetime.enabled)
                {
                    colorOverLifetimeModules.Add(colorOverLifetime);
                }
            }
        }
        
        for (int i = 0; i < colorOverLifetimeModules.Count; i++)
        {
            ParticleSystem.ColorOverLifetimeModule module = colorOverLifetimeModules[i];

            Gradient newGradient = new Gradient();

            GradientColorKey[] colors = originalGradients[i].colorKeys;
            GradientAlphaKey[] alphas = originalGradients[i].alphaKeys;

            for (int j = 0; j < alphas.Length; j++)
            {
                alphas[j].alpha = 255;
            }

            newGradient.SetKeys(colors, alphas);
            module.color = new ParticleSystem.MinMaxGradient(newGradient);
        }
        
        objectPooling.ReturnObjectToPool(gameObject, ObjectPooling.PoolType.VFX);
        isFading = false;
    }


    void OnDestroy()
    {
        if (_targetMoveRow != null)
        {
            _targetMoveRow.OnMovementFinished -= OnTargetMovementFinished;
        }
    }
}