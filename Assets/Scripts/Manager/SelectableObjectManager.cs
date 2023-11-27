using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableObjectManager : MonoBehaviour, IPublisher
{
    public void Init(VoidSelectObjectTypeDelegate _selectObjectCallback, PF_Grid _grid)
    {
        // 현재 선택한 오브젝트가 무엇인지 반환하는 콜백
        selectObjectCallback = _selectObjectCallback;
        grid = _grid;
        RegisterBroker();

        dicNodeUnderFriendlyUnit = new Dictionary<int, PF_Node>();
        dicNodeUnderEnemyUnit = new Dictionary<int, PF_Node>();
        tempListSelectableObject = new List<SelectableObject>();
        listSelectedFriendlyObject = new List<FriendlyObject>();
        unitInfoContainer = new UnitInfoContainer();
        listFriendlyUnitInfo = new List<SFriendlyUnitInfo>(12);

        deadEffectMemoryPool = new MemoryPool(unitDeadEffect, 5, transform);
        arrMemoryPool = new MemoryPool[arrUnitPrefab.Length];

        for (int i = 0; i < listFriendlyUnitInfo.Capacity; ++i)
            listFriendlyUnitInfo.Add(new SFriendlyUnitInfo());
        for (int i = 0; i < arrUnitPrefab.Length; ++i)
            arrMemoryPool[i] = new MemoryPool(arrUnitPrefab[i], 5, transform);

        arrCrowd = new List<FriendlyObject>[9];
        for (int i = 0; i < arrCrowd.Length; ++i)
            arrCrowd[i] = new List<FriendlyObject>();

        ArrayHUDCommand.Use(EHUDCommand.INIT_DISPLAY_GROUP_INFO, listFriendlyUnitInfo);
        ArrayHUDCommand.Use(EHUDCommand.INIT_DISPLAY_SINGLE_INFO, unitInfoContainer);

        StartCoroutine("CheckNodeBuildableCoroutine");
    }

    /// <summary>
    /// 매 주기마다 현재 생성되어있는 모든 유닛들의 주위를 검사해 그 주위를 건설가능구역으로 변경해주는 함수.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckNodeBuildableCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (true)
        {
            ArrayCheckNodeBuildableCommand.Use(ECheckNodeBuildable.CHECK_NODE_BUILDABLE_UNIT, dicNodeUnderFriendlyUnit.Values.ToArray<PF_Node>());

            yield return wait;
        }
    }

    public delegate void VoidSelectObjectTypeDelegate(EObjectType _objectType);
    public static bool IsFriendlyUnit => isFriendlyUnitInList;
    public static bool IsEnemyUnitInList => isEnemyObjectInList;
    public static bool IsListFull => listSelectedFriendlyObject.Count > 11;
    public static bool IsListEmpty => listSelectedFriendlyObject.Count < 1;
    public static int LevelRangedUnitDmgUpgrade => levelRangedUnitDmgUpgrade;
    public static int LevelRangedUnitHpUpgrade => levelRangedUnitHpUpgrade;
    public static int LevelMeleeUnitHpUpgrade => levelMeleeUnitHpUpgrade;
    public static int LevelMeleeUnitDmgUpgrade => levelMeleeUnitDmgUpgrade;
    public static float DelayUnitUpgrade => delayUnitUpgrade;
    public static Dictionary<int, PF_Node> DicNodeUnderFriendlyUnit => dicNodeUnderFriendlyUnit;
    public static Dictionary<int, PF_Node> DicNodeUnderEnemyUnit => dicNodeUnderEnemyUnit;

    public static FriendlyObject GetFirstSelectedObjectInList()
    {
        if (listSelectedFriendlyObject.Count > 0)
            return listSelectedFriendlyObject[0];
        else
            return null;
    }
    #region 유닛 노드 인덱스 초기화
    // 아군 유닛들의 노드 인덱스를 지정해주는 함수.
    public static void InitNodeFriendly(Vector3 _pos, out int _idx)
    {
        dicNodeUnderFriendlyUnit.Add(dicFriendlyIdx, grid.GetNodeFromWorldPoint(_pos));
        _idx = dicFriendlyIdx;
        ++dicFriendlyIdx;
    }
    public static void InitNodeEnemy(Vector3 _pos, out int _idx)
    {
        dicNodeUnderEnemyUnit.Add(dicEnemyIdx, grid.GetNodeFromWorldPoint(_pos));
        _idx = dicEnemyIdx;
        ++dicEnemyIdx;
    }
    #endregion

    #region 유닛 노드 워커블 갱신
    // 유닛들의 walkable을 갱신해주는 함수.
    public static void UpdateFriendlyNodeWalkable(Vector3 _pos, int _idx)
    {
        if (dicNodeUnderFriendlyUnit.TryGetValue(_idx, out var prevNode))
            prevNode.walkable = true;

        PF_Node curNode = grid.GetNodeFromWorldPoint(_pos);
        curNode.walkable = false;
        dicNodeUnderFriendlyUnit[_idx] = curNode;
    }
    public static void UpdateEnemyNodeWalkable(Vector3 _pos, int _idx)
    {
        if (dicNodeUnderEnemyUnit.TryGetValue(_idx, out var prevNode))
            prevNode.walkable = true;

        PF_Node curNode = grid.GetNodeFromWorldPoint(_pos);
        curNode.walkable = false;
        dicNodeUnderEnemyUnit[_idx] = curNode;
    }
    #endregion

    #region 유닛 사망시 노드 워커블 초기화
    // 유닛 사망시 해당 위치의 walkable을 true로 만들어주는 함수.
    public static void ResetHeroUnitNode(Vector3 _pos)
    {
        dicNodeUnderFriendlyUnit[0].walkable = true;
        grid.GetNodeFromWorldPoint(_pos).walkable = true;
    }
    public static void ResetFriendlyNodeWalkable(Vector3 _pos, int _idx)
    {
        dicNodeUnderFriendlyUnit[_idx].walkable = true;
        grid.GetNodeFromWorldPoint(_pos).walkable = true;
        dicNodeUnderFriendlyUnit.Remove(_idx);
    }
    public static void ResetEnemyNodeWalkable(Vector3 _pos, int _idx)
    {
        dicNodeUnderEnemyUnit[_idx].walkable = true;
        grid.GetNodeFromWorldPoint(_pos).walkable = true;
        dicNodeUnderEnemyUnit.Remove(_idx);
    }
    #endregion

    // 매개변수로 받은 벡터위치를 노드 위치로 변환하여 유닛의 위치를 조정해줌.
    // 주로 최초 생성시 사용함.
    public static Vector3 ResetPosition(Vector3 _pos)
    {
        PF_Node unitNode = grid.GetNodeFromWorldPoint(_pos);

        // walkable이 아닐 경우 주위의 walkable인 노드를 찾아 반환해줌.
        if (!grid.GetNodeFromWorldPoint(_pos).walkable)
            return grid.GetAccessibleNodeWithoutTargetNode(unitNode).worldPos;

        return unitNode.worldPos;
    }
    #region crowd(부대지정)관련
    public void SetListToCrowd(int _arrIdx)
    {
        if (isEnemyObjectInList || listSelectedFriendlyObject.Count < 1) return;

        for (int i = 0; i < arrCrowd[_arrIdx].Count; ++i)
            arrCrowd[_arrIdx][i].ResetCrowdIdx();
        arrCrowd[_arrIdx].Clear();

        arrCrowd[_arrIdx].AddRange(listSelectedFriendlyObject.ToArray());
        for (int i = 0; i < arrCrowd[_arrIdx].Count; ++i)
            arrCrowd[_arrIdx][i].CrowdIdx = _arrIdx;
    }
    public void LoadCrowdWithIdx(int _arrIdx)
    {
        tempListSelectableObject.AddRange(arrCrowd[_arrIdx].ToArray());
        if (tempListSelectableObject.Count < 1)
        {
            listSelectedFriendlyObject.Clear();
            UpdateInfo();
        }
        else
            SelectFinish();
    }
    public void RemoveAtCrowd(int _arrIdx, FriendlyObject _removeObj)
    {
        arrCrowd[_arrIdx].Remove(_removeObj);
    }
    #endregion

    // 유닛 선택 시작시 1번 호출, 이전에 선택한 오브젝트들의 선택을 알리는 표시 제거.
    public void SelectStart()
    {
        for (int i = 0; i < listSelectedFriendlyObject.Count; ++i)
            listSelectedFriendlyObject[i].DeActivateCircle();
    }

    // 선택한 리스트에서 유닛을 제외하는 함수
    // 시프트 + 클릭시 리스트에 존재하면 제외시키는 등의 역할
    public void RemoveUnitAtList(FriendlyObject _removeObj)
    {
        if (!_removeObj) return;

        bool isDeleted = false;

        for (int i = 0; i < listSelectedFriendlyObject.Count;)
        {
            if (isDeleted)
                listSelectedFriendlyObject[i].UpdatelistIdx(i - 1);
            else if (listSelectedFriendlyObject[i].Equals(_removeObj))
            {
                listSelectedFriendlyObject[i].unSelect();
                listSelectedFriendlyObject.RemoveAt(i);
                isDeleted = true;
                continue;
            }
            ++i;
        }

        UpdateInfo();
    }

    // 시프트+ 클릭으로 부대에 추가하는 경우 사용.
    public void AddToList(FriendlyObject _addObj)
    {
        if (_addObj == null) return;
        if (_addObj.GetUnitType.Equals(EUnitType.NONE)) return;
        if (isEnemyObjectInList || isFriendlyStructureInList) return;
        if (listSelectedFriendlyObject.Count > 11) return;

        _addObj.Select(listSelectedFriendlyObject.Count);
        _addObj.ActivateCircle();
        listSelectedFriendlyObject.Add(_addObj);

        UpdateInfo();
    }

    #region 벙커 관련
    public void InUnit(FriendlyObject _friObj)
    {
        curBunker.InUnit(_friObj);
    }
    public void OutOneUnit()
    {
        if (listSelectedFriendlyObject[0].GetObjectType().Equals(EObjectType.BUNKER))
            listSelectedFriendlyObject[0].GetComponent<StructureBunker>().OutOneUnit();
    }
    public void OutAllUnit()
    {
        if (listSelectedFriendlyObject[0].GetObjectType().Equals(EObjectType.BUNKER))
            listSelectedFriendlyObject[0].GetComponent<StructureBunker>().OutAllUnit();
    }

    // 벙커에 유닛이 들어가는 순간 다른 유닛이 벙커에 들어가려는 행위를 막는 함수.
    public void ResetTargetBunker()
    {
        foreach (FriendlyObject obj in listSelectedFriendlyObject)
            obj.ResetTargetBunker();
    }
    #endregion

    #region 배럭 관련
    public bool CanSpawnunit()
    {
        if (listSelectedFriendlyObject[0].GetObjectType().Equals(EObjectType.BARRACK))
            return listSelectedFriendlyObject[0].GetComponent<StructureBarrack>().CanSpawnUnit();
        else
            return false;
    }
    public void RequestSpawnUnit(EUnitType _unitType)
    {
        if (listSelectedFriendlyObject[0].GetObjectType().Equals(EObjectType.BARRACK))
            listSelectedFriendlyObject[0].GetComponent<StructureBarrack>().StartSpawnUnit(_unitType);
    }
    public void SpawnUnit(EUnitType _unitType, Vector3 _spawnPos, Vector3 _rallyPoint, Transform _rallyTr = null)
    {
        FriendlyObject tempObj = arrMemoryPool[(int)_unitType].ActivatePoolItem(_spawnPos, 5, transform).GetComponent<FriendlyObject>();
        tempObj.Position = ResetPosition(tempObj.Position);
        tempObj.Init();

        if (_rallyTr != null)
            tempObj.FollowTarget(_rallyTr);
        else if (!_rallyPoint.Equals(_spawnPos))
            tempObj.MoveByPos(_rallyPoint);
    }
    public void SetRallyPoint(Vector3 _pos)
    {
        listSelectedFriendlyObject[0].GetComponent<StructureBarrack>().SetRallyPoint(_pos);
    }

    public void SetRallyPoint(Transform _targetTr)
    {
        listSelectedFriendlyObject[0].GetComponent<StructureBarrack>().SetRallyPoint(_targetTr);
    }
    #endregion

    // 아군유닛 사망시 호출
    public void DeactivateUnit(GameObject _removeGo, EUnitType _unitType, FriendlyObject _fObj)
    {
        GameObject deadEffectGo = deadEffectMemoryPool.ActivatePoolItem(5, transform);
        deadEffectGo.GetComponent<EffectUnitDead>().Init(_removeGo.transform.position, DeactivateEffectDead);
        arrMemoryPool[(int)_unitType].DeactivatePoolItem(_removeGo);
        RemoveUnitAtList(_fObj);
    }
    public void DeactivateEffectDead(Transform _tr)
    {
        deadEffectMemoryPool.DeactivatePoolItem(_tr.gameObject);
    }

