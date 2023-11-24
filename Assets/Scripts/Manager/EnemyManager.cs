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
        waitDotZeroFiveSecond = new WaitForSeconds(0.05f);
        waitOneSecond = new WaitForSeconds(1f);

        waveEnemyHolder = GetComponentInChildren<WaveEnemyHolder>().GetTransform();
        mapEnemyHolder = GetComponentInChildren<MapEnemyHolder>().GetTransform();

        memoryPoolWave = new MemoryPool(enemySmallPrefab, 5, waveEnemyHolder);
        memoryPoolMap = new MemoryPool(enemySmallPrefab, 5, mapEnemyHolder);
        memoryPoolEnemyDeadEffect = new MemoryPool(enemyDeadEffect, 5, transform);
        arrBigEnemyTr = new Transform[3];

        ArrayHUDCommand.Use(EHUDCommand.INIT_WAVE_TIME, bigWaveDelay_sec);
        SpawnMapEnemy();
        StartCoroutine("WaveControl");
        
    }

    public void StartBigWaveCheat()
    {
        bigWaveTimeDelay = bigWaveDelay_sec;
    }

    public void SpawnWaveEnemy(Vector3 _targetPos, int _count)
    {
        StartCoroutine(SpawnWaveEnemyCoroutine(_targetPos, _count));
    }

    // 적 유닛이 사망할 경우 이펙트 출력 후 비활성화, Wave
    public void DeactivateWaveEnemy(GameObject _removeGo, int _waveEnemyIdx)
    {
        DisplayEnemyDeadEffect(_removeGo.transform.position);
        memoryPoolWave.DeactivatePoolItemWithIdx(_removeGo, _waveEnemyIdx);
    }

    // 적 유닛이 사망할 경우 이펙트 출력 후 비활성화, Map
    public void DeactivateMapEnemy(GameObject _removeGo, int _mapEnemyIdx)
    {
        DisplayEnemyDeadEffect(_removeGo.transform.position);
        memoryPoolMap.DeactivatePoolItemWithIdx(_removeGo, _mapEnemyIdx);
    }

    public void CheckPause(bool _isPause)
    {
        isPause = _isPause;
    }


    private IEnumerator WaveControl()
    {
        bigWaveCnt = 0;
        bigWaveTimeDelay = 0f;
        smallWaveTimeDelay = 0f;

        while (bigWaveCnt < totalBigWaveCnt)
        {
            // 빅 웨이브 시간되기 전까지 반복할 코루틴
            while (bigWaveTimeDelay <= bigWaveDelay_sec)
            {
                // 일시정지일 경우 대기
                while (isPause)
                    yield return null;

                ArrayHUDCommand.Use(EHUDCommand.UPDATE_WAVE_TIME, bigWaveDelay_sec - bigWaveTimeDelay);
                SpawnSmallWave();

                yield return waitOneSecond;
                bigWaveTimeDelay += 1f;
                smallWaveTimeDelay += 1f;
            }

            // 빅 웨이브 시작
            ++bigWaveCnt;
            // 웨이브 사운드 출력
            ArrayAlertCommand.Use(EAlertCommand.WAVE_START);

            // 마지막 웨이브일 경우 코루틴 탈출
            if (bigWaveCnt.Equals(totalBigWaveCnt))
            {
                FinalWaveStart();
                yield break;
            }

            SpawnBigWave();
            StartCoroutine("UpdateEnemyCntCoroutine");
            ArrayHUDMinimapCommand.Use(EHUDMinimapCommand.BIG_ENEMY_SIGNAL, arrBigEnemyTr);
        }
    }

    private void SpawnSmallWave()
    {
        if (smallWaveCnt < 2 && smallWaveTimeDelay >= smallWaveDelay_sec)
        {
            SpawnWaveEnemy(arrWaveStartPoint[bigWaveCnt].GetPos, 20 + bigWaveCnt * 10);
            smallWaveTimeDelay = 0f;

            ++smallWaveCnt;
        }
    }

    private void SpawnBigWave()
    {
        for (int i = 0; i < bigWaveCnt; ++i)
        {
            SpawnWaveEnemy(arrWaveStartPoint[i].GetPos, /*bigWaveCnt * */100);
            bigWaveTimeDelay = 0f;
            smallWaveTimeDelay = 0f;
            smallWaveCnt = 0;


            GameObject bigEnemyGo = Instantiate(enemyBigPrefab, arrWaveStartPoint[i].GetPos, Quaternion.identity);
            EnemyObject enemyObj = bigEnemyGo.GetComponent<EnemyObject>();
            enemyObj.Init();
            enemyObj.Init(EnemyObject.EEnemySpawnType.WAVE_SPAWN, waveEnemyIdx, mainBasePos);
            enemyObj.MoveAttack(mainBasePos);
            arrBigEnemyTr[i] = bigEnemyGo.transform;
        }
    }

    /// <summary>
    /// 빅 웨이브 남은 적 수를 알려주기 위한 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateEnemyCntCoroutine()
    {
        ArrayHUDCommand.Use(EHUDCommand.INIT_REMAIN_ENEMY_CNT, memoryPoolWave.ActiveCnt + arrBigEnemyTr.Length);

        yield return waitOneSecond;

        while (memoryPoolWave.ActiveCnt > 1)
        {
            ArrayHUDCommand.Use(EHUDCommand.UPDATE_REMAIN_ENEMY_CNT, memoryPoolWave.ActiveCnt + arrBigEnemyTr.Length);
            yield return waitOneSecond;
        }

        ArrayHUDCommand.Use(EHUDCommand.WAVE_FINISH);
    }

    /// <summary>
    /// 최종 웨이브 시작, 맵에 있는 적들도 모두 아군 기지로 몰려옴
    /// </summary>
    private void FinalWaveStart()
    {
        AudioManager.instance.StopAudio_BGM_WithFade(3.0f);
        
        AudioManager.instance.PlayAudio_WaveBGM_WithDelay(5.0f);
        
        ArrayHUDCommand.Use(EHUDCommand.UPDATE_WAVE_TIME, 0f);
        for (int i = 0; i < totalBigWaveCnt; ++i)
        {
            SpawnWaveEnemy(arrWaveStartPoint[i].GetPos, /*totalBigWaveCnt * */100);

            GameObject bigGo = Instantiate(enemyBigPrefab, arrWaveStartPoint[i].GetPos, Quaternion.identity);
            EnemyObject enemyObj = bigGo.GetComponent<EnemyObject>();
            enemyObj.Init();
            enemyObj.Init(EnemyObject.EEnemySpawnType.WAVE_SPAWN, waveEnemyIdx, mainBasePos);
            enemyObj.MoveAttack(mainBasePos);
            arrBigEnemyTr[i] = bigGo.transform;
        }

        arrAllMapEnemy = mapEnemyHolder.GetComponentsInChildren<EnemyObject>();
        for (int i = 0; i < arrAllMapEnemy.Length; ++i)
            arrAllMapEnemy[i].MoveAttack(mainBasePos);

        ArrayHUDMinimapCommand.Use(EHUDMinimapCommand.BIG_ENEMY_SIGNAL, arrBigEnemyTr);

        StartCoroutine("CheckIsGameClearCoroutine");
    }

    /// <summary>
    /// 메모리 풀에 한 마리도 활성화된 적이 남아있지 않으면 승리
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckIsGameClearCoroutine()
    {
        ArrayHUDCommand.Use(EHUDCommand.INIT_REMAIN_ENEMY_CNT, memoryPoolWave.ActiveCnt + memoryPoolMap.ActiveCnt + arrBigEnemyTr.Length);
        yield return waitOneSecond;

        while (true)
        {
            ArrayHUDCommand.Use(EHUDCommand.UPDATE_REMAIN_ENEMY_CNT, memoryPoolWave.ActiveCnt + memoryPoolMap.ActiveCnt + arrBigEnemyTr.Length);

            if (memoryPoolMap.ActiveCnt < 1 && memoryPoolWave.ActiveCnt < 1)
            {
                UIManager.GameClear();
                yield break;
            }

            yield return waitOneSecond;
        }
    }


    private void SpawnMapEnemy()
    {
        StartCoroutine("SpawnMapEnemyCoroutine");
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
                enemyObj.Init(EnemyObject.EEnemySpawnType.MAP_SPAWN, mapEnemyIdx, mainBasePos);
                ++mapEnemyIdx;
                ++unitCnt;
            }
            yield return waitDotZeroFiveSecond;
        }
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

        // 한마리씩 소환하게 해서 부하를 줄임.
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
            enemyObj.Init(EnemyObject.EEnemySpawnType.WAVE_SPAWN, waveEnemyIdx, mainBasePos);
            enemyObj.MoveAttack(mainBasePos);
            ++waveEnemyIdx;
            ++unitCnt;
            yield return waitDotZeroFiveSecond;
        }
    }

   

 

    [Header("-Information")]
    [SerializeField]
    private GameObject enemySmallPrefab = null;
    [SerializeField]
    private GameObject enemyBigPrefab = null;
    [SerializeField]
    private EnemyMapSpawnPoint[] arrMapSpawnPoint = null;
    [SerializeField]
    private GameObject enemyDeadEffect = null;
    [SerializeField]
    private Transform wayPoint = null;


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

    private bool isPause = false;
    private int bigWaveCnt = 0;
    private float bigWaveTimeDelay = 0f;
    private float smallWaveTimeDelay = 0f;

    private PF_Grid grid = null;

    private MemoryPool memoryPoolEnemyDeadEffect = null;
    private Transform[] arrBigEnemyTr = null;
    private WaitForSeconds waitOneSecond = null;
    private WaitForSeconds waitDotZeroFiveSecond = null;
    private EnemyObject[] arrAllMapEnemy = null;
}
