using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

public class PF_PathFinding : MonoBehaviour
{
    public delegate void FinishPathFindDelegate(PF_Node[] _waypoints, bool _isPathSuccess, PF_Node _newTargetNode = null);
    public bool isTest;
    public float waitTimeForTest;

    public void Init(FinishPathFindDelegate _finishPathFindCallback, float _gridWorldSizeX, float _gridWorldSizeY, PF_Grid _grid)
    {
        grid = _grid;
        grid.Init(_gridWorldSizeX, _gridWorldSizeY);
        finishPathFindCallback = _finishPathFindCallback;

        openSet = new PF_Heap<PF_Node>(searchLimitCnt + 50);
        closedSet = new HashSet<PF_Node>(searchLimitCnt + 50);
        listNeighbor = new List<PF_Node>(9);
        listWayNode = new List<PF_Node>(searchLimitCnt / 2);
    }

    public void CheckNodeBuildable(PF_Node[] _arrFriendlyObject)
    {
        grid.CheckBuildableTest(_arrFriendlyObject);
    }

    public void StartFindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        startNode = grid.GetNodeFromWorldPoint(_startPos);
        targetNode = grid.GetNodeFromWorldPoint(_targetPos);

        if (startNode.Equals(targetNode))
        {
            finishPathFindCallback?.Invoke(new PF_Node[1] { targetNode }, true);
            return;
        }

        UpdateUniTask(startNode, targetNode).Forget();
    }

    private async UniTaskVoid UpdateUniTask(PF_Node startNode, PF_Node targetNode)
    {
        // ��� Ž�� ����� �⺻������ ���з� ����
        bool isPathSuccess = false;

        // Ÿ�ٳ�忡 ���� �Ұ����� ��� �ش� ��� ������ ���� ������ ��� Ž��, Ÿ�� ��� ����
        if (!targetNode.walkable)
        {
            targetNode = grid.GetAccessibleNodeWithoutTargetNode(targetNode);
        }

        if (targetNode != null)
        {
            // ������ ����Ǿ��ִ� �����͵� ��� ����
            listWayNode.Clear();
            listNeighbor.Clear();
            curNode = null;
            neighborNode = null;

            while (openSet.Count > 0)
                openSet.RemoveFirstItem();

            // Ŭ�������� �˻� �Ϸ��� ���� �����ϴ� �ؽ���
            closedSet.Clear();

            // ���۳�� �ؽ��¿� ����
            // ���¼��� �˻��� ���� �����ϴ� �ؽ���
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                listNeighbor.Clear();
                curNode = openSet.RemoveFirstItem();

                closedSet.Add(curNode);

                // ���� ������ ���� openset��
                // ���� ���� �̻� ����� Ž������
                if (openSet.Count > searchLimitCnt)
                {
                    isPathSuccess = true;
                    break;
                }

                // ���� Ž������ ��尡 Ÿ�� ����� ���
                // ���Ž�� �Ϸ��̹Ƿ� Ž�� ����
                if (curNode.Equals(targetNode))
                {
                    isPathSuccess = true;
                    break;
                }

                ///������� A* �˰���
                ///���� ����� �̿� ��� ����
                listNeighbor = grid.GetNeighbors(curNode);

                // �̿� ��� ��� �˻�
                for (int i = 0; i < listNeighbor.Count; ++i)
                {
                    neighborNode = listNeighbor[i];
                    // �˻� ���� ����
                    if (!neighborNode.walkable || closedSet.Contains(neighborNode)) continue;

                    // ���ο� �ڽ�Ʈ ���
                    newGCostToNeighbor = curNode.gCost + CalcLowestCostWithNode(curNode, neighborNode);
                    newHCostToNeighbor = CalcLowestCostWithNode(neighborNode, targetNode);
                    newFCostToNeighbor = newGCostToNeighbor + newHCostToNeighbor;

                    // FCost�� ���Ͽ� ����
                    if (newFCostToNeighbor < neighborNode.FCost || !openSet.Contains(neighborNode))
                    {
                        neighborNode.gCost = newGCostToNeighbor;
                        neighborNode.hCost = newHCostToNeighbor;
                        neighborNode.parentNode = curNode;

                        // openSet�� ������� �ʾ����� �߰�, �̹� �ִٸ� �ش� ����� ���� ����Ǿ����Ƿ�
                        // ����ִ� ���� ������
                        if (!openSet.Contains(neighborNode))
                            openSet.Add(neighborNode);
                        else
                            openSet.UpdateItem(neighborNode);
                    }
                }
            }
        }

        await UniTask.Yield(PlayerLoopTiming.Update);

        // ��� Ž�� ������ ������� ��ȯ
        if (isPathSuccess)
            listWayNode = RetracePath(startNode);

        /// @@@@@@@@@@@@@@�׽�Ʈ ���� ���ǹ� ���Ŀ� ������ ��@@@@@@@@@@@@@@
        /// @@@@@@@@@@@@@@�׽�Ʈ ���� ���ǹ� ���Ŀ� ������ ��@@@@@@@@@@@@@@
        /// @@@@@@@@@@@@@@�׽�Ʈ ���� ���ǹ� ���Ŀ� ������ ��@@@@@@@@@@@@@@
        if (isTest)
            await UniTask.Delay(TimeSpan.FromSeconds(waitTimeForTest));

        finishPathFindCallback?.Invoke(listWayNode.ToArray(), isPathSuccess, targetNode);
    }

   

    private List<PF_Node> RetracePath(PF_Node _startNode)
    {
        path.Clear();

        while (curNode != null && !curNode.Equals(_startNode))
        {
            path.Add(curNode);
            curNode = curNode.parentNode;
        }

        path.Reverse();
        return path;
    }

    

    private int CalcLowestCostWithNode(PF_Node _nodeA, PF_Node _nodeB)
    {
        distX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        distY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    private int distX = 0;
    private int distY = 0;

    [SerializeField]
    private int searchLimitCnt = 200;

    private PF_Grid grid;
    private PF_Node curNode = null;
    private PF_Node neighborNode = null;

    private FinishPathFindDelegate finishPathFindCallback = null;

    private PF_Heap<PF_Node> openSet = null;
    private HashSet<PF_Node> closedSet = new HashSet<PF_Node>();
    private List<PF_Node> listNeighbor = new List<PF_Node>();
    private List<PF_Node> listWayNode = new List<PF_Node>();
    List<PF_Node> path = new List<PF_Node>();

    private int newGCostToNeighbor = 0;
    private int newHCostToNeighbor = 0;
    private int newFCostToNeighbor = 0;

    private PF_Node startNode = null;
    private PF_Node targetNode = null;
}