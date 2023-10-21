using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : SelectableObject
{
    [System.Serializable]
    public enum EEnemySpawnType { NONE = -1, WAVE_SPAWN, MAP_SPAWN, LENGTH }

    public void Init(EEnemySpawnType _spawnType, int _myIdx)
    {
        spawnType = _spawnType;
        myIdx = _myIdx;
        gameObject.layer = LayerMask.NameToLayer("EnemySelectableObject");
    }

    public override void UpdateCurNode()
    {
        SelectableObjectManager.UpdateEnemyNodeWalkable(transform.position, nodeIdx);
    }

    public override void GetDmg(float _dmg)
    {
        if (statusHp.DecreaseHpAndCheckIsDead(_dmg))
        {
            StopAllCoroutines();
            DeactivateUnit();
        }
        else
            effectCtrl.EffectOn(0);
        //else if (isSelect)
        //{
        //    SelectableObjectManager.UpdateHp(listIdx);
        //}
    }

    private void DeactivateUnit()
    {
        // Enemy Dead Audio
        AudioManager.instance.PlayAudio_Destroy(objectType);
        ArrayPauseCommand.Use(EPauseCommand.REMOVE, this);

        if (objectType.Equals(EObjectType.ENEMY_BIG))
        {
            for (int i = 0; i < 15; ++i)
            {
                Instantiate(powerCorePrefab, transform.position + Functions.GetRandomPosition(3, 0), Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else
        {
            SelectableObjectManager.ResetEnemyNodeWalkable(transform.position, nodeIdx);

            ArrayEnemyObjectCommand.Use((EEnemyObjectCommand)spawnType, gameObject, myIdx);
            if (Random.Range(0.0f, 100.0f) < 30f)
                Instantiate(powerCorePrefab, transform.position, Quaternion.identity);

        }
    }

    protected override void RequestPath(Vector3 _startPos, Vector3 _endPos)
    {
        PF_PathRequestManager.EnemyRequestPath(_startPos, _endPos, OnPathFound);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetEffectPosition()
    {
        Vector3 offset = new Vector3(0, 1, 0);
        return transform.position + offset;
    }
    
    
    [SerializeField]
    private GameObject powerCorePrefab = null;

    private EEnemySpawnType spawnType = EEnemySpawnType.NONE;
    private int myIdx = 0;
}
