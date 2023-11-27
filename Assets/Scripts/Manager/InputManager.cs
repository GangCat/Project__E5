using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IMinimapObserver, IPauseObserver
{
    public void Init(SelectableObject _mainbaseObj, VoidVoidDelegate _onDebugModeCallback)
    {
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        selectArea = GetComponentInChildren<SelectArea>();
        selectArea.Init();
        mainbaseObejct = _mainbaseObj;
        onDebugModeCallback = _onDebugModeCallback;
        isInGame = true;
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

    #region OnClickMethods
    public void OnClickMoveButton()
    {
        if (isMoveClick) return;
        isMoveClick = true;

        OnClickUnitCtrlButton();
    }

    public void OnClickAttackButton()
    {
        if (isAttackClick) return;
        isAttackClick = true;

        OnClickUnitCtrlButton();
    }

    public void OnClickPatrolButton()
    {
        if (isPatrolClick) return;
        isPatrolClick = true;

        OnClickUnitCtrlButton();
    }

    public void OnClickLaunchNuclearButton()
    {
        if (isLaunchNuclearClick) return;
        isLaunchNuclearClick = true;

        OnClickUnitCtrlButton();
    }

    private void OnClickUnitCtrlButton()
    {
        AudioManager.instance.PlayAudio_UI(); // CLICK Audio
        ClearCurFunc();
        pickPosDisplayGo = Instantiate(pickPosPrefab, transform);
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.DISPLAY_CANCLE_BUTTON);
    }

    public void OnClickRallyPointButton()
    {
        if (isRallyPointClick) return;
        isRallyPointClick = true;

        AudioManager.instance.PlayAudio_UI(); // CLICK Audio
        ClearCurFunc();
        pickPosDisplayGo = Instantiate(pickPosPrefab, transform);
        ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DISPLAY_CANCLE_BUTTON);
    }
    #endregion

    #region CancleMethods
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
    #endregion

    /// <summary>
    /// 모든 bool값 초기화 및 버튼 숨기기
    /// </summary>
    private void ClearCurFunc()
    {
        isMoveClick = false;
        isAttackClick = false;
        isPatrolClick = false;
        isRallyPointClick = false;
        isBuildOperation = false;
        isLaunchNuclearClick = false;
        Destroy(pickPosDisplayGo);
        ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.HIDE_CANCLE_BUTTON);
        ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.HIDE_CANCLE_BUTTON);
    }

    
    private void Update()
    {
        if (isPause) return;

        // 키 변경하는 기능 넣으려고 작성했지만 시간이 부족해 넣지 못함.
        if (isDetectingChangeKey)
        {
            DetectChangeKey();
        }

        elapsedTime += Time.deltaTime;

        // 더블클릭 확인
        if (isCheckDoubleClick)
        {
            if (leftClickElapsedTime > doubleClickDelay)
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

        if (pickPosDisplayGo != null && Functions.Picking(out var hit))
            pickPosDisplayGo.transform.position = hit.point;


        if (Input.anyKey)
            CheckIsHotkey();

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (isBuildOperation)
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
            if (isBuildOperation)
            {
                ArrayMainbaseCommand.Use(EMainbaseCommnad.CANCLE);
                return;
            }

            if (SelectableObjectManager.GetFirstSelectedObjectInList() == null)
                return;

            if (SelectableObjectManager.GetFirstSelectedObjectInList().GetObjectType().Equals(EObjectType.ENEMY_SMALL))
                return;

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
        if (isInGame)
        {
            ZoomCamera();
            MoveCamera();
        }
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

        if (Input.GetKey(arrDebugModeHotkey[(int)EDeveloperMenuKey.COMMAND_KEY]))
            if (DeveloperMenuHotkeyAction())
                return;

        if (SelectableObjectManager.IsListEmpty) return;
        if (SelectableObjectManager.IsEnemyUnitInList) return;
        if (!SelectableObjectManager.GetFirstSelectedObjectInList()) return;
        
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
                else
                    HeroHotkeyAction();
                break;
            case EObjectType.MAIN_BASE:
                if (StructureDefaultHotkeyAction())
                    break;
                else
                    MainbaseBuildHotkeyAction();
                break;
            case EObjectType.TURRET:
                StructureDefaultHotkeyAction();
                break;
            case EObjectType.BUNKER:
                if (StructureDefaultHotkeyAction())
                    break;
                else
                    BunkerHotkeyAction();
                break;
            case EObjectType.WALL:
                StructureDefaultHotkeyAction();
                break;
            case EObjectType.BARRACK:
                if (StructureDefaultHotkeyAction())
                    break;
                else
                    BarrackHotkeyAction();
                break;
            case EObjectType.NUCLEAR:
                if (StructureDefaultHotkeyAction())
                    break;
                else
                    NuclearHotkeyAction();
                break;
            default:
                break;
        }
    }
    #region HotkeyActions
    private void CrowdHotkeyAction(bool _isCrowdCommandReady)
    {
        for (int i = (int)ECrowdFuncKey.NUM1; i <= (int)ECrowdFuncKey.NUM9; ++i)
        {
            if (!Input.GetKeyDown(arrCrowdFuncHotkey[i]))
                continue;

            if (_isCrowdCommandReady)
            {
                ArraySelectCommand.Use(ESelectCommand.SAVE_LIST_TO_CROWD, i);
                return;
            }
            else
            {
                ArraySelectCommand.Use(ESelectCommand.LOAD_CROWD_WITH_IDX, i);
                return;
            }
        }
    }

    private void NuclearHotkeyAction()
    {
        if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.SPAWN_NUCLEAR]))
            ArrayNuclearCommand.Use(ENuclearCommand.SPAWN_NUCLEAR);
    }

    private void HeroHotkeyAction()
    {
        if (Input.GetKeyDown(arrUnitFuncHotkey[(int)EUnitFuncKey.LAUNCH_NUCLEAR]))
            ArrayUnitFuncButtonCommand.Use(EUnitFuncButtonCommand.LAUNCH_NUCLEAR);
    }

    private bool DeveloperMenuHotkeyAction()
    {
        if (Input.GetKeyDown(arrDebugModeHotkey[(int)EDeveloperMenuKey.DISPLAY_STATE_AND_FOGTEXTURE]))
            onDebugModeCallback?.Invoke();
        else if (Input.GetKeyDown(arrDebugModeHotkey[(int)EDeveloperMenuKey.TOGGLE_FOG]))
            ArrayDebugModeCommand.Use(EDebugModeCommand.TOGGLE_FOG);
        else if (Input.GetKeyDown(arrDebugModeHotkey[(int)EDeveloperMenuKey.FAST_BUILD]))
            ArrayDebugModeCommand.Use(EDebugModeCommand.DELAY_FAST);
        else if (Input.GetKeyDown(arrDebugModeHotkey[(int)EDeveloperMenuKey.MONEY_INFLATION]))
            ArrayDebugModeCommand.Use(EDebugModeCommand.MONEY_INFLATION);
        else
            return false;

        return true;
    }

    private void BunkerHotkeyAction()
    {
        if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.OUT_ONE_UNIT]))
            ArrayBunkerCommand.Use(EBunkerCommand.OUT_ONE_UNIT);
        else if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.OUT_ALL_UNIT]))
            ArrayBunkerCommand.Use(EBunkerCommand.OUT_ALL_UNIT);
        else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.WALL]))
            ArrayBunkerCommand.Use(EBunkerCommand.EXPAND_WALL);
    }

    private void BarrackHotkeyAction()
    {
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

    private void MainbaseBuildHotkeyAction()
    {
        if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.TURRET]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.TURRET);
        else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.BUNKER]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.BUNKER);
        else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.BARRACK]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.BARRACK);
        else if (Input.GetKeyDown(arrBuildFuncHotkey[(int)EBuildFuncKey.NUCLEAR]))
            ArrayMainbaseCommand.Use(EMainbaseCommnad.BUILD_STRUCTURE, EObjectType.NUCLEAR);
        else if(Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.UPGRADE_ENERGY_SUPPLY]))
            ArrayCurrencyCommand.Use(ECurrencyCommand.UPGRADE_ENERGY_SUPPLY);
        else if (Input.GetKeyDown(arrStructureFuncHotkey[(int)EStructureFuncKey.UPGRADE_POPULATION_MAX]))
            ArrayPopulationCommand.Use(EPopulationCommand.UPGRADE_MAX_POPULATION);
    }
    #endregion

    /// <summary>
    /// 피킹한 위치를 배럭의 랠리포인트로 설정
    /// </summary>
    private void SetRallyPoint()
    {
        Vector3 pickPos = Vector3.zero;

        if (Functions.Picking(selectableLayer, out var hit))
            ArrayBarrackCommand.Use(EBarrackCommand.RALLYPOINT_CONFIRM_TR, hit.transform);
        else if (Functions.Picking("StageFloor", floorLayer, ref pickPos)) // ref는 변수를 참조하는 것이기 때문에 사전에 자료형을 정해줘야함.
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
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 pickPos = Vector3.zero;
        if (Functions.Picking(selectableLayer, out var hit))
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
        // 0.2초 안에 또 우클릭을 할때는 코드가 호출되지 않도록, 너무 빠른 입력을 받지 않도록 예외처리
        if (elapsedTime < 0.2f)
            return;
        else
            elapsedTime = 0f;

        // UI 위를 우클릭할때는 이동하지 못하도록 예외처리
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 pickPos = Vector3.zero;
        if (Functions.Picking(selectableLayer, out var hit))
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
        if (Functions.Picking(selectableLayer, out var hit))
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
        if (SelectableObjectManager.IsListEmpty) return;

        EUnitType curUnitType = SelectableObjectManager.GetFirstSelectedObjectInList().GetUnitType;
        // 건물일 경우 예외처리
        if (curUnitType.Equals(EUnitType.NONE)) return;

        ArraySelectCommand.Use(ESelectCommand.REMOVE_FROM_LIST, SelectableObjectManager.GetFirstSelectedObjectInList());

        // 카메라의 위치에서 바라보는 방향으로 오버랩 박스를 해서 충돌하는 모든 선택가능한 오브젝트를 불러옴.
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, 1000f, floorLayer);
        Collider[] arrCol = Physics.OverlapBox(
            hit.point, 
            new Vector3(
                Camera.main.orthographicSize * Camera.main.aspect, 
                0f, 
                Camera.main.orthographicSize / Mathf.Cos(90 - rotAngleXForDoubleClick * Mathf.Deg2Rad)), 
            Quaternion.Euler(new Vector3(0f, 45f, 0f)), 
            friendlyLayer
        );

        FriendlyObject tempFriendlyObj = null;

        for (int i = 0; i < arrCol.Length; ++i)
        {
            tempFriendlyObj = arrCol[i].GetComponent<FriendlyObject>();

            if (tempFriendlyObj.GetUnitType.Equals(curUnitType))
            {
                if (SelectableObjectManager.IsListFull) return;
                ArraySelectCommand.Use(ESelectCommand.ADD_TO_LIST, tempFriendlyObj);
            }
        }

    }

    /// <summary>
    /// 시프트 클릭으로 여러마리 지정할 때 리스트에 넣을지 뺄지 결정하는 코드
    /// </summary>
    private void SelectUnitWithCommand()
    {
        if (SelectableObjectManager.GetFirstSelectedObjectInList().GetUnitType.Equals(EUnitType.NONE)) return;

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

    /// <summary>
    /// 방향키, 키보드로 카메라 조작
    /// </summary>
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

    /// <summary>
    /// 미니맵 우클릭으로 이동시키는 경우
    /// </summary>
    /// <param name="_pos"></param>
    public void GetUnitTargetPos(Vector3 _pos)
    {
        ArrayUnitActionCommand.Use(EUnitActionCommand.MOVE_WITH_POS, _pos);
    }

    /// <summary>
    /// 미니맵 좌클릭으로 카메라 이동시키는 경우
    /// </summary>
    /// <param name="_pos"></param>
    public void GetCameraTargetPos(Vector3 _pos)
    {
        ArrayCameraMoveCommand.Use(ECameraCommand.WARP_WITH_POS, _pos);
    }

    public void CheckPause(bool _isPause)
    {
        isPause = _isPause;
    }

    // UI에서 핫키 바꾸는 버튼을 누르면 호출됨.
    // 이 때 어떤 키를 바꿀지는 UI에서 누른 버튼마다 다름. 
    #region ChangeHotkey
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

    /// <summary>
    /// 자료형이 Enum형인 T만 받는 함수.
    /// 현재 바꾸기 원하는 키가 어떤 키인지 확인.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_curChangeHotKey"></param>
    /// <param name="_unitHotkeyIdx"></param>
    private void SetCurChangeHotKey<T>(KeyCode[] _curChangeHotKey, T _unitHotkeyIdx) where T : Enum
    {
        curChangeKeyCode = _curChangeHotKey;
        curChangeKeyIdx = Convert.ToInt32(_unitHotkeyIdx);
    }

    // 바꾸기 원하는 키에 해당하는 키 변경 버튼을 누린 뒤 여기로 들어옴.
    private void DetectChangeKey()
    {
        if (Input.anyKey)
        {
            KeyCode curKey = FindInputKey();
            if (CheckIsChangable(curKey))
            {
                curChangeKeyCode[curChangeKeyIdx] = curKey;

                // 배열을 먼저 확인하고 해당하는 키를 변경하도록 커맨드 사용
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
            // 모든 키코드를 전부 다 확인해서 동일한거 나오면 해당 키코드 반환.
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
        if (!CheckIsChangable(_changeKey, arrUnitFuncHotkey))
            return false;
        if (!CheckIsChangable(_changeKey, arrBuildFuncHotkey))
            return false;
        if (!CheckIsChangable(_changeKey, arrBarrackFuncHotkey))
            return false;
        if (!CheckIsChangable(_changeKey, arrStructureFuncHotkey))
            return false;
        if (!CheckIsChangable(_changeKey, arrCrowdFuncHotkey))
            return false;
        if (!CheckIsChangable(_changeKey, arrOtherFuncHotkey))
            return false;
        if (!CheckIsChangable(_changeKey, arrDebugModeHotkey))
            return false;
        if (_changeKey.Equals(cancleKey))
            return false;

        return true;
    }

    private bool CheckIsChangable(KeyCode _changeKey, KeyCode[] _targetKeyArray)
    {
        for(int i = 0; i < _targetKeyArray.Length; ++i)
        {
            if (_changeKey.Equals(_targetKeyArray[i]))
                return false;
        }
        return true;
    }
    #endregion

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
    [SerializeField]
    private float rotAngleXForDoubleClick = 0f;
    [SerializeField]
    private float doubleClickDelay = 0.3f;

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
    [SerializeField]
    private bool isDetectingChangeKey = false;

    [Header("-Debug Mode Hotkey")]
    [SerializeField]
    private KeyCode[] arrDebugModeHotkey = null;

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
    private bool isInGame = false;

    private GameObject pickPosDisplayGo = null;
    private Vector3 dragStartPos = Vector3.zero;
    private Vector3 dragEndPos = Vector3.zero;
    private SelectArea selectArea = null;
    private SelectableObject mainbaseObejct = null;
    private VoidVoidDelegate onDebugModeCallback = null;

    private KeyCode[] curChangeKeyCode = null;
    private int curChangeKeyIdx = -1;

}