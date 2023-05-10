using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductToSell : MonoBehaviour
{
    [SerializeField] Image imgIcon;
    [SerializeField] FoodID foodID;

    [SerializeField] GameObject objPromotion;
    [SerializeField] GameObject objSuppend;
    [SerializeField] GameObject objWait;
    [SerializeField] GameObject objHot;

    [SerializeField] Text txtTitle;
    [SerializeField] Text txtTimePromo;
    [SerializeField] Text txtTimeNextPromo;
    [SerializeField] Text txtProfitExpect;

    [SerializeField] Button btnPromo;
    [SerializeField] Button btnSuppend;


    bool showTimeWait;
    bool showTimeSuppend;

    MenuSave menuSave;
    PanelMarketing panelMarketing;

    private void Start()
    {
        btnPromo.onClick.AddListener(MarketingThisProduct);
        btnSuppend.onClick.AddListener(ShowPanelConfirm);
    }

    IMachineController machineController;

    public void Setup(FoodDataConfig foodDataConfig)
    {
        foodID = foodDataConfig.foodID;
        imgIcon.sprite = foodDataConfig.sprIcon;

        menuSave = ProfileManager.Instance.playerData.GetMenuSave();

        machineController = GameManager.Instance.kitchenManager.GetFoodController(foodDataConfig.foodID);

        txtProfitExpect.text = (machineController.GetFoodProfit() * GameManager.Instance.GetFoodServedTime() * 5f).ToString();
        txtTitle.text = machineController.GetFoodName();
    }

    void MarketingThisProduct()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.GetMenuSave().OnMarketingFood(foodID);
        if (panelMarketing == null)
            panelMarketing = UIManager.instance.GetPanel(UIPanelType.PanelMarketing).GetComponent<PanelMarketing>();
        panelMarketing.ReLoadProductCell();
    }

    public void OnPromotionMode() {
        objPromotion.SetActive(true);
        objSuppend.SetActive(false);
        objWait.SetActive(false);
        showTimeWait = false;
        showTimeSuppend = false;
    }

    public void OnSuppendMode() {
        objPromotion.SetActive(false);
        objSuppend.SetActive(true);
        objWait.SetActive(false);
        showTimeWait = false;
        showTimeSuppend = true;
    }

    public void OnWaitMode() {
        objPromotion.SetActive(false);
        objSuppend.SetActive(false);
        objWait.SetActive(true);
        showTimeWait = true;
        showTimeSuppend = false;
    }

    private void Update()
    {
        if (showTimeSuppend)
        {
            txtTimePromo.text = TimeUtil.TimeToString(menuSave.foodMarketingTimeRemain);
        }

        if (showTimeWait)
        {
            txtTimeNextPromo.text = TimeUtil.TimeToString(menuSave.foodMarketingCoolDown);
        }
    }

    public void ShowPanelConfirm() {
        if (panelMarketing == null)
            panelMarketing = UIManager.instance.GetPanel(UIPanelType.PanelMarketing).GetComponent<PanelMarketing>();
        panelMarketing.ShowPanelConfirmSuppend(this);
    }

    public void OnSuppend() {
        OnWaitMode();
    }
}
