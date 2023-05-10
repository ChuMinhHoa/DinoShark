using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FoodDataConfig
{
    public Sprite sprIcon;
    public Mesh meshFood;
    public FoodID foodID;
    public Sprite GetIcon() { return sprIcon; }

}

[CreateAssetMenu(fileName = "MenuDataConfigLevel", menuName = "ScriptAbleObjects/New Menu Data By Level")]
public class MenuDataByLevel : ScriptableObject
{
    public int levelMax;
    public List<FoodDataConfig> listFoodConfig;

    public FoodDataConfig GetFoodData(FoodID foodID)
    {
        for (int i = 0; i < listFoodConfig.Count; i++)
        {
            if (listFoodConfig[i].foodID == foodID)
            {
                return listFoodConfig[i];
            }
        }
        return null;
    }

    public bool IsFirstFood(FoodID foodID)
    {
        if(foodID == listFoodConfig[0].foodID)
        {
            return true;
        }
        return false;
    }

    public Sprite GetFoodSprByID(FoodID id)
    {
        for (int i = 0; i < listFoodConfig.Count; i++)
        {
            if (listFoodConfig[i].foodID == id)
            {
                return listFoodConfig[i].sprIcon;
            }
        }
        return listFoodConfig[0].sprIcon;
    }

    public int GetAllFoodMaxLevel()
    {
        int maxLevel = 0;
        for (int i = 0; i < listFoodConfig.Count; i++)
        {
            maxLevel += levelMax;
        }
        return maxLevel;
    }

    public int GetLevelMax() { return levelMax; }

}
