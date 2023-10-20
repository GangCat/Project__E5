using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IPauseObserver
{
    public void Init(PF_Grid _grid, Vector3 _mainBasePos)
    {
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        grid = _grid;
        mainBasePos = _mainBasePos;

        waveEnemyHolder = GetComponentInChildren<WaveEnemyHolder>().GetTransform();
        mapEnemyHolder = GetComponentInChildren<MapEnemyHolder>().GetTransform();

        memoryPoolWave = new MemoryPool(enemySmallPrefab, 5, waveEnemyHolder);
        memoryPoolMap = new MemoryPool(enemySmallPrefab, 5, mapEnemyHolder);
        memoryPoolEnemyDeadEffect = new MemoryPool(enemyDeadEffect, 5, transform);

        ArrayHUDCommand.Use(EHUDCommand.INIT_WAVE_TIME, bigWaveDelay_sec);
        SpawnMapEnemy();
        StartCoroutine("WaveControll");
        
    }

    public void StartBigWaveCheat()
    {
        bigWaveTimeDelay = bigWaveDelay_sec;
    }

    private IEnumerator WaveControll()
    {
        bigWaveCnt = 0;
        bigWaveTimeDelay = 0f;
        smallWaveTimeDelay = 0f;

        while (bigWaveCnt < totalBigWaveCnt)
        {
            while (bigWaveTimeDelay <= bigWaveDelay_sec)
            {
                while (isPause)
                    yield return null;

                ArrayHUDCommand.Use(EHUDCommand.UPDATE_WAVE_TIME, bigWaveDelay_sec - bigWaveTimeDelay);
                if (smallWaveCnt < 2 && smallWaveTimeDelay >= smallWaveDelay_sec)
                {
                    SpawnWaveEnemy(arrWaveStartPoint[bigWaveCnt].GetPos, 20 + bigWaveCnt * 10);
                    smallWaveTimeDelay = 0f;

                    ++smallWaveCnt;
                }

                yield return new WaitForSeconds(1f);
                bigWaveTimeDelay += 1f;
                smallWaveTimeDelay += 1f;
            }

            ++bigWaveCnt;

            if (bigWaveCnt.Equals(totalBigWaveCnt))
            {
                FinalWaveStart();
                yield break;
            }

            for (int i = 0; i < bigWaveCnt; ++i)
            {
                SpawnWaveEnemy(arrWaveStartPoint[i].GetPos, bigWaveCnt * 100);
                bigWaveTimeDelay = 0f;
                smallWaveTimeDelay = 0f;
                smallWaveCnt = 0;

                //GameObject bigGo = Instantiate(enemyBigPrefab, arrWaveStartPoint[i].GetPos, Quaternion.identity);
                //EnemyObject enemyObj = bigGo.GetComponent<EnemyObject>();
                //enemyObj.Init();
                //enemyObj.Init(EnemyObject.EEnemySpawnType.WAVE_SPAWN, waveEnemyIdx);
                //enemyObj.MoveAttack(mainBasePos);
            }
        }

    }

    private void FinalWaveStart()
    {
        AudioManager.instance.StopAudio_BGM_WithFade(3.0f);
        
        AudioManager.instance.PlayAudio_WaveBGM_WithDelay(5.0f);
        
        ArrayHUDCommand.Use(EHUDCommand.UPDATE_WAVE_TIME, 0f);
        for (int i = 0; i < totalBigWaveCnt; ++i)
        {
            SpawnWaveEnemy(arrWaveStartPoint[i].GetPos, totalBigWaveCnt * 100);

            //GameObject bigGo = Instantiate(enemyBigPrefab, arrWaveStartPoint[i].GetPos, Quaternion.identity);
            //EnemyObject enemyObj = bigGo.GetComponent<EnemyObject>();
            //enemyObj.Init();
            //enemyObj.Init(EnemyObject.EEnemySpawnType.WAVE_SPAWN, waveEnemyIdx);
            //enemyObj.MoveAttack(mainBasePos);
        }

        EnemyObject[] arrAllMapEnemy = mapEnemyHolder.GetComponentsInChildren<EnemyObject>();
        for (int i = 0; i < arrAllMapEnemy.Length; ++i)
            arrAllMapEnemy[i].MoveAttack(mainBasePos);
    }

    public void SpawnWaveEnemy(Vector3 _targetPos, int _count)
    {
        StartCoroutine(SpawnWaveEnemyCoroutine(_targetPos, _count));
    }

    private void SpawnMapEnemy()
    {
        StartCoroutine("SpawnMapEnemyCoroutine");
    }

    public void DeactivateWaveEnemy(GameObject _removeGo, int _waveEnemyIdx)
    {
        DisplayEnemyDeadEffect(_removeGo.transform.position);
        memoryPoolWave.DeactivatePoolItemWithIdx(_removeGo, _waveEnemyIdx);
    }

    public void DeactivateMapEnemy(GameObject _removeGo, int _mapEnemyIdx)
    {
        DisplayEnemyDeadEffect(_removeGo.transform.position);
        memoryPoolMap.DeactivatePoolItemWithIdx(_removeGo, _mapEnemyIdx);
    }

    private void DisplayEnemyDeadEffect(Vector3 _enemyPos)
    {
        GameObject effectGo = memoryPoolEnemyDeadEffect.ActivatePoolItem(5, transform);
        effectGo.GetComponent<EffectUnitDead>().Init(new Vector3(_enemyPos.x, _enemyPos.y + 1f, _enemyPos.z), DeactivateEffect);
    }

    private void DeactivateEffect(Transform _tr)
    {
        memoryPoolEnemyDeadEffect.DeactivatePoolItem(_tr.gameObject);
    }

    private IEnumerator SpawnWaveEnemyCoroutine(Vector3 _spawnPos, int _count)
    {
        int unitCnt = 0;
        while (unitCnt < _count)
        {
            Vector3 spawnPos = _spawnPos + Functions.GetRandomPosition(WaveOuterCircleRad, WaveInnerCircleRad);
            PF_Node spawnNode = grid.GetNodeFromWorldPoint(spawnPos);
            if (!spawnNode.walkable)
                continue;

            GameObject enemyGo = memoryPoolWave.ActivatePoolItemWithIdx(waveEnemyIdx, 5, waveEnemyHolder);
            EnemyObject enemyObj = enemyGo.GetComponent<EnemyObject>();
            enemyObj.Position = spawnPos;
            enemyObj.Init();
            enemyObj.Init(EnemyObject.EEnemySpawnType.WAVE_SPAWN, waveEnemyIdx);
            enemyObj.MoveAttack(mainBasePos);
            ++waveEnemyIdx;
            ++unitCnt;
            yield return null;
        }
    }

    private IEnumerator SpawnMapEnemyCoroutine()
    {
        for (int i = 0; i < arrMapSpawnPoint.Length; ++i)
        {
            int unitCnt = 0;
            while (unitCnt < mapSpawnCnt)
            {
                Vector3 spawnPos = arrMapSpawnPoint[i].GetPos + Functions.GetRandomPosition(outerCircleRad, innerCircleRad);
                PF_Node spawnNode = grid.GetNodeFromWorldPoint(spawnPos);
                if (!spawnNode.walkable)
                    continue;

                EnemyObject enemyObj = memoryPoolMap.ActivatePoolItemWithIdx(mapEnemyIdx, 5, mapEnemyHolder).GetComponent<EnemyObject>();
                enemyObj.Position = spawnNode.worldPos;
                enemyObj.Rotate(Random.Range(0, 360));
                enemyObj.Init();
                enemyObj.Init(EnemyObject.EEnemySpawnType.MAP_SPAWN, mapEnemyIdx);
                ++mapEnemyIdx;
                ++unitCnt;
            }
            yield return null;
        }
    }

    public void CheckPause(bool _isPause)
    {
        isPause = _isPause;
    }

    [SerializeField]
    private GameObject enemySmallPrefab = null;
    [SerializeField]
    private GameObject enemyBigPrefab = null;
    [SerializeField]
    private EnemyMapSpawnPoint[] arrMapSpawnPoint = null;
    [SerializeField]
    private GameObject enemyDeadEffect = null;

    [Header("-Enemy Map Random Spawn(outer > inner)")]
    [SerializeField]
    private float outerCircleRad = 0f;
    [SerializeField]
    private float innerCircleRad = 0f;
    [SerializeField]
    private int mapSpawnCnt = 0;

    [Header("-Wave Attribute(outer > inner)")]
    [SerializeField]
    private float smallWaveDelay_sec = 3;
    [SerializeField]
    private float bigWaveDelay_sec = 10;
    [SerializeField]
    private int totalBigWaveCnt = 3;
    [SerializeField]
    private WaveStartPoint[] arrWaveStartPoint = null;
    [SerializeField]
    private float WaveOuterCircleRad = 0f;
    [SerializeField]
    private float WaveInnerCircleRad = 0f;


    private MemoryPool memoryPoolWave = null;
    private MemoryPool memoryPoolMap = null;

    private Transform waveEnemyHolder = null;
    private Transform mapEnemyHolder = null;

    private Vector3 mainBasePos = Vector3.zero;

    private int waveEnemyIdx = 0;
    private int mapEnemyIdx = 0;
    private int smallWaveCnt = 0;

    private bool isBigWaveTurn = false;
    private bool isPause = false;
    private int bigWaveCnt = 0;
    private float bigWaveTimeDelay = 0f;
    private float smallWaveTimeDelay = 0f;

    private PF_Grid grid = null;

    private MemoryPool memoryPoolEnemyDeadEffect = null;

    private AudioManager audioMng = null;
    // private AudioPlayer_BGM audioType;
    
}
