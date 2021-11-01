public class MultiplayerBotManager : MultiplayerSpawnerManager
{
    public static MultiplayerBotManager Current;

    public override string tagName => "BotSpawnPoint";
    public override string objectName => "Bot";

    //public static MultiplayerBotManager Current;

    //Dictionary<uint, Bot> bots = new Dictionary<uint, Bot>();

    //GameObject[] botSpawnPoints;

    public override void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);

        base.Awake();
        //botSpawnPoints = GameObject.FindGameObjectsWithTag("BotSpawnPoint");
    }

    //private void Start ()
    //{
    //    MultiplayerGameManager.Current.OnClientConnected += OnClientConnected;
    //}

    //Coroutine spawnBotsCoroutine; 

    //private void OnClientConnected ()
    //{
    //    MultiplayerGameManager.Current.OnClientConnected -= OnClientConnected;

    //    spawnBotsCoroutine = StartCoroutine(SpawnBotsDelay());
    //}

    //IEnumerator SpawnBotsDelay()
    //{
    //    while(true)
    //    {
    //        yield return new WaitForSeconds(2f);

    //        if (bots.Keys.Count < 5)
    //        {
    //            SpawnBot(GetPatrolPoint(MultiplayerGamePoolManager.POOL_POSITION));
    //        } else
    //        {
    //            break;
    //        }
    //    }
    //}

    //void SpawnBot (Vector3 startPosition)
    //{
    //    UIConsole.Current.AddConsole($"SpawnBot");
    //    MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer("Bot");
    //    obj.transform.position = startPosition;
    //    obj.transform.rotation = Quaternion.identity;

    //    obj.gameObject.SetActive(true);
    //}

    //public Vector3 GetPatrolPoint(Vector3 currentPoint)
    //{
    //    Vector3 nextPoint = botSpawnPoints[UnityEngine.Random.Range(0, botSpawnPoints.Length - 1)].transform.position;

    //    Vector3 positionDiference = currentPoint - nextPoint;
    //    if (positionDiference.sqrMagnitude > 1)
    //    {
    //        return nextPoint;
    //    }
    //    else
    //    {
    //        return GetPatrolPoint(currentPoint);
    //    }
    //}

    //public void AddBot(Bot bot)
    //{
    //    if (bots.ContainsKey(bot.ID) == true)
    //        return;

    //    bots.Add(bot.ID, bot);
    //}

    //public void RemoveBot(uint id)
    //{
    //    if (bots.ContainsKey(id) == false)
    //        return;

    //    if(spawnBotsCoroutine != null)
    //        StopCoroutine(spawnBotsCoroutine);

    //    bots.Remove(id);

    //    spawnBotsCoroutine = StartCoroutine(SpawnBotsDelay());
    //}
}
