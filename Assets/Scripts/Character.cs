using UnityEngine;


public class Character : MonoBehaviour
{
    [SerializeField]
    private SOMultiplayerCharacter stats;
    public SOMultiplayerCharacter Stats => stats;

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
