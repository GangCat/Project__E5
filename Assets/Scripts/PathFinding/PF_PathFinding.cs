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
        // 경로 탐색 결과를 기본적으로 실패로 설정
        bool isPathSuccess = false;

        // 타겟노드에 접근 불가능일 경우 해당 노드 주위의 접근 가능한 노드 탐색, 타겟 노드 갱신
        if (!targetNode.walkable)
        {
            targetNode = grid.GetAccessibleNodeWithoutTargetNode(targetNode);
        }

        if (targetNode != null)
        {
            // 기존에 저장되어있던 데이터들 모두 리셋
            listWayNode.Clear();
            listNeighbor.Clear();
            curNode = null;
            neighborNode = null;

            while (openSet.Count > 0)
                openSet.RemoveFirstItem();

            // 클로즈드셋은 검사 완료한 대상들 저장하는 해쉬셋
            closedSet.Clear();

            // 시작노드 해쉬셋에 저장
            // 오픈셋은 검사할 대상들 저장하는 해쉬셋
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                listNeighbor.Clear();
                curNode = openSet.RemoveFirstItem();

                closedSet.Add(curNode);

                // 부하 조절을 위해 openset이
                // 설정 개수 이상 저장시 탐색종료
                if (openSet.Count > searchLimitCnt)
                {
                    isPathSuccess = true;
                    break;
                }

                // 현재 탐색중인 노드가 타겟 노드일 경우
                // 경로탐색 완료이므로 탐색 종료
                if (curNode.Equals(targetNode))
                {
                    isPathSuccess = true;
                    break;
                }

                ///여기부터 A* 알고리즘
                ///현재 노드의 이웃 노드 저장
                listNeighbor = grid.GetNeighbors(curNode);

                // 이웃 노드 모두 검사
                for (int i = 0; i < listNeighbor.Count; ++i)
                {
                    neighborNode = listNeighbor[i];
                    // 검사 제외 조건
                    if (!neighborNode.walkable || closedSet.Contains(neighborNode)) continue;

                    // 새로운 코스트 계산
                    newGCostToNeighbor = curNode.gCost + CalcLowestCostWithNode(curNode, neighborNode);
                    newHCostToNeighbor = CalcLowestCostWithNode(neighborNode, targetNode);
                    newFCostToNeighbor = newGCostToNeighbor + newHCostToNeighbor;

                    // FCost를 비교하여 저장
                    if (newFCostToNeighbor < neighborNode.FCost || !openSet.Contains(neighborNode))
                    {
                        neighborNode.gCost = newGCostToNeighbor;
                        neighborNode.hCost = newHCostToNeighbor;
                        neighborNode.parentNode = curNode;

                        // openSet에 저장되지 않았으면 추가, 이미 있다면 해당 노드의 값이 변경되었으므로
                        // 들어있는 노드들 재정렬
                        if (!openSet.Contains(neighborNode))
                            openSet.Add(neighborNode);
                        else
                            openSet.UpdateItem(neighborNode);
                    }
                }
            }
        }

        await UniTask.Yield(PlayerLoopTiming.Update);

        // 경로 탐색 성공시 최종경로 반환
        if (isPathSuccess)
            listWayNode = RetracePath(startNode);

        /// @@@@@@@@@@@@@@테스트 위한 조건문 추후에 삭제할 것@@@@@@@@@@@@@@
        /// @@@@@@@@@@@@@@테스트 위한 조건문 추후에 삭제할 것@@@@@@@@@@@@@@
        /// @@@@@@@@@@@@@@테스트 위한 조건문 추후에 삭제할 것@@@@@@@@@@@@@@
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