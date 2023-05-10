using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProduct : MonoBehaviour
{
    Merchandise merchandise;
    int currentLevel;
    [SerializeField] Image iconUI;
    [SerializeField] Text levelUI;
    string LEVEL = "Level ";
    [SerializeField] Text desUI;
    [SerializeField] Text increaseUI;
    string NOPE = "";
    [SerializeField] Text priceUI;
    [SerializeField] GameObject gemIcon;
    [SerializeField] Button buyBtn;
    [SerializeField] RectTransform priceRect;

    [SerializeField] GameObject lockObject;
    [SerializeField] Image lockIconUI;

    [SerializeField] GameObject conditionObj;
    // Start is called before the first frame update
    public void Setup(Merchandise info)
    {
        merchandise = info;
        buyBtn.onClick.AddListener(BuyProduct);
        currentLevel = ProfileManager.Instance.playerData.merchandiseSave.GetMerchandiseLevel(merchandise.merchantType);
        //UpdateProduct();
    }

    public void UpdateProduct()
    {
        if (CheckNeededCondition())
        {
            conditionObj.SetActive(false);
            if (!CheckMaxLevel())
            {
                if (currentLevel == 0)
                {
                    lockObject.SetActive(true);
                    lockIconUI.sprite = merchandise.sprOff;
                }
                else
                {
                    lockObject.SetActive(false);
                    iconUI.sprite = merchandise.sprOn;
                    levelUI.text = LEVEL + currentLevel.ToString();
                    desUI.text = ProfileManager.Instance.dataConfig.merchantDataConfig.GetValueByLevel(merchandise.merchantType, currentLevel).ToString() + merchandise.description;
                }
                increaseUI.text = ProfileManager.Instance.dataConfig.merchantDataConfig.GetValueIncreaseFromLevel(merchandise.merchantType, currentLevel);
                int price = ProfileManager.Instance.dataConfig.merchantDataConfig.GetPriceByLevel(merchandise.merchantType, currentLevel + 1);
                LayoutRebuilder.MarkLayoutForRebuild(priceRect);
                priceUI.text = price.ToString();
                gemIcon.SetActive(true);
                if(price == 0)
                {
                    priceUI.text = "Free";
                    gemIcon.SetActive(false);
                }
                if (ProfileManager.Instance.playerData.globalResourceSave.IsEnoughGem(price))
                {
                    buyBtn.interactable = true;
                }
                else
                {
                    buyBtn.interactable = false;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(priceRect);
                StartCoroutine(RectCoroutine());
            }
            else
            {
                lockObject.SetActive(false);
                iconUI.sprite = merchandise.sprOn;
                levelUI.text = LEVEL + currentLevel.ToString();
                desUI.text = ProfileManager.Instance.dataConfig.merchantDataConfig.GetValueByLevel(merchandise.merchantType, currentLevel).ToString() + merchandise.description;
                increaseUI.text = NOPE;
                buyBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            lockObject.SetActive(true);
            lockIconUI.sprite = merchandise.sprOff;
            buyBtn.interactable = false;
            increaseUI.text = ProfileManager.Instance.dataConfig.merchantDataConfig.GetValueIncreaseFromLevel(merchandise.merchantType, currentLevel);
            conditionObj.SetActive(true);
        }
    }
    IEnumerator RectCoroutine()
    {
        yield return new WaitForSeconds(0.35f);
        LayoutRebuilder.MarkLayoutForRebuild(priceRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(priceRect);
    }
    bool CheckMaxLevel()
    {
        if (currentLevel == ProfileManager.Instance.dataConfig.merchantDataConfig.GetMaxLevelByType(merchandise.merchantType))
        {
            return true;
        }
        return false;
    }

    bool CheckNeededCondition()
    {
        return ProfileManager.Instance.playerData.merchandiseSave.CheckPreviousMerchandiseBought(merchandise.merchantType);
    }

    PanelMerchandise panelMerchandise;
    void BuyProduct()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (ProfileManager.Instance.playerData.merchandiseSave.BuyMerchandise(merchandise.merchantType))
        {
            currentLevel++;
            if(panelMerchandise == null)
            {
                panelMerchandise = UIManager.instance.GetPanel(UIPanelType.PanelMerchandise).GetComponent<PanelMerchandise>();
            }
            panelMerchandise.Reload();
        }
    }
}
