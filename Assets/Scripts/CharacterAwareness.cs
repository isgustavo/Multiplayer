using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CharacterAwareness : MonoBehaviour
{
    GameObjectSensor awernessSensor;
    GameObjectSensor closestSensor;

    public event Action<Collider> OnAwernessTriggerEnter;
    public event Action<Collider> OnAwernessTriggerExit;

    public event Action<Collider> OnClosestTriggerEnter;
    public event Action<Collider> OnClosestTriggerExit;

    protected virtual void Awake ()
    {
        awernessSensor = transform.GetComponentInChildren<GameObjectSensor>();
        closestSensor = awernessSensor.transform.GetChild(0).GetComponentInChildren<GameObjectSensor>();
    }

    protected virtual void Start ()
    {
        awernessSensor.OnSensorTriggerEnter += OnSensorTriggerEnter;
        awernessSensor.OnSensorTriggerExit += OnSensorTriggerExit;

        closestSensor.OnSensorTriggerEnter += OnCloserSensorTriggerEnter;
        closestSensor.OnSensorTriggerExit += OnCloserSensorTriggerExit;
    }

    protected virtual void OnDisable ()
    {
        awernessSensor.OnSensorTriggerEnter -= OnSensorTriggerEnter;
        awernessSensor.OnSensorTriggerExit -= OnSensorTriggerExit;

        closestSensor.OnSensorTriggerEnter -= OnCloserSensorTriggerEnter;
        closestSensor.OnSensorTriggerExit -= OnCloserSensorTriggerExit;
    }

    public virtual void UpdateCloseSensorYPosition (float value)
    {
        closestSensor.transform.position = new Vector3(closestSensor.transform.position.x, value, closestSensor.transform.position.z);
    }

    protected virtual void OnSensorTriggerEnter (Collider collider)
    {
        OnAwernessTriggerEnter?.Invoke(collider);
    }

    protected virtual void OnSensorTriggerExit (Collider collider)
    {
        OnAwernessTriggerExit?.Invoke(collider);
    }

    protected virtual void OnCloserSensorTriggerEnter (Collider collider)
    {
        OnClosestTriggerEnter?.Invoke(collider);
    }

    protected virtual void OnCloserSensorTriggerExit (Collider collider)
    {
        OnClosestTriggerExit?.Invoke(collider);
    }

}

