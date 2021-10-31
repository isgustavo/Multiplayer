using UnityEngine;


public class Character : MonoBehaviour
{
    [SerializeField]
    private SOMultiplayerCharacter stats;
    public SOMultiplayerCharacter Stats => stats;
    
    int currentLife;
    public int CurrentLife
    {
        get
        {
            return currentLife;
        }
        set
        {
            if (currentLife == value)
                return;

            currentLife = value;

            if (currentLife <= 0)
            {
                Dead();
            }
        }
    }

    public CharacterCollision CharacterCollision { get; private set; }

    public Transform Visual => gameObject.transform;

    public virtual void Awake ()
    {
        CharacterCollision = GetComponent<CharacterCollision>();
        CharacterCollision.OnCollision += OnCollision;
    }

    public virtual void OnEnable ()
    {
        CurrentLife = stats.MaxLive;
    }

    public virtual void UpdateCharacter ()
    {
        
    }

    public virtual void FixedUpdateCharacter ()
    {

    }

    public virtual void LateUpdateCharacter ()
    {

    }

    public virtual void OnCollision (Collider collider) { }

    public bool IsAlive () => CurrentLife > 0;

    protected virtual void TakeDamage (int damage)
    {
        CurrentLife -= damage;
    }

    protected virtual void Dead ()
    {

    }
}
