using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraMovement : MonoBehaviour
{
    public void Init()
    {
        mainCamera = GetComponent<Camera>();
        targetZoom = mainCamera.orthographicSize;

        oriCullingLayer = mainCamera.cullingMask;
        oriQuaternion = transform.rotation;
    }

    public void WarpCameraWithPos(Vector3 _pos)
    {
        // transform.position = _pos + cameraOffset;
        
        Vector3 newPos = _pos + cameraOffset;
        
        // ī�޶� ��ġ ����
        transform.position = ClampCameraPosition(newPos);
        
    }

    public void ZoomCamera(float _zoomRatio)
    {
        targetZoom -= _zoomRatio * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, targetZoom, ref currentZoomVelocity, smoothTime);
    }

    public void MoveCameraWithMouse(Vector2 _mousePos)
    {
        screenMovePos = transform.position;

        
        if (_mousePos.x < screenOffsetX)
            screenMovePos -= transform.right * camMoveSpeed * Time.deltaTime;
        else if (_mousePos.x > Screen.width - screenOffsetX)
            screenMovePos += transform.right * camMoveSpeed * Time.deltaTime;

        if (_mousePos.y < screenOffsetY)
            screenMovePos -= Quaternion.Euler(0f, 45f, 0f) * Vector3.forward * camMoveSpeed * Time.deltaTime;
        else if (_mousePos.y > Screen.height - screenOffsetY)
            screenMovePos += Quaternion.Euler(0f, 45f, 0f) * Vector3.forward * camMoveSpeed * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, screenMovePos, Time.deltaTime);
        
        // ī�޶� ��ġ ����
        transform.position = ClampCameraPosition(Vector3.Lerp(transform.position, screenMovePos, Time.deltaTime));
    }

    public void MoveCameraWithKey(Vector2 _arrowKeyInput)
    {
        screenMovePos = transform.position;

        screenMovePos += transform.right * _arrowKeyInput.x * camMoveSpeed * Time.deltaTime;
        screenMovePos += Quaternion.Euler(0f, 45f, 0f) * Vector3.forward * _arrowKeyInput.y * camMoveSpeed * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, screenMovePos, Time.deltaTime);
        
        // ī�޶� ��ġ ����
        transform.position = ClampCameraPosition(Vector3.Lerp(transform.position, screenMovePos, Time.deltaTime));
    }

    public void MoveCameraWithObject(Vector3 _objectPos)
    {
        // transform.position = _objectPos + cameraOffset;
        
        Vector3 newPos = _objectPos + cameraOffset;
        
        // ī�޶� ��ġ ����
        transform.position = ClampCameraPosition(newPos);
        
    }
    
    
    // ī�޶� �̵� ���� ����
    private Vector3 ClampCameraPosition(Vector3 _targetPos)
    {
        // ī�޶��� ���� orthographicSize�� ���� ���� ������ ���
        float aspectRatio = mainCamera.aspect;  // ī�޶��� ����/���� ����
        float cameraHeight = mainCamera.orthographicSize;  // ī�޶��� ����
        float cameraWidth = cameraHeight * aspectRatio;  // ī�޶��� �ʺ�

        // ���� ī�޶� ũ�⸦ ����Ͽ� ���� ���� ������ ���
        float actualMinX = minX + cameraWidth;
        float actualMaxX = maxX - cameraWidth;
        float actualMinZ = minZ + cameraHeight;
        float actualMaxZ = maxZ - cameraHeight;

        // 45�� ȸ������ ������ ��ġ�� ����մϴ�.
        float rotatedX = _targetPos.x * Mathf.Cos(Mathf.Deg2Rad * 45) - _targetPos.z * Mathf.Sin(Mathf.Deg2Rad * 45);
        float rotatedZ = _targetPos.x * Mathf.Sin(Mathf.Deg2Rad * 45) + _targetPos.z * Mathf.Cos(Mathf.Deg2Rad * 45);
    
        // ������ ��ġ�� �����մϴ�.
        float clampedX = Mathf.Clamp(rotatedX, actualMinX, actualMaxX);
        float clampedZ = Mathf.Clamp(rotatedZ, actualMinZ, actualMaxZ);
    
        // ���ѵ� ������ ��ġ�� �ٽ� ��ȸ������ ���� ��ġ�� ����ϴ�.
        float unrotatedX = clampedX * Mathf.Cos(Mathf.Deg2Rad * -45) - clampedZ * Mathf.Sin(Mathf.Deg2Rad * -45);
        float unrotatedZ = clampedX * Mathf.Sin(Mathf.Deg2Rad * -45) + clampedZ * Mathf.Cos(Mathf.Deg2Rad * -45);
    
        return new Vector3(unrotatedX, _targetPos.y, unrotatedZ);
    }

    public void RenderFog()
    {
        curSize = mainCamera.orthographicSize;
        prevPos = transform.position;

        transform.position = fogCamPos;
        transform.rotation = fogCamRot;
        mainCamera.orthographicSize = fogCamSize;
        mainCamera.targetTexture = fogRenderTexture;
        mainCamera.cullingMask = visibleLayer;
        mainCamera.Render();
        
        mainCamera.targetTexture = mapRenderTexture;
        mainCamera.cullingMask = mapLayer;
        mainCamera.Render();

        transform.position = prevPos;
        transform.rotation = oriQuaternion;
        mainCamera.targetTexture = null;
        mainCamera.cullingMask = oriCullingLayer;
        mainCamera.orthographicSize = curSize;
        //mainCamera.Render();
    }

    [SerializeField]
    private Vector3 cameraOffset = Vector3.zero;
    [SerializeField]
    private float zoomSpeed = 5f;
    [SerializeField]
    private float camMoveSpeed = 20f;
    [SerializeField]
    private float minZoom = 5f;
    [SerializeField]
    private float maxZoom = 20f;
    [SerializeField]
    private float smoothTime = 0.2f;
    [SerializeField]
    private float screenOffsetX = 20f;
    [SerializeField]
    private float screenOffsetY = 10f;
    [SerializeField]
    private float minX, maxX, minZ, maxZ;  // ī�޶� �̵� ���� �ִ� ��ǥ

    [Header("-Fog Of Warr Attribute")]
    [SerializeField]
    private Vector3 fogCamPos = Vector3.zero;
    [SerializeField]
    private Quaternion fogCamRot;
    [SerializeField]
    private float fogCamSize = 0f;
    [SerializeField]
    private RenderTexture fogRenderTexture = null;
    [SerializeField]
    private RenderTexture mapRenderTexture = null;
    [SerializeField]
    private LayerMask visibleLayer;
    [SerializeField]
    private LayerMask mapLayer;

    private float targetZoom = 0f;
    private float currentZoomVelocity = 0f;

    private Vector3 screenMovePos = Vector3.zero;

    private Camera mainCamera = null;

    private LayerMask oriCullingLayer;
    private Vector3 prevPos = Vector3.zero;
    private float curSize = 0f;
    private Quaternion oriQuaternion;

}
