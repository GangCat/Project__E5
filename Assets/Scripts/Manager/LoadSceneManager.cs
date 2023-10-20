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


    // 모든 게임 오브젝트를 리스트에 저장하는 함수
    private List<GameObject> GetAllGameObjects()
    {
        listInitialGO.Clear(); // 리스트 초기화

        // 하이어라키에서 모든 게임 오브젝트를 찾아 리스트에 추가
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootGameObjects)
        {
            GetAllChildGameObjects(obj);
        }

        return listInitialGO;
    }

    // 하위 자식 게임 오브젝트를 재귀적으로 찾아 리스트에 추가하는 함수
    private void GetAllChildGameObjects(GameObject parent)
    {
        listInitialGO.Add(parent); // 부모 오브젝트 추가

        // 하위 자식 오브젝트를 찾아 리스트에 추가
        foreach (Transform child in parent.transform)
        {
            GetAllChildGameObjects(child.gameObject);
        }
    }

    private List<GameObject> listInitialGO = null;
    private GameManager curMng = null;
}
