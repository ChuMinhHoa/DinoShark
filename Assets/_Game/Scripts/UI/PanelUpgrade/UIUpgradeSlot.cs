using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeSlot : MonoBehaviour
{
    [SerializeField] Text txtName;
    [SerializeField] Text txtDes;
    [SerializeField] Text txtPrice;
    [SerializeField] Image imgIcon;
    [SerializeField] Image imgTypeBoot;
    [SerializeField] Button btnUpgrade;
    [SerializeField] RectTransform priceRect;
    [SerializeField] GameObject condition;
    [SerializeField] RectTransform coinRect;
    int floor;
    BigNumber price = new BigNumber();
    UpgradeData upgradeData;
    PanelUpgrade panelUpgrade;
    private void Awake()
    {
        btnUpgrade.onClick.AddListener(OnUpgrade);
        EventManager.AddListener(EventName.ChangeMoney.ToString(), CheckEnoughMoney);
        
    }

    public void CheckUpgraded()
    {
        gameObject.SetActive(!ProfileManager.Instance.playerData.GetUpgradeSave().IsUpgraded(upgradeData.upgradeID));
    }
    public void InitData(UpgradeData upgradeData, Transform pointAddGem = null, float floorIn = -1) {
        if (pointAddGem)
        {
            panelUpgrade = UIManager.instance.GetPanel(UIPanelType.PanelUpgrade).GetComponent<PanelUpgrade>();
        }
        this.upgradeData = upgradeData;

        CheckUpgraded();

        if (upgradeData.sprIcon != null)
            imgIcon.sprite = upgradeData.sprIcon;
        else imgIcon.sprite = null;

        if (upgradeData.sprIconBootType != null) {
            imgTypeBoot.gameObject.SetActive(true);
            imgTypeBoot.sprite = upgradeData.sprIconBootType;
        }
        else {
            imgTypeBoot.gameObject.SetActive(false);
        }
        //LayoutRebuilder.MarkLayoutForRebuild(priceRect);
        SetText();
        price.exp = upgradeData.priceUpgrade.exp;
        price.value = upgradeData.priceUpgrade.value;
        if(price.exp == 0 && price.value == 0)
        {
            if (priceRect)
            {
                priceRect.anchoredPosition = new Vector2(0, 0);
                txtPrice.text = "Free";
            }
            if (coinRect)
            {
                coinRect.gameObject.SetActive(false);
            }
        }
        else
        {
            txtPrice.text = price.ToString();
            // Manual Set Size
            float stringLen = (float)(txtPrice.text.Length);
            if (priceRect)
                priceRect.anchoredPosition = new Vector2(-21, 0);
            if (txtPrice.text.Contains("."))
            {
                //stringLen += 0.5f;
            }
            else
            {
                stringLen += 0.5f;
            }

            if (coinRect)
            {
                coinRect.gameObject.SetActive(true);
                coinRect.anchoredPosition = new Vector2(stringLen * 42 / 2, 0);
            }
                
        }
        
        //LayoutRebuilder.ForceRebuildLayoutImmediate(priceRect); 
        gameObject.SetActive(true);
        targetGemEffectMove = pointAddGem;
        transform.SetAsLastSibling();
        floor = (int)floorIn;
        CheckEnoughMoney();
    }
    void SetText() {
        switch (upgradeData.upgradeType)
        {
            case UpgradeType.AddCheff:
                txtName.text = "Hire staff.";
                txtDes.text = "+1 staff.";
                break;
            case UpgradeType.AddCustomer:
                txtName.text = "Get another customer";
                txtDes.text = "+1 customer.";
                break;
            case UpgradeType.ReduceTime:
                txtName.text = "Quick " + upgradeData.foodID + " plating.";
                txtDes.text = "Reduced " + upgradeData.foodID + " making time.";
                break;
            case UpgradeType.IncreasePrice:
                txtName.text = "Increase money";
                txtDes.text = "x" + upgradeData.refAmount + " " + upgradeData.foodID + " price.";
                break;
            case UpgradeType.IncreaseSpeedCheff:
                txtName.text = "Faster staff.";
                txtDes.text = "Increase staff speed.";
                break;
            case UpgradeType.AddTable:
                txtName.text = "Unlock new table.";
                txtDes.text = "+" + upgradeData.refAmount + " customers.";
                break;
            case UpgradeType.AddTotalBoot:
                txtName.text = "Inscrease Price";
                txtDes.text = "x" + upgradeData.refAmount + " profits.";
                break;
            case UpgradeType.AddFloor:
                txtName.text = "New Investments";
                txtDes.text = "Expand restaurant area";
                break;
            case UpgradeType.AddWaiter:
                txtName.text = "Hire a new waiter";
                txtDes.text = "+1 waiter.";
                break;
            default:
                break;
        }
    }

    void CheckEnoughMoney()
    {
        if(!CheckFloorBuilt())
        {
            btnUpgrade.interactable = false;
        }
        else
        {
            btnUpgrade.interactable = GameManager.Instance.IsHaveEnoughMoney(price);
        } 
    }

    bool CheckFloorBuilt()
    {
        if (floor == -1)
        {
            condition.SetActive(false);
            return true;
        }
        else
        {
            if (ProfileManager.Instance.playerData.GetUpgradeSave().IsFloorUnlocked(floor))
            {
                condition.SetActive(false);
                return true;
            }
            else
            {
                condition.SetActive(true);
                return false;
            }
        }
    }

    Transform targetGemEffectMove;
    void OnUpgrade() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.OnUpgrade(upgradeData);
        if(targetGemEffectMove != null)
        {
            UIValueChangeMove uIValueChange = GameManager.Instance.pooling.GetValueChangeEffect(Vector3.zero);
            uIValueChange.transform.SetParent(UIManager.instance.transform);
            uIValueChange.DoMove(targetGemEffectMove, btnUpgrade.transform, ValueType.Gem, 10, panelUpgrade.SupriseSkarkAnim);
            panelUpgrade.CheckOutOfUpgade();
            gameObject.SetActive(false);
        }
        ProfileManager.Instance.playerData.ConsumeMoney(price);
    }
}
