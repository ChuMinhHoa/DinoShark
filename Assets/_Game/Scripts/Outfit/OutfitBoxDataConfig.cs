using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OutfitBox
{
    public string boxName;
    public List<RateByRarity> rateByRarities;
}

[System.Serializable] 
public class RateByRarity
{
    public Rarity rarity;
    public float rate;
}

[CreateAssetMenu(fileName = "OutfitBoxDataConfig", menuName = "ScriptAbleObjects/OutfitBoxDataConfig")]
public class OutfitBoxDataConfig : ScriptableObject
{
    public List<OutfitBox> outfitBoxes; 

    public OutfitBox GetBoxByOffer(OfferID offer)
    {
        switch (offer)
        {
            case OfferID.PeasantSuitcase:
                return outfitBoxes[0];
            case OfferID.NobleSuitcase:
                return outfitBoxes[1];
            case OfferID.RoyalSuitcase:
                return outfitBoxes[2];
            default:
                return outfitBoxes[0];
        }
    }
}
