using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : IState
{
    public void Start(ref SUnitState _structState)
    {
        myTr = _structState.myTr;
        targetTr = _structState.targetTr;
        attRate = _structState.attRate;
        attDmg = _structState.attDmg;
        objectType = _structState.objectType;
        effectCtrl = _structState.effectCtrl;
    }

    public void Update(ref SUnitState _structState)
    {
        if (targetTr == null) return;
        if (targetTr.gameObject.activeSelf == false) return;
        if (_structState.isPause) return;
     
        dir = targetTr.position - myTr.position;
        dir.y = 0f;
        myTr.rotation = Quaternion.LookRotation(dir);
        elapsedTime += Time.deltaTime;

        if (elapsedTime > attRate)
        {
            elapsedTime = 0f;
            // ���� �ִϸ��̼� ���

            AudioManager.instance.PlayAudio_Attack(objectType);     // ���� Audio
            effectCtrl.EffectOn(1);
            targetTr.GetComponent<IDamageable>().GetDmg(attDmg);
        }
    }

    public void End(ref SUnitState _structState)
    {
    }

    private float attDmg = 0;
    private float elapsedTime = 0f;
    private float attRate = 0f;
    private EObjectType objectType;

    private Transform targetTr = null;
    private Transform myTr = null;

    private Vector3 dir = Vector3.zero;
    private EffectController effectCtrl = null;
}
