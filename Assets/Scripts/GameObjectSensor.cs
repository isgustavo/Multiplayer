using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSensor : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMaskSensor;

    public List<Collider> Colliders { get; private set; } = new List<Collider>();

    public event Action<Collider> OnSensorTriggerEnter;
    public event Action<Collider> OnSensorTriggerExit;

    public void SetRadius (float radius)
    {
        transform.localScale = new Vector3(radius, radius, radius);
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (1 << collision.gameObject.layer == layerMaskSensor)
        {
            Colliders.Add(collision.collider);
            OnSensorTriggerEnter?.Invoke(collision.collider);
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (1 << other.gameObject.layer == layerMaskSensor)
        {
            Colliders.Add(other);
            OnSensorTriggerEnter?.Invoke(other);
        }
    }

    void OnTriggerExit (Collider other)
    {
        Colliders.Remove(other);
        OnSensorTriggerExit?.Invoke(other);
    }
}

