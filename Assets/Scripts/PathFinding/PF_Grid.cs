using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        CreateGrid();
    }

    private void CreateGrid()
    {
        // �׸��忡 2���� ��� �迭 ���� �Ҵ�
        grid = new PF_Node[gridSizeX, gridSizeY];
        // �׸����� �߽��� 0,0�� �ִٰ� �����ϰ� ���ϴ��� ��ǥ�� ����.
        Vector3 worldBottomLeft = transform.position - (Vector3.right * (gridWorldSize.x * 0.5f)) - (Vector3.forward * (gridWorldSize.y * 0.5f));

        // grid�� ���ϴܿ������� �ϳ��� ��带 �Ҵ����ִ� �ݺ���
        // ���� 2�� �ݺ����̶� �������� ������ ����
        // �񱳸� �� �����Ѵ�.
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

    /// <summary>
    /// �ش� Pos�� ��带 ��ȯ�ϴ� �Լ�.
    /// </summary>
    /// <param name="_worldPos"></param>
    /// <returns></returns>
    public PF_Node GetNodeFromWorldPoint(Vector3 _worldPos)
    {
        // grid�� �߽��� 0�̴ϱ� �� percentX�� 0.5�϶� worldPos.x�� 0�� �ƴ϶� gridWorldSize.x * 0.5f�ϱ� �̷��� �ۼ���.
        float percentX = (_worldPos.x + gridWorldSize.x * 0.5f) / gridWorldSize.x;
        float percentY = (_worldPos.z + gridWorldSize.y * 0.5f) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    /// <summary>
    /// _curNode�� ������ ��� ������ ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="_curNode"></param>
    /// <returns></returns>
    public List<PF_Node> GetNeighbors(PF_Node _curNode)
    {
        List<PF_Node> neighbours = new List<PF_Node>();

        // _curNode�������� 3by3��ġ�� ������ ��ȯ�ϱ� ���� �ݺ���
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

    //public PF_Node GetAccessibleNode(PF_Node _targetNode)
    //{
    //    queueNotVisitedNode.Clear();
    //    hashSetVisitedNode.Clear();

    //    queueNotVisitedNode.Enqueue(_targetNode);

    //    while (queueNotVisitedNode.Count > 0)
    //    {
    //        listNeighborNode.Clear();
    //        PF_Node currentNode = queueNotVisitedNode.Dequeue();

    //        if (currentNode.walkable)
    //        {
    //            return currentNode;
    //        }

    //        listNeighborNode = GetNeighbors(currentNode);
    //        foreach (PF_Node neighbor in listNeighborNode)
    //        {
    //            if (!hashSetVisitedNode.Contains(neighbor))
    //            {
    //                hashSetVisitedNode.Add(neighbor);
    //                queueNotVisitedNode.Enqueue(neighbor);
    //            }
    //        }
    //    }

    //    return null;
    //}


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
        return grid[_gridX, _gridY].walkable;
    }

    public PF_Node GetNodeWithGrid(int _gridX, int _gridY)
    {
        // ����ó��
        return grid[_gridX, _gridY];
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
    }

    [SerializeField]
    private bool displayNodeGizmos = false;
    [SerializeField]
    private bool displayUnwalkableNodeGizmos = false;
    [SerializeField]
    private LayerMask unWalkableMask;
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
}
