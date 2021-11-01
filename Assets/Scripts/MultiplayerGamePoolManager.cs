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
    Dictionary<string, Queue<NonPlayer>> serverObjects = new Dictionary<string, Queue<NonPlayer>>();
    Dictionary<uint, NonPlayer> clientObjects = new Dictionary<uint, NonPlayer>();

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
                serverObjects.Add(obj.prefab.name, new Queue<NonPlayer>());

            for (int i = 0; i < obj.amount; i++)
            {
                GameObject newObj = GameObject.Instantiate(obj.prefab, POOL_POSITION, Quaternion.identity);
                newObj.name = obj.prefab.name;
                NonPlayer poolID = newObj.GetComponent<NonPlayer>();                
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
                NonPlayer poolID = newObj.GetComponent<NonPlayer>();
                poolID.ID = poolValue++;
                clientObjects.Add(poolID.ID, poolID);
                newObj.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }

        IsInitialized = true;
        OnObjectPoolFinished?.Invoke();
    }

    public NonPlayer SpawnOnServer (string objName)
    {
        if (serverObjects.ContainsKey(objName) == false || serverObjects[objName].Count <= 0)
        {
            Debug.LogError($"Pool is empty {objName}");
            return null;
        }
        else
        {
            return serverObjects[objName].Dequeue();
        }
    }

    public NonPlayer SpawnOnClient (uint objId)
    {
        if (clientObjects.ContainsKey(objId) == false)
        {
            Debug.LogError($"Pool is empty {objId}");
            return null;
        }
        else
        {
            return clientObjects[objId];
        }
    }

    public void Despawn (NonPlayer obj)
    {
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(false);

        if (NetworkServer.active)
            serverObjects[obj.name].Enqueue(obj);

    }
}