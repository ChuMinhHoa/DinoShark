using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductCargo : MonoBehaviour
{
    [SerializeField] RectTransform rt;
    [SerializeField] GridLayoutGroup gridLayout;
    protected List<OfferData> listOfferData = new List<OfferData>();
    [SerializeField] Transform productContainer;
    [SerializeField] UIShopOffer product;
    List<UIShopOffer> uIShopOffers = new List<UIShopOffer>();
    
    public void GenerateProduct(List<OfferData> listOffer)
    {
        float cellWidth = (rt.rect.width - 80 - 120) / 3;
        //gridLayout.cellSize = new Vector2(cellWidth, cellWidth * 3 / 2);
        gridLayout.cellSize = new Vector2(cellWidth, gridLayout.cellSize.y);
        listOfferData = listOffer;
        for (int i = 0; i < listOfferData.Count; i++)
        {
            UIShopOffer newProduct = Instantiate(product, productContainer);
            newProduct.Init(listOfferData[i]);
            uIShopOffers.Add(newProduct);
        }
        //Debug.Log(GetComponent<RectTransform>().sizeDelta);
    }

    public void ReloadValue()
    {
        for (int i = 0; i < uIShopOffers.Count; i++)
        {
            uIShopOffers[i].LoadValue();
        }
    }
}
