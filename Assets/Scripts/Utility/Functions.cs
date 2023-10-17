using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Functions
{
    /// <summary>
    /// euler������ transform�� ȸ����Ű�� �޼���
    /// </summary>
    /// <param name="_tr"></param>
    /// <param name="_euler"></param>
    public static void SetRotation(Transform _tr, Vector3 _euler)
    {
        _tr.rotation = Quaternion.Euler(_euler);
    }

    /// <summary>
    /// y�� ���� _angle������ transform�� ȸ����Ű�� �޼���
    /// </summary>
    /// <param name="_tr"></param>
    /// <param name="_angle_degree"></param>
    public static void RotateYaw(Transform _tr, float _angle_degree)
    {
        _tr.rotation = Quaternion.Euler(0f, -_angle_degree + 90f, 0f);
    }

    /// <summary>
    /// z�� ���� _angle������ transform�� ȸ����Ű�� �޼���
    /// </summary>
    /// <param name="_tr"></param>
    /// <param name="_angle_degree"></param>
    public static void RotatePitch(Transform _tr, float _angle_degree)
    {
        _tr.rotation = Quaternion.Euler(0f, 0f, -_angle_degree + 90f);
    }

    /// <summary>
    /// x�� ���� _angle������ transform�� ȸ����Ű�� �޼���
    /// </summary>
    /// <param name="_tr"></param>
    /// <param name="_angle_degree"></param>
    public static void RotateRoll(Transform _tr, float _angle_degree)
    {
        _tr.rotation = Quaternion.Euler(-_angle_degree + 90f, 0f, 0f);
    }

    /// <summary>
    /// _oriPos�� _targetPos�� �̷�� degree�� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <param name="_oriPos"></param>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    public static float CalcAngleToTarget(Vector3 _oriPos, Vector3 _targetPos)
    {
        Vector3 oriPos = _oriPos;
        oriPos.y = 0f;
        Vector3 targetPos = _targetPos;
        targetPos.y = 0f;

        Vector3 dirToTarget = (targetPos - oriPos).normalized;
        return Mathf.Atan2(dirToTarget.z, dirToTarget.x) * Mathf.Rad2Deg;
    }

    public static bool Picking(out RaycastHit _hit)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if(Physics.Raycast(ray, out _hit))
            return true;

        return false;
    }

    /// <summary>
    /// �±׸��� ������ ��ŷ�ϴ� �Լ�.
    /// _point�� ��ŷ�� ��ġ�� �����ϰ� ���� ���θ� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="_tag"></param>
    /// <param name="_point"></param>
    /// <returns></returns>
    public static bool Picking(string _tag, ref Vector3 _point)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag(_tag))
            {
                _point = hit.point;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// �±׿� ���̾��ũ�� �̿��� ��ŷ�ϴ� �޼���
    /// _point�� ��ŷ�� ��ġ�� �����ϰ� ���� ���θ� ��ȯ�Ѵ�.
    /// _layerMask�� int������ �־���� �Ѵ�.
    /// </summary>
    /// <param name="_tag"></param>
    /// <param name="_layerMask"></param>
    /// <param name="_point"></param>
    /// <returns></returns>
    public static bool Picking(string _tag, LayerMask _layerMask, ref Vector3 _point)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, _layerMask))
        {
            if (hit.transform.CompareTag(_tag))
            {
                _point = hit.point;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ���̾��ũ�� ������ ��ŷ�ϴ� �Լ�.
    /// </summary>
    /// <param name="_layerMask"></param>
    /// <param name="_hit"></param>
    /// <returns></returns>
    public static bool Picking(LayerMask _layerMask, out RaycastHit _hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 3f);
        if (Physics.Raycast(ray, out _hit, 1000f, _layerMask))
            return true;

        return false;
    }

    /// <summary>
    /// _rad �������� ������ �� ���� ������ ��ġ�� ��ȯ�ϴ� �޼���.
    /// �� �� y���� 0���� ����
    /// </summary>
    /// <param name="_rad"></param>
    /// <returns></returns>
    public static Vector3 ComputeRandomPointInCircle(float _rad)
    {
        Vector2 rnd = Random.insideUnitCircle * _rad;
        return new Vector3(rnd.x, 0f, rnd.y);
    }

    /// <summary>
    /// _outerCircle�� _innerCircle ������ ���� �� ������ ��ġ�� ��ȯ�ϴ� �޼���
    /// _outerCircleRad < _innerCircleRad�� ��� Vector3.zero ��ȯ.
    /// </summary>
    /// <param name="_outerCircleRad"></param>
    /// <param name="_innerCircleRad"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPosition(float _outerCircleRad, float _innerCircleRad)
    {
        if (_outerCircleRad < _innerCircleRad)
        {
            Debug.LogError("OuterCircleRad must bigger than InnerCircleRad");
            Debug.Break();
            return Vector3.zero;
        }

        float rndLength = Random.Range(_outerCircleRad, _innerCircleRad);
        float rndAngle = Random.Range(-180, 180);
        return new Vector3(rndLength * Mathf.Cos(rndAngle), 0f, rndLength * Mathf.Sin(rndAngle));
    }

    /// <summary>
    /// ���� ���� ���� int���� �����ϴ��� Ȯ���Ѵ�.
    /// �ּ� <= Ÿ�� < �ִ�
    /// </summary>
    /// <param name="_targetInt"></param>
    /// <param name="_minRange"></param>
    /// <param name="_maxRange"></param>
    /// <returns></returns>
    public static bool CheckRange(int _targetInt, int _minRange, int _maxRange)
    {
        if (_minRange > _maxRange)
        {
            Debug.LogError("Range Parameter Error!!");
            Debug.Break();
        }

        if (_targetInt >= _minRange)
        {
            if (_targetInt < _maxRange)
                return true;
        }
        return false;
    }

    /// <summary>
    /// ���� ���� ���� float���� �����ϴ��� Ȯ���Ѵ�.
    /// �ּ� <= Ÿ�� <= �ִ�
    /// </summary>
    /// <param name="_targetFloat"></param>
    /// <param name="_minRange"></param>
    /// <param name="_maxRange"></param>
    /// <returns></returns>
    public static bool CheckRange(float _targetFloat, float _minRange, float _maxRange)
    {
        if (_minRange > _maxRange)
        {
            Debug.LogError("Range Parameter Error!!");
            Debug.Break();
        }

        if (_targetFloat >= _minRange)
        {
            if (_targetFloat <= _maxRange)
                return true;
        }
        return false;
    }

    public static uint ClampMaxWithUInt(uint _value, uint _maxValue)
    {
        return _value < _maxValue ? _value : _maxValue;
    }

    public static RaycastHit[] OthographCameraBoxcastScreen(LayerMask _layerMask, Camera _mainCam)
    {
        Vector3 worldCamPos = _mainCam.transform.position;
        // ��ũ���� �߾� ��ǥ�� ����մϴ�.
        //Vector3 centerOfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 minBoxPos = _mainCam.ScreenToWorldPoint(new Vector3(0f, 0f, _mainCam.nearClipPlane));
        Vector3 maxBoxPos = _mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCam.nearClipPlane));
        //Vector3 worldScreenCenterPos = _mainCam.ScreenToWorldPoint(new Vector3(centerOfScreen.x, centerOfScreen.y, _mainCam.nearClipPlane));
        return Physics.BoxCastAll(worldCamPos, new Vector3(maxBoxPos.x - minBoxPos.x, maxBoxPos.y - minBoxPos.y, 0.1f), _mainCam.transform.forward, _mainCam.transform.rotation, 1000f, _layerMask);

    }
}

