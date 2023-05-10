
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelMachineInfor : UIPanel, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public override void Awake()
    {
        panelType = UIPanelType.PanelMachineInfor;
        base.Awake();
        TextAnimManager.Instance.RegisterText(txtLevel);
        TextAnimManager.Instance.RegisterText(txtProfit);
    }
    [Header("PanelMachineInfor")]
    [SerializeField] Text txtLevel;
    [SerializeField] Text txtName;
    [SerializeField] Text txtProfit;
    [SerializeField] Text txtTimeMake;
    [SerializeField] Text txtPrice;
    [SerializeField] Slider sliderUpgradeProcess;
    [SerializeField] Image imgButtonUpgrade;
    [SerializeField] Image imgIcon;
    [SerializeField] Button btnClose;
    [SerializeField] List<Image> stars;
    [SerializeField] Sprite blackStarColor;
    [SerializeField] List<Sprite> starOnState;
    
    [SerializeField] Sprite sprButtonOnable, sprButtonDisable;
    [SerializeField] GameObject wrapUnlock;
    [SerializeField] GameObject levelMaxObject;
    [SerializeField] GameObject priceObjectButton;
    [SerializeField] GameObject upgradeStarDone;
    [SerializeField] Transform pointSpawnValueEffect;
    [SerializeField] RectTransform priceRect;
    [SerializeField] RectTransform coinRect;
    Transform pointGemTarget;
    IMachineController currentMachine;
    BigNumber currentPriceUpgrade;
    bool onUpgrade;
    float timeReEnter;
    bool checkMachineAbleToUpgrade;
    float timeAnim = .025f;
    int level;
    int currentStar;
    int maxStar;
    private void Start()
    {
        btnClose.onClick.AddListener(UIManager.instance.ClosePanelMachineInfor);
        pointGemTarget = UIManager.instance.panelTotal.pointGemAdd;
        sliderUpgradeProcess.maxValue = 1;
        sliderUpgradeProcess.minValue = 0;
    }
    private void Update()
    {
        int level = currentMachine.GetFoodLevel();
        int levelMax = currentMachine.GetFoodLevelMax();
        checkMachineAbleToUpgrade = (level < levelMax) && (GameManager.Instance.IsHaveEnoughMoney(currentPriceUpgrade));
        imgButtonUpgrade.sprite = checkMachineAbleToUpgrade ? sprButtonOnable : sprButtonDisable;
        if (onUpgrade && timeReEnter <= 0f && checkMachineAbleToUpgrade)
        {
            if(currentMachine == null)
            return;
            SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
            ProfileManager.Instance.playerData.ConsumeMoney(currentPriceUpgrade);
            currentMachine.OnUpgradeFood(AddGemAnimation);
            SetUpToShow(currentMachine, true);
            timeReEnter = .1f;
            if (openAndCloseAnim != null)
                openAndCloseAnim.ButtonActive();
            UIManager.instance.panelTotal.UpdateProcess();
        }

        if (timeReEnter > 0)
        {
            timeReEnter -= Time.deltaTime;
        }

        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            mouseIsOver = RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition);
            if (!mouseIsOver && !UIManager.instance.IsHaveTutorial())
            {
                UIManager.instance.ClosePanelMachineInfor();
                CameraController.Instance.StayCamera();
            }
        }
    }

    void AddGemAnimation() {
        UIValueChangeMove valueChangeMove = GameManager.Instance.pooling.GetValueChangeEffect(pointSpawnValueEffect.position);
        valueChangeMove.transform.SetParent(UIManager.instance.transform);
        valueChangeMove.DoMove(pointGemTarget, pointSpawnValueEffect, ValueType.Gem, 2);
    }

    public void SetUpToShow(IMachineController currentMachine, bool onUpgrade = false) {
        if (currentMachine == null)
            UIManager.instance.ClosePanelMachineInfor();
        level = currentMachine.GetFoodLevel();
        int levelMax = currentMachine.GetFoodLevelMax();
        this.currentMachine = currentMachine;
        imgIcon.sprite = currentMachine.GetIconSprite();
        levelMaxObject.SetActive(level == levelMax);
        if (level > 0)
        {
            wrapUnlock.SetActive(true);
            priceObjectButton.SetActive(!(level == levelMax));
            TextAnimManager.Instance.ChangeText(txtLevel, "Level " + level.ToString(), timeAnim);
            txtName.text = currentMachine.GetFoodName();
            txtProfit.transform.parent.gameObject.SetActive(level > 0);
            txtTimeMake.transform.parent.gameObject.SetActive(level > 0);
            BigNumber profit = currentMachine.GetFoodBaseProfit();
            if (level > 0)
                TextAnimManager.Instance.ChangeText(txtProfit, profit.ToString(), timeAnim);
            else txtProfit.text = "";
            float timeMake = currentMachine.GetTimeMakeFood();
            txtTimeMake.text = (level > 0) ? timeMake.ToString("F1") + "s" : "";
            currentPriceUpgrade = new BigNumber(currentMachine.GetFoodPriceUpgrade());
            SetUpStar(levelMax, onUpgrade);
            txtPrice.text = (level < levelMax) ? currentPriceUpgrade.ToString() : "";
            float stringLen = (float)(txtPrice.text.Length);
            if(priceRect)
            {
                priceRect.anchoredPosition = new Vector2(-31, 0);
            }
            if (txtPrice.text.Contains("."))
            {
                //stringLen += 1;
            }
            else
            {
                stringLen += 0.5f;
            }
            if (coinRect)
                coinRect.anchoredPosition = new Vector2(stringLen * 61 / 2, 0);
        }
        else
        {
            wrapUnlock.SetActive(false);
            priceObjectButton.SetActive(true);
            txtLevel.text = "New Menu!";
            currentPriceUpgrade = new BigNumber(currentMachine.GetFoodPriceUpgrade());
            txtPrice.text = currentPriceUpgrade.ToString();
            float stringLen = (float)(txtPrice.text.Length);
            if (priceRect)
            {
                priceRect.anchoredPosition = new Vector2(-31, 0);
            }
            if (txtPrice.text.Contains("."))
            {
                //stringLen += 1;
            }
            else
            {
                stringLen += 0.5f;
            }
            if (coinRect)
                coinRect.anchoredPosition = new Vector2(stringLen * 61 / 2, 0);
        }
    }
    void SetUpStar(int levelMax, bool onUpgrade = false) {
        maxStar = ProfileManager.Instance.dataConfig.menuDataConfig.GetMaxStar(ProfileManager.Instance.dataConfig.GetMenuDataByLevel().levelMax);
        currentStar = ProfileManager.Instance.dataConfig.menuDataConfig.GetCurrentStar(level);
        int currentStarIndex = currentStar % 5;
        if (currentStarIndex == 0 && currentStar > 0) currentStarIndex = 5;
        int stackStar = 0;
        int starState = (int)(currentStar / 5);
        if(currentStar % 5 == 0 && currentStar != 0)
        {
            starState = starState - 1;
        }
        if (maxStar < 5)
        {
            stackStar = maxStar % 5;
        }
        else if(maxStar == 5)
        {
            stackStar = 5;
        }
        else
        {
            if(currentStar <= 5 * ((int)(maxStar / 5)))
            {
                stackStar = 5;
            }
            else
            {
                stackStar = maxStar % 5;
            }
        }

        for (int i = 0; i < stars.Count; i++)
        {
            if (i < stackStar)
            {
                stars[i].gameObject.SetActive(true);
                stars[i].sprite = blackStarColor;
            }
            else
            {
                stars[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < currentStarIndex; i++)
        {
            stars[i].sprite = starOnState[starState];
        }    

        float lastStarLevel = currentMachine.GetLastLevelStar();
        float nextStarLevel = currentMachine.GetNextLevelStar();

        if (level == levelMax)
        {
            sliderUpgradeProcess.value = 1;
            if (onUpgrade)
                ShowX2Revenue();
        }
        else if (nextStarLevel == lastStarLevel)
        {
            sliderUpgradeProcess.value = 0;
            if (onUpgrade)
                ShowX2Revenue();
        }
        else sliderUpgradeProcess.value = (level - lastStarLevel) / (nextStarLevel - lastStarLevel);
    }

    public void Upgrade(bool Upgrade) {
        onUpgrade = Upgrade; 
    }

    public void Close() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        onUpgrade = false;
        gameObject.SetActive(false);
    }

    public void OnOpen() {
        onUpgrade = false;
        gameObject.SetActive(true);
    }

    void ShowX2Revenue() {
        upgradeStarDone.SetActive(true);
        Invoke("TurnOffX2Revenue", 2f);
    }

    void TurnOffX2Revenue() {
        upgradeStarDone.SetActive(false);
    }

    private bool mouseIsOver = false;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    public void OnDeselect(BaseEventData eventData)
    {
    //    Debug.Log("dd");
    //    if (!mouseIsOver && !UIManager.instance.IsHaveTutorial())
    //        UIManager.instance.ClosePanelMachineInfor();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //mouseIsOver = true;
        //EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //mouseIsOver = false;
        //EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
