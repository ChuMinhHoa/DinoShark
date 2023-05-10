using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GroupShowRarityBoot: MonoBehaviour
{
    [SerializeField] Text txtBoot;
    [SerializeField] GameObject lockObject;
    [SerializeField] GameObject lockIcon;
    [SerializeField] Button btnShowTierUp;
    [SerializeField] Button btnShowInforTierUp;

    public void InitData(BootOutfit bootOutfit, string outfitName = "", int level = 0, bool isUnlock = false)
    {
        if (bootOutfit.bootType != OutfitBootType.x2Revenue)
            txtBoot.text = ConvertBootTypeToString(bootOutfit.bootType) + " <color=#927169>+" + bootOutfit.bootValue.ToString() + "%</color>";
        else
            txtBoot.text = outfitName + " " + ConvertBootTypeToString(bootOutfit.bootType) + " <color=#089A17>(" + (bootOutfit.bootValue * level).ToString() + "%)</color>";
        lockObject.SetActive(!isUnlock);
        lockIcon.SetActive(!isUnlock);
    }

    string ConvertBootTypeToString(OutfitBootType outfitBootType)
    {
        switch (outfitBootType)
        {
            case OutfitBootType.PerfectDishRate:
                return "Perfect Dish Rate";
            case OutfitBootType.MoveSpeed:
                return "Move Speed";
            case OutfitBootType.ExtremelyGenerousTipRate:
                return "Extremely Generous Tip Rate";
            case OutfitBootType.ProductionSpeed:
                return "Production Speed";
            case OutfitBootType.AllEmployeesProductionSpeed:
                return "All Employees Production Speed";
            case OutfitBootType.InstantDish:
                return "Instant Completed Making Food";
            case OutfitBootType.x2Revenue:
                return "x2 Revenue";
            case OutfitBootType.AllPerfectDishRate:
                return "All Staff Perfect Dish Rate";
            case OutfitBootType.UpgradeDiscount:
                return "Upgrade Discount";
        }
        return "";
    }
}
