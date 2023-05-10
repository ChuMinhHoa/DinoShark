using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodDefaultProperty
{
    public BigNumber unlockPrice;
    public BigNumber firstUpgrade;
    public BigNumber firstProfit;
    public float timeMake;

    public BigNumber GetPriceUpgrade(int level)
    {
        if(level == 0)
        {
            return new BigNumber(unlockPrice);
        }
        BigNumber price = new BigNumber(firstUpgrade);
        for (int i = 0; i < level - 1; i++)
        {
            BigNumber newPrice = price * 1.2f;
            //if ((newPrice - price) > 1)
            if (newPrice.IsBigger(price + 1))
            {
                //price = newPrice;
                price.Multiply(1.2f);
            }
            else
            {
                //price += 1f;
                price.Add(1f);
            }
        }
        return price;
    }
}
[CreateAssetMenu(fileName = "MenuDataConfig", menuName = "ScriptAbleObjects/New Menu Data")]
public class MenuDataConfig : ScriptableObject
{
    public List<MenuDataByLevel> menuDataByLevels;
    public List<FoodDefaultProperty> foodDefaultProperties;
    public List<int> machineUnlock;
    public List<int> starUnlock;
    public MenuDataByLevel GetMenuDataByLevel(int level) {
        if (level >= menuDataByLevels.Count)
            return null;
        return menuDataByLevels[level];
    }

    public Sprite GetFoodSprByID(int level, FoodID id)
    {
        return GetMenuDataByLevel(level).GetFoodSprByID(id);
    }

    public bool IsUnlockStar(int level)
    {
        for (int i = 0; i < starUnlock.Count; i++)
        {
            if(level == starUnlock[i])
            {
                return true;
            }
        }
        return false;
    }

    public int GetMaxStar(int maxLevel)
    {
        int star = 0;
        for (int i = 0; i < starUnlock.Count; i++)
        {
            if(maxLevel >= starUnlock[i])
            {
                star++;
            }
        }
        return star;
    }

    public int GetCurrentStar(int level)
    {
        int star = 0;
        for (int i = 0; i < starUnlock.Count; i++)
        {
            if (level >= starUnlock[i])
            {
                star++;
            }
            else
            {
                break;
            }
        }
        return star;
    }

    public int GetLastLevelStar(int level)
    {
        int levelReturn = 0;
        for (int i = 0; i < starUnlock.Count; i++)
        {
            if (level >= starUnlock[i])
            {
                levelReturn = starUnlock[i];
            }
            else break;
        }
        return levelReturn;
    }
    public int GetNextLevelStar(int level)
    {
        int levelReturn = 0;
        for (int i = 0; i < starUnlock.Count; i++)
        {
            if (level <= starUnlock[i])
            {
                levelReturn = starUnlock[i];
                break;
            }
        }
        return levelReturn;
    }
    public int GetMilestone(int level)
    {
        int milestone = 0;
        for (int i = 0; i < starUnlock.Count; i++)
        {
            if (level == starUnlock[i])
            {
                milestone = starUnlock[i] + 1;
                break;
            }
        }
        return milestone;
    }

    public string GetFoodName(FoodID foodID)
    {
        switch (foodID)
        {
            case FoodID.None:
                return "None";
            case FoodID.Burger:
                return "Burger";
            case FoodID.MilkTea:
                return "Milk Tea";
            case FoodID.Fries:
                return "Fries";
            case FoodID.Croiossants:
                return "Croissant";
            case FoodID.Lemonade:
                return "Lemonade";
            case FoodID.CupcakeIceCream:
                return "Cupcake Ice Cream";
            case FoodID.CupcakeSocola:
                return "Cupcake Chocolate";
            case FoodID.CupcakeStrawBerry:
                return "Cupcake Strawberry";
            case FoodID.CupcakeStrawBerryS:
                return "Cupcake Strawberry Supper";
            case FoodID.Donut:
                return "Donut";
            case FoodID.DonutBlueberry:
                return "Donut Blueberry";
            case FoodID.DonutEgg:
                return "Donut Egg";
            case FoodID.DonutSocola:
                return "Donut Chocolate";
            case FoodID.DonutStrawberry:
                return "Donut Strawberry";
            case FoodID.FriedChicken:
                return "Fried Chicken";
            case FoodID.IceCreamBlueberry:
                return "Ice Cream Blueberry";
            case FoodID.IceCream:
                return "Ice Cream";
            case FoodID.IceCreamMatcha:
                return "Ice Cream Matcha";
            case FoodID.IceCreamSocola:
                return "Ice Cream Chocolate";
            case FoodID.IceCreamStrawberry:
                return "Ice Cream Strawberry";
            case FoodID.SHotdog:
                return "Hot Dog Supper";
            case FoodID.HotDog:
                return "HotDog";
            case FoodID.Pizza:
                return "Pizza";
            case FoodID.CreamOrange:
                return "Cream Orange";
            default:
                return "No name";
        }
    }
}
