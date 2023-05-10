using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : Singleton<ProfileManager>
{
    public PlayerData playerData;
    public DataConfig dataConfig;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        dataConfig.Start(playerData);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            playerData.GetResource().AddMoney(10000f);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            playerData.SaveCurrentLevel(playerData.currentLevel);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerPrefs.DeleteKey("MenuSave_" + playerData.currentLevel);
            PlayerPrefs.DeleteKey("ResourceSave_" + playerData.currentLevel);
            PlayerPrefs.DeleteKey("UpgradeSave_" + playerData.currentLevel);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(TutorialType)).Length; i++)
            {
                playerData.tutorialSave.AddTutorialDone((TutorialType)i);
            }
            playerData.SaveData();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerData.tutorialSave.IsMarkChangeData();
            playerData.SaveData();
        }
        playerData.Update();
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerData.dailyRewardManager.IsMarkChangeData();
            playerData.SaveData();
        }
    }

    public bool IsMusicOn()
    {
        return playerData.globalResourceSave.musicOn;
    }

    public bool IsSoundOn()
    {
        return playerData.globalResourceSave.soundOn;
    }
}
