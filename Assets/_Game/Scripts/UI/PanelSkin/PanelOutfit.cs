using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelOutfit : UIPanel
{
    [Header("==============General==============")]
    public static PanelOutfit Instance;
    bool isChangeData = true;
    [SerializeField] Button btnClose;
    public RectTransform m_PanelContent;
    public RectTransform m_PanelContent2;
    bool outfitTab;
    [SerializeField] Image tabImg;
    [SerializeField] Button switchTabBtn;
    [SerializeField] GameObject oufitChangeTab;
    [SerializeField] Sprite changeSpr;
    [SerializeField] GameObject oufitMergeTab;
    [SerializeField] Sprite mergerSpr;

    [SerializeField] OutfitSlot outfitSlot;
    [SerializeField] Transform outfitSlotParent;
    public EquipmentSlot hatSlot;
    public EquipmentSlot clothesSlot;
    public EquipmentSlot toolSlot;
    public List<OutfitSlot> listOutfitSlots = new List<OutfitSlot>();
    public List<OutfitSlot> listOutfitSlotsNoneUse = new List<OutfitSlot>();

    [Header("==============Change Outfit==============")]
    [SerializeField] EquipOutfit equipOutfit;
    OutfitSlot newOutfitSlot;
    [SerializeField] Text txtTotalBootOfOutfit;
    [SerializeField] GameObject objSkinEmpty;
    int indexOfListCount;
    float totalBootOfOutfit;
    [SerializeField] RectTransform rawImgRt;
    [SerializeField] RectTransform mainRt;
    [SerializeField] Transform staffTransform;

    [Header("==============Merge Outfit==============")]
    public List<EquipmentSlot> mergeSlots;
    public GameObject plusSymbol;
    public Button combineBtn;
    public GameObject combineHintObj;
    CombineCondition combineCondition;
    int choosedCount = 0;

    public GameObject newInfoObj;
    public EquipmentSlot newItem;
    public Text c_outfitName;
    public Text c_outfitBuff;


    public override void Awake()
    {
        panelType = UIPanelType.PanelSkin;
        Instance = this;
        staffTransform = RawImageManager.Instance.GetStaffTransform();
        defaultAngle = staffTransform.eulerAngles;
        equipOutfit = RawImageManager.Instance.GetEquipOutfit();
        base.Awake();
        CheckScreenObstacle();
    }

    void CheckScreenObstacle()
    {
        float screenRatio = (float)Screen.height / (float)Screen.width;
        if (screenRatio > 2.1f) // Now we got problem 
        {
            m_PanelContent.sizeDelta = new Vector2(0, -100);
            m_PanelContent.anchoredPosition = new Vector2(0, -50);
            m_PanelContent2.sizeDelta = new Vector2(0, -100);
            m_PanelContent2.anchoredPosition = new Vector2(0, -50);
        }
        else
        {
            m_PanelContent.sizeDelta = new Vector2(0, 0);
            m_PanelContent.anchoredPosition = new Vector2(0, 0);
            m_PanelContent2.sizeDelta = new Vector2(0, -100);
            m_PanelContent2.anchoredPosition = new Vector2(0, -50);
        }
    }

    private void Start()
    {
        btnClose.onClick.AddListener(ClosePanel);
        switchTabBtn.onClick.AddListener(SwitchTab);
        combineBtn.onClick.AddListener(CombineOutfit);
        rawImgRt.sizeDelta = new Vector2(mainRt.rect.width * 2 / 3, mainRt.rect.width *2 / 3);
    }

    private void OnEnable()
    {
        staffTransform.eulerAngles = defaultAngle;
        outfitTab = true;
        oufitChangeTab.SetActive(outfitTab);
        ShowAllOutfit();
        oufitMergeTab.SetActive(!outfitTab);
        tabImg.sprite = mergerSpr;
    }

    public void ChangeData()
    {
        isChangeData = true;
    }

    public void InitData() {
        if(isChangeData)
        {
            hatSlot.DeSelectOutfit();
            clothesSlot.DeSelectOutfit();
            toolSlot.DeSelectOutfit();
            equipOutfit.EquipDefault(OutfitType.Hat);
            equipOutfit.EquipDefault(OutfitType.Clothes);
            equipOutfit.EquipDefault(OutfitType.Tool);
            indexOfListCount = 0;
            totalBootOfOutfit = 0;

            List<OwnedOutfit> ownedOutfits = ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfits();

            for (int i = 0; i < ownedOutfits.Count; i++)
            {
                if (!ownedOutfits[i].onUsing)
                {
                    if (indexOfListCount >= listOutfitSlots.Count)
                    {
                        if(listOutfitSlotsNoneUse.Count > 0)
                        {
                            newOutfitSlot = listOutfitSlotsNoneUse[0];
                            listOutfitSlotsNoneUse.Remove(listOutfitSlotsNoneUse[0]);
                            newOutfitSlot.gameObject.SetActive(true);
                        }
                        else
                        {
                            newOutfitSlot = Instantiate(outfitSlot, outfitSlotParent);
                        }
                        newOutfitSlot.InitData(ownedOutfits[i]);
                        listOutfitSlots.Add(newOutfitSlot);
                    }
                    else
                    {
                        listOutfitSlots[indexOfListCount].gameObject.SetActive(true);
                        listOutfitSlots[indexOfListCount].InitData(ownedOutfits[i]);
                    }
                    indexOfListCount++;
                }
                else
                    InitEquipmentSlot(ownedOutfits[i]);
            }

            if(indexOfListCount < listOutfitSlots.Count)
            {
                int delta = listOutfitSlots.Count - indexOfListCount;
                for (int i = 0; i < delta; i++)
                {
                    listOutfitSlotsNoneUse.Add(listOutfitSlots[listOutfitSlots.Count - 1]);
                    listOutfitSlots[listOutfitSlots.Count - 1].gameObject.SetActive(false);
                    listOutfitSlots.Remove(listOutfitSlots[listOutfitSlots.Count - 1]);
                }
            }

            UpdateTotalBoot();
            objSkinEmpty.gameObject.SetActive(ownedOutfits.Count == 0);
            isChangeData = false;
        }
    }

    public void InitEquipmentSlot(OwnedOutfit ownedOutfit) {
        switch (ownedOutfit.outfitType)
        {
            case OutfitType.Hat:
                hatSlot.gameObject.SetActive(true);
                hatSlot.InitData(ownedOutfit);
                equipOutfit.Equip(ownedOutfit.outfitID, OutfitType.Hat);
                break;
            case OutfitType.Clothes:
                clothesSlot.gameObject.SetActive(true);
                clothesSlot.InitData(ownedOutfit);
                equipOutfit.Equip(ownedOutfit.outfitID, OutfitType.Clothes);
                break;
            case OutfitType.Tool:
                toolSlot.gameObject.SetActive(true);
                toolSlot.InitData(ownedOutfit);
                equipOutfit.Equip(ownedOutfit.outfitID, OutfitType.Tool);
                break;
            default:
                break;
        }
    }

    void ClosePanel() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        openAndCloseAnim.OnClose(UIManager.instance.ClosePanelSkin);
    }

    public void OutfitOnClick(int saveId)
    {
        if(outfitTab)
        {
            ShowDetail(saveId);
        }
        else
        {
            OutfitOnChoseToMerge(saveId);
        }
    }
    public void ShowDetail(int saveId) {
        UIManager.instance.ShowPanelOutfitDetail(saveId);
    }

    public void UpdateDataOnSlot(int saveID) {
        for (int i = 0; i < listOutfitSlots.Count; i++)
        {
            if (listOutfitSlots[i].ownedOutfitID == saveID)
            {
                listOutfitSlots[i].ReloadLevel();
                return;
            }
        }
        hatSlot.ReloadLevel();
        clothesSlot.ReloadLevel();
        toolSlot.ReloadLevel();
    }

    public void UpdateTotalBoot() {
        totalBootOfOutfit = ProfileManager.Instance.playerData.outfitSave.GetTotalOutfitBoot();
        txtTotalBootOfOutfit.text = totalBootOfOutfit.ToString() + "%";
    }

    Vector3 defaultAngle = Vector3.zero;
    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPrevDelta = Vector3.zero;
    [SerializeField] float rotateSpeed = 0.3f;
    public void Drag()
    {
        if(Input.GetMouseButton(0))
        {
            mPrevDelta = Input.mousePosition - mPrevPos;
            staffTransform.Rotate(staffTransform.up, Vector3.Dot(mPrevDelta, -1 * Camera.main.transform.right) * rotateSpeed, Space.World);
        }
        mPrevPos = Input.mousePosition;
    }

    void SwitchTab()
    {
        outfitTab = !outfitTab;
        oufitChangeTab.SetActive(outfitTab);
        oufitMergeTab.SetActive(!outfitTab);
        if (!outfitTab)
        {
            ShowAllOutfitCombinable();
            tabImg.sprite = changeSpr;
        }
        else
        {
            ShowAllOutfit();
            tabImg.sprite = mergerSpr;
        }
            
    }

    void ShowAllOutfit()
    {
        for (int i = 0; i < listOutfitSlots.Count; i++)
        {
            listOutfitSlots[i].gameObject.SetActive(true);
        }
    }
    void ShowAllOutfitCombinable()
    {
        newItem.DeSelectOutfit();
        c_outfitName.text = "";
        c_outfitBuff.text = "";
        newInfoObj.SetActive(false);
        for (int i = 0; i < mergeSlots.Count; i++)
        {
            mergeSlots[i].DeSelectOutfit();
            if (i != 0)
                mergeSlots[i].gameObject.SetActive(false);
        }
        plusSymbol.SetActive(false);
        for (int i = 0; i < listOutfitSlots.Count; i++)
        {
            if(listOutfitSlots[i].myRarityInfor.rarity != Rarity.Mystic &&
                !ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfit(listOutfitSlots[i].ownedOutfitID).onUsing)
                listOutfitSlots[i].gameObject.SetActive(true);
            else
                listOutfitSlots[i].gameObject.SetActive(false);
        }
        choosedCount = 0;
        combineBtn.gameObject.SetActive(false);
        combineHintObj.SetActive(true);
    }


    void OutfitOnChoseToMerge(int ownedOutfitID)
    {
        if(!IsOutfitChoosed(ownedOutfitID))
        {
            OwnedOutfit ownedOutfit = ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfit(ownedOutfitID);
            if (!mergeSlots[0].choosed)
            {
                choosedCount = 0;
                combineCondition = ProfileManager.Instance.dataConfig.outfitDataConfig.GetCombineConditionByRarity(ownedOutfit.rarity);
                SetCombineSlot(combineCondition.amount);
                newInfoObj.SetActive(true);
                OwnedOutfit newOutfit = new OwnedOutfit();
                newOutfit.outfitType = ownedOutfit.outfitType;
                newOutfit.outfitID = ownedOutfit.outfitID;
                newOutfit.rarity = ownedOutfit.rarity + 1;
                newOutfit.level = ownedOutfit.level;
                newItem.InitData(newOutfit);
                OutfitData outfitData = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(ownedOutfit.outfitID, ownedOutfit.outfitType);
                c_outfitName.text = outfitData.outfitName;
                c_outfitBuff.text = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitIncreaseByRarity(ownedOutfit.rarity);
            }
            if (choosedCount < combineCondition.amount)
            {
                mergeSlots[choosedCount].InitData(ownedOutfit);
                choosedCount++;
                ShowCombineCheck();
                ShowCompatibleOutfit(ownedOutfit.outfitType, ownedOutfit.rarity, ownedOutfit.outfitID);
            }
        } 
        else
        {
            OutfitOnDeSelect(ownedOutfitID);
        }
    }

    void SetCombineSlot(int slot)
    {
        for (int i = 0; i < mergeSlots.Count; i++)
        {
            if(i < slot)
            {
                mergeSlots[i].gameObject.SetActive(true);
            }
            else
            {
                mergeSlots[i].gameObject.SetActive(false);
            }
        }
        plusSymbol.SetActive(true);
    }

    void OutfitOnDeSelect(int id)
    {
        for (int i = 0; i < mergeSlots.Count; i++)
        {
            if (mergeSlots[i].ownedOutfitID == id)
            {
                if(i == 0)
                {
                    ShowAllOutfitCombinable();
                }
                else
                {
                    OwnedOutfit ownedOutfit = ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfit(id);
                    mergeSlots[i].DeSelectOutfit();
                    choosedCount--;
                    ShowCombineCheck();
                    ShowCompatibleOutfit(ownedOutfit.outfitType, ownedOutfit.rarity, ownedOutfit.outfitID);
                }
            }
        }
    }


    void ShowCompatibleOutfit(OutfitType type, Rarity rarity, int id)
    {
        for (int i = 0; i < listOutfitSlots.Count; i++)
        {
            if(listOutfitSlots[i].outfitType == type && listOutfitSlots[i].myRarityInfor.rarity == rarity && !IsOutfitChoosed(listOutfitSlots[i].ownedOutfitID) &&
                !ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfit(listOutfitSlots[i].ownedOutfitID).onUsing)
            {
                if(combineCondition.sameId)
                {
                    if(listOutfitSlots[i].myOutfitData.outfitID == id)
                        listOutfitSlots[i].gameObject.SetActive(true);
                    else
                        listOutfitSlots[i].gameObject.SetActive(false);

                }
                else
                {
                    listOutfitSlots[i].gameObject.SetActive(true);
                }
                
            }
            else
            {
                listOutfitSlots[i].gameObject.SetActive(false);
            }
        }
    }

    bool IsOutfitChoosed(int id)
    {
        for (int i = 0; i < mergeSlots.Count; i++)
        {
            if(mergeSlots[i].ownedOutfitID == id)
            {
                return true;
            }
        }
        return false;
    }

    void CombineOutfit()
    {
        OwnedOutfit ownedOutfit = ProfileManager.Instance.playerData.outfitSave.OnCombineOutfit(mergeSlots[0].ownedOutfitID);
        ProfileManager.Instance.playerData.outfitSave.RemoveOwnedOutfit(mergeSlots[1].ownedOutfitID);
        ProfileManager.Instance.playerData.outfitSave.RemoveOwnedOutfit(mergeSlots[2].ownedOutfitID);
        ProfileManager.Instance.playerData.SaveData();
        mergeSlots[0].InitData(ownedOutfit);
        mergeSlots[1].DeSelectOutfit();
        mergeSlots[2].DeSelectOutfit();
        PanelOutfit.Instance.ChangeData();
        PanelOutfit.Instance.InitData();
        choosedCount = 1;
        ShowCompatibleOutfit(ownedOutfit.outfitType, ownedOutfit.rarity, ownedOutfit.outfitID);
        ShowCombineCheck();
    }

    void ShowCombineCheck()
    {
        if(choosedCount == combineCondition.amount)
        {
            combineBtn.gameObject.SetActive(true);
            combineHintObj.SetActive(false);
        }
        else
        {
            combineBtn.gameObject.SetActive(false);
            combineHintObj.SetActive(true);
        }
    }
}
