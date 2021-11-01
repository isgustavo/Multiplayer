using System;
using Mirror;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private SOMultiplayerCharacter stats;
    public SOMultiplayerCharacter Stats => stats;

    protected Renderer characterRenderer;

    float currentHealth;
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            if (currentHealth == value)
                return;

            currentHealth = value;
            OnCurrentHealthChanged?.Invoke();

            if (currentHealth <= 0)
            {
                Dead();
            }
        }
    }

    public event Action OnCurrentHealthChanged;
    public event Action OnDeadEvent;

    public CharacterCollision CharacterCollision { get; private set; }

    public Transform Visual => gameObject.transform;

    public virtual void Awake ()
    {
        characterRenderer = GetComponentInChildren<Renderer>();

        CharacterCollision = GetComponent<CharacterCollision>();
    }

    public virtual void OnEnable ()
    {
        CurrentHealth = stats.MaxHealth;
        CharacterCollision.OnCollision += OnCollision;

        if(NetworkServer.active == false)
            SetVisual();
    }

    public virtual void UpdateCharacter () {  }

    public virtual void FixedUpdateCharacter () {  }

    public virtual void LateUpdateCharacter () {  }

    public virtual void OnCollision (Collider collider) { }

    public bool IsAlive () => CurrentHealth > 0;

    protected virtual void SetVisual() { }

    protected virtual void TakeDamage (float damage, uint ownerID)
    {
        if (currentHealth <= 0)
            return;

        CurrentHealth -= damage;

        TryScore(ownerID);
    }

    protected virtual void Dead ()
    {
        CharacterCollision.OnCollision -= OnCollision;
        OnDeadEvent?.Invoke();
    }

    void TryScore (uint ownerId)
    {
        if (MultiplayerObjectGameManager.Current.Players.ContainsKey(ownerId))
            MultiplayerObjectGameManager.Current.Players[ownerId].AddStore(Stats.HitPoint);
    }
}
