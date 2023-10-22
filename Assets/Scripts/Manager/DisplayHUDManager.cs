using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayHUDManager : MonoBehaviour
{
    public void Init()
    {
        canvasMinimap = GetComponentInChildren<CanvasMinimap>();
        canvasWaveInfo = GetComponentInChildren<CanvasWaveInfo>();
        canvasUnitInfo = GetComponentInChildren<CanvasUnitInfo>();
        canvaHeroRessurection = GetComponentInChildren<CanvasHeroRessurection>();
        canvasSpawnUnitInfo = GetComponentInChildren<CanvasSpawnUnitInfo>();
        canvasUpgradeInfo = GetComponentInChildren<CanvasUpgradeInfo>();
        canvasConstructInfo = GetComponentInChildren<CanvasConstructInfo>();
        canvasDemolishInfo = GetComponentInChildren<CanvasDemolishInfo>();
        canvasNuclearInfo = GetComponentInChildren<CanvasSpawnNuclearInfo>();
        canvasMenu = GetComponentInChildren<CanvasMenu>();
        canvasTooltip = GetComponentInChildren<CanvasTooltip>();
        canvasAlert = GetComponentInChildren<CanvasAlert>();

        canvasMinimap.Init();
        canvasWaveInfo.Init();
        canvasUnitInfo.Init();
        canvaHeroRessurection.Init();
        canvasSpawnUnitInfo.Init();
        canvasUpgradeInfo.Init();
        canvasConstructInfo.Init();
        canvasDemolishInfo.Init();
        canvasNuclearInfo.Init();
        canvasMenu.Init();
        canvasTooltip.Init();
        canvasAlert.Init();

        ArrayHUDCommand.Add(EHUDCommand.INIT_WAVE_TIME, new CommandInitWaveTime(canvasWaveInfo));
        ArrayHUDCommand.Add(EHUDCommand.UPDATE_WAVE_TIME, new CommandUpdateWaveTime(canvasWaveInfo));

        ArrayHUDCommand.Add(EHUDCommand.INIT_DISPLAY_GROUP_INFO, new CommandInitDisplayGroupUnitInfo(canvasUnitInfo));
        ArrayHUDCommand.Add(EHUDCommand.INIT_DISPLAY_SINGLE_INFO, new CommandInitDisplaySingleUnitInfo(canvasUnitInfo));
        ArrayHUDCommand.Add(EHUDCommand.DISPLAY_GROUP_INFO, new CommandDisplayGroupUnitInfo(canvasUnitInfo, canvasSpawnUnitInfo));
        ArrayHUDCommand.Add(EHUDCommand.DISPLAY_SINGLE_INFO, new CommandDisplaySingleUnitInfo(canvasUnitInfo));
        ArrayHUDCommand.Add(EHUDCommand.DISPLAY_SINGLE_STRUCTURE_INFO, new CommandDisplaySingleStructureInfo(canvasUnitInfo));
        ArrayHUDCommand.Add(EHUDCommand.HIDE_UNIT_INFO, new CommandHideUnitInfo(canvasUnitInfo));
        ArrayHUDCommand.Add(EHUDCommand.HERO_RESURRECTION_UPDATE, new CommandHeroRessurectionUpdate(canvaHeroRessurection));
        ArrayHUDCommand.Add(EHUDCommand.HERO_RESSURECTION_FINISH, new CommandHeroRessurectionFinish(canvaHeroRessurection));
        ArrayHUDCommand.Add(EHUDCommand.HIDE_ALL_INFO, new CommandHideAllInfo(this));
        ArrayHUDCommand.Add(EHUDCommand.DISPLAY_TOOLTIP, new CommandDisplayTooltip(canvasTooltip));
        ArrayHUDCommand.Add(EHUDCommand.HIDE_TOOLTIP, new CommandHideTooltip(canvasTooltip));

        ArrayHUDUpgradeCommand.Add(EHUDUpgradeCommand.DISPLAY_UPGRADE_INFO, new CommandDisplayUpgradeInfo(canvasUpgradeInfo));
        ArrayHUDUpgradeCommand.Add(EHUDUpgradeCommand.UPDATE_UPGRADE_TIME, new CommandUpdateUpgradeTime(canvasUpgradeInfo));

        ArrayHUDSpawnUnitCommand.Add(EHUDSpawnUnitCommand.UPDATE_SPAWN_UNIT_LIST, new CommandUpdateSpawnUnitList(canvasSpawnUnitInfo));
        ArrayHUDSpawnUnitCommand.Add(EHUDSpawnUnitCommand.UPDATE_SPAWN_UNIT_TIME, new CommandUpdateSpawnUnitTime(canvasSpawnUnitInfo));
        ArrayHUDSpawnUnitCommand.Add(EHUDSpawnUnitCommand.DISPLAY_SPAWN_UNIT_INFO, new CommandDisplaySpawnUnitInfo(canvasSpawnUnitInfo));

        ArrayHUDConstructCommand.Add(EHUDConstructCommand.DISPLAY_CONSTRUCT_INFO, new CommandDisplayConstructInfo(canvasConstructInfo));
        ArrayHUDConstructCommand.Add(EHUDConstructCommand.UPDATE_CONSTRUCT_TIME, new CommandUpdateConstructTime(canvasConstructInfo));
        ArrayHUDConstructCommand.Add(EHUDConstructCommand.UPDATE_CONSTRUCT_STRUCTURE, new CommandUpdateConstructStructure(canvasConstructInfo));
        ArrayHUDConstructCommand.Add(EHUDConstructCommand.DISPLAY_DEMOLISH_INFO, new CommandDisplayDemolishInfo(canvasDemolishInfo));
        ArrayHUDConstructCommand.Add(EHUDConstructCommand.UPDATE_DEMOLISH_TIME, new CommandUpdateDemolishTime(canvasDemolishInfo));
        ArrayHUDConstructCommand.Add(EHUDConstructCommand.UPDATE_DEMOLISH_STRUCTURE, new CommandUpdateDemolishStructure(canvasDemolishInfo));

        ArrayHUDSpawnNuclearCommand.Add(EHUDSpawnNuclearCommand.DISPLAY_SPAWN_NUCLEAR_INFO, new CommandDisplaySpawnNuclearInfo(canvasNuclearInfo));
        ArrayHUDSpawnNuclearCommand.Add(EHUDSpawnNuclearCommand.UPDATE_SPAWN_NUCLEAR_TIME, new CommandUpdateSpawnNuclearTime(canvasNuclearInfo));

        ArrayAlertCommand.Add(EAlertCommand.UNDER_ATTACK, new CommandAlertUnderAttack(canvasAlert));
        ArrayAlertCommand.Add(EAlertCommand.UPGRADE_COMPLETE, new CommandAlertUpgradeComplete(canvasAlert));
        ArrayAlertCommand.Add(EAlertCommand.BUILD_COMPLETE, new CommandAlertBuildComplete(canvasAlert));
        ArrayAlertCommand.Add(EAlertCommand.WAVE_START, new CommandAlertWaveStart(canvasAlert));
    }

    public void HideDisplay()
    {
        canvasUnitInfo.HideDisplay();
        canvasSpawnUnitInfo.HideDisplay();
        canvasUpgradeInfo.HideDisplay();
        canvasConstructInfo.HideDisplay();
        canvasDemolishInfo.HideDisplay();
        canvasNuclearInfo.HideDisplay();
        canvasTooltip.HideTooltip();
    }

    public void HeroDead()
    {
        canvaHeroRessurection.SetActive(true);
    }

    private CanvasMinimap canvasMinimap = null;
    private CanvasWaveInfo canvasWaveInfo = null;
    private CanvasUnitInfo canvasUnitInfo = null;
    private CanvasHeroRessurection canvaHeroRessurection = null;
    private CanvasSpawnUnitInfo canvasSpawnUnitInfo = null;
    private CanvasUpgradeInfo canvasUpgradeInfo = null;
    private CanvasConstructInfo canvasConstructInfo = null;
    private CanvasDemolishInfo canvasDemolishInfo = null;
    private CanvasSpawnNuclearInfo canvasNuclearInfo = null;
    private CanvasMenu canvasMenu = null;
    private CanvasTooltip canvasTooltip = null;
    private CanvasAlert canvasAlert = null;
}
