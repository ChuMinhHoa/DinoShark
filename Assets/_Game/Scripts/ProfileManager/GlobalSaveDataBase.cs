using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalSaveDataBase
{
    public bool IsChangeData;
    string stringSave;
    public virtual void SetStringSave(string stringSave) { this.stringSave = stringSave; }
    public virtual void LoadData() { }

    public virtual void IsMarkChangeData()
    {
        IsChangeData = true;
    }

    public virtual void SaveData()
    {
        if (!IsChangeData) return;
        IsChangeData = false;
        PlayerPrefs.SetString(stringSave, JsonUtility.ToJson(this).ToString());
    }

    public string GetJsonData()
    {
        return PlayerPrefs.GetString(stringSave);
    }
}
