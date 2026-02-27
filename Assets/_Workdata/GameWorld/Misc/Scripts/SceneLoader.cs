using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Action<float /*Progress*/> OnSceneLoading;

    public void SetTargetScene(string sceneName)
    {
        SceneLoaderData.targetScene = sceneName;
    }

    public void SetTargetSceneAndLoad(string sceneName)
    {
        SceneLoaderData.targetScene = sceneName;
        LoadSceneByNameAsync();
        StartLoadingScreen();
    }

    public void ReloadSceneAndLoad()
    {
        SceneLoaderData.targetScene = SceneManager.GetActiveScene().name;
        LoadSceneByNameAsync();
        StartLoadingScreen();
    }
    
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartLoadingScreen()
    {
        SceneManager.LoadScene("LoadingScreen");
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadSceneByNameAsync()
    {
        if (SceneLoaderData.targetScene == null) return;
        StartCoroutine(LoadSceneAsync());
    }
    
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneLoaderData.targetScene);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            OnSceneLoading?.Invoke(asyncLoad.progress);
            yield return null;
        }
        
        OnSceneLoading?.Invoke(asyncLoad.progress);
        
        asyncLoad.allowSceneActivation = true;
    }
}