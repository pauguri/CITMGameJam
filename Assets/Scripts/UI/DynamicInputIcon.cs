using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DynamicInputIcon : MonoBehaviour
{
    [SerializeField] InputIcon[] icons;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        InputUser.onChange += OnInputChange;

        if (PlayerController.instance != null)
        {
            if (PlayerController.instance.playerInput != null)
            {
                UpdateIcon(PlayerController.instance.playerInput.currentControlScheme);
            }
        }
    }

    void OnInputChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged && PlayerController.instance != null)
        {
            UpdateIcon(user.controlScheme.Value.name);
        }
    }

    void UpdateIcon(string controlScheme)
    {
        InputIcon icon = Array.Find(icons, i => i.controlScheme == controlScheme);
        if (icon != null)
        {
            image.sprite = icon.sprite;
        }
    }

    [Serializable]
    class InputIcon
    {
        public string controlScheme;
        public Sprite sprite;
    }
}
