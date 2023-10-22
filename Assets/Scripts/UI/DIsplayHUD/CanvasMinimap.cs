using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMinimap : MonoBehaviour
{
    public void Init()
    {
        imageMinimap.Init();
        ArrayHUDMinimapCommand.Add(EHUDMinimapCommand.ADD_STRUCTURE_NODE_TO_MINIMAP, new CommandAddStructureNodeToMinimap(imageMinimap));
        ArrayHUDMinimapCommand.Add(EHUDMinimapCommand.REMOVE_STRUCTURE_NODE_FROM_MINIMAP, new CommandRemoveStructureNodeFromMinimap(imageMinimap));
        ArrayHUDMinimapCommand.Add(EHUDMinimapCommand.FRIENDLY_SIGNAL, new CommandFriendlySignal(imageMinimap));
        ArrayHUDMinimapCommand.Add(EHUDMinimapCommand.ATTACK_SIGNAL, new CommandAttackSignal(imageMinimap));
        ArrayHUDMinimapCommand.Add(EHUDMinimapCommand.BIG_ENEMY_SIGNAL, new CommandBigEnemySignal(imageMinimap));
        
        //ArrayHUDCommand.Add(EHUDCommand.ADD_STRUCTURE_NODE_TO_MINIMAP, new CommandAddStructureNodeToMinimap(imageMinimap));
        //ArrayHUDCommand.Add(EHUDCommand.REMOVE_STRUCTURE_NODE_FROM_MINIMAP, new CommandRemoveStructureNodeFromMinimap(imageMinimap));

    }


    [SerializeField]
    private ImageMinimap imageMinimap = null;
}
