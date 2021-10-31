using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MultiplayerBotManager : MonoBehaviour
{
    public static MultiplayerBotManager Current;

    Dictionary<uint, Bot> bots = new Dictionary<uint, Bot>();

    GameObject[] botSpawnPoints;

    private void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);

        botSpawnPoints = GameObject.FindGameObjectsWithTag("BotSpawnPoint");
    }

    private void Start ()
    {
        MultiplayerGameManager.Current.OnClientConnected += OnClientConnected;
    }

    private void OnClientConnected ()
    {
        MultiplayerGameManager.Current.OnClientConnected -= OnClientConnected;

        StartCoroutine(SpawnBotsDelay());
    }

    IEnumerator SpawnBotsDelay()
    {
        while(true)
        {
            if(bots.Keys.Count < 5)
            {
                SpawnBot(GetPatrolPoint(MultiplayerGamePoolManager.POOL_POSITION));
            }
            yield return new WaitForSeconds(2f);
        }
    }

    void SpawnBot (Vector3 startPosition)
    {
        UIConsole.Current.AddConsole($"SpawnBot");
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer("Bot");
        obj.transform.position = startPosition;
        obj.transform.rotation = Quaternion.identity;

        obj.gameObject.SetActive(true);
    }

    public Vector3 GetPatrolPoint(Vector3 currentPoint)
    {
        Vector3 nextPoint = botSpawnPoints[UnityEngine.Random.Range(0, botSpawnPoints.Length - 1)].transform.position;

        Vector3 positionDiference = currentPoint - nextPoint;
        if (positionDiference.sqrMagnitude > 1)
        {
            return nextPoint;
        }
        else
        {
            return GetPatrolPoint(currentPoint);
        }
    }

    public void AddBot(Bot bot)
    {
        if (bots.ContainsKey(bot.ID) == true)
            return;

        bots.Add(bot.ID, bot);
    }

    public void RemoveBot(uint id)
    {
        if (bots.ContainsKey(id) == false)
            return;

        bots.Remove(id);
    }
}
