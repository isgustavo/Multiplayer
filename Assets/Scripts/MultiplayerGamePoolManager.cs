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

    //Isso deveria esta em 2 projetos diferetes de Master e Client
    Dictionary<string, Queue<MultiplayerPoolID>> serverObjects = new Dictionary<string, Queue<MultiplayerPoolID>>();
    Dictionary<uint, MultiplayerPoolID> clientObjects = new Dictionary<uint, MultiplayerPoolID>();

    public bool IsInitialized { get; private set; } = false;
    public event Action OnObjectPoolFinished;

    private void Awake ()
    {
        if (Current == null)
            Current = this;

    }

    private void Start ()
    {
        MultiplayerGameManager.Current.OnServerConnected += OnServerConnected;
        MultiplayerGameManager.Current.OnClientStarted += OnClientStarted;

        UIConsole.Current.AddConsole($"NetworkServer.active {NetworkServer.active}");
        
    }

    private void OnClientStarted ()
    {
        MultiplayerGameManager.Current.OnClientStarted -= OnClientStarted;
        StartCoroutine(CreatePoolOnClient());
    }

    private void OnServerConnected ()
    {
        MultiplayerGameManager.Current.OnServerConnected -= OnServerConnected;
        StartCoroutine(CreatePoolOnServer());
    }

    uint poolValue = 100;
    IEnumerator CreatePoolOnServer ()
    {
        foreach (ObjectPool obj in listOfObject)
        {
            if (serverObjects.ContainsKey(obj.prefab.name) == false)
                serverObjects.Add(obj.prefab.name, new Queue<MultiplayerPoolID>());

            for (int i = 0; i < obj.amount; i++)
            {
                GameObject newObj = GameObject.Instantiate(obj.prefab, POOL_POSITION, Quaternion.identity);
                newObj.name = obj.prefab.name;
                MultiplayerPoolID poolID = newObj.GetComponent<MultiplayerPoolID>();                
                poolID.ID = poolValue++;
                serverObjects[obj.prefab.name].Enqueue(poolID);
                newObj.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }

        IsInitialized = true;
        OnObjectPoolFinished?.Invoke();
    }

    IEnumerator CreatePoolOnClient ()
    {
        foreach (ObjectPool obj in listOfObject)
        {
            for (int i = 0; i < obj.amount; i++)
            {
                GameObject newObj = GameObject.Instantiate(obj.prefab, POOL_POSITION, Quaternion.identity);
                newObj.name = obj.prefab.name;
                MultiplayerPoolID poolID = newObj.GetComponent<MultiplayerPoolID>();
                poolID.ID = poolValue++;
                clientObjects.Add(poolID.ID, poolID);
                newObj.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }

        IsInitialized = true;
        OnObjectPoolFinished?.Invoke();
    }

    public MultiplayerPoolID SpawnOnServer (string objName)
    {
        if (serverObjects.ContainsKey(objName) == false || serverObjects[objName].Count <= 0)
        {
            Debug.LogError($"Pool is empty {objName}");
            UIConsole.Current.AddConsole($"Pool is empty {objName}");

            return null;
        }
        else
        {
            return serverObjects[objName].Dequeue();
        }
    }

    public MultiplayerPoolID SpawnOnClient (uint objId)
    {
        if (clientObjects.ContainsKey(objId) == false)
        {
            Debug.LogError($"Pool is empty {objId}");
            UIConsole.Current.AddConsole($"Pool is empty {objId}");
            return null;
        }
        else
        {
            return clientObjects[objId];
        }
    }

    public void Despawn (MultiplayerPoolID obj)
    {
        UIConsole.Current.AddConsole($"Despawn {obj.ID}");
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(false);

        if (NetworkServer.active)
            serverObjects[obj.name].Enqueue(obj);

    }
}