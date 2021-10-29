using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    Camera cam;

    private void Awake ()
    {
        cam = transform.GetComponentInChildren<Camera>(true);
        if (cam == false)
        {
            Debug.Log("CharacterCamera without camera");
        }
    }

    public void StartCamera ()
    {
        gameObject.SetActive(true);
    }
}
