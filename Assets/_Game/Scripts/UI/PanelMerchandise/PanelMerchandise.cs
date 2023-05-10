using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMerchandise : UIPanel
{
    [SerializeField] UIProduct productPrefab;
    [SerializeField] List<UIProduct> products;
    [SerializeField] Transform productContainer;
    [SerializeField] Button btnExit;
    [SerializeField] Button bgExit;
    public override void Awake()
    {
        panelType = UIPanelType.PanelMerchandise;
        base.Awake();
        GenerateProduct();
        btnExit.onClick.AddListener(ClosePanel);
        bgExit.onClick.AddListener(ClosePanel);
        Reload();
    }

    private void OnEnable()
    {
        Reload();
    }

    public void Reload()
    {
        if (products.Count > 0)
        {
            for (int i = 0; i < products.Count; i++)
            {
                products[i].UpdateProduct();
            }
        }
    }

    void GenerateProduct()
    {
        List<Merchandise> merchandises = ProfileManager.Instance.dataConfig.merchantDataConfig.merchandises;
        for(int i = 0; i < merchandises.Count; i++)
        {
            UIProduct product = Instantiate(productPrefab, productContainer);
            product.Setup(merchandises[i]);
            products.Add(product);
        }
    }

    void ClosePanel()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (openAndCloseAnim != null)
            openAndCloseAnim.OnClose(UIManager.instance.ClosePanelMerchandise);
        else UIManager.instance.ClosePanelMerchandise();
    }
}
