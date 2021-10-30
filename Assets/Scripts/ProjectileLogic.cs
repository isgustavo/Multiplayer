using UnityEngine;
using Mirror;

public class ProjectileLogic : MonoBehaviour
{
    MultiplayerPoolID poolID;

    public float speed = 1f;
    public float lifetime = 2f;
    public float currentLifetime;

    private void Awake ()
    {
        poolID = GetComponent<MultiplayerPoolID>();
    }

    private void OnEnable ()
    {
        currentLifetime = 0f;
    }

    private void Update ()
    {
        if (currentLifetime > lifetime)
            MultiplayerGamePoolManager.Current.Despawn(poolID);

        currentLifetime += Time.deltaTime;

        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

}
