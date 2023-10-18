using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PF_PathFinding : MonoBehaviour
{
    public delegate void FinishPathFindDelegate(PF_Node[] _waypoints, bool _isPathSuccess);

    public void Init(FinishPathFindDelegate _finishPathFindCallback, float _gridWorldSizeX, float _gridWorldSizeY)
    {
        grid = GetComponent<PF_Grid>();
        grid.Init(_gridWorldSizeX, _gridWorldSizeY);
        finishPathFindCallback = _finishPathFindCallback;

        openSet = new PF_Heap<PF_Node>(grid.MaxSize);
    }

    public void CheckNodeBuildable(PF_Node[] _arrFriendlyObject)
    {
        grid.CheckBuildableTest(_arrFriendlyObject);
    }

    public void StartFindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        PF_Node startNode = grid.GetNodeFromWorldPoint(_startPos);
        PF_Node targetNode = grid.GetNodeFromWorldPoint(_targetPos);

        if (startNode.Equals(targetNode))
        {
            finishPathFindCallback?.Invoke(new PF_Node[1] { targetNode }, true);
            return;
        }

        StartCoroutine(FindPath(startNode, targetNode));
        ++i;
    }

    private int i = 0;
    private int j = 0;

    private IEnumerator FindPath(PF_Node startNode, PF_Node targetNode)
    {
        bool isPathSuccess = false;

        if (!targetNode.walkable)
        {
            targetNode = grid.GetAccessibleNodeWithoutTargetNode(targetNode);
        }
        if (targetNode != null)
        {
            listWayNode.Clear();
            listNeighbor.Clear();
            curNode = null;
            neighbor = null;

            listNeighbor = grid.GetNeighbors(targetNode);

            bool isTargetAccessible = false;
            for (int i = 0; i < listNeighbor.Count; ++i)
            {
                if (listNeighbor[i].walkable)
                {
                    isTargetAccessible = true;
                    break;
                }
            }
            if (!isTargetAccessible)
                targetNode = grid.GetAccessibleNodeWithoutTargetNode(targetNode);

            while (openSet.Count > 0)
                openSet.RemoveFirstItem();
            closedSet.Clear();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                ++j;

                listNeighbor.Clear();
                curNode = openSet.RemoveFirstItem();

                closedSet.Add(curNode);

                if (openSet.Count > 50)
                {
                    isPathSuccess = true;
                    break;
                }

                // �����ߴٸ�
                if (curNode.Equals(targetNode))
                {
                    isPathSuccess = true;
                    break;
                }

                listNeighbor = grid.GetNeighbors(curNode);

                for (int i = 0; i < listNeighbor.Count; ++i)
                {
                    neighbor = listNeighbor[i];
                    if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                    int newGCostToNeighbor = curNode.gCost + CalcLowestCostWithNode(curNode, neighbor);

                    if (newGCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newGCostToNeighbor;
                        neighbor.hCost = CalcLowestCostWithNode(neighbor, targetNode);
                        neighbor.parentNode = curNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                        else
                            openSet.UpdateItem(neighbor);
                    }
                }
            }
        }

        yield return null;

        if (isPathSuccess)
        {
            listWayNode = RetracePath(startNode, curNode);
        }

        finishPathFindCallback?.Invoke(listWayNode.ToArray(), isPathSuccess);
    }

    private PF_Node curNode = null;
    private PF_Node neighbor = null;

    /// <summary>
    /// �ش� ����� �θ� Ÿ�� �ö󰡼� ��θ� ��Ž���ϰ� �������� �ٽ� ������ ����� ��θ� ������.
    /// </summary>
    /// <param name="_startNode"></param>
    /// <param name="_endNode"></param>
    private List<PF_Node> RetracePath(PF_Node _startNode, PF_Node _endNode)
    {
        List<PF_Node> path = new List<PF_Node>();
        PF_Node curNode = _endNode;

        while (!curNode.Equals(_startNode))
        {
            path.Add(curNode);
            curNode = curNode.parentNode;
        }

        //Vector3[] waypoints = SimplifyPath(path);
        path.Reverse();
        return path;
    }

    /// <summary>
    /// �̵��ϴ� ��θ� �ε巴�� ������ִ� �Լ�.
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    //private Vector3[] SimplifyPath(List<PF_Node> _path)
    //{
    //    List<Vector3> waypoints = new List<Vector3>();
    //    Vector2 directionOld = Vector2.zero;

    //    for (int i = 1; i < _path.Count; ++i)
    //    {
    //        // ���� �˻��� ��尡 ���ϴ� ������ �����ϸ� ���� ����Ʈ�� ���� ����. ����ȭ
    //        Vector2 directionNew = new Vector2(_path[i - 1].gridX - _path[i].gridX, _path[i - 1].gridY - _path[i].gridY);
    //        if (directionNew != directionOld)
    //            waypoints.Add(_path[i].worldPos);

    //        directionOld = directionNew;
    //    }
    //    return waypoints.ToArray();
    //}

    /// <summary>
    /// nodeA���� nodeB�� ���� �ִܰŸ��� ���Ƿ� ����ؼ� �� ���� ��ȯ�ϴ� �Լ�.
    /// ��Ŭ����� �Ÿ� ���
    /// </summary>
    /// <param name="_nodeA"></param>
    /// <param name="_nodeB"></param>
    /// <returns></returns>
    private int CalcLowestCostWithNode(PF_Node _nodeA, PF_Node _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        int distY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    private PF_Grid grid;

    private FinishPathFindDelegate finishPathFindCallback = null;

    private PF_Heap<PF_Node> openSet = null;
    private HashSet<PF_Node> closedSet = new HashSet<PF_Node>();
    private List<PF_Node> listNeighbor = new List<PF_Node>();
    private List<PF_Node> listWayNode = new List<PF_Node>();
}