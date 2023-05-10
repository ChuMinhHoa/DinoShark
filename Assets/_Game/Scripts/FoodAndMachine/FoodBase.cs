using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FoodID {
    None,
    Burger,
    MilkTea,
    Fries,
    Croiossants,
    Lemonade,
    CupcakeIceCream,
    CupcakeSocola,
    CupcakeStrawBerry,
    CupcakeStrawBerryS,
    Donut,
    DonutBlueberry,
    DonutEgg,
    DonutSocola,
    DonutStrawberry,
    FriedChicken,
    IceCreamBlueberry,
    IceCream,
    IceCreamMatcha,
    IceCreamSocola,
    IceCreamStrawberry,
    SHotdog,
    HotDog,
    Pizza,
    CreamOrange
}
[System.Serializable]
public class FoodBase
{
    public int level;
    public FoodID foodID;
    public List<Machine> machines;
    public BigNumber profit = new BigNumber();
    public bool able = false;
    public float totalBoot = 1;
    public float timeBoost = 1;
    public void OffMachines() {
        for (int i = 0; i < machines.Count; i++)
            machines[i].ChangeMachineStatus(MachineStatus.Lock);
    }
    public Machine GetMachineFree(int countMachineActive) {
        for (int i = 0; i < countMachineActive; i++)
        {
            if (machines[i].IsFree())
                return machines[i];
        }
        return null;
    }
}
