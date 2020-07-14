using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    Image loadingScreen;
    public Sprite tutorialScreen;
    public Sprite level1Screen;
    public Sprite level2Screen;

    void Awake()
    {
        MakeSingleton();
        loadingScreen = transform.GetChild(0).GetComponent<Image>();
    }

    void MakeSingleton()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(LoadLevelAsync(level));
    }

    IEnumerator LoadLevelAsync(int level)
    {
        if (level == 1)
            loadingScreen.sprite = tutorialScreen;
        else if (level == 2)
            loadingScreen.sprite = level1Screen;
        else if (level == 3)
            loadingScreen.sprite = level2Screen;

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        loadingScreen.gameObject.SetActive(true);

        while (!operation.isDone)
        {
            if (operation.progress >= 1)
                loadingScreen.gameObject.SetActive(false);
            yield return null;
        }
    }
}
