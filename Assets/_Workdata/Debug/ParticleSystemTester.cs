using MyBox;
using UnityEngine;

public class ParticleSystemTester : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;

    [ButtonMethod]
    public void Play()
    {
        particleSystem.Play();
    }

    [ButtonMethod]
    public void Stop()
    {
        particleSystem.Stop();
    }
}