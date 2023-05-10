using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum UpgradeType {
    AddCheff,
    AddCustomer,
    ReduceTime,
    IncreasePrice,
    IncreaseSpeedCheff,
    AddTable,
    AddTotalBoot,
    AddFloor,
    AddWaiter,
    IncreaseSpeedWaiter
}
[CreateAssetMenu(fileName = "UpgradeDataConfig", menuName = "ScriptAbleObjects/UpgradeData/New UpgradeData")]
public class UpgradeDataConfig : ScriptableObject
{
    public List<UpgradeDataByLevel> upgradeDatas;
    public UpgradeDataByLevel GetUpgradeDataByLevel(int level) { return upgradeDatas[level]; }
}
