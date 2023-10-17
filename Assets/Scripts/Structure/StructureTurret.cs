using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureTurret : Structure
{
    public override void Init(int _structureIdx)
    {
        base.Init(_structureIdx);
        myObj.SetMyTr(turretHeadTr);
        myStructureIdx = _structureIdx;
        upgradeHpCmd = new CommandUpgradeStructureHP(GetComponent<StatusHp>());
        upgradeDmgCmd = new CommandUpgradeStructureAttDmg(myObj);
        upgradeRangeCmd = new CommandUpgradeStructureAttRange(myObj);
    }

    protected override void BuildComplete()
    {
        base.BuildComplete();
        myObj.Hold();
    }

    protected override void UpgradeComplete()
    {
        base.UpgradeComplete();
        upgradeHpCmd.Execute(upgradeHpAmount);
        upgradeDmgCmd.Execute(upgradeDmgAmount);
        upgradeRangeCmd.Execute(upgradeRangeAmount);
        Debug.Log("UpgradeCompleteTurret");
    }

    [SerializeField]
    private Transform turretHeadTr = null;

    [Header("-Upgrade Attribute")]
    [SerializeField]
    private float upgradeDmgAmount = 0f;
    [SerializeField]
    private float upgradeRangeAmount = 0f;
    [SerializeField]
    private float upgradeHpAmount = 0f;


    private CommandUpgradeStructureHP upgradeHpCmd = null;
    private CommandUpgradeStructureAttDmg upgradeDmgCmd = null;
    private CommandUpgradeStructureAttRange upgradeRangeCmd = null;
}
