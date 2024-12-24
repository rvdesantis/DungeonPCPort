using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreenUI : MonoBehaviour
{
    public TextMeshProUGUI loadingTXT; // Reference to the TMP text component

    void Start()
    {
        // Start loading the scene
        StartCoroutine(LoadSceneAsync("Builder"));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return new WaitForSeconds(.5f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);


        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingTXT.text = "Loading";
            if (asyncLoad.progress >= 0.1f && asyncLoad.progress < 0.2f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.2f && asyncLoad.progress < 0.3f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.3f && asyncLoad.progress < 0.4f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.4f && asyncLoad.progress < 0.5f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.5f && asyncLoad.progress < 0.6f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.6f && asyncLoad.progress < 0.7f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.7f && asyncLoad.progress < 0.8f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.8f && asyncLoad.progress < 0.9f)
            {
                loadingTXT.text = loadingTXT.text + ".";
            }
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
