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

        openSet = new PF_Heap<PF_Node>(searchLimitCnt + 100);
        closedSet = new HashSet<PF_Node>(searchLimitCnt + 100);
        listNeighbor = new List<PF_Node>(8);
        listWayNode = new List<PF_Node>();
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
    }

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
                listNeighbor.Clear();
                curNode = openSet.RemoveFirstItem();

                closedSet.Add(curNode);

                if (openSet.Count > 200)
                {
                    isPathSuccess = true;
                    break;
                }

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

    //private Vector3[] SimplifyPath(List<PF_Node> _path)
    //{
    //    List<Vector3> waypoints = new List<Vector3>();
    //    Vector2 directionOld = Vector2.zero;

    //    for (int i = 1; i < _path.Count; ++i)
    //    {
    //        Vector2 directionNew = new Vector2(_path[i - 1].gridX - _path[i].gridX, _path[i - 1].gridY - _path[i].gridY);
    //        if (directionNew != directionOld)
    //            waypoints.Add(_path[i].worldPos);

    //        directionOld = directionNew;
    //    }
    //    return waypoints.ToArray();
    //}

    private int CalcLowestCostWithNode(PF_Node _nodeA, PF_Node _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        int distY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    [SerializeField]
    private int searchLimitCnt = 200;

    private PF_Grid grid;

    private FinishPathFindDelegate finishPathFindCallback = null;

    private PF_Heap<PF_Node> openSet = null;
    private HashSet<PF_Node> closedSet = new HashSet<PF_Node>();
    private List<PF_Node> listNeighbor = new List<PF_Node>();
    private List<PF_Node> listWayNode = new List<PF_Node>();

    private Dictionary<Tuple<PF_Node, PF_Node>, int> distanceCache = new Dictionary<Tuple<PF_Node, PF_Node>, int>();
}