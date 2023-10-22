using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureNuclear : Structure
{
    public override void Init(int _structureIdx)
    {
        base.Init(_structureIdx);
        upgradeLevel = 3;
    }

    public override void SetNuclearSpawnDelay(float _SpawnNuclearDelay)
    {
        nuclearProduceDelay = _SpawnNuclearDelay;
    }

    public void UpdateSpawnNuclearInfo()
    {
        ArrayHUDSpawnNuclearCommand.Use(EHUDSpawnNuclearCommand.UPDATE_SPAWN_NUCLEAR_TIME, progressPercent);
    }

    public bool IsProcessingSpawnNuclear => isProcessingSpawnNuclear;

    public override void CancleCurAction()
    {
        if (isProcessingUpgrade)
        {
            StopCoroutine("UpgradeCoroutine");
            isProcessingUpgrade = false;
            curUpgradeType = EUpgradeType.NONE;
        }
        else if (isProcessingConstruct)
        {
            StopCoroutine("BuildStructureCoroutine");
            isProcessingConstruct = false;
            ArrayRefundCurrencyCommand.Use(ERefuncCurrencyCommand.BUILD_STRUCTURE, myObj.GetObjectType());
            ArrayStructureFuncButtonCommand.Use(EStructureButtonCommand.DEMOLISH_COMPLETE, myStructureIdx);
            DestroyStructure();
        }
        else if (isProcessingDemolish)
        {
            StopCoroutine("DemolishCoroutine");
            isProcessingDemolish = false;
        }
        else if (isProcessingSpawnNuclear)
        {
            ArrayRefundCurrencyCommand.Use(ERefuncCurrencyCommand.SPAWN_NUCLEAR);
            StopCoroutine("SpawnNuclearCoroutine");
            isProcessingSpawnNuclear = false;
        }

        if (myObj.IsSelect)
            UpdateInfo();
    }

    public void SpawnNuclear(VoidNuclearDelegate _spwnCompleteCallback)
    {
        if(!isProcessingSpawnNuclear && !hasNuclear)
        {
            StartCoroutine("SpawnNuclearCoroutine", _spwnCompleteCallback);
        }
    }

    private IEnumerator SpawnNuclearCoroutine(VoidNuclearDelegate _spwnCompleteCallback)
    {
        isProcessingSpawnNuclear = true;
        if (myObj.IsSelect)
            ArrayUICommand.Use(EUICommand.UPDATE_INFO_UI);
        float elapsedTime = 0f;
        progressPercent = elapsedTime / nuclearProduceDelay;
        while (progressPercent < 1)
        {
            while (isPause)
                yield return null;

            if (myObj.IsSelect)
                ArrayHUDSpawnNuclearCommand.Use(EHUDSpawnNuclearCommand.UPDATE_SPAWN_NUCLEAR_TIME, progressPercent);
            // ui ǥ��
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
            progressPercent = elapsedTime / nuclearProduceDelay;
        }

        isProcessingSpawnNuclear = false;
        if (myObj.IsSelect)
            ArrayUICommand.Use(EUICommand.UPDATE_INFO_UI);
        SpawnComplete(_spwnCompleteCallback);
    }

    private void SpawnComplete(VoidNuclearDelegate _spwnCompleteCallback)
    {
        hasNuclear = true;
        curNuclear = Instantiate(myNuclear, nuclearSpawnPos, Quaternion.identity, transform).GetComponent<MissileNuclear>();
        curNuclear.SetPos(nuclearSpawnPos);
        curNuclear.Init();
        _spwnCompleteCallback?.Invoke(this);
        
        // Nuclear Ready Audio
        audioType = EAudioType_Advisor.NUCLEAR_READY;
        AudioManager.instance.PlayAudio_Advisor(audioType);
    }

    public void LaunchNuclear(Vector3 _destPos)
    {
        curNuclear.Launch(_destPos);
        hasNuclear = false;
        if (myObj.IsSelect)
            ArrayUICommand.Use(EUICommand.UPDATE_INFO_UI);
        
        // Nuclear Ready Audio
        audioType = EAudioType_Advisor.NUCLEAR_LAUNCH;
        AudioManager.instance.PlayAudio_Advisor(audioType);

        curNuclear = null;
    }



    [SerializeField]
    private Vector3 nuclearSpawnPos = Vector3.zero;
    [SerializeField]
    private GameObject myNuclear = null;

    private MissileNuclear curNuclear = null;
    private bool hasNuclear = false;
    private bool isProcessingSpawnNuclear = false;

    private float nuclearProduceDelay = 0f;
}
