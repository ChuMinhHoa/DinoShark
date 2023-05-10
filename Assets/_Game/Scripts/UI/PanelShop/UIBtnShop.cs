using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtnShop : MonoBehaviour
{
    [SerializeField] GameObject objNotice;
    [SerializeField] Image imgIcon;
    //[SerializeField] UIButtonAnim animIcon;
    PlayerData playerData;
    GlobalResourceSave globalResource;
    ShopDataConfig shopDataConfig;
    public List<OfferID> listSuicases = new List<OfferID>();
    float timeChangeNotice;
    public float timeChangeNoticeSetting;
    int indexSuicaseShow;
    string strCurrentSuicase;
    bool isDoneTutorial;

    private void Start()
    {
        shopDataConfig = ProfileManager.Instance.dataConfig.shopDataConfig;
        EventManager.AddListener(EventName.ChangeSuiteCase.ToString(), LoadData);
    }

    public void LoadData() {
        CheckIsHaveChest();
        CheckIsDoneTutorial();
        if (isDoneTutorial && haveSuicase)
        {
            ChangeSuicase();
            objNotice.SetActive(true);
            timeChangeNotice = timeChangeNoticeSetting;
        }
        else objNotice.SetActive(false);
    }

    private void Update()
    {
        if (haveSuicase && isDoneTutorial)
        {
            if (timeChangeNotice >= timeChangeNoticeSetting)
            {
                ChangeSuicase();
                timeChangeNotice = 0;
            }
            else { timeChangeNotice += Time.deltaTime; }
            //animIcon.OnActive2(10f);
        }
        
    }

    void ChangeSuicase() {
        if (indexSuicaseShow >= listSuicases.Count)
            indexSuicaseShow = 0;
        switch (listSuicases[indexSuicaseShow])
        {
            case OfferID.PeasantSuitcase:
                strCurrentSuicase = "PSuitcase";
                break;
            case OfferID.NobleSuitcase:
                strCurrentSuicase = "NSuitcase";
                break;
            case OfferID.RoyalSuitcase:
                strCurrentSuicase = "RSuitcase";
                break;
            default:
                break;
        }
        if(shopDataConfig == null)
            shopDataConfig = ProfileManager.Instance.dataConfig.shopDataConfig;
        //Debug.Log(shopDataConfig.GetSpriteByName(strCurrentSuicase));
        imgIcon.sprite = shopDataConfig.GetSpriteByName(strCurrentSuicase);
        indexSuicaseShow++;
        
    }
    void CheckIsDoneTutorial() {
        playerData = ProfileManager.Instance.playerData;
        isDoneTutorial = playerData.tutorialSave.IsDoneTutorial(TutorialType.BuyFirstPack);
    }
    bool haveSuicase = false;
    void CheckIsHaveChest() {
        haveSuicase = false;
        playerData = ProfileManager.Instance.playerData;
        globalResource = playerData.globalResourceSave;
        listSuicases.Clear();
        if (globalResource.IsHasSuitcase(OfferID.NobleSuitcase)){
            haveSuicase = true;
            listSuicases.Add(OfferID.NobleSuitcase);
        }

        if (globalResource.IsHasSuitcase(OfferID.PeasantSuitcase))
        {
            haveSuicase = true;
            listSuicases.Add(OfferID.PeasantSuitcase);
        }

        if (globalResource.IsHasSuitcase(OfferID.RoyalSuitcase))
        {
            haveSuicase = true;
            listSuicases.Add(OfferID.RoyalSuitcase);
        }
    }
}
