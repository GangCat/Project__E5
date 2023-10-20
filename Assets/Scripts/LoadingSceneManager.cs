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

    public static void LoadScene(string sceneName) // �� ��ȯ�� ȣ��.
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation operation = StartSceneLoading(); // �񵿱� �ڷ�ƾ ����
        while (!operation.isDone) // �ε� ���������� ���ư�
        {
            progressBar.UpdateLength(operation.progress);
            /* ���൵ ������Ʈ 
               Mathf.Clamp01(operation.progress / 0.9f); // progress�� 0~0.9���� �۵��ϱ⿡ ����ȭ�� ����.
               if (operation.progress >= 0.9f)
                {
                operation.allowSceneActivation = true; // �� ��ȯ ���
                }
             �̷��� ���ư�
            */

            yield return null;
        }
    }

    private AsyncOperation StartSceneLoading()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene); // �ش� ���� �ε�
        operation.allowSceneActivation = false; // �ε������� �� ��ȯ ��
        return operation;
    }

}

