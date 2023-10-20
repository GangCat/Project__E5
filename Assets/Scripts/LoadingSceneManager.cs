using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LodingSceneManager : MonoBehaviour
{
    [SerializeField]
    ImageProgressbar progressBar = null;

    public static string nextScene;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName) // 씬 전환시 호출.
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation operation = StartSceneLoading(); // 비동기 코루틴 실행
        while (!operation.isDone) // 로딩 끝날때까지 돌아감
        {
            progressBar.UpdateLength(operation.progress);
            /* 진행도 업데이트 
               Mathf.Clamp01(operation.progress / 0.9f); // progress는 0~0.9까지 작동하기에 정규화를 해줌.
               if (operation.progress >= 0.9f)
                {
                operation.allowSceneActivation = true; // 씬 전환 허용
                }
             이렇게 돌아감
            */

            yield return null;
        }
    }

    private AsyncOperation StartSceneLoading()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene); // 해당 씬을 로드
        operation.allowSceneActivation = false; // 로딩끝나면 씬 전환 끔
        return operation;
    }

}

