using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMarketing : UIPanel
{
    public override void Awake()
    {
        panelType = UIPanelType.PanelMarketing;
        base.Awake();
        btnExit.onClick.AddListener(OnExit);
        btnConfirmSuppend.onClick.AddListener(OnConfirmSuppend);
        btnCloseConfirmSuppendPanel.onClick.AddListener(ClosePanelSuppend);
    }
    public List<ProductToSell> productToSells = new List<ProductToSell>();
    [SerializeField] ProductToSell productToSellPref;
    [SerializeField] Transform productParent;
    
    [SerializeField] GameObject objConfirmSuppendPanel;
    [SerializeField] Button btnCloseConfirmSuppendPanel;
    [SerializeField] Button btnConfirmSuppend;
    [SerializeField] Button btnExit;
    int countFoodMachine = 0;
    int currentLevel;
    FoodID currentPromoFoodID;
    MenuSave menuSave;

    public void InitData() {
        menuSave = ProfileManager.Instance.playerData.GetMenuSave(); 
        countFoodMachine = GameManager.Instance.kitchenManager.foodControllers.Count;
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
        currentPromoFoodID = menuSave.marketedFood;
        if (productToSells.Count < countFoodMachine)
            CreateNewProductCell(countFoodMachine - productToSells.Count);
        else if (productToSells.Count > countFoodMachine)
            RemoveOldProduct();
        LoadData();
    }
    
    void CreateNewProductCell(int totalCreateNew) {
        for (int i = 0; i < totalCreateNew; i++)
        {
            ProductToSell newProductToCell = Instantiate(productToSellPref, productParent);
            productToSells.Add(newProductToCell);
        }
    }

    void RemoveOldProduct() {
        while (productToSells.Count > countFoodMachine) {
            Destroy(productToSells[productToSells.Count - 1].gameObject);
            productToSells.Remove(productToSells[productToSells.Count - 1]);
        }
    }

    MenuDataByLevel menuDataByLevel;
    void LoadData() {
        if (productToSells.Count == 0)
            return;
        menuDataByLevel = ProfileManager.Instance.dataConfig.menuDataConfig.GetMenuDataByLevel(currentLevel);
        for (int i = 0; i < productToSells.Count; i++)
        {
            productToSells[i].Setup(menuDataByLevel.listFoodConfig[i]);
            if (menuSave.onWaitPromotion)
            {
                if (menuSave.onPromotion && currentPromoFoodID == menuDataByLevel.listFoodConfig[i].foodID)
                {
                    productToSells[i].OnSuppendMode();
                }
                else productToSells[i].OnWaitMode();
            }
            else {
                productToSells[i].OnPromotionMode();
            }
        }
    }

    public void ReLoadProductCell() {
        for (int i = 0; i < productToSells.Count; i++)
        {
            if (menuSave.onWaitPromotion)
            {
                if (menuSave.onPromotion && currentPromoFoodID == menuDataByLevel.listFoodConfig[i].foodID)
                {
                    productToSells[i].OnSuppendMode();
                }
                else productToSells[i].OnWaitMode();
            }
            else
            {
                productToSells[i].OnPromotionMode();
            }
        }
    }

    void OnExit()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        openAndCloseAnim.OnClose(() => {
            UIManager.instance.ClosePanelMarketing();
        });
    }

    ProductToSell currentCellMarketing;
    public void ShowPanelConfirmSuppend(ProductToSell productToSell) {
        currentCellMarketing = productToSell;
        objConfirmSuppendPanel.SetActive(true);
    }

    public void OnConfirmSuppend() {
        currentCellMarketing.OnSuppend();
        ClosePanelSuppend();
    }

    public void ClosePanelSuppend() {
        objConfirmSuppendPanel.SetActive(true);
    }
}
