using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MultiplayerSpawnerManager : MonoBehaviour
{

    Dictionary<uint, GameObject> objects = new Dictionary<uint, GameObject>();

    GameObject[] spawnPoints;

    public virtual string tagName => "";
    public virtual string objectName => "";

    public  virtual void Awake ()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag(tagName);
    }

    private void Start ()
    {
        MultiplayerGameManager.Current.OnClientConnected += OnClientConnected;
    }

    Coroutine spawnBotsCoroutine;

    private void OnClientConnected ()
    {
        MultiplayerGameManager.Current.OnClientConnected -= OnClientConnected;

        spawnBotsCoroutine = StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay ()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (objects.Keys.Count < 4)
            {
                SpawnObject(GetRandomPoint(MultiplayerGamePoolManager.POOL_POSITION));
            }
            else
            {
                break;
            }
        }
    }

    protected virtual void SpawnObject (Vector3 startPosition)
    {
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer(objectName);
        if (obj == null)
            return;

        obj.transform.position = startPosition;
        obj.transform.rotation = Quaternion.identity;

        obj.gameObject.SetActive(true);
    }

    public Vector3 GetRandomPoint (Vector3 currentPoint)
    {
        Vector3 nextPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length - 1)].transform.position;

        Vector3 positionDiference = currentPoint - nextPoint;
        if (positionDiference.sqrMagnitude > 1)
        {
            return nextPoint;
        }
        else
        {
            return GetRandomPoint(currentPoint);
        }
    }

    public void Add (uint id, GameObject obj)
    {
        if (objects.ContainsKey(id) == true)
            return;

        objects.Add(id, obj);
    }

    public void Remove (uint id)
    {
        if (objects.ContainsKey(id) == false)
            return;

        if (spawnBotsCoroutine != null)
            StopCoroutine(spawnBotsCoroutine);

        objects.Remove(id);

        spawnBotsCoroutine = StartCoroutine(SpawnDelay());
    }
}
