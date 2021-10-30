using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[Serializable]
public class ObjectPool
{
    public GameObject prefab;
    public int amount;
}

public class MultiplayerGamePoolManager : MonoBehaviour
{
    public static Vector3 POOL_POSITION = Vector3.one * 2000;

    public static MultiplayerGamePoolManager Current;

    [SerializeField]
    List<ObjectPool> listOfObject;

    Dictionary<string, Queue<MultiplayerPoolID>> objects = new Dictionary<string, Queue<MultiplayerPoolID>>();

    public bool IsInitialized { get; private set; } = false;
    public event Action OnObjectPoolFinished;

    private void Awake ()
    {
        if (Current == null)
            Current = this;

    }

    private void Start ()
    {
        //MultiplayerGameManager.Current.OnServerConnected += OnServerConnected;
        MultiplayerGameManager.Current.OnClientConnected += OnClientConnected;

        StartCoroutine(CreatePool());
    }

    private void OnClientConnected ()
    {
        //Dictionary<uint, NetworkIdentity> values = NetworkServer.spawned;

        //foreach(NetworkIdentity n in values.Values)
        //{
        //    UIConsole.Current.AddConsole($"OnClientConnected {n.assetId}");
        //    NetworkServer.Spawn(n.gameObject);
        //}
    }

    private void OnServerConnected ()
    {
        //MultiplayerGameManager.Current.OnServerConnected -= OnServerConnected;
        //StartCoroutine(CreatePool());
    }

    uint poolValue = 100;
    IEnumerator CreatePool ()
    {
        foreach (ObjectPool obj in listOfObject)
        {
            if (objects.ContainsKey(obj.prefab.name) == false)
                objects.Add(obj.prefab.name, new Queue<MultiplayerPoolID>());

            for (int i = 0; i < obj.amount; i++)
            {
                GameObject newObj = GameObject.Instantiate(obj.prefab, POOL_POSITION, Quaternion.identity);
                newObj.name = obj.prefab.name;
                MultiplayerPoolID poolID = newObj.GetComponent<MultiplayerPoolID>();                
                poolID.ID = poolValue++;
                objects[obj.prefab.name].Enqueue(poolID);
                newObj.SetActive(false);
                // NetworkServer.Spawn(newObj);
                //StartCoroutine(test(newObj));
                yield return new WaitForEndOfFrame();
            }
        }

        IsInitialized = true;
        OnObjectPoolFinished?.Invoke();
    }

    //IEnumerator test (GameObject newObj)
    //{
    //    yield return new WaitForEndOfFrame();
    //    newObj.SetActive(false);
    //}

    public MultiplayerPoolID Spawn (string objName)
    {
        if (objects.ContainsKey(objName) == false || objects[objName].Count <= 0)
        {
            Debug.LogError($"Pool is empty {objName}");
            UIConsole.Current.AddConsole($"Pool is empty {objName}");

            return null;
        }
        else
        {
            return objects[objName].Dequeue();
        }
    }

    public void Despawn (MultiplayerPoolID obj)
    {
        UIConsole.Current.AddConsole($"Despawn {obj.ID}");
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(false);
        objects[obj.name].Enqueue(obj);
    }
}