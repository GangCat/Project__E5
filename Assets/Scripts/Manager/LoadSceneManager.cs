using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void ChangeScene(string _sceneName)
    {
        //ResetAll();
        SceneManager.LoadScene(_sceneName);
    }

    private static void ResetAll()
    {

    }


    // ��� ���� ������Ʈ�� ����Ʈ�� �����ϴ� �Լ�
    private List<GameObject> GetAllGameObjects()
    {
        listInitialGO.Clear(); // ����Ʈ �ʱ�ȭ

        // ���̾��Ű���� ��� ���� ������Ʈ�� ã�� ����Ʈ�� �߰�
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootGameObjects)
        {
            GetAllChildGameObjects(obj);
        }

        return listInitialGO;
    }

    // ���� �ڽ� ���� ������Ʈ�� ��������� ã�� ����Ʈ�� �߰��ϴ� �Լ�
    private void GetAllChildGameObjects(GameObject parent)
    {
        listInitialGO.Add(parent); // �θ� ������Ʈ �߰�

        // ���� �ڽ� ������Ʈ�� ã�� ����Ʈ�� �߰�
        foreach (Transform child in parent.transform)
        {
            GetAllChildGameObjects(child.gameObject);
        }
    }

    private List<GameObject> listInitialGO = null;
    private GameManager curMng = null;
}
