using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGamePoolManager : MonoBehaviour
{
    public static Vector3 POOL_POSITION = Vector3.one * 2000;

    public static VisualGamePoolManager Current;

    [SerializeField]
    List<ObjectPool> listOfObject;

    Dictionary<string, Queue<GameObject>> objects = new Dictionary<string, Queue<GameObject>>();

    public bool IsInitialized { get; private set; } = false;
    public event Action OnObjectPoolFinished;

    private void Awake ()
    {
        if (Current == null)
            Current = this;

    }

    private void Start ()
    {
        MultiplayerGameManager.Current.OnClientStarted += OnClientStarted;

    }

    private void OnClientStarted ()
    {
        MultiplayerGameManager.Current.OnClientStarted -= OnClientStarted;
        StartCoroutine(CreatePool());
    }

    IEnumerator CreatePool ()
    {
        foreach (ObjectPool obj in listOfObject)
        {
            if (objects.ContainsKey(obj.prefab.name) == false)
                objects.Add(obj.prefab.name, new Queue<GameObject>());

            for (int i = 0; i < obj.amount; i++)
            {
                GameObject newObj = GameObject.Instantiate(obj.prefab, POOL_POSITION, Quaternion.identity);
                newObj.name = obj.prefab.name;
                objects[obj.prefab.name].Enqueue(newObj);
                newObj.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }

        IsInitialized = true;
        OnObjectPoolFinished?.Invoke();
    }

    public GameObject Spawn (string objName)
    {
        if (objects.ContainsKey(objName) == false || objects[objName].Count <= 0)
        {
            return null;
        }
        else
        {
            return objects[objName].Dequeue();
        }
    }

   
    public void Despawn (GameObject obj)
    {
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(false);

        objects[obj.name].Enqueue(obj);

    }
}
