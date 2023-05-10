using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteDataConfig", menuName = "ScriptAbleObjects/Sprite Data Config")]
public class SpriteDataConfig : ScriptableObject
{
    public List<Sprite> spritesUpgradeDatabylevel;

    public Sprite GetSpriteUpgradeDataByLevelByName(string sprName)
    {
        for (int i = 0; i < spritesUpgradeDatabylevel.Count; i++)
        {
            if (spritesUpgradeDatabylevel[i].name == sprName)
                return spritesUpgradeDatabylevel[i];
        }
        return null;
    }

    public List<Sprite> offerSprites;

    public Sprite GetSpriteDailyReward(string sprName) {
        for (int i = 0; i < offerSprites.Count; i++)
        {
            if (offerSprites[i].name == sprName)
                return offerSprites[i];
        }
        return null;
    }
}
