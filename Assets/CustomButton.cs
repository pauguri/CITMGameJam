using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private ShuffleText shuffleText;
    private TextMeshProUGUI buttonText;
    private Button button;
    private bool interactable = false;

    private void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!interactable) return;
        Focus();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!interactable) return;
        Blur();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable) return;
        if (MenuManager.instance != null)
        {
            MenuManager.instance.SetSelectedGameObject(gameObject);
        }
    }

    void Focus()
    {
        shuffleText.BurstShuffle(0.2f);
        buttonText.fontStyle = FontStyles.Underline;
    }

    void Blur()
    {
        buttonText.fontStyle = FontStyles.Normal;
    }

    public void Disable()
    {
        interactable = false;
        button.interactable = false;
    }

    public void Enable()
    {
        interactable = true;
        button.interactable = true;
    }
}
