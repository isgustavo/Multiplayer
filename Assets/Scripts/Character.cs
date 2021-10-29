using UnityEngine;

public class CharacterStats : ScriptableObject
{

}

public class Character : MonoBehaviour
{
    [SerializeField]
    private CharacterStats stats;
    public CharacterStats Stats => stats;

    public Transform Visual => gameObject.transform;

    public virtual void UpdateCharacter ()
    {
        
    }

    public virtual void FixedUpdateCharacter ()
    {

    }

    public virtual void LateUpdateCharacter ()
    {

    }

}
