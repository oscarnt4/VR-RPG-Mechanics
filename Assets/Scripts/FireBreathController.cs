using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreathController : MonoBehaviour
{
    ParticleSystem fireBreath;
    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();

    void Start()
    {
        fireBreath = GetComponent<ParticleSystem>();
        fireBreath.Stop();
    }

    public IEnumerator FireBreathSequence()
    {
        fireBreath.Play();
        yield return new WaitForSeconds(2f);
        fireBreath.Stop();
        yield return null;
    }

    public void StopFireBreath()
    {
        fireBreath.Stop();
    }

    void OnParticleTrigger(GameObject other)
    {
        Debug.Log("aaa");
        int count = fireBreath.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
        for(int i=0; i < count; i++){
            other.GetComponent<PlayerHealth>().TakeDamage(2);
        }
    }
}
