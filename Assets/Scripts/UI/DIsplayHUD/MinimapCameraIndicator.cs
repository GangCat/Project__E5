using UnityEngine;

public class MinimapCameraIndicator : MonoBehaviour
{

    private void Update()
    {
        UpdateCameraIndicator();
    }

    private void UpdateCameraIndicator()
    {
        // ī�޶��� ���� ��ǥ�� �̴ϸ��� ��ǥ�� ��ȯ
        Vector2 minimapPosition = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.z) * minimapScale;
        
        // ī�޶��� ũ�⸦ �̴ϸ��� ũ��� ��ȯ
        float cameraHeight = mainCamera.orthographicSize * 2f * minimapScale;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        // �̹����� ��ġ�� ũ�⸦ ������Ʈ
        minimapCameraIndicator.anchoredPosition = minimapPosition;
        minimapCameraIndicator.sizeDelta = new Vector2(cameraWidth, cameraHeight);
    }
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform minimapCameraIndicator;
    [SerializeField] private float minimapScale = 1f;  // ���� ��ǥ�� �̴ϸ� ��ǥ�� ��ȯ�� �� ����� ������
    
}
