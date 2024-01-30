using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public EventSystem eventSystem;

    public static MenuManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSelectedGameObject(GameObject go)
    {
        eventSystem.SetSelectedGameObject(go);
    }

    public void ExitGame()
    {
        // TODO: confirm exit
        Application.Quit();
    }
}
