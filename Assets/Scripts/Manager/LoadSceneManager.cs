using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public void Init()
    {
        DontDestroyOnLoad(gameObject);

        if (!FindFirstObjectByType<LoadSceneManager>().Equals(this))
            Destroy(FindFirstObjectByType<LoadSceneManager>().gameObject);
    }

    public static void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }
}
