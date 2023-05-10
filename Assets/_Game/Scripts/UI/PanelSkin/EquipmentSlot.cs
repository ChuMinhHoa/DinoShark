using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : OutfitSlot
{
    [SerializeField] GameObject objMain;
    public bool choosed;
    public override void InitData(OwnedOutfit ownedOutfit)
    {
        base.InitData(ownedOutfit);
        objMain.SetActive(true);
        choosed = true;
    }

    public override void DeSelectOutfit()
    {
        base.DeSelectOutfit();
        objMain.SetActive(false);
        choosed = false;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }
}
