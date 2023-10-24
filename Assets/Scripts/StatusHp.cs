using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHp : MonoBehaviour
{
    public void Init()
    {
        curHp = maxHp;
    }

    public int MaxHp => (int)maxHp;
    public float GetCurHpPercent => curHp / maxHp;

    /// <summary>
    /// ü���� 0 ���Ϸ� �������� true ��ȯ
    /// </summary>
    /// <param name="_dmg"></param>
    /// <returns></returns>
    public bool DecreaseHpAndCheckIsDead(float _dmg)
    {
        curHp -= _dmg;
        return curHp <= 0 ? true : false;
    }

    public void IncreaseCurHp(float _heal)
    {
        curHp = Mathf.Min(curHp + _heal, maxHp);
    }

    public void UpgradeHp(float _increaseHp)
    {
        maxHp += _increaseHp;
        curHp += _increaseHp;
    }

    [SerializeField]
    private float curHp = 0f;
    [SerializeField]
    private float maxHp = 100f;
}
