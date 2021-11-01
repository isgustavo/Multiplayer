using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    public Camera Cam { get; private set; }

    private void Awake ()
    {
        Cam = transform.GetComponentInChildren<Camera>(true);
        if (Cam == false)
        {
            Debug.Log("CharacterCamera without camera");
        }
    }

    public void StartCamera ()
    {
        gameObject.SetActive(true);
    }
}
