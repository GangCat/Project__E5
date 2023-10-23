using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableObjectManager : MonoBehaviour, IPublisher
{
    public void Init(VoidSelectObjectTypeDelegate _selectObjectCallback, PF_Grid _grid)
    {
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

    private IEnumerator CheckNodeBuildableCoroutine()
    {
        while (true)
        {
            ArrayCheckNodeBuildableCommand.Use(ECheckNodeBuildable.CHECK_NODE_BUILDABLE_UNIT, dicNodeUnderFriendlyUnit.Values.ToArray<PF_Node>());

            yield return new WaitForSeconds(0.5f);
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

    public static void UpdateFriendlyNodeWalkable(Vector3 _pos, int _idx)
    {
        PF_Node prevNode = null;
        if (dicNodeUnderFriendlyUnit.TryGetValue(_idx, out prevNode))
            prevNode.walkable = true;

        PF_Node curNode = grid.GetNodeFromWorldPoint(_pos);
        curNode.walkable = false;
        dicNodeUnderFriendlyUnit[_idx] = curNode;
    }

    public static void UpdateEnemyNodeWalkable(Vector3 _pos, int _idx)
    {
        PF_Node prevNode = null;
        if (dicNodeUnderEnemyUnit.TryGetValue(_idx, out prevNode))
            prevNode.walkable = true;

        PF_Node curNode = grid.GetNodeFromWorldPoint(_pos);
        curNode.walkable = false;
        dicNodeUnderEnemyUnit[_idx] = curNode;
    }

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

    public static Vector3 ResetPosition(Vector3 _pos)
    {
        PF_Node unitNode = grid.GetNodeFromWorldPoint(_pos);

        if (!grid.GetNodeFromWorldPoint(_pos).walkable)
            return grid.GetAccessibleNodeWithoutTargetNode(unitNode).worldPos;

        return unitNode.worldPos;
    }

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

    public void SelectStart()
    {
        for (int i = 0; i < listSelectedFriendlyObject.Count; ++i)
            listSelectedFriendlyObject[i].DeActivateCircle();
    }

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

    public void SetRallyPoint(Vector3 _pos)
    {
        listSelectedFriendlyObject[0].GetComponent<StructureBarrack>().SetRallyPoint(_pos);
    }

    public void SetRallyPoint(Transform _targetTr)
    {
        listSelectedFriendlyObject[0].GetComponent<StructureBarrack>().SetRallyPoint(_targetTr);
    }

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
        if (tempListSelectableObject.Count < 1)
        {
            for (int i = 0; i < listSelectedFriendlyObject.Count; ++i)
                listSelectedFriendlyObject[i].ActivateCircle();
            return;
        }

        foreach (SelectableObject obj in listSelectedFriendlyObject)
            obj.unSelect();

        if (enemyCurSelected)
        {
            enemyCurSelected.unSelect();
            enemyCurSelected = null;
        }


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
        // ������Ʈ�� �ϳ��ϳ� �˻�.
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
            // AudioManager.instance.PlayAudio_Select(/*tempObj.GetObjectType()*/EObjectType.UNIT_01);     // Select Audio(Unit)
             AudioManager.instance.PlayAudio_Select(tempObj.GetObjectType());     // Select Audio(Unit)
        }
        else
        {
            AudioManager.instance.PlayAudio_Select(listSelectedFriendlyObject[0].GetObjectType());     // Select Audio(Struct)
        }
        
        // �ӽ� ����Ʈ�� �� ���ָ� ���� ���
        if (isEnemyObjectInList)
        {
            //tempObj.ActivateCircle();
            selectObjectCallback?.Invoke(tempObj.GetObjectType());
            InputOtherUnitInfo(tempObj);
            enemyCurSelected = tempObj;
            enemyCurSelected.Select();
            DisplaySingleUnitInfo();
        }
        // �ӽ� ����Ʈ�� �Ʊ� �ǹ��� ���� ���
        else if (isFriendlyStructureInList)
        {
            //tempObj.ActivateCircle();
            InputOtherUnitInfo(tempObj);
            listSelectedFriendlyObject.Add(tempObj.GetComponent<FriendlyObject>());
            Structure structureInList = tempObj.GetComponent<Structure>();
            // �Ʊ� �ǹ��� �Ǽ����� �ǹ��� ���
            if (structureInList.IsProcessingConstruct)
            {
                selectObjectCallback?.Invoke(EObjectType.NONE);
                structureInList.UpdateConstructInfo();
                ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_CONSTRUCT_INFO);
                ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
            }
            // �Ʊ� �ǹ��� ���׷��̵����� ���
            else if (structureInList.IsProcessingUpgrade)
            {
                selectObjectCallback?.Invoke(EObjectType.NONE);
                structureInList.UpdateUpgradeInfo();
                ArrayHUDUpgradeCommand.Use(EHUDUpgradeCommand.DISPLAY_UPGRADE_INFO, structureInList.CurUpgradeType);
                ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
            }
            // �Ʊ� �ǹ��� ��ü���� ���
            else if (structureInList.IsProcessingDemolish)
            {
                selectObjectCallback?.Invoke(EObjectType.NONE);
                structureInList.UpdateDemolishInfo();
                ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_DEMOLISH_INFO);
                ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
            }
            // �Ʊ� �ǹ��� �跰�� ��
            else if (tempObj.GetObjectType().Equals(EObjectType.BARRACK))
            {
                StructureBarrack tempBarrack = tempObj.GetComponent<StructureBarrack>();
                selectObjectCallback?.Invoke(EObjectType.BARRACK);
                // �跰�� ������ �������� ���
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
            // �Ʊ� �ǹ��� �� ���� �ǹ��� ���
            else if (tempObj.GetObjectType().Equals(EObjectType.NUCLEAR))
            {
                StructureNuclear tempNuclear = listSelectedFriendlyObject[0].GetComponent<StructureNuclear>();

                // ���� �������� ���
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
            // �ƹ��͵� ���� �ʴ� ������ �ǹ��� ���
            else
            {
                selectObjectCallback?.Invoke(tempObj.GetObjectType());
                InputOtherUnitInfo(tempObj);
                DisplaySingleUnitInfo(structureInList);
            }
        }
        // �ӽ� ����Ʈ�� �Ʊ� ������ ������ ���
        else if (isFriendlyUnitInList)
        {
            selectObjectCallback?.Invoke(listSelectedFriendlyObject[0].GetObjectType());
            //for (int i = 0; i < listSelectedFriendlyObject.Count; ++i)
            //    listSelectedFriendlyObject[i].ActivateCircle();
            // �Ʊ� ���� 1������ ������ ���
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

    public void UpdateInfo()
    {
        ArrayHUDCommand.Use(EHUDCommand.HIDE_ALL_INFO);
        // ����Ʈ�� ������� ���� ���
        if (listSelectedFriendlyObject.Count > 0)
        {
            if (isFriendlyStructureInList)
            {
                Structure curStructure = listSelectedFriendlyObject[0].GetComponent<Structure>();
                // �ش� �ǹ��� ���� ���׷��̵带 �������� ���
                if (curStructure.IsProcessingUpgrade)
                {
                    curStructure.UpdateUpgradeInfo();
                    selectObjectCallback?.Invoke(EObjectType.NONE);
                    ArrayHUDUpgradeCommand.Use(EHUDUpgradeCommand.DISPLAY_UPGRADE_INFO, curStructure.CurUpgradeType);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                // �ش� �ǹ��� ���� ��ü���� ���
                else if (curStructure.IsProcessingDemolish)
                {
                    curStructure.UpdateDemolishInfo();
                    selectObjectCallback?.Invoke(EObjectType.NONE);
                    ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_DEMOLISH_INFO, curStructure);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                // �ش� �ǹ��� ���� �Ǽ����� ���
                else if (curStructure.IsProcessingConstruct)
                {
                    curStructure.UpdateConstructInfo();
                    selectObjectCallback?.Invoke(EObjectType.NONE);
                    ArrayHUDConstructCommand.Use(EHUDConstructCommand.DISPLAY_CONSTRUCT_INFO, curStructure);
                    ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                }
                // �׷��� �ʴٸ�
                else
                {
                    StructureBarrack tempBarrack = listSelectedFriendlyObject[0].GetComponent<StructureBarrack>();
                    StructureNuclear tempNuclear = listSelectedFriendlyObject[0].GetComponent<StructureNuclear>();
                    // ���� �ǹ��� �跰�̰� �������̶��
                    if (tempBarrack != null && tempBarrack.IsProcessingSpawnUnit)
                    {
                        selectObjectCallback?.Invoke(EObjectType.BARRACK);
                        tempBarrack.UpdateSpawnInfo();
                        ArrayHUDSpawnUnitCommand.Use(EHUDSpawnUnitCommand.DISPLAY_SPAWN_UNIT_INFO);
                        ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
                    }
                    // ���� �ǹ��� �� �ǹ��̰� �� �������̶��
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
            // ����Ʈ�� �Ʊ� ���� ������ ���
            else if (isFriendlyUnitInList)
            {
                // ����Ʈ�� ������ �ϳ��� ������ ���
                if (listSelectedFriendlyObject.Count < 2)
                {
                    InputOtherUnitInfo(listSelectedFriendlyObject[0]);
                    DisplaySingleUnitInfo();
                }
                // ����Ʈ�� ������ �ټ� ������ ���
                else
                {
                    InputFriendlyUnitInfo();
                    ArrayHUDCommand.Use(EHUDCommand.DISPLAY_GROUP_INFO, listSelectedFriendlyObject.Count);
                }
                selectObjectCallback?.Invoke(listSelectedFriendlyObject[0].GetObjectType());
            }
            // ����Ʈ�� �Ʊ� �ǹ��� ������ ���

        }
        // ����Ʈ�� ����ְų� �� ���ָ� ������ ���
        else
        {
            if (enemyCurSelected != null)
                DisplaySingleUnitInfo();
            else
                ArrayHUDCommand.Use(EHUDCommand.HIDE_UNIT_INFO);

            selectObjectCallback?.Invoke(EObjectType.NONE);
        }
    }

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

    public static void UpdateHp(int _listIdx = -2)
    {
        if (_listIdx.Equals(-2))
            unitInfoContainer.curHpPercent = listSelectedFriendlyObject[0].GetCurHpPercent;
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
            _obj.ActivateCircle();
        }
        else
        {
            unitInfoContainer.unitType = _obj.GetComponent<FriendlyObject>().GetUnitType;
            _obj.GetComponent<FriendlyObject>().Select();
        }
            
    }

    public void ResetTargetBunker()
    {
        foreach (FriendlyObject obj in listSelectedFriendlyObject)
            obj.ResetTargetBunker();
    }

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
            // ������ ��ġ�� 5�� ����� ���ĸ�
            CalcNewFormation(_targetPos);
        }
        else
        {
            // �뿭 �����ϸ鼭 ���̱�
            Vector3 centerPos = CalcFormationCenterPos(_targetPos.y);
            foreach (FriendlyObject obj in listSelectedFriendlyObject)
                obj.MoveByPos(_targetPos + CalcPosInFormation(obj.Position, centerPos));
        }
    }

    public static void MoveWaveEnemy(Vector3 _targetPos, SelectableObject[] _arrEnemy)
    {
        Vector3 centerPos = CalcFormationCenterPos(_targetPos.y);
        foreach (SelectableObject obj in _arrEnemy)
            obj.MoveAttack(_targetPos + CalcPosInFormation(obj.Position, centerPos));
    }

    public void Stop()
    {
        AudioManager.instance.PlayAudio_UI(objectType); // CLICK Audio
        foreach (FriendlyObject obj in listSelectedFriendlyObject)
            obj.Stop();
    }

    public void Hold()
    {
        AudioManager.instance.PlayAudio_UI(objectType); // CLICK Audio
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

    public void RegisterBroker()
    {
        Broker.Regist(this, EPublisherType.SELECTABLE_MANAGER);
    }

    public void PushMessageToBroker(EMessageType _message)
    {
        Broker.AlertMessageToSub(_message, EPublisherType.SELECTABLE_MANAGER);
    }

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

    [Header("-Melee/Ranged")]
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

    private List<SelectableObject> tempListSelectableObject = null;
    private static List<FriendlyObject> listSelectedFriendlyObject = null;

    private VoidSelectObjectTypeDelegate selectObjectCallback = null;

    private StructureBunker curBunker = null;

    private static PF_Grid grid = null;
    private static Dictionary<int, PF_Node> dicNodeUnderFriendlyUnit = null;
    private static Dictionary<int, PF_Node> dicNodeUnderEnemyUnit = null;
    private static int dicFriendlyIdx = 0;
    private static int dicEnemyIdx = 0;

    private static UnitInfoContainer unitInfoContainer = null;
    private static List<SFriendlyUnitInfo> listFriendlyUnitInfo = null;

    private MemoryPool[] arrMemoryPool = null;

    private List<FriendlyObject>[] arrCrowd = null;

    private SelectableObject enemyCurSelected = null;
    
    private EObjectType objectType;
    private EAudioType_Advisor audioType;
}
