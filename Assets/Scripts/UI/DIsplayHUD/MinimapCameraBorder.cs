using UnityEngine;

public class MinimapCameraBorder : MonoBehaviour
{


    private void Update()
    {
        DrawMinimapBorder();
    }

    void DrawMinimapBorder()
    {
        // ���� ī�޶��� 4�� �𼭸� ��ǥ�� ���
        Vector3[] corners = new Vector3[4];
        corners[0] = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.farClipPlane));
        corners[1] = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.farClipPlane));
        corners[2] = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.farClipPlane));
        corners[3] = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.farClipPlane));

        // ���η������� �� ������ ����
        lineRenderer.positionCount = 5;
        
        // �� ������ ����
        for (int i = 0; i < 4; i++)
        {
            Vector3 minimapPos = WorldToMinimap(corners[i]);
            lineRenderer.SetPosition(i, new Vector3(minimapPos.x, minimapPos.y, 0)); // Z ���� 0����
        }
        // ù ��° �������� �������� �ٽ� �߰��Ͽ� �簢���� �ϼ�
        lineRenderer.SetPosition(4, lineRenderer.GetPosition(0));
    }

    Vector2 WorldToMinimap(Vector3 worldPos)
    {
        Vector3 relativePos = worldPos - minimapWorldCenter;
        float xPercent = (relativePos.x + minimapWorldSize.x * 0.5f) / minimapWorldSize.x;
        float zPercent = (relativePos.z + minimapWorldSize.z * 0.5f) / minimapWorldSize.z;

        return new Vector2(xPercent * minimapRect.sizeDelta.x, zPercent * minimapRect.sizeDelta.y);
    }
    
    
    public Camera mainCamera; // ���� ī�޶�
    public LineRenderer lineRenderer; // ���� ������
    public RectTransform minimapRect; // �̴ϸ� RectTransform

    public Vector3 minimapWorldSize; // �̴ϸ��� ǥ���ϴ� ������ ũ��
    public Vector3 minimapWorldCenter; // �̴ϸ��� ǥ���ϴ� ������ �߽� ��ǥ
    
}