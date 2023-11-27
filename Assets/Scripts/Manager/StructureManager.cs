using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public enum EStructureType { NONE = -1, TURRET, BUNKER, BARRACK, NUCLEAR, WALL, LENGTH }
    public void Init(PF_Grid _grid, StructureMainBase _mainBase)
    {
        grid = _grid;
        dicStructure = new Dictionary<int, Structure>();
        listNuclearStructure = new List<StructureNuclear>();
        memorypoolDestroyEffect = new MemoryPool(destroyEffect, 5, transform);
        dicStructure.Add(0, _mainBase);
        ++structureIdx;
    }

    public static int UpgradeLimit
    {
        get => upgradeLimit;
        set
        {
            if (value < 3)
                upgradeLimit = value;
            else
                upgradeLimit = 3;
        }
    }

    // �Ǽ��ӵ� �� ������ �ϱ����� ġƮ
    public void SetDelayFast()
    {
        for (int i = 0; i < buildDelay.Length; ++i)
            buildDelay[i] = 1f;

        for (int i = 0; i < arrSpawnUnitDelay.Length; ++i)
            arrSpawnUnitDelay[i] = 1f;

        nuclearSpawnDelay = 1f;
        upgradeDelay = 1f;

        foreach (Structure structure in dicStructure.Values)
        {
            structure.SetNuclearSpawnDelay(nuclearSpawnDelay);
            structure.SetUnitSpawnDelay(arrSpawnUnitDelay);
            structure.SetUpgradeDelay(upgradeDelay);
        }
    }

    public EObjectType CurStructureType => curStructureObjType;

    public bool CanBuildNuclear => upgradeLimit >= 3;

    #region �Ǽ� ���� �Լ�
    // û���� ����ִ� �Լ�.
    public void ShowBluepirnt(EObjectType _structureType)
    {
        // ������ �̹� û������ ��� ���¸� ���� û���� ����.
        if (isBlueprint)
        {
            Destroy(curStructure.gameObject);
            StopAllCoroutines();
        }

        // �Ű������� ���� Ÿ�Կ� ���� û���� ����
        switch (_structureType)
        {
            case EObjectType.TURRET:
                {
                    curStructureType = EStructureType.TURRET;
                    curStructureObjType = EObjectType.TURRET;
                    curStructure = Instantiate(arrBlueprintPrefab[(int)EStructureType.TURRET], transform).GetComponent<Structure>();
                }
                break;
            case EObjectType.BUNKER:
                {
                    curStructureType = EStructureType.BUNKER;
                    curStructureObjType = EObjectType.BUNKER;
                    curStructure = Instantiate(arrBlueprintPrefab[(int)EStructureType.BUNKER], transform).GetComponent<Structure>();
                }
                break;
            case EObjectType.NUCLEAR:
                {
                    curStructureType = EStructureType.NUCLEAR;
                    curStructureObjType = EObjectType.NUCLEAR;
                    curStructure = Instantiate(arrBlueprintPrefab[(int)EStructureType.NUCLEAR], transform).GetComponent<Structure>();
                }
                break;
            case EObjectType.BARRACK:
                {
                    curStructureType = EStructureType.BARRACK;
                    curStructureObjType = EObjectType.BARRACK;
                    curStructure = Instantiate(arrBlueprintPrefab[(int)EStructureType.BARRACK], transform).GetComponent<Structure>();
                }
                break;
            default:
                break;
        }

        StartCoroutine("ShowBlueprint");
    }

    // ���� û������ �����ϴ� �Լ�.
    public void ShowBluepirnt(Transform _bunkerTr)
    {
        curStructureType = EStructureType.WALL;
        curStructureObjType = EObjectType.WALL;
        curStructure = Instantiate(arrBlueprintPrefab[(int)EStructureType.WALL], transform).GetComponent<Structure>();
        StartCoroutine("ShowWallBlueprint", _bunkerTr);
    }


    // û������ ���콺�� ����ٴϵ��� �ϴ� �ڷ�ƾ
    private IEnumerator ShowBlueprint()
    {
        isBlueprint = true;

        if (curStructure == null) yield break;

        curStructure.Init(grid);

        RaycastHit hit;
        while (true)
        {
            Functions.Picking(1 << LayerMask.NameToLayer("StageFloor"), out hit);
            curNode = grid.GetNodeFromWorldPoint(hit.point);
            curStructure.SetPos(curNode.worldPos);

            yield return null;
        }
    }

    // ���� ��� ���Ǵ� û���� ǥ�� �ڷ�ƾ
    private IEnumerator ShowWallBlueprint(Transform _bunkerTr)
    {
        isBlueprint = true;

        if (curStructure == null) yield break;

        curStructure.Init(grid);
        Vector3 bunkerPos = _bunkerTr.position;
        Vector3 wallPos = Vector3.zero;
        float angle = 0f;
        int xLength = curStructure.GridX;
        int yLength = curStructure.GridY;

        RaycastHit hit;
        while (true)
        {
            Functions.Picking(1 << LayerMask.NameToLayer("StageFloor"), out hit);
            angle = Functions.CalcAngleToTarget(bunkerPos, hit.point);
            wallPos = bunkerPos;

            // ������ �°� ȸ����Ű��
            // left -135 ~ 135
            if (angle > 135 || angle < -135)
            {
                curStructure.SetGrid(xLength, yLength);
                curStructure.SetFactor(-1, -1);
                curStructure.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                wallPos.x = bunkerPos.x - 0.5f;
                wallPos.z = bunkerPos.z + 0.5f;
                curStructure.SetPos(wallPos);
            }
            // up 135 ~ 45
            else if (angle > 45)
            {
                curStructure.SetGrid(yLength, xLength);
                curStructure.SetFactor(-1, 1);
                curStructure.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                wallPos.x = bunkerPos.x + 0.5f;
                wallPos.z = bunkerPos.z + 1;
                curStructure.SetPos(wallPos);
            }
            // right 45 ~ -45
            else if (angle > -45)
            {
                curStructure.SetGrid(xLength, yLength);
                curStructure.SetFactor(1, 1);
                curStructure.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                wallPos.x = bunkerPos.x + 1;
                curStructure.SetPos(wallPos);
            }
            // down -45 ~ -135
            else
            {
                curStructure.SetGrid(yLength, xLength);
                curStructure.SetFactor(1, -1);
                curStructure.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                wallPos.z = bunkerPos.z - 0.5f;
                curStructure.SetPos(wallPos);
            }

            yield return null;
        }
    }

    // û������ ����� ���¿��� ����� ��� ȣ��
    public bool CancleBuild()
    {
        StopAllCoroutines();
        curStructure.BuildCancle();
        Destroy(curStructure.gameObject);
        isBlueprint = false;
        return false;
    }

    // ���� �ǹ��� �Ǽ��ϴ� �Լ�.
    public bool BuildStructure()
    {
        if (!curStructure.IsBuildable)
            return false;

        StopBuildCoroutine();

        Structure newStructure = Instantiate(arrStructurePrefab[(int)curStructureType], curStructure.transform.position, curStructure.transform.rotation).GetComponent<Structure>();
        newStructure.Init(grid);
        newStructure.Init(structureIdx);
        newStructure.SetUpgradeDelay(upgradeDelay);
        dicStructure.Add(structureIdx, newStructure);
        ++structureIdx;
        switch (curStructureType)
        {
            case EStructureType.BARRACK:
                newStructure.SetUnitSpawnDelay(arrSpawnUnitDelay);
                break;
            case EStructureType.NUCLEAR:
                newStructure.SetNuclearSpawnDelay(nuclearSpawnDelay);
                break;
            case EStructureType.WALL:
                newStructure.SetGrid(curStructure.GridX, curStructure.GridY);
                newStructure.SetFactor(curStructure.FactorX, curStructure.FactorY);
                break;
            default:
                break;
        }

        Destroy(curStructure.gameObject);
        newStructure.transform.parent = transform;
        newStructure.BuildStart(buildDelay[(int)curStructureType]);
        isBlueprint = false;

        return true;
    }

    // �Ǽ� ������ ��� ȣ��Ǵ� �Լ�.
    private void StopBuildCoroutine()
    {
        StopCoroutine("ShowWallBlueprint");
        StopCoroutine("ShowBlueprint");
    }
    #endregion

    #region �ǹ��ı� ����
    // �ǹ��� �ı��� �� ȣ��Ǵ� �Լ�.
    public void DestroyStructure(int _structureIdx, StructureNuclear _nuclear = null)
    {
        if (_structureIdx.Equals(0)) return;

        Structure structure = null;
        if (!dicStructure.TryGetValue(_structureIdx, out structure)) return;

        if (_nuclear != null)
            listNuclearStructure.Remove(_nuclear);

        dicStructure.Remove(_structureIdx);
        GameObject effectGo = memorypoolDestroyEffect.ActivatePoolItem(5, transform);
        effectGo.GetComponent<EffectStructureDestroy>().Init(structure.transform.position, DeactivateEffectDestroy);
        structure.DestroyStructure();
        InstantiateRuin(structure);
    }

    public void DeactivateEffectDestroy(Transform _tr)
    {
        memorypoolDestroyEffect.DeactivatePoolItem(_tr.gameObject);
    }
    #endregion

    #region Ruin
    private void InstantiateRuin(Structure _structure)
    {
        StructureCollider[] arrCol = _structure.GetComponentsInChildren<StructureCollider>();

        for (int i = 0; i < arrCol.Length; ++i)
        {
            GameObject ruinGo = Instantiate(ruinPrefab, arrCol[i].transform.position, Quaternion.identity);
            ruinGo.GetComponent<Structure>().Init();
        }
    }
    #endregion

    #region �ٰǹ� ����
    public void SpawnNuclear(int _structureIdx)
    {
        Structure nuclearStructure = null;
        dicStructure.TryGetValue(_structureIdx, out nuclearStructure);

        if (nuclearStructure != null)
        {
            StructureNuclear nuclear = nuclearStructure.GetComponent<StructureNuclear>();
            nuclear.SpawnNuclear(SpawnNuclearComplete);
        }
    }

    private void SpawnNuclearComplete(StructureNuclear _nuclear)
    {
        listNuclearStructure.Add(_nuclear);
    }

    public void RemoveNuclearAtList(StructureNuclear _removeNuclear)
    {
        listNuclearStructure.Remove(_removeNuclear);
    }

    public void LaunchNuclear(Vector3 _destPos)
    {
        if (listNuclearStructure.Count > 0)
        {
            listNuclearStructure[0].LaunchNuclear(_destPos);
            listNuclearStructure.RemoveAt(0);
        }
    }
    #endregion

    public bool UpgradeStructure(int _structureIdx)
    {
        dicStructure.TryGetValue(_structureIdx, out var structure);
        return structure.StartUpgrade();
    }


    [Header("-StructurePrefab(TURRET, BUNKER, BARRACK, NUCLEAR, WALL)")]
    [SerializeField]
    private GameObject[] arrStructurePrefab = null;

    [Header("-BlueprintPrefab(TURRET, BUNKER, BARRACK, NUCLEAR, WALL)")]
    [SerializeField]
    private GameObject[] arrBlueprintPrefab = null;

    [Header("-OtherPrefab")]
    [SerializeField]
    private GameObject ruinPrefab = null;

    [Header("-Build Delay(TURRET, BUNKER, BARRACK, NUCLEAR, WALL)")]
    [SerializeField]
    private float[] buildDelay = new float[(int)EStructureType.LENGTH];

    [Header("-Unit Spawn Delay(MELEE, RANGED)")]
    [SerializeField]
    private float[] arrSpawnUnitDelay = null;

    [Header("-Nuclear Spawn Delay")]
    [SerializeField]
    private float nuclearSpawnDelay = 0f;

    [Header("-Upgrade Delay")]
    [SerializeField]
    private float upgradeDelay = 0f;

    [Header("-ETC")]
    [SerializeField]
    private GameObject destroyEffect = null;

    private Dictionary<int, Structure> dicStructure = null;
    private List<StructureNuclear> listNuclearStructure = null;
    private Structure curStructure = null;
    private EObjectType curStructureObjType = EObjectType.NONE;
    private EStructureType curStructureType = EStructureType.NONE;
    private PF_Grid grid = null;
    private PF_Node curNode = null;

    private bool isBlueprint = false;
    private int structureIdx = 0;

    private static int upgradeLimit = 1;

    private MemoryPool memorypoolDestroyEffect = null;

}
