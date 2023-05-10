using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositCargo : MonoBehaviour
{
    public UIDepositPack depositPack;

    public void InitOffer(OfferData revenueData)
    {
        depositPack.Init(revenueData);
    }

    public void Reload()
    {
        depositPack.Reload();
    }
}
