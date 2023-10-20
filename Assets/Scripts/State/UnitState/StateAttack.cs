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
        projectileGo = _structState.ProjectileGo;
        spawnTr = _structState.projectileSpawnTr;
        heroSpawnTr = _structState.heroProjectileSpawnTr;
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

            AudioManager.instance.PlayAudio_Attack(objectType);     // Attack Audio
            effectCtrl.EffectOn(1);

            if (spawnTr)
            {
                GameObject missile = GameObject.Instantiate(projectileGo, spawnTr.position, spawnTr.rotation);
                missile.GetComponent<ProjectileBase>().Init(targetTr.position);
            }
            else
            {
                GameObject missile1 = GameObject.Instantiate(projectileGo, heroSpawnTr[0].position, heroSpawnTr[0].rotation);
                GameObject missile2 = GameObject.Instantiate(projectileGo, heroSpawnTr[1].position, heroSpawnTr[1].rotation);
                missile1.GetComponent<ProjectileBase>().Init(targetTr.position);
                missile2.GetComponent<ProjectileBase>().Init(targetTr.position);
            }

            if (objectType.Equals(EObjectType.ENEMY_BIG)) return;

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
    private Transform spawnTr = null;
    private Transform[] heroSpawnTr = null;
    private Vector3 dir = Vector3.zero;
    private EffectController effectCtrl = null;

    private GameObject projectileGo = null;
}
