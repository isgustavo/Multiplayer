using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualParticleSystemRemoveDelay : MonoBehaviour
{
    [SerializeField]
    private float lifetime;

    private float currentLifetime;
    private ParticleSystem visualParticleSystem;

    private void Awake ()
    {
        visualParticleSystem = GetComponent<ParticleSystem>();
    }
    private void OnEnable ()
    {
        visualParticleSystem.Play();
        currentLifetime = 0f;
    }

    private void Update ()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime > lifetime)
            VisualGamePoolManager.Current.Despawn(this.gameObject);
    }
}
