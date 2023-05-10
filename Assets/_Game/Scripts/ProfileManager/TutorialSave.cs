using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TutorialType {
    UpgradeMachineLevel1,
    Upgrade,
    ExpandMap,
    Skin,
    Merchandise,
    InviteStaff,
    Marketing,
    BuyFirstPack,
    UpgradeMachineLevel2,
    UpgradePros,
    UnlockNewStaff,
    UnlockNewFloor,
    UnlockNewTable
}

[System.Serializable]
public class TutorialSave : SaveDataBase
{

    public List<TutorialType> tutorial = new List<TutorialType>();

    public override void LoadData()
    {
        SetStringSave("Tutorial_");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            TutorialSave dataSave = JsonUtility.FromJson<TutorialSave>(jsonData);
            tutorial = dataSave.tutorial;
        }
        else
        {
            IsMarkChangeData();
            SaveData();
        }
    }

    public bool IsDoneTutorial(TutorialType tutorialType) {
        for (int i = 0; i < tutorial.Count; i++)
        {
            if (tutorial[i] == tutorialType)
                return true;
        }
        return false;
    }

    public void AddTutorialDone(TutorialType tutorialType) { 
        tutorial.Add(tutorialType);
        IsMarkChangeData();
    }
}