#region 드래그 선택 관련
    public void AddSelectedObject(SelectableObject _object)
    {
        if (!_object || _object.IsTempSelect) return;

        tempListSelectableObject.Add(_object);
        _object.IsTempSelect = true;
        _object.ActivateCircle();
    }

    public void RemoveSelectedObject(SelectableObject _object)
    {
        tempListSelectableObject.Remove(_object);
        _object.IsTempSelect = false;
        _object.DeActivateCircle();
    }

    public void SelectFinish()
    {
        // 선택한 유닛이 없을 경우
        if (tempListSelectableObject.Count < 1)
        {
            for (int i = 0; i < listSelectedFriendlyObject.Count; ++i)
                listSelectedFriendlyObject[i].ActivateCircle();
            return;
        }

        // 이미 선택되었던 유닛들 선택해제
        foreach (SelectableObject obj in listSelectedFriendlyObject)
            obj.unSelect();

        // 이미 선택된 유닛이 적 유닛이었을 경우
        if (enemyCurSelected != null)
        {
            enemyCurSelected.unSelect();
            enemyCurSelected = null;
        }

        // 드래그해서 선택된 모든 유닛들의 경우 임시 선택 상태를 해제(false)
        // 선택되었음을 나타내는 서클도 보이지않도록 함수 호출
        for (int i = 0; i < tempListSelectableObject.Count; ++i)
        {
            tempListSelectableObject[i].IsTempSelect = false;
            tempListSelectableObject[i].DeActivateCircle();
        }


        listSelectedFriendlyObject.Clear();
        ArrayHUDCommand.Use(EHUDCommand.HIDE_ALL_INFO);
        SelectableObject tempObj = null;
        isFriendlyUnitInList = false;
        isFriendlyStructureInList = false;
        isEnemyObjectInList = false;
        // 오브젝트를 하나하나 검사.
        foreach (SelectableObject obj in tempListSelectableObject)
        {
            if (obj == null) continue;

            if (listSelectedFriendlyObject.Count > 11) break;

            switch (obj.GetObjectType())
            {
                case EObjectType.UNIT_01:
                case EObjectType.UNIT_02:
                case EObjectType.UNIT_HERO:
                    if (!isFriendlyUnitInList)
                    {
                        listSelectedFriendlyObject.Add(obj.GetComponent<FriendlyObject>());
                        isFriendlyUnitInList = true;
                        isFriendlyStructureInList = false;
                        isEnemyObjectInList = false;
                        tempObj = null;
                    }
                    else
                        listSelectedFriendlyObject.Add(obj.GetComponent<FriendlyObject>());
                    break;
                case EObjectType.MAIN_BASE:
                case EObjectType.TURRET:
                case EObjectType.BUNKER:
                case EObjectType.WALL:
                case EObjectType.BARRACK:
                case EObjectType.NUCLEAR:
                    if (isFriendlyUnitInList) break;
                    isFriendlyStructureInList = true;
                    isEnemyObjectInList = false;
                    if (!tempObj) tempObj = obj;
                    break;
                case EObjectType.ENEMY_SMALL:
                case EObjectType.ENEMY_STRUCTURE:
                case EObjectType.ENEMY_BIG:
                    if (isFriendlyUnitInList) break;
                    if (isFriendlyStructureInList) break;
                    isEnemyObjectInList = true;
                    if (!tempObj) tempObj = obj;
                    break;
                default:
                    break;
            }
        }

        
        if( tempObj != null)
        {
             AudioManager.instance.PlayAudio_Select(tempObj.GetObjectType());
        }
        else
        {
            AudioManager.instance.PlayAudio_Select(listSelectedFriendlyObject[0].GetObjectType());
        }
        
        // 임시 리스트에 적 유닛만 있을 경우
        if (isEnemyObjectInList)
        {
            selectObjectCallback?.Invoke(tempObj.GetObjectType());
            InputOtherUnitInfo(tempObj);
            enemyCurSelected = tempObj;
            enemyCurSelected.Select();
            DisplaySingleUnitInfo();
        }
        // 임시 리스트에 아군 건물만 있을 경우
        else if (isFriendlyStructureInList)
        {
            InputOtherUnitInfo(tempObj);
            listSelectedFriendlyObject.Add(tempObj.GetComponent<FriendlyObject>());
            Structure structureInList = tempObj.GetComponent<Structure>();
            // 아군 건물이 건설중인 건물일 경우
            if (structureInList.IsProcessingConstruct)
            {
                selectObjectCallback?.Invoke(EObjectType.NONE);
                structureInList.UpdateConstructInfo();
                ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_CONSTRUCT_INFO);
                ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
            }
            // 아군 건물이 업그레이드중일 경우
            else if (structureInList.IsProcessingUpgrade)
            {
                selectObjectCallback?.Invoke(EObjectType.NONE);
                structureInList.UpdateUpgradeInfo();
                ArrayHUDUpgradeCommand.Use(EHUDUpgradeCommand.DISPLAY_UPGRADE_INFO, structureInList.CurUpgradeType);
                ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
            }
            // 아군 건물이 해체중일 경우
            else if (structureInList.IsProcessingDemolish)
            {
                selectObjectCallback?.Invoke(EObjectType.NONE);
                structureInList.UpdateDemolishInfo();
                ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_DEMOLISH_INFO);
                ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
            }
            // 아군 건물이 배럭일 때
            else if (tempObj.GetObjectType().Equals(EObjectType.BARRACK))
            {
                StructureBarrack tempBarrack = tempObj.GetComponent<StructureBarrack>();
                selectObjectCallback?.Invoke(EObjectType.BARRACK);
                // 배럭이 유닛을 생산중일 경우
                if (tempBarrack.IsProcessingSpawnUnit)
                {
                    tempBarrack.UpdateSpawnInfo();
                    ArrayHUDSpawnUnitCommand.Use(EHUDSpawnUnitCommand.DISPLAY_SPAWN_UNIT_INFO);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                else
                {
                    InputOtherUnitInfo(tempObj);
                    DisplaySingleUnitInfo(structureInList);
                }
            }
            // 아군 건물이 핵 생산 건물일 경우
            else if (tempObj.GetObjectType().Equals(EObjectType.NUCLEAR))
            {
                StructureNuclear tempNuclear = listSelectedFriendlyObject[0].GetComponent<StructureNuclear>();

                // 핵을 생산중일 경우
                if (tempNuclear.IsProcessingSpawnNuclear)
                {
                    tempNuclear.UpdateSpawnNuclearInfo();
                    ArrayHUDSpawnNuclearCommand.Use(EHUDSpawnNuclearCommand.DISPLAY_SPAWN_NUCLEAR_INFO);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                else
                {
                    selectObjectCallback?.Invoke(EObjectType.NUCLEAR);
                    InputOtherUnitInfo(tempObj);
                    DisplaySingleUnitInfo(structureInList);
                }
            }
            // 아무것도 하지 않는 상태의 건물일 경우
            else
            {
                selectObjectCallback?.Invoke(tempObj.GetObjectType());
                InputOtherUnitInfo(tempObj);
                DisplaySingleUnitInfo(structureInList);
            }
        }
        // 임시 리스트에 아군 유닛이 존재할 경우
        else if (isFriendlyUnitInList)
        {
            selectObjectCallback?.Invoke(listSelectedFriendlyObject[0].GetObjectType());
            // 아군 유닛 1마리만 존재할 경우
            if (listSelectedFriendlyObject.Count < 2)
            {
                InputOtherUnitInfo(listSelectedFriendlyObject[0]);
                DisplaySingleUnitInfo();
            }
            else
            {
                InputFriendlyUnitInfo();
                ArrayHUDCommand.Use(EHUDCommand.DISPLAY_GROUP_INFO, listSelectedFriendlyObject.Count);
            }
        }

        tempListSelectableObject.Clear();
        return;
    }
    #endregion

    // 다른 오브젝트를 선택할 경우 해당 오브젝트의 상태에 따라 UI 갱신
    // 추후에 시간되면 기능별로(UpdateStructureInfo, UpdateUnitInfo) 함수 만들어서 구분할 것.
    // SelectFinish에 넣을 수 있도록 수정해 볼 것
    public void UpdateInfo()
    {
        ArrayHUDCommand.Use(EHUDCommand.HIDE_ALL_INFO);
        // 리스트가 비어있지 않을 경우
        if (listSelectedFriendlyObject.Count > 0)
        {
            if (isFriendlyStructureInList)
            {
                Structure curStructure = listSelectedFriendlyObject[0].GetComponent<Structure>();
                // 해당 건물이 현재 업그레이드를 진행중일 경우
                if (curStructure.IsProcessingUpgrade)
                {
                    curStructure.UpdateUpgradeInfo();
                    selectObjectCallback?.Invoke(EObjectType.NONE);
                    ArrayHUDUpgradeCommand.Use(EHUDUpgradeCommand.DISPLAY_UPGRADE_INFO, curStructure.CurUpgradeType);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                // 해당 건물이 현재 해체중일 경우
                else if (curStructure.IsProcessingDemolish)
                {
                    curStructure.UpdateDemolishInfo();
                    selectObjectCallback?.Invoke(EObjectType.NONE);
                    ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_DEMOLISH_INFO, curStructure);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                // 해당 건물이 현재 건설중일 경우
                else if (curStructure.IsProcessingConstruct)
                {
                    curStructure.UpdateConstructInfo();
                    selectObjectCallback?.Invoke(EObjectType.NONE);
                    ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_CONSTRUCT_INFO, curStructure);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                // 그렇지 않다면
                else
                {
                    StructureBarrack tempBarrack = listSelectedFriendlyObject[0].GetComponent<StructureBarrack>();
                    StructureNuclear tempNuclear = listSelectedFriendlyObject[0].GetComponent<StructureNuclear>();
                    // 만일 건물이 배럭이고 생산중이라면
                    if (tempBarrack != null && tempBarrack.IsProcessingSpawnUnit)
                    {
                        selectObjectCallback?.Invoke(EObjectType.BARRACK);
                        tempBarrack.UpdateSpawnInfo();
                        ArrayHUDSpawnUnitCommand.Use(EHUDSpawnUnitCommand.DISPLAY_SPAWN_UNIT_INFO);
                        ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                    }
                    // 만일 건물이 핵 건물이고 핵 생산중이라면
                    else if (tempNuclear != null && tempNuclear.IsProcessingSpawnNuclear)
                    {
                        selectObjectCallback?.Invoke(EObjectType.NUCLEAR);
                        tempNuclear.UpdateSpawnNuclearInfo();
                        ArrayHUDSpawnNuclearCommand.Use(EHUDSpawnNuclearCommand.DISPLAY_SPAWN_NUCLEAR_INFO);
                        ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                    }
                    else
                    {
                        selectObjectCallback?.Invoke(listSelectedFriendlyObject[0].GetObjectType());
                        InputOtherUnitInfo(listSelectedFriendlyObject[0]);
                        DisplaySingleUnitInfo(curStructure);
                    }
                }
            }
            // 리스트에 아군 유닛 존재할 경우
            else if (isFriendlyUnitInList)
            {
                // 리스트에 유닛이 하나만 존재할 경우
                if (listSelectedFriendlyObject.Count < 2)
                {
                    InputOtherUnitInfo(listSelectedFriendlyObject[0]);
                    DisplaySingleUnitInfo();
                }
                // 리스트에 유닛이 다수 존재할 경우
                else
                {
                    InputFriendlyUnitInfo();
                    ArrayHUDCommand.Use(EHUDCommand.DISPLAY_GROUP_INFO, listSelectedFriendlyObject.Count);
                }
                selectObjectCallback?.Invoke(listSelectedFriendlyObject[0].GetObjectType());
            }
        }
        // 리스트가 비어있거나 적 유닛만 존재할 경우
        else
        {
            if (enemyCurSelected != null)
                DisplaySingleUnitInfo();
            else
                ArrayHUDCommand.Use(EHUDCommand.HIDE_UNIT_INFO);

            selectObjectCallback?.Invoke(EObjectType.NONE);
        }
    }

    // 하나의 유닛만 선택되었을 경우 호출
    private void DisplaySingleUnitInfo(Structure _structure = null)
    {
        if (listSelectedFriendlyObject.Count < 1)
        {
            ArrayHUDCommand.Use(EHUDCommand.DISPLAY_SINGLE_INFO, enemyCurSelected.GetObjectName, enemyCurSelected.GetObjectDescription);
            return;
        }

        SelectableObject tempObj = listSelectedFriendlyObject[0];

        if (_structure == null)
            ArrayHUDCommand.Use(EHUDCommand.DISPLAY_SINGLE_INFO, tempObj.GetObjectName, tempObj.GetObjectDescription);
        else
            ArrayHUDCommand.Use(EHUDCommand.DISPLAY_SINGLE_STRUCTURE_INFO, tempObj.GetObjectName, tempObj.GetObjectDescription, _structure.UpgradeLevel);
    }

    // 유닛이 어떤 인덱스를 지니고 있냐에 따라 HP를 업데이트.
    public static void UpdateUnitHp(int _listIdx)
    {
        if (_listIdx.Equals(-3))
        {
            unitInfoContainer.curHpPercent = enemyCurSelected.GetCurHpPercent;
            return;
        }
        else if (_listIdx.Equals(-2))
        {
            unitInfoContainer.curHpPercent = listSelectedFriendlyObject[0].GetCurHpPercent;
            return;
        }
        else if (_listIdx.Equals(-1))
            return;
        else if (listSelectedFriendlyObject[_listIdx] != null)
        {
            if (listSelectedFriendlyObject.Count < 2)
            {
                unitInfoContainer.curHpPercent = listSelectedFriendlyObject[0].GetCurHpPercent;
            }
            else
            {
                SFriendlyUnitInfo tempInfo = listFriendlyUnitInfo[_listIdx];
                tempInfo.curHpPercent = listSelectedFriendlyObject[_listIdx].GetCurHpPercent;
                listFriendlyUnitInfo[_listIdx] = tempInfo;
            }
        }
    }

    // 아군 유닛의 정보를 구조체에 입력
    private void InputFriendlyUnitInfo()
    {
        for (int i = 0; i < listSelectedFriendlyObject.Count; ++i)
        {
            SFriendlyUnitInfo tempInfo = listFriendlyUnitInfo[i];
            tempInfo.unitType = listSelectedFriendlyObject[i].GetUnitType;
            tempInfo.curHpPercent = listSelectedFriendlyObject[i].GetCurHpPercent;
            listFriendlyUnitInfo[i] = tempInfo;
            listSelectedFriendlyObject[i].Select(i);
        }
    }

    // 아군 유닛 외의 유닛의 정보를 UI에 표시하기 위한 컨테이너 클래스에 입력
    private void InputOtherUnitInfo(SelectableObject _obj)
    {
        unitInfoContainer.objectType = _obj.GetObjectType();
        unitInfoContainer.maxHp = _obj.MaxHp;
        unitInfoContainer.curHpPercent = _obj.GetCurHpPercent;
        unitInfoContainer.attDmg = _obj.AttDmg;
        unitInfoContainer.attRange = _obj.AttRange;
        unitInfoContainer.attRate = _obj.AttRate;

        if (_obj.GetObjectType().Equals(EObjectType.ENEMY_SMALL) || _obj.GetObjectType().Equals(EObjectType.ENEMY_BIG))
        {
            unitInfoContainer.unitType = EUnitType.NONE;
            _obj.ActivateCircle();
        }
        else
        {
            unitInfoContainer.unitType = _obj.GetComponent<FriendlyObject>().GetUnitType;
            _obj.GetComponent<FriendlyObject>().Select();
        }
            
    }

    // wave때 몰려오는 적들의 이동을 시키는 함수.
    public static void MoveWaveEnemy(Vector3 _targetPos, SelectableObject[] _arrEnemy)
    {
        Vector3 centerPos = CalcFormationCenterPos(_targetPos.y);
        foreach (SelectableObject obj in _arrEnemy)
            obj.MoveAttack(_targetPos + CalcPosInFormation(obj.Position, centerPos));
    }
    #region 아군유닛 조작관련
    // 선택한 유닛들을 바닥 우클릭으로 이동시키는 함수.
    public void MoveUnitByPicking(Vector3 _targetPos, bool isAttackMove = false)
    {
        if (IsListEmpty) return;
        if (isFriendlyStructureInList) return;
        if (isEnemyObjectInList) return;

        // Click Move Order Audio
        AudioManager.instance.PlayAudio_Order(objectType);
        
        if (isAttackMove)
        {
            AudioManager.instance.PlayAudio_Order(objectType);      // Click Order Audio
            
            Vector3 centerPos = CalcFormationCenterPos(_targetPos.y);
            foreach (FriendlyObject obj in listSelectedFriendlyObject)
                obj.MoveAttack(_targetPos + CalcPosInFormation(obj.Position, centerPos));
        }
        else if (IsGroupMaxDistOverRange())
        {
            // 지정된 위치에 5열 종대로 헤쳐모여
            CalcNewFormation(_targetPos);
        }
        else
        {
            // 대열 유지하면서 모이기
            Vector3 centerPos = CalcFormationCenterPos(_targetPos.y);
            foreach (FriendlyObject obj in listSelectedFriendlyObject)
                obj.MoveByPos(_targetPos + CalcPosInFormation(obj.Position, centerPos));
        }
    }
    public void MoveUnitByPicking(Transform _targetTr)
    {
        if (IsListEmpty) return;
        if (isFriendlyStructureInList) return;
        if (isEnemyObjectInList) return;

        // Click Move Order Audio
        AudioManager.instance.PlayAudio_Order(objectType);

        if (_targetTr.GetComponent<IGetObjectType>().GetObjectType().Equals(EObjectType.BUNKER))
        {
            curBunker = _targetTr.GetComponent<StructureBunker>();
            foreach (FriendlyObject obj in listSelectedFriendlyObject)
                obj.FollowTarget(_targetTr, true);
        }
        else
        {
            foreach (FriendlyObject obj in listSelectedFriendlyObject)
                obj.FollowTarget(_targetTr);
        }
    }

    public void Stop()
    {
        AudioManager.instance.PlayAudio_UI(); // CLICK Audio
        foreach (FriendlyObject obj in listSelectedFriendlyObject)
            obj.Stop();
    }
    public void Hold()
    {
        AudioManager.instance.PlayAudio_UI(); // CLICK Audio
        foreach (FriendlyObject obj in listSelectedFriendlyObject)
            obj.Hold();
    }
    public void Patrol(Vector3 _wayPointTo)
    {
        CalcNewFormation(_wayPointTo, true);
    }

    private void CalcNewFormation(Vector3 _targetPos, bool _isPatrol = false)
    {
        if (isFriendlyStructureInList) return;
        if (isEnemyObjectInList) return;

        int unitCnt = listSelectedFriendlyObject.Count;
        int col = Mathf.Clamp(unitCnt, 1, 5);

        float posX = 0f;
        float posZ = 0f;
        Vector3 destPos = Vector3.zero;

        for (int i = 0; i < unitCnt; ++i)
        {
            posX = i % col;
            posZ = i / col;
            destPos = _targetPos + new Vector3(posX, 0f, posZ);

            if (_isPatrol)
                listSelectedFriendlyObject[i].Patrol(destPos);
            else
                listSelectedFriendlyObject[i].MoveByPos(destPos);
        }
    }

    private static Vector3 CalcFormationCenterPos(float _targetPosY)
    {
        Vector3 centerPos = Vector3.zero;
        foreach (FriendlyObject obj in listSelectedFriendlyObject)
        {
            centerPos.x += obj.Position.x;
            centerPos.z += obj.Position.z;
        }

        centerPos /= listSelectedFriendlyObject.Count;
        centerPos.y += _targetPosY;

        return centerPos;
    }

    private static Vector3 CalcPosInFormation(Vector3 _myPos, Vector3 _centerPos)
    {
        return _myPos - _centerPos;
    }

    private bool IsGroupMaxDistOverRange()
    {
        Vector3 objPos = listSelectedFriendlyObject[0].Position;
        float maxX = objPos.x;
        float minX = objPos.x;
        float maxZ = objPos.z;
        float minZ = objPos.z;

        foreach (FriendlyObject obj in listSelectedFriendlyObject)
        {
            objPos = obj.Position;
            if (objPos.x > maxX) maxX = objPos.x;
            else if (objPos.x < minX) minX = objPos.x;

            if (objPos.z > maxZ) maxZ = objPos.z;
            else if (objPos.z < minZ) minZ = objPos.z;
        }

        return Mathf.Abs(maxX - minX) > rangeGroupLimitDist || Mathf.Abs(maxZ - minZ) > rangeGroupLimitDist;
    }
    #endregion

    public void RegisterBroker()
    {
        Broker.Regist(this, EPublisherType.SELECTABLE_MANAGER);
    }

    public void PushMessageToBroker(EMessageType _message)
    {
        Broker.AlertMessageToSub(_message, EPublisherType.SELECTABLE_MANAGER);
    }
    #region UnitUpgradeComplete
    // 업그레이드 완료시 호출되는 함수.
    public void CompleteUpgradeRangedUnitDmg()
    {
        ++levelRangedUnitDmgUpgrade;
        PushMessageToBroker(EMessageType.UPGRADE_RANGED_DMG);
        AudioManager.instance.PlayAudio_Advisor(EAudioType_Advisor.UPGRADE);      // Advisor Audio
    }

    public void CompleteUpgradeRangedUnitHp()
    {
        ++levelRangedUnitHpUpgrade;
        PushMessageToBroker(EMessageType.UPGRADE_RANGED_HP);
        AudioManager.instance.PlayAudio_Advisor(EAudioType_Advisor.UPGRADE);      // Advisor Audio
    }

    public void CompleteUpgradeMeleeUnitDmg()
    {
        ++levelMeleeUnitDmgUpgrade;
        PushMessageToBroker(EMessageType.UPGRADE_MELEE_DMG);
        AudioManager.instance.PlayAudio_Advisor(EAudioType_Advisor.UPGRADE);      // Advisor Audio
    }

    public void CompleteUpgradeMeleeUnitHp()
    {
        ++levelMeleeUnitHpUpgrade;
        PushMessageToBroker(EMessageType.UPGRADE_MELEE_HP);
        AudioManager.instance.PlayAudio_Advisor(EAudioType_Advisor.UPGRADE);      // Advisor Audio
    }
    #endregion

    [Header("-Melee/Ranged Prefabs")]
    [SerializeField]
    private GameObject[] arrUnitPrefab = null;
    [SerializeField]
    private float rangeGroupLimitDist = 5f;
    [SerializeField]
    private GameObject unitDeadEffect = null;

    private MemoryPool deadEffectMemoryPool = null;

    private static int levelRangedUnitDmgUpgrade = 1;
    private static int levelRangedUnitHpUpgrade = 1;
    private static int levelMeleeUnitDmgUpgrade = 1;
    private static int levelMeleeUnitHpUpgrade = 1;
    private static float delayUnitUpgrade = 5f;

    private static bool isFriendlyUnitInList = false;
    private static bool isFriendlyStructureInList = false;
    private static bool isEnemyObjectInList = false;

    private static List<FriendlyObject> listSelectedFriendlyObject = null;

    private static PF_Grid grid = null;
    private static Dictionary<int, PF_Node> dicNodeUnderFriendlyUnit = null;
    private static Dictionary<int, PF_Node> dicNodeUnderEnemyUnit = null;
    private static int dicFriendlyIdx = 0;
    private static int dicEnemyIdx = 0;

    private static UnitInfoContainer unitInfoContainer = null;
    private static List<SFriendlyUnitInfo> listFriendlyUnitInfo = null;
    private static SelectableObject enemyCurSelected = null;

    private List<SelectableObject> tempListSelectableObject = null;
    private MemoryPool[] arrMemoryPool = null;
    private List<FriendlyObject>[] arrCrowd = null;
    private EObjectType objectType = EObjectType.NONE;
    private VoidSelectObjectTypeDelegate selectObjectCallback = null;
    private StructureBunker curBunker = null;
}
