using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameManager : MonoBehaviour
{
    public Vector2 worldSlize;
    public GameObject[] moveObjs;
    PF_PathRequestManager pathMng;

    private void Awake()
    {
        pathMng = FindFirstObjectByType<PF_PathRequestManager>();
        pathMng.Init(worldSlize.x, worldSlize.y);
    }

    public void StartPathFinding()
    {
        foreach (var obj in moveObjs)
        {
            if(obj.activeSelf)
                obj.GetComponent<MoveObj>().RequestPath();
        }
            
    }
}
