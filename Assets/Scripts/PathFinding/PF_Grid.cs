using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class PF_Grid : MonoBehaviour
{
    public int MaxSize => gridSizeX * gridSizeY;

    public void Init(float _gridWorldSizeX, float _gridWorldSizeY)
    {
        gridWorldSize.x = _gridWorldSizeX;
        gridWorldSize.y = _gridWorldSizeY;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        listPrevBuildableNode = new List<PF_Node>();
        CreateGrid();
    }

    private void CreateGrid()
    {
        // 그리드에 2차원 노드 배열 공간 할당
        grid = new PF_Node[gridSizeX, gridSizeY];
        // 그리드의 중심이 0,0에 있다고 가정하고 좌하단의 좌표를 구함.
        Vector3 worldBottomLeft = transform.position - (Vector3.right * (gridWorldSize.x * 0.5f)) - (Vector3.forward * (gridWorldSize.y * 0.5f));

        // grid에 좌하단에서부터 하나씩 노드를 할당해주는 반복문
        // 위에 2중 반복문이랑 비교했을때 장점이 뭘까
        // 비교를 좀 적게한다.
        int idx = 0;
        int maxNodeCount = gridSizeX * gridSizeY;
        int idxX = 0;
        int idxY = 0;

        while (idx < maxNodeCount)
        {
            idxX = idx % gridSizeX;
            idxY = idx / gridSizeY;
            Vector3 worldPos = worldBottomLeft + Vector3.right * (idxX * nodeDiameter + nodeRadius) + Vector3.forward * (idxY * nodeDiameter + nodeRadius);
            bool walkable = !Physics.CheckSphere(worldPos, nodeRadius, unWalkableMask);
            grid[idxX, idxY] = new PF_Node(walkable, worldPos, idxX, idxY);
            ++idx;
        }
        
    }

    public void CheckBuildableTest(PF_Node[] _arrFriendlyObject)
    {
        for (int i = 0; i < listPrevBuildableNode.Count; ++i)
            listPrevBuildableNode[i].buildable = false;

        listPrevBuildableNode.Clear();

        for (int i = 0; i < _arrFriendlyObject.Length; ++i)
        {
            GetBuildableNode(_arrFriendlyObject[i], 18);
        }
    }

    //public void CheckBuildableTest()
    //{
    //    int maxNodeCount = gridSizeX * gridSizeY;
    //    int idxX = 0;
    //    int idxY = 0;
    //    PF_Node node = null;

    //    for (int idx = 0; idx < grid.Length; ++idx)
    //    {
    //        idxX = idx % gridSizeX;
    //        idxY = idx / gridSizeY;
    //        node = grid[idxX, idxY];
    //        if (node.walkable)
    //            node.buildable = Physics.CheckSphere(node.worldPos, nodeRadius, buildableMask);
    //    }

    //    Invoke("CheckBuildableTest", 1f);
    //}

    //private void GetBuildableNode(PF_Node _targetWorldPos, int _radius)
    //{
    //    int centerX = _targetWorldPos.gridX;
    //    int centerY = _targetWorldPos.gridY;
    //    int factor = 1;
    //    int i = 0;

    //    do
    //    {
    //        int startX = Mathf.Max(centerX - i, 0);
    //        int endX = Mathf.Min(centerX + i, grid.GetLength(0) - 1);
    //        int startY = Mathf.Max(centerY - i, 0);
    //        int endY = Mathf.Min(centerY + i, grid.GetLength(1) - 1);

    //        for (int x = startX; x <= endX; x++)
    //        {
    //            for (int y = startY; y <= endY; y++)
    //            {
    //                grid[x, y].buildable = true;
    //                listPrevBuildableNode.Add(grid[x, y]);
    //            }
    //        }

    //        if (i == _radius)
    //            factor = -1;
    //        i += factor;
    //    }
    //    while (i >= 0);
    //}

    private void GetBuildableNode(PF_Node _targetWorldPos, int _radius)
    {
        int centerX = _targetWorldPos.gridX;
        int centerY = _targetWorldPos.gridY;
        int ttlCnt = _radius * (2 * _radius + 2) + 1;
        int factor = 1;
        int i = 0;
        int j = 0;
        int k = 0;
        int l = 0;

        int curIdxX = centerX;
        int idxY = centerY + _radius;
        int endIdxX = centerX + 1;

        for(int h = 0; h < ttlCnt; ++h)
        {
            int tempIdxX = Mathf.Clamp(curIdxX, 0, gridSizeX);
            int tempIdxY = Mathf.Clamp(idxY, 0, gridSizeY);
            grid[tempIdxX, tempIdxY].buildable = true;
            listPrevBuildableNode.Add(grid[tempIdxY, tempIdxY]);
            ++curIdxX;
            if(curIdxX.Equals(endIdxX))
            {
                if (i == _radius)
                    factor = -1;
                i += factor;
                curIdxX = centerX - i;
                endIdxX = centerX + i + 1;
                idxY = centerY + (_radius - i) * factor;
            }
        }

        //do
        //{
        //    k = centerX - i;
        //    j = centerX + i + 1;
        //    l = centerY + (_radius - i) * factor;
        //    while (k < j)
        //    {
        //        grid[k, l].buildable = true;
        //        listPrevBuildableNode.Add(grid[k, l]);
        //        ++k;
        //    }
        //    if (i.Equals(_radius))
        //        factor = -1;
        //    i += factor;
        //}
        //while (i >= 0);

    }

    //private void GetBuildableNode(Vector3 _targetWorldPos, int _radius)
    //{
    //    PF_Node centerNode = GetNodeFromWorldPoint(_targetWorldPos);
    //    int centerX = centerNode.gridX;
    //    int centerY = centerNode.gridY;
    //    int factor = 1;
    //    int i = 0;
    //    int j = 0;
    //    int k = 0;
    //    int l = 0;

    //    do
    //    {
    //        k = centerX - i;
    //        j = centerX + i + 1;
    //        l = centerY + (_radius - i) * factor;
    //        while (k < j)
    //        {
    //            grid[k, l].buildable = true;
    //            ++k;
    //        }
    //        if (i.Equals(_radius))
    //            factor = -1;
    //        i += factor;
    //    }
    //    while (i >= 0);

    //}

    //private IEnumerator CheckBuildableTest()
    //{
    //    int maxNodeCount = gridSizeX * gridSizeY;
    //    int idxX = 0;
    //    int idxY = 0;
    //    PF_Node node = null;
    //    while (true)
    //    {
    //        for (int idx = 0; idx < grid.Length; ++idx)
    //        {
    //            idxX = idx % gridSizeX;
    //            idxY = idx / gridSizeY;
    //            node = grid[idxX, idxY];
    //            if (node.walkable)
    //                node.buildable = Physics.CheckSphere(node.worldPos, nodeRadius, buildableMask);

    //            if (idx / 128 == 0)
    //                yield return null;
    //        }

    //        yield return null;
    //    }

    //}


    /// <summary>
    /// 해당 Pos의 노드를 반환하는 함수.
    /// </summary>
    /// <param name="_worldPos"></param>
    /// <returns></returns>
    public PF_Node GetNodeFromWorldPoint(Vector3 _worldPos)
    {
        // grid의 중심이 0이니까 즉 percentX가 0.5일때 worldPos.x가 0이 아니라 gridWorldSize.x * 0.5f니까 이렇게 작성됨.
        float percentX = (_worldPos.x + gridWorldSize.x * 0.5f) / gridWorldSize.x;
        float percentY = (_worldPos.z + gridWorldSize.y * 0.5f) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * percentX), 0, gridSizeX);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSizeY - 1) * percentY), 0, gridSizeY);

        return grid[x, y];
    }

    /// <summary>
    /// _curNode와 인접한 모든 노드들을 반환하는 함수
    /// </summary>
    /// <param name="_curNode"></param>
    /// <returns></returns>
    public List<PF_Node> GetNeighbors(PF_Node _curNode)
    {
        List<PF_Node> neighbours = new List<PF_Node>();

        // _curNode기준으로 3by3위치의 노드들을 반환하기 위한 반복문
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0) continue;

                int checkX = _curNode.gridX + x;
                int checkY = _curNode.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    public PF_Node GetAccessibleNodeWithoutTargetNode(PF_Node _targetNode)
    {
        queueNotVisitedNode.Clear();
        hashSetVisitedNode.Clear();

        listNeighborNode.Clear();

        listNeighborNode = GetNeighbors(_targetNode);
        foreach (PF_Node neighbor in listNeighborNode)
        {
            if (!hashSetVisitedNode.Contains(neighbor))
            {
                hashSetVisitedNode.Add(neighbor);
                queueNotVisitedNode.Enqueue(neighbor);
            }
        }

        while (queueNotVisitedNode.Count > 0)
        {
            listNeighborNode.Clear();
            PF_Node currentNode = queueNotVisitedNode.Dequeue();

            if (currentNode.walkable)
            {
                return currentNode;
            }

            listNeighborNode = GetNeighbors(currentNode);
            foreach (PF_Node neighbor in listNeighborNode)
            {
                if (!hashSetVisitedNode.Contains(neighbor))
                {
                    hashSetVisitedNode.Add(neighbor);
                    queueNotVisitedNode.Enqueue(neighbor);
                }
            }
        }

        return null;
    }

    public void UpdateNodeWalkable(PF_Node _node, bool _isWalkable)
    {
        _node.walkable = _isWalkable;
    }

    public bool GetNodeIsWalkable(int _gridX, int _gridY)
    {
        
        return grid[Mathf.Clamp(_gridX, 0, gridSizeX), Mathf.Clamp(_gridY,0,gridSizeY)].walkable;
    }

    public PF_Node GetNodeWithGrid(int _gridX, int _gridY)
    {
        // 예외처리
        return grid[Mathf.Clamp(_gridX, 0, gridSizeX), Mathf.Clamp(_gridY,0,gridSizeY)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y));
        if (grid != null && displayUnwalkableNodeGizmos)
        {
            foreach (PF_Node node in grid)
            {
                if (!node.walkable)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(node.worldPos, new Vector3(nodeRadius * 2, 0.1f, nodeRadius * 2));
                }
            }
        }

        if(grid != null && displayNodeGizmos)
        {
            foreach (PF_Node node in grid)
            {
                if (node.walkable)
                {
                    Color color = Color.white;
                    color.a = 0.3f;
                    Gizmos.color = color;
                    Gizmos.DrawCube(node.worldPos, new Vector3(nodeRadius * 2, 0.1f, nodeRadius * 2));
                }
            }
        }

        if(grid != null && displayBuildableNodeGizmos)
        {
            foreach(PF_Node node in grid)
            {
                if (node.buildable)
                {
                    Color color = Color.green;
                    color.a = 0.3f;
                    Gizmos.color = color;
                    Gizmos.DrawCube(node.worldPos, new Vector3(nodeRadius * 2, 0.1f, nodeRadius * 2));
                }
            }
        }
    }

    [SerializeField]
    private bool displayNodeGizmos = false;
    [SerializeField]
    private bool displayUnwalkableNodeGizmos = false;
    [SerializeField]
    private bool displayBuildableNodeGizmos = false;
    [SerializeField]
    private LayerMask unWalkableMask;
    [SerializeField]
    private LayerMask buildableMask;
    [SerializeField]
    private float nodeRadius = 0f;

    private float nodeDiameter = 0f;
    private int gridSizeX = 0;
    private int gridSizeY = 0;
    private Vector2 gridWorldSize = Vector2.zero;

    private Queue<PF_Node> queueNotVisitedNode = new Queue<PF_Node>();
    private HashSet<PF_Node> hashSetVisitedNode = new HashSet<PF_Node>();
    private List<PF_Node> listNeighborNode = new List<PF_Node>();

    private PF_Node[,] grid = null;

    private List<PF_Node> listPrevBuildableNode = null;
}
