using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardSlot : MonoBehaviour
{
    public Button btnGet;

    public Text txtTitle;
    public Text txtAmout;
    public Text txtTime;

    public GameObject objGet;
    public GameObject objCheck;
    public GameObject objTime;
    public GameObject objAlert;
    public GameObject objBlur;

    public Image icon;
    public Image imgCheck;

    public Sprite sprCheck;
    public Sprite sprShark;

    public int day;

    bool showTime;
    public virtual void Start()
    {
        btnGet.onClick.AddListener(GetReward);
    }
    public OutfitData outfitData;
    public virtual void InitData(DailyReward data) {
        txtTitle.text = "Day "+data.day.ToString();
        day = data.day;
        if (data.itemEquips.Count > 0)
        {
            outfitData = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(data.itemEquips[0].itemId, data.itemEquips[0].outfitType);
            icon.sprite = outfitData.outfitIcon;
        }
        else {
            icon.sprite = data.itemRewards[0].icon;
            txtAmout.text = data.itemRewards[0].amount.ToString();
        }
    }

    public virtual void ReceivedMode() {
        objTime.SetActive(false);
        objCheck.SetActive(true);
        objGet.SetActive(true);
        objAlert.SetActive(false);
        objBlur.SetActive(true);
        btnGet.gameObject.SetActive(false);
        imgCheck.sprite = sprCheck;
        showTime = false;
    }

    public virtual void CanReceiveMode() {
        objTime.SetActive(false);
        objCheck.SetActive(false);
        objGet.SetActive(false);
        objAlert.SetActive(true);
        objBlur.SetActive(false);
        btnGet.gameObject.SetActive(true);
        showTime = false;
    }

    public virtual void LockMode(bool timeShow) {
        showTime = timeShow;
        objTime.SetActive(timeShow);
        objCheck.SetActive(!timeShow);
        objGet.SetActive(false);
        objAlert.SetActive(false);
        objBlur.SetActive(false);
        btnGet.gameObject.SetActive(false);
        imgCheck.sprite = sprShark;
    }

    public virtual void GetReward() {
        ReceivedMode();
        ProfileManager.Instance.playerData.dailyRewardManager.GetReward(day);        
    }

    public virtual void Update()
    {
        if (showTime)
        {
            txtTime.text = ProfileManager.Instance.playerData.dailyRewardManager.GetTimeRemain();
        }
    }
}
