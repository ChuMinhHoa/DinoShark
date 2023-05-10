using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCheat : UIPanel
{
    [SerializeField] Button btnExit;
    [SerializeField] Button bgExit;
    [SerializeField] UISwitch removeAdsSwitch;
    [SerializeField] Button btnAddGems;
    [SerializeField] Button btnAddMoneys;
    BigNumber moneyToAdd = new BigNumber();
    [SerializeField] Button unlockItem;
    [SerializeField] Button showUIBtn;
    [SerializeField] Button btnDropdown;
    [SerializeField] Dropdown dropDownLevel;
    bool enablePanel = false;
    bool activate = false;
    int currentLevel;

    public override void Awake()
    {
        panelType = UIPanelType.PanelCheat;
        base.Awake();
        btnExit.onClick.AddListener(ClosePanel);
        bgExit.onClick.AddListener(ClosePanel);
        btnAddGems.onClick.AddListener(AddGems);
        btnAddMoneys.onClick.AddListener(AddMoneys);
        removeAdsSwitch.Setup(ProfileManager.Instance.playerData.globalResourceSave.removeAds);
        removeAdsSwitch.AddListener(RemoveAdsSwitch);
        unlockItem.onClick.AddListener(UnlockItem);
        unlockItem.interactable = !ProfileManager.Instance.playerData.outfitSave.cheated;
        showUIBtn.onClick.AddListener(ShowUI);
        OnLoadLevel();
        btnDropdown.onClick.AddListener(DropdownOnClick);
        dropDownLevel.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropDownLevel);
        });
    }

    private void OnEnable()
    {
        enablePanel = true;
        dropDownLevel.value = ProfileManager.Instance.playerData.currentLevel;
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
    }

    void ClosePanel()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (openAndCloseAnim != null)
            openAndCloseAnim.OnClose(UIManager.instance.ClosePanelCheat);
        else UIManager.instance.ClosePanelCheat();
    }

    void AddGems()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.globalResourceSave.AddGem(10000);
    }
    void SetupMoney()
    {
        moneyToAdd.value = 9;
        moneyToAdd.exp = 27 * 3;
    }
    void AddMoneys()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        SetupMoney();
        ProfileManager.Instance.playerData.AddMoney(moneyToAdd);
    }

    void RemoveAdsSwitch()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.globalResourceSave.SetRemoveAds(!ProfileManager.Instance.playerData.globalResourceSave.removeAds);
    }

    void UnlockItem()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.outfitSave.UnlockAllItem();
        unlockItem.interactable = false;
    }

    void ShowUI()
    {
        UIManager.instance.GetTotalPanel().ShowUI();
        ClosePanel();
    }

    void OnLoadLevel()
    {
        dropDownLevel.options.Clear();
        for (int i = 0; i < 7; i++)
        {
            dropDownLevel.options.Add(new Dropdown.OptionData() { text = "Level " + (i + 1).ToString() });
        }
        dropDownLevel.value = ProfileManager.Instance.playerData.currentLevel;
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
    }

    public void DropdownOnClick()
    {
        enablePanel = false;
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        activate = !activate;
        if (activate)
            dropDownLevel.Show();
        else
            dropDownLevel.Hide();
    }

    void DropdownValueChanged(Dropdown change)
    {
        if(!enablePanel)
        {
            SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
            activate = false;
            if (currentLevel != change.value)
            {
                ChangeLevel(change.value);  
            }
        }
    }

    void ChangeLevel(int level)
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(TutorialType)).Length; i++)
        {
            ProfileManager.Instance.playerData.tutorialSave.AddTutorialDone((TutorialType)i);
        }
        ProfileManager.Instance.playerData.SaveCurrentLevel(level);
        ProfileManager.Instance.playerData.SaveData();
        LoadSceneManager.Instance.ChangeLevel(currentLevel, true);
        currentLevel = level;
        ClosePanel();
    }
}
