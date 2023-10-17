using UnityEngine;

public class MinimapCameraViewport : MonoBehaviour
{


    private void OnGUI()
    {
        // ī�޶��� ���� ��ǥ�� �̴ϸ��� ��ǥ�� ��ȯ
        Vector2 minimapPosition = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.z) * minimapScale;

        // ī�޶��� ũ�⸦ �̴ϸ��� ũ��� ��ȯ
        float cameraHeight = mainCamera.orthographicSize * 2f * minimapScale;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // �ε��������� ��ġ�� ũ�⸦ Rect�� ����
        Rect cameraIndicatorRect = new Rect(minimapPosition.x, minimapPosition.y, cameraWidth, cameraHeight);

        // �ε������͸� ȭ�鿡 �׸�
        GUI.DrawTexture(cameraIndicatorRect, cameraIndicatorTexture);
    }
    
    public Camera mainCamera;
    public float minimapScale = 1f;
    public Texture2D cameraIndicatorTexture;
}
