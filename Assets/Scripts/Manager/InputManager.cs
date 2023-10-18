using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IMinimapObserver, IPauseObserver
{
    public void MainInit()
    {
        DontDestroyOnLoad(gameObject);
        isMainMenu = true;
    }

    public void Init(SelectableObject _mainbaseObj, VoidVoidDelegate _onDebugModeCallback)
    {
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        selectArea = GetComponentInChildren<SelectArea>();
        selectArea.Init();
        mainbaseObejct = _mainbaseObj;
        onDebugModeCallback = _onDebugModeCallback;
        isMainMenu = false;
    }
    public bool IsBuildOperation
    {
        get => isBuildOperation;
        set
        {
            ClearCurFunc();
            isBuildOperation = value;
        }
    }

    public Vector3 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void OnClickMoveButton()
    {
        AudioManager.instance.PlayAudio_UI(objectType); // CLICK Audio

        if (isMoveClick) return;
        ClearCurFunc();
        pickPosDisplayGo = Instantiate(pickPosPrefab, transform);
        isMoveClick = true;
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.DISPLAY_CANCLE_BUTTON);
    }

    public void OnClickAttackButton()
    {
        AudioManager.instance.PlayAudio_UI(objectType); // CLICK Audio

        if (isAttackClick) return;
        ClearCurFunc();
        pickPosDisplayGo = Instantiate(pickPosPrefab, transform);
        isAttackClick = true;
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.DISPLAY_CANCLE_BUTTON);
    }

    public void OnClickPatrolButton()
    {
        AudioManager.instance.PlayAudio_UI(objectType); // CLICK Audio

        if (isPatrolClick) return;
        ClearCurFunc();
        pickPosDisplayGo = Instantiate(pickPosPrefab, transform);
        isPatrolClick = true;
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.DISPLAY_CANCLE_BUTTON);
    }

    public void OnClickRallyPointButton()
    {
        AudioManager.instance.PlayAudio_UI(objectType); // CLICK Audio

        if (isRallyPointClick) return;
        ClearCurFunc();
        pickPosDisplayGo = Instantiate(pickPosPrefab, transform);
        isRallyPointClick = true;
        ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
    }

    public void OnClickLaunchNuclearButton()
    {
        AudioManager.instance.PlayAudio_UI(objectType); // CLICK Audio

        if (isLaunchNuclearClick) return;
        ClearCurFunc();
        pickPosDisplayGo = Instantiate(pickPosPrefab, transform);
        isLaunchNuclearClick = true;
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.DISPLAY_CANCLE_BUTTON);
    }

    public void CancleFunc()
    {
        isMoveClick = false;
        isAttackClick = false;
        isPatrolClick = false;
        isBuildOperation = false;
        isLaunchNuclearClick = false;
        Destroy(pickPosDisplayGo);
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.HIDE_CANCLE_BUTTON);
    }

    public void CancleRallypoint()
    {
        if (isRallyPointClick)
        {
            isRallyPointClick = false;
            Destroy(pickPosDisplayGo);
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.HIDE_CANCLE_BUTTON);
        }
    }

    private void ClearCurFunc()
    {
        isMoveClick = false;
        isAttackClick = false;
        isPatrolClick = false;
        isRallyPointClick = false;
        isBuildOperation = false;
        isLaunchNuclearClick = false;
        Destroy(pickPosDisplayGo);
        // ��� ��ɰ� ���õ� bool�� ��� �ʱ�ȭ
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.HIDE_CANCLE_BUTTON);
        ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.HIDE_CANCLE_BUTTON);
    }

    private void Update()
    {
        if (isPause) return;

        if (isDetectingChangeKey)
        {
            if (isMainMenu)
                DetectChangeKey();
            else
            {
                isDetectingChangeKey = false;
                return;
            }
        }

        elapsedTime += Time.deltaTime;

        if (isCheckDoubleClick)
        {
            if (leftClickElapsedTime > 0.5f)
            {
                isCheckDoubleClick = false;
                leftClickElapsedTime = 0f;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                isCheckDoubleClick = false;
                DoubleClickFunc();
                return;
            }
            else
                leftClickElapsedTime += Time.deltaTime;
        }

        RaycastHit hit;
        if (pickPosDisplayGo != null && Functions.Picking(out hit))
            pickPosDisplayGo.transform.position = hit.point;


        if (Input.anyKey)
            CheckIsHotkey();

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (IsBuildOperation)
            {
                ArrayMainbaseCommand.Use(EMainbaseCommnad.CONFIRM);
                return;
            }
            else if (isAttackClick)
            {
                AttackMoveWithMouseClick();
            }
            else if (isMoveClick)
            {
                MoveWithMouseClick();
            }
            else if (isPatrolClick)
            {
                PatrolWithMouseClick();
            }
            else if (isRallyPointClick)
            {
                SetRallyPoint();
            }
            else if (isLaunchNuclearClick)
                LaunchNuclear();
            else if (Input.GetKey(UnitSelectComandKey))
                SelectUnitWithCommand();
            else
                DragOperateWithMouseClick();

            if (pickPosDisplayGo != null)
                Destroy(pickPosDisplayGo);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (IsBuildOperation)
            {
                ArrayMainbaseCommand.Use(EMainbaseCommnad.CANCLE);
                return;
            }

            if (SelectableObjectManager.GetFirstSelectedObjectInList() == null)
            {
                return;
            }

            if (SelectableObjectManager.GetFirstSelectedObjectInList().GetObjectType().Equals(EObjectType.ENEMY_UNIT))
            {
                return;
            }

            if (SelectableObjectManager.GetFirstSelectedObjectInList().GetObjectType().Equals(EObjectType.BARRACK))
            {
                if (isRallyPointClick)
                    ClearCurFunc();
            }


            if (isAttackClick || isMoveClick || isPatrolClick || isRallyPointClick || isLaunchNuclearClick)
                ClearCurFunc();
            else
                MoveWithMouseClick();
        }

        CrowdHotkeyAction(Input.GetKey(arrCrowdFuncHotkey[0]));
    }

    private void LateUpdate()
    {
        ZoomCamera();
        MoveCamera();
    }

    private void CheckIsHotkey()
    {
        if (Input.GetKeyDown(arrOtherFuncHotkey[(int)EOtherFuncKey.SELECT_MAINBASE]))
        {
            ArraySelectCommand.Use(ESelectCommand.SELECT_START);
            ArraySelectCommand.Use(ESelectCommand.TEMP_SELECT, mainbaseObejct);
            ArraySelectCommand.Use(ESelectCommand.SELECT_FINISH);
            return;
        }

        if (Input.GetKeyDown(arrDeveloperMenuHotkey[(int)EDeveloperMenuKey.DISPLAY_STATE_AND_FOG]))
            onDebugModeCallback?.Invoke();


        if (SelectableObjectManager.IsListEmpty) return;

        EObjectType objType = SelectableObjectManager.GetFirstSelectedObjectInList().GetObjectType();
        switch (objType)
        {
            case EObjectType.UNIT_01:
            case EObjectType.UNIT_02:
                if (UnitDefaultHotkeyAction())
                    break;
                break;
            case EObjectType.UNIT_HERO:
                if (UnitDefaultHotkeyAction())
                    break;
                if (Input.GetKeyDown(arrUnitFuncHotkey[(int)EUnitFuncKey.LAUNCH_NUCLEAR]))
                    ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.LAUNCH_NUCLEAR);
                break;
            case EObjectType.MAIN_BASE:
                if (StructureDefaultHotkeyAction())
                    break;
                if (MainbaseBuildHotkeyAction())
                    break;
                if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.UPGRADE_ENERGY_SUPPLY]))
                    ArrayCurrencyCommand.Use(ECurrencyCommand.UPGRADE_ENERGY_SUPPLY);
                else if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.UPGRADE_POPULATION_MAX]))
                    ArrayPopulationCommand.Use(EPopulationCommand.UPGRADE_MAX_POPULATION);

                break;
            case EObjectType.TURRET:
                StructureDefaultHotkeyAction();
                break;
            case EObjectType.BUNKER:
                if (StructureDefaultHotkeyAction())
                    break;
                if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.OUT_ONE_UNIT]))
                    ArrayBunkerCommand.Use(EBunkerCommand.OUT_ONE_UNIT);
                else if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.OUT_ALL_UNIT]))
                    ArrayBunkerCommand.Use(EBunkerCommand.OUT_ALL_UNIT);
                else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.WALL]))
                    ArrayBunkerCommand.Use(EBunkerCommand.EXPAND_WALL);
                break;
            case EObjectType.WALL:
                StructureDefaultHotkeyAction();
                break;
            case EObjectType.BARRACK:
                if (StructureDefaultHotkeyAction())
                    break;
                if (Input.GetKeyDown(arrBarrackFuncHotkey[(int)EBarrackFuncKey.SPAWN_MELEE]))
                    ArrayBarrackCommand.Use(EBarrackCommand.SPAWN_UNIT, EUnitType.MELEE);
                else if (Input.GetKeyDown(arrBarrackFuncHotkey[(int)EBarrackFuncKey.SPAWN_RANGED]))
                    ArrayBarrackCommand.Use(EBarrackCommand.SPAWN_UNIT, EUnitType.RANGED);
                else if (Input.GetKeyDown(arrBarrackFuncHotkey[(int)EBarrackFuncKey.SET_RALLYPOINT]))
                    ArrayBarrackCommand.Use(EBarrackCommand.RALLYPOINT);
                else if (Input.GetKeyDown(arrBarrackFuncHotkey[(int)EBarrackFuncKey.UPGRADE_RANGED_DMG]))
                    ArrayBarrackCommand.Use(EBarrackCommand.UPGRADE_UNIT, EUnitUpgradeType.RANGED_UNIT_DMG);
                else if (Input.GetKeyDown(arrBarrackFuncHotkey[(int)EBarrackFuncKey.UPGRADE_RANGED_HP]))
                    ArrayBarrackCommand.Use(EBarrackCommand.UPGRADE_UNIT, EUnitUpgradeType.RANGED_UNIT_HP);
                else if (Input.GetKeyDown(arrBarrackFuncHotkey[(int)EBarrackFuncKey.UPGRADE_MELEE_DMG]))
                    ArrayBarrackCommand.Use(EBarrackCommand.UPGRADE_UNIT, EUnitUpgradeType.MELEE_UNIT_DMG);
                else if (Input.GetKeyDown(arrBarrackFuncHotkey[(int)EBarrackFuncKey.UPGRADE_MELEE_HP]))
                    ArrayBarrackCommand.Use(EBarrackCommand.UPGRADE_UNIT, EUnitUpgradeType.MELEE_UNIT_HP);
                break;
            case EObjectType.NUCLEAR:
                if (StructureDefaultHotkeyAction())
                    break;
                if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.SPAWN_NUCLEAR]))
                    ArrayNuclearCommand.Use(ENuclearCommand.SPAWN_NUCLEAR);
                break;
            default:
                break;
        }


    }

    private bool DeveloperMenuHotkeyActione()
    {
        if (Input.GetKeyDown(arrDeveloperMenuHotkey[(int)EDeveloperMenuKey.DISPLAY_STATE_AND_FOG]))
        {
            Debug.Log("1");
        }
        //Array ���� �Ʒ����� �������� Ŀ�ǵ� �����
        else if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.DEMOLISH]))
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DEMOLISH);
        else if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.UPGRADE]))
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.UPGRADE);
        else if (Input.GetKeyDown(cancleKey))
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.CANCLE_CURRENT_FUNCTION);
        else
            return false;
        return true;
    }

    private void CrowdHotkeyAction(bool _isCrowdCommandReady)
    {
        for (int i = (int)ECrowdFuncKey.NUM1; i <= (int)ECrowdFuncKey.NUM9; ++i)
        {
            if (Input.GetKeyDown(arrCrowdFuncHotkey[i]))
            {
                if (_isCrowdCommandReady)
                {
                    ArraySelectCommand.Use(ESelectCommand.SET_LIST_TO_CROWD, i);
                    return;
                }
                else
                {
                    ArraySelectCommand.Use(ESelectCommand.LOAD_CROWD_WITH_IDX, i);
                    return;
                }
            }
        }
    }

    private bool StructureDefaultHotkeyAction()
    {
        if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.DEMOLISH]))
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DEMOLISH);
        else if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.UPGRADE]))
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.UPGRADE);
        else if (Input.GetKeyDown(cancleKey))
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.CANCLE_CURRENT_FUNCTION);
        else
            return false;
        return true;
    }


    private bool UnitDefaultHotkeyAction()
    {
        if (Input.GetKeyDown(arrUnitFuncHotkey[(int)EUnitFuncKey.MOVE]))
            ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.MOVE);
        else if (Input.GetKeyDown(arrUnitFuncHotkey[(int)EUnitFuncKey.STOP]))
            ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.STOP);
        else if (Input.GetKeyDown(arrUnitFuncHotkey[(int)EUnitFuncKey.HOLD]))
            ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.HOLD);
        else if (Input.GetKeyDown(arrUnitFuncHotkey[(int)EUnitFuncKey.PATROL]))
            ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.PATROL);
        else if (Input.GetKeyDown(arrUnitFuncHotkey[(int)EUnitFuncKey.ATTACK]))
            ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.ATTACK);
        else
            return false;

        return true;
    }


    private bool MainbaseBuildHotkeyAction()
    {
        if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.TURRET]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.TURRET);
        else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.BUNKER]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.BUNKER);
        else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.BARRACK]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.BARRACK);
        else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.NUCLEAR]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.NUCLEAR);
        else
            return false;
        return true;
    }



    private void SetRallyPoint()
    {
        Vector3 pickPos = Vector3.zero;
        RaycastHit hit;

        if (Functions.Picking(selectableLayer, out hit))
            ArrayBarrackCommand.Use(EBarrackCommand.RALLYPOINT_CONFIRM_TR, hit.transform);
        else if (Functions.Picking("StageFloor", floorLayer, ref pickPos))
            ArrayBarrackCommand.Use(EBarrackCommand.RALLYPOINT_CONFIRM_POS, pickPos);

        GameObject pickPosDisplayGo = Instantiate(pickPosPrefab, pickPos, Quaternion.identity, transform);
        StartCoroutine("DestroypickPosDisplay", pickPosDisplayGo);

        ClearCurFunc();
    }

    private void LaunchNuclear()
    {
        Vector3 pickPos = Vector3.zero;

        if (Functions.Picking("StageFloor", floorLayer, ref pickPos))
            ArrayNuclearCommand.Use(ENuclearCommand.LAUNCH_NUCLEAR, pickPos);

        ClearCurFunc();
    }

    private void MoveWithMouseClick()
    {
        if (elapsedTime < 0.2f)
            return;
        else
            elapsedTime = 0f;


        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 pickPos = Vector3.zero;
        RaycastHit hit;
        if (Functions.Picking(selectableLayer, out hit))
        {
            ArrayUnitActionCommand.Use(EUnitActionCommand.FOLLOW_OBJECT, hit.transform);
        }
        else if (Functions.Picking("StageFloor", floorLayer, ref pickPos))
        {
            GameObject pickPosDisplayGo = Instantiate(pickPosPrefab, pickPos, Quaternion.identity, transform);
            StartCoroutine("DestroypickPosDisplay", pickPosDisplayGo);
            ArrayUnitActionCommand.Use(EUnitActionCommand.MOVE_WITH_POS, pickPos);
        }

        ClearCurFunc();
    }

    private void AttackMoveWithMouseClick()
    {
        if (elapsedTime < 0.2f)
            return;
        else
            elapsedTime = 0f;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 pickPos = Vector3.zero;
        RaycastHit hit;
        if (Functions.Picking(selectableLayer, out hit))
            ArrayUnitActionCommand.Use(EUnitActionCommand.FOLLOW_OBJECT, hit.transform);
        else if (Functions.Picking("StageFloor", floorLayer, ref pickPos))
        {
            GameObject pickPosDisplayGo = Instantiate(pickPosPrefab, pickPos, Quaternion.identity, transform);
            StartCoroutine("DestroypickPosDisplay", pickPosDisplayGo);
            ArrayUnitActionCommand.Use(EUnitActionCommand.MOVE_ATTACK, pickPos);
        }

        ClearCurFunc();
    }

    private void PatrolWithMouseClick()
    {
        if (elapsedTime < 0.2f)
            return;
        else
            elapsedTime = 0f;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 pickPos = Vector3.zero;
        RaycastHit hit;
        if (Functions.Picking(selectableLayer, out hit))
            ArrayUnitActionCommand.Use(EUnitActionCommand.FOLLOW_OBJECT, hit.transform);
        else if (Functions.Picking("StageFloor", floorLayer, ref pickPos))
        {
            GameObject pickPosDisplayGo = Instantiate(pickPosPrefab, pickPos, Quaternion.identity, transform);
            StartCoroutine("DestroypickPosDisplay", pickPosDisplayGo);
            ArrayUnitActionCommand.Use(EUnitActionCommand.PATROL, pickPos);
        }

        ClearCurFunc();
    }

    private IEnumerator DestroypickPosDisplay(GameObject _go)
    {
        yield return new WaitForSeconds(pickPosDisplayHideDelay);
        Destroy(_go);
    }

    private void DoubleClickFunc()
    {
        Debug.Log("doubleClick");
        //RaycastHit hit;

        //if (Functions.Picking(selectableLayer, out hit))
        //{
        //    SelectableObject sObj = hit.transform.GetComponent<SelectableObject>();

        //    if (hit.transform.Equals(SelectableObjectManager.GetFirstSelectedObjectInList().transform))
        //    {
        //        Debug.Log("double Click");
        //        // ���⼭ ī�޶� Ŀ�ǵ�� �ڽ�ĳ��Ʈ�� ������ �ڽ��� ȭ�鳻�� selectable�� ã�Ƴ���
        //        // �� �迭 �޾ƿͼ� �� �迭�� �ִ� �ֵ� �� hit�� Ÿ�� ���� �ֵ鸸 ��� temp�� ������ �� �־��ֱ�
        //    }

        //    isCheckDoubleClick = false;
        //    return;
        //}
    }

    private void SelectUnitWithCommand()
    {
        if (SelectableObjectManager.GetFirstSelectedObjectInList().GetUnitType.Equals(EUnitType.NONE)) return;
        Debug.Log("!");

        RaycastHit hit;

        if (Functions.Picking(friendlyLayer, out hit))
        {
            FriendlyObject tempFObj = hit.transform.GetComponent<FriendlyObject>();

            if (tempFObj.IsSelect)
                ArraySelectCommand.Use(ESelectCommand.REMOVE_FROM_LIST, tempFObj);
            else
                ArraySelectCommand.Use(ESelectCommand.ADD_TO_LIST, tempFObj);
        }
    }

    private void DragOperateWithMouseClick()
    {
        RaycastHit hit;

        if (Functions.Picking(selectableLayer, out hit))
        {
            ArraySelectCommand.Use(ESelectCommand.SELECT_START);
            ArraySelectCommand.Use(ESelectCommand.TEMP_SELECT, hit.transform.GetComponent<SelectableObject>());
        }

        Functions.Picking("StageFloor", floorLayer, ref dragStartPos);
        selectArea.SetPos(dragStartPos);
        selectArea.SetLocalScale(Vector3.zero);
        selectArea.SetActive(true);
        isCheckDoubleClick = true;

        StartCoroutine("DragCoroutine");
    }

    private IEnumerator DragCoroutine()
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
                break;

            Functions.Picking("StageFloor", floorLayer, ref dragEndPos);
            selectArea.SetLocalScale(Quaternion.Euler(0f, -45f, 0f) * (dragEndPos - dragStartPos));

            yield return null;
        }

        ArraySelectCommand.Use(ESelectCommand.SELECT_FINISH);
        selectArea.SetActive(false);
    }

    private void ZoomCamera()
    {
        ArrayCameraMoveCommand.Use(ECameraCommand.ZOOM, Input.GetAxisRaw("Mouse ScrollWheel"));
    }

    private void MoveCamera()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!SelectableObjectManager.IsListEmpty)
                ArrayCameraMoveCommand.Use(ECameraCommand.MOVE_WITH_OBJECT);
        }
        else if (Input.GetAxisRaw("Horizontal Arrow").Equals(0) && Input.GetAxisRaw("Vertical Arrow").Equals(0))
            ArrayCameraMoveCommand.Use(ECameraCommand.MOVE_WITH_MOUSE, Input.mousePosition);
        else
            ArrayCameraMoveCommand.Use(
                ECameraCommand.MOVE_WITH_KEY,
                new Vector2(Input.GetAxisRaw("Horizontal Arrow"), Input.GetAxisRaw("Vertical Arrow"))
                );
    }

    public void GetUnitTargetPos(Vector3 _pos)
    {
        ArrayUnitActionCommand.Use(EUnitActionCommand.MOVE_WITH_POS, _pos);
    }

    public void GetCameraTargetPos(Vector3 _pos)
    {
        ArrayCameraMoveCommand.Use(ECameraCommand.WARP_WITH_POS, _pos);
    }

    public void CheckPause(bool _isPause)
    {
        isPause = _isPause;
    }

    public void ChangeUnitHotkey(EUnitFuncKey _targetHotkey)
    {
        isDetectingChangeKey = true;
        SetCurChangeHotKey(arrUnitFuncHotkey, _targetHotkey);
    }

    public void ChangeBuildHotkey(EBuildFuncKey _targetHotkey)
    {
        isDetectingChangeKey = true;
        SetCurChangeHotKey(arrBuildFuncHotkey, _targetHotkey);
    }

    public void ChangeBarrackHotkey(EBarrackFuncKey _targetHotkey)
    {
        isDetectingChangeKey = true;
        SetCurChangeHotKey(arrBarrackFuncHotkey, _targetHotkey);
    }

    public void ChangeStructureFuncHotkey(EStructureFuncKey _targetHotkey)
    {
        isDetectingChangeKey = true;
        SetCurChangeHotKey(arrStructureFuncHotkey, _targetHotkey);
    }

    public void ChangeOtherFuncHotkey(EOtherFuncKey _targetHotkey)
    {
        isDetectingChangeKey = true;
        SetCurChangeHotKey(arrStructureFuncHotkey, _targetHotkey);
    }

    private void SetCurChangeHotKey<T>(KeyCode[] _curChangeHotKey, T _unitHotkeyIdx) where T : Enum
    {
        curChangeKeyCode = _curChangeHotKey;
        curChangeKeyIdx = Convert.ToInt32(_unitHotkeyIdx);
    }

    private void DetectChangeKey()
    {
        if (Input.anyKey)
        {
            KeyCode curKey = FindInputKey();
            if (CheckIsChangable(curKey))
            {
                curChangeKeyCode[curChangeKeyIdx] = curKey;
                if (curChangeKeyCode.Equals(arrUnitFuncHotkey))
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.CONFIRM_UNIT_FUNC_BUTTON, curChangeKeyIdx, curKey);
                else if (curChangeKeyCode.Equals(arrStructureFuncHotkey))
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.CONFIRM_STRUCTURE_FUNC_BUTTON, curChangeKeyIdx, curKey);
                else if (curChangeKeyCode.Equals(arrBuildFuncHotkey))
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.CONFIRM_BUILD_FUNC_BUTTON, curChangeKeyIdx, curKey);
                else if (curChangeKeyCode.Equals(arrBarrackFuncHotkey))
                    ArrayChangeHotkeyCommand.Use(EChangeHotkeyCommand.CONFIRM_BARRACK_FUNC_BUTTON, curChangeKeyIdx, curKey);

            }
            curChangeKeyCode = null;
            curChangeKeyIdx = -1;

            isDetectingChangeKey = false;
        }
    }

    private KeyCode FindInputKey()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            // �ش� Ű�� ���ȴ��� Ȯ���մϴ�.
            if (Input.GetKeyDown(keyCode))
            {
                return keyCode;
            }
        }
        return KeyCode.None;
    }

    private bool CheckIsChangable(KeyCode _changeKey)
    {
        if (_changeKey.Equals(KeyCode.None))
            return false;

        for (int i = 0; i < arrUnitFuncHotkey.Length; ++i)
        {
            if (_changeKey.Equals(arrUnitFuncHotkey[i]))
                return false;
        }

        for (int i = 0; i < arrBuildFuncHotkey.Length; ++i)
        {
            if (_changeKey.Equals(arrBuildFuncHotkey[i]))
                return false;
        }

        for (int i = 0; i < arrBarrackFuncHotkey.Length; ++i)
        {
            if (_changeKey.Equals(arrBarrackFuncHotkey[i]))
                return false;
        }

        for (int i = 0; i < arrStructureFuncHotkey.Length; ++i)
        {
            if (_changeKey.Equals(arrStructureFuncHotkey[i]))
                return false;
        }

        for (int i = 0; i < arrCrowdFuncHotkey.Length; ++i)
        {
            if (_changeKey.Equals(arrCrowdFuncHotkey[i]))
                return false;
        }

        for (int i = 0; i < arrOtherFuncHotkey.Length; ++i)
        {
            if (_changeKey.Equals(arrOtherFuncHotkey[i]))
                return false;
        }

        for (int i = 0; i < arrDeveloperMenuHotkey.Length; ++i)
        {
            if (_changeKey.Equals(arrDeveloperMenuHotkey[i]))
                return false;
        }

        if (_changeKey.Equals(cancleKey))
            return false;

        return true;
    }

    [SerializeField]
    private GameObject pickPosPrefab = null;
    [SerializeField]
    private float pickPosDisplayHideDelay = 0.3f;
    [SerializeField]
    private LayerMask floorLayer;
    [SerializeField]
    private LayerMask selectableLayer;
    [SerializeField]
    private LayerMask friendlyLayer;

    [Header("-Hotkeys")]
    [SerializeField]
    private KeyCode[] arrUnitFuncHotkey = null;
    [SerializeField]
    private KeyCode[] arrBuildFuncHotkey = null;
    [SerializeField]
    private KeyCode[] arrBarrackFuncHotkey = null;
    [SerializeField]
    private KeyCode[] arrStructureFuncHotkey = null;
    [SerializeField]
    private KeyCode[] arrCrowdFuncHotkey = null;
    [SerializeField]
    private KeyCode[] arrOtherFuncHotkey = null;
    [SerializeField]
    private KeyCode cancleKey = KeyCode.Escape;
    [SerializeField]
    private KeyCode UnitSelectComandKey = KeyCode.LeftShift;

    [Header("-Debug Mode Hotkey")]
    [SerializeField]
    private KeyCode[] arrDeveloperMenuHotkey = null;

    private float elapsedTime = 0f;
    private float leftClickElapsedTime = 0f;

    private bool isMoveClick = false;
    private bool isAttackClick = false;
    private bool isPatrolClick = false;
    private bool isBuildOperation = false;
    private bool isRallyPointClick = false;
    private bool isLaunchNuclearClick = false;
    private bool isCheckDoubleClick = false;
    private bool isPause = false;

    private GameObject pickPosDisplayGo = null;

    private Vector3 dragStartPos = Vector3.zero;
    private Vector3 dragEndPos = Vector3.zero;

    private SelectArea selectArea = null;

    private EObjectType objectType = EObjectType.NONE;
    private SelectableObject mainbaseObejct = null;
    private VoidVoidDelegate onDebugModeCallback = null;

    [SerializeField]
    private bool isMainMenu = false;
    [SerializeField]
    private bool isDetectingChangeKey = false;

    private KeyCode[] curChangeKeyCode = null;
    private int curChangeKeyIdx = -1;
}