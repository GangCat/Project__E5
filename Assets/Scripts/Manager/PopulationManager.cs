using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour, IPublisher
{
    public void Init()
    {
        ArrayPopulationCommand.Use(EPopulationCommand.UPDATE_CURRENT_POPULATION_HUD, curPopulation);
        ArrayPopulationCommand.Use(EPopulationCommand.UPDATE_CURRENT_MAX_POPULATION_HUD, curMaxPopulation);
        RegisterBroker();
    }

    public uint CurPopulation => curPopulation;

    public bool CanUpgradePopulation()
    {
        return curMaxPopulation < maxPopulation;
    }

    /// <summary>
    /// 유닛 생성이 끝나서 실제로 게임에 나올 때 호출, 현재인구수를 증가시킴.
    /// </summary>
    /// <param name="_unitType"></param>
    public void SpawnUnit(EUnitType _unitType)
    {
        IncreaseCurPopulation(unitPopulation[(int)_unitType]);
    }

    public void UnitDead(EUnitType _unitType)
    {
        DecreasePopulation(unitPopulation[(int)_unitType]);
    }

    private void DecreasePopulation(uint _decreaseAmount)
    {
        curPopulation -= _decreaseAmount;
        ArrayPopulationCommand.Use(EPopulationCommand.UPDATE_CURRENT_POPULATION_HUD, curPopulation);
        if (CanSpawnUnit())
            PushMessageToBroker(EMessageType.START_SPAWN);
    }

    public void UpgradeMaxPopulation()
    {
        curMaxPopulation += 20;
        ArrayPopulationCommand.Use(EPopulationCommand.UPDATE_CURRENT_MAX_POPULATION_HUD, curMaxPopulation);
        if (CanSpawnUnit())
            PushMessageToBroker(EMessageType.START_SPAWN);
    }

    public void RegisterBroker()
    {
        Broker.Regist(this, EPublisherType.POPULATION_MANAGER);
    }

    public void PushMessageToBroker(EMessageType _message)
    {
        Broker.AlertMessageToSub(_message, EPublisherType.POPULATION_MANAGER);
    }

    private bool CanSpawnUnit()
    {
        return curPopulation < curMaxPopulation;
    }

    private void IncreaseCurPopulation(uint _increaseAmount)
    {
        curPopulation += _increaseAmount;
        ArrayPopulationCommand.Use(EPopulationCommand.UPDATE_CURRENT_POPULATION_HUD, curPopulation);
        if (!CanSpawnUnit())
            PushMessageToBroker(EMessageType.STOP_SPAWN);
    }

    

    [SerializeField]
    private uint maxPopulation = 0;
    [SerializeField]
    private uint curMaxPopulation = 0;
    [SerializeField]
    private uint curPopulation = 0;
    [SerializeField]
    private uint[] unitPopulation = null;
}
