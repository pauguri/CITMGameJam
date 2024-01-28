using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private Image overlay;

    public static TransitionManager instance;

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

    void Start()
    {
        overlay.DOFade(0f, 0f);
    }

    public void TransitionToScene(string sceneName)
    {
        overlay.DOFade(1f, 1f).OnComplete(() =>
        {
            StartCoroutine(LoadScene(sceneName));
        });
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            // loading screen?
            yield return null;
        }
        overlay.DOFade(0f, 1f);
    }
}
