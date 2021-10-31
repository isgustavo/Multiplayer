using System;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMaskCollision;

    public event Action<Collider> OnCollision;

    void OnTriggerEnter (Collider collision)
    {
        if (1 << collision.gameObject.layer == layerMaskCollision)
        {
            OnCollision?.Invoke(collision);
        }
    }
}

