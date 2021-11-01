using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    Character character;
    Transform barGroup;
    Image fillImage;

    private void Awake ()
    {
        barGroup = transform.Find("Bar");
        fillImage = barGroup.transform.Find("Fill").GetComponent<Image>();
    }

    private void OnEnable ()
    {
        character = transform.parent.GetComponent<Character>();
        character.OnCurrentHealthChanged += OnCurrentHealthChanged;

        fillImage.fillAmount = 1;
    }

    private void OnDisable ()
    {
        character.OnCurrentHealthChanged -= OnCurrentHealthChanged;
    }

    private void OnCurrentHealthChanged ()
    {
        fillImage.fillAmount = Mathf.Clamp(character.CurrentHealth / character.Stats.MaxHealth, 0, character.Stats.MaxHealth);
    }
}
