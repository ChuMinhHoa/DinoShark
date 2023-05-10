using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("=====LobbyController=====")]
    public LobbyManager lobbyManager;
    [Header("=====KitchenController=====")]
    public KitchenManager kitchenManager;
    [Header("=====Pooling=====")]
    public Pooling pooling;
    [Header("=====FloorManager=====")]
    public FloorManager floorManager;

    public bool IsPauseGame;

    public Transform spawnPosition;
    public Transform getInPosition;
    public Transform getOutPosition;

    public IMachineController currentMachineSelected;
    Machine currentMachine;
    Vector3 touchUp, touchDown;
    bool isTouching;

    UIManager uiManager;
    PlayerData playerData;


    private void Awake()
    {
        Instance = this;
        playerData = ProfileManager.Instance.playerData;
        if (floorManager != null)
            floorManager.InitFloorManager();
    }

    private void Start()
    {
        //playerData = ProfileManager.Instance.playerData; 
        uiManager = UIManager.instance;
    }
    int capture = 0;
    private void Update()
    {
        MouseInputHandel();
        if (Input.GetKeyDown(KeyCode.H))
        {
            ScreenCapture.CaptureScreenshot("DinoSceenShot"+ capture + ".png");
            capture++;
            Debug.Log(Application.persistentDataPath);
        }
    }

    void MouseInputHandel() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (uiManager.IsHavePopUpOnScene()) return;
        if (TutorialManager.Instance.levelHaveTutOrder && !TutorialManager.Instance.isDoneOrder) return;
        if (Input.GetMouseButton(0) && TutorialManager.Instance.onTutorial && TutorialManager.Instance.isDoneOrder && !TutorialManager.Instance.onUnlockMachine)
        {
            touchDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            OnTouching();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            touchDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isTouching = true;
            if (TutorialManager.Instance.onTutorial && !TutorialManager.Instance.onUnlockMachine) OnTouching();
        }
        if (isTouching)
        {
            if (Input.GetMouseButtonUp(0) && !TutorialManager.Instance.onTutorial && !TutorialManager.Instance.onUnlockMachine)
                OnTouching();
        }
    }

    Ray ray;
    RaycastHit hit;
    void OnTouching() {
        //Debug.Log("Touching");
        isTouching = false;
        touchUp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector3.Distance(touchDown, touchUp) >= 0.1f) return;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        //Debug.Log(hit.distance);
        if (hit.collider != null)
        {
            if(hit.collider.GetComponent<IMachineController>() != null)
            {
                currentMachineSelected = hit.collider.GetComponent<IMachineController>();
                if (currentMachineSelected != null)
                {
                    currentMachineSelected.OpenControlerInfo();
                    return;
                }
            }
            else if (hit.collider.GetComponent<Machine>() != null)
            {
                currentMachine = hit.collider.GetComponent<Machine>();
                if (currentMachine != null)
                {
                    currentMachine.MachineOnClick();
                    return;
                }
            }
            else if (hit.collider.GetComponent<AlertClicker>() != null)
            {
                hit.collider.GetComponent<AlertClicker>().AlertOnClick();
            }
        }
    }

    DataFood dataFood;
    public bool IsUnLockFood(FoodID foodID) {
        dataFood = playerData.GetMenuSave().GetDataFood(foodID);
        if (dataFood == null)
            return false;
        return dataFood.able;
    }

    public Transform GetSpawnPosition()
    {
        return spawnPosition;
    }

    public Transform GetOutPosition()
    {
        return getOutPosition;
    }

    public bool IsHaveEnoughMoney(BigNumber price) {
        return playerData.GetResource().IsHaveEnoughMoney(price);
    }

    public bool IsHaveEnoughGem(int value) {
        return playerData.globalResourceSave.IsEnoughGem(value);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            playerData.GetResource().SaveOffline();
        }
        else
        {
            GetOfflineRevenue();
        }
    }

    float minOfflineTimeByMinute = 3f;
    public void GetOfflineRevenue()
    {
        //Debug.Log("Check Offline Revenue");
        string lastTimeTxt = playerData.GetResource().GetLastOffline();
        if (String.IsNullOrEmpty(lastTimeTxt))
        {
            playerData.GetResource().GetOfflineRev();
            return;
        }
        DateTime lastTime = DateTime.Parse(lastTimeTxt);
        
        TimeSpan span = DateTime.Now.Subtract(lastTime);
        float totalOfflineSec = (float)(span.TotalSeconds);
        if(totalOfflineSec > GetMaxOfflineTimeToSecond())
        {
            totalOfflineSec = GetMaxOfflineTimeToSecond();
        }
        if (totalOfflineSec > minOfflineTimeByMinute * 60)
        {
            if (playerData.GetMenuSave().dataFoodSave.GetAvgProfit() > 0)
            {
                uiManager.ShowPanelOffline(span);
            }
        }
        else
        {
            playerData.GetResource().GetOfflineRev();
        }
    }

    [SerializeField] float baseOfflineTime = 3;
    [SerializeField] float baseFoodServedTime = 15;

    float GetMaxOfflineTimeToSecond()
    {
        float merchandiseOffTime = playerData.merchandiseSave.GetOfflineTime();
        return 3600f * (baseOfflineTime + merchandiseOffTime);
    }

    public float GetBaseOfflineTime()
    {
        return baseOfflineTime;
    }

    public float GetFoodServedTime()
    {
        return baseFoodServedTime;
    }

    public void ClaimItemReward(List<ItemReward> itemRewards)
    {
        for (int i = 0; i < itemRewards.Count; i++)
        {
            Claim(itemRewards[i]);
        }
        playerData.SaveData();
    }
    BigNumber min4HRevenue = new BigNumber(2500);
    void Claim(ItemReward item)
    {
        Debug.Log(item.itemType);
        switch (item.itemType)
        {
            case OfferID.PeasantSuitcase:
            case OfferID.NobleSuitcase:
            case OfferID.RoyalSuitcase:
                playerData.globalResourceSave.AddSuitcase(item.itemType, (int)item.amount);
                break;
            case OfferID.H4Revenue:
                BigNumber profitAvg = playerData.GetMenuSave().dataFoodSave.GetAvgProfit();
                float serveTime = GetFoodServedTime();
                BigNumber profit = 4 * 3600 * profitAvg / serveTime;                // Profit in 4h
                if (min4HRevenue.IsBigger(profit))
                    profit = new BigNumber(min4HRevenue);
                playerData.AddMoney(profit);
                break;
            case OfferID.X2Revenue:
            case OfferID.X5Revenue:
                playerData.boostSave.AddGemBuyBoost(item.itemType, item.amount * 60);
                break;
            case OfferID.Gems:
                playerData.globalResourceSave.AddGem((int)(item.amount));
                break;
            case OfferID.NoAds:
                playerData.globalResourceSave.SetRemoveAds(true);
                break;
            case OfferID.AllRevenue:
                playerData.globalResourceSave.SetX2Revenue(true);
                break;
            case OfferID.DepositPack:
                playerData.globalResourceSave.CollectDeposit();
                break;      
            default:
                break;
        }
    }
    public void ClaimItemEquip(List<ItemEquip> itemEquips)
    {
        if(itemEquips != null)
        {
            for (int i = 0; i < itemEquips.Count; i++)
            {
                playerData.outfitSave.AddOutfit(itemEquips[i].outfitType, itemEquips[i].itemId, itemEquips[i].rarity, false);
                UIManager.instance.ShowPanelOpenOutfit(OfferID.RoyalSuitcase, itemEquips[i].itemId, itemEquips[i].outfitType, itemEquips[i].rarity, true);
            }
        }
    }

    public void InitDataToAsset() {
        //initdata to machine;
        kitchenManager.InitData();
        //initdata to staff;
        kitchenManager.InitStaff();
        //initdata to waiter;
        lobbyManager.InitWaiters();
        //Init Data to Floor
        if (floorManager != null)
            floorManager.InitFloor();
        //initdata to table
        lobbyManager.InitTable();
        //Instance custommer;
        lobbyManager.InitCustomer(); 
        //Check tutorial
        LoadSceneManager.Instance.AddTotalProgress(30);
    }

    public void OnCollectRewardIAPPackage(OfferData currentIAPPackage)
    {
        //Debug.Log("Get Reward");
        //if (currentIAPPackage.offerID == OfferID.NoAds || currentIAPPackage.offerID == OfferID.Vip1Pack || currentIAPPackage.offerID == OfferID.Vip2Pack || currentIAPPackage.offerID == OfferID.Vip3Pack || currentIAPPackage.offerID == OfferID.AllRevenue) 
        //{  
        //}
        playerData = ProfileManager.Instance.playerData;
        playerData.globalResourceSave.OnSaveBoughtIAPPackage(currentIAPPackage.offerID);
        List<ItemReward> rewards = currentIAPPackage.itemRewards;
        ClaimItemReward(rewards);
        ClaimItemEquip(currentIAPPackage.itemEquips);
        ProfileManager.Instance.playerData.SaveData();
        //playerData.globalResourceSave.OnSaveBoughtIAPPackage(currentIAPPackage.offerID);
        UIManager.instance.GetTotalPanel().CheckQuickIAP();
    }
}
