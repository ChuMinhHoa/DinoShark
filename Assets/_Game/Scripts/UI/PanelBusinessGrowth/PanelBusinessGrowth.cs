using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBusinessGrowth : UIPanel
{
    [SerializeField] Button btnExit;
    [SerializeField] Button btnExpand;
    [SerializeField] Text txtCondition;
    [SerializeField] Text txtCurrentMap;
    [SerializeField] Text txtNextMap;
    [SerializeField] Image imgIconLevelCurrent;
    [SerializeField] Image imgIconLevelNext;
    [SerializeField] Animator animBtnExpand;
    int levelMaxOfMap;
    int maxLevel;
    int currentLevel;
    bool canExpand;
    LevelDataConfig levelDataConfig;
    [SerializeField] GameObject hiden;
    [SerializeField] GameObject mainObj;

    public override void Awake()
    {
        panelType = UIPanelType.PanelBusinessGrowth;
        base.Awake();
        btnExit.onClick.AddListener(OnClose);
        btnExpand.onClick.AddListener(OnExpand);
        levelDataConfig = ProfileManager.Instance.dataConfig.levelDataConfig;
        maxLevel = 7;
        btnExpand.interactable = false;
    }
    public void OnOpen() {
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
        levelMaxOfMap = ProfileManager.Instance.dataConfig.menuDataConfig.GetMenuDataByLevel(currentLevel).GetLevelMax();
        canExpand = ProfileManager.Instance.playerData.GetMenuSave().CanExpandNewMap();
        imgIconLevelCurrent.sprite = levelDataConfig.levelDatas[currentLevel].levelSpriteOn;
        txtCurrentMap.text = levelDataConfig.levelDatas[currentLevel].levelName;
        if ((currentLevel < maxLevel - 1))
        {
            txtNextMap.text = levelDataConfig.levelDatas[currentLevel + 1].levelName;
            if (canExpand)
                imgIconLevelNext.sprite = levelDataConfig.levelDatas[currentLevel + 1].levelSpriteOn;
            else
                imgIconLevelNext.sprite = levelDataConfig.levelDatas[currentLevel + 1].levelSpriteOff;
        }
        else
        {
            imgIconLevelNext.sprite = levelDataConfig.sprLevelComingSoon;
            txtNextMap.text = "Comming Soon!";
        }
        btnExpand.interactable = canExpand && (currentLevel < maxLevel - 1);
        txtCondition.gameObject.SetActive(!btnExpand.interactable);
        animBtnExpand.SetBool("Interact", btnExpand.interactable);
        txtCondition.text = "Reach <color=green>" + levelMaxOfMap + "</color> on all worktops!";
        hiden.SetActive(currentLevel >= maxLevel - 1);
        mainObj.SetActive(currentLevel < maxLevel - 1);
    }

    public void OnClose() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        openAndCloseAnim.OnClose(()=> {
            UIManager.instance.ClosePanelBusinessGrowth();
        });
    }

    void OnExpand() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.globalResourceSave.AddSuitcase(OfferID.NobleSuitcase, 2);
        ProfileManager.Instance.playerData.SaveCurrentLevel((ProfileManager.Instance.playerData.currentLevel + 1));
        LoadSceneManager.Instance.ChangeLevel();
        UIManager.instance.ClosePanelBusinessGrowth();
    }
}
