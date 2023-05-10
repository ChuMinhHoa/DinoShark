using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveDataBase
{
    public bool IsChangeData;
    public string stringSave;
    public virtual void SetStringSave(string stringSave) { this.stringSave = stringSave; }
    public virtual void LoadData(int level) { }
    public virtual void LoadData() { }

    public virtual void IsMarkChangeData()
    {
        IsChangeData = true;
    }

    public virtual void SaveData(int level)
    {
        if (!IsChangeData) return;
        IsChangeData = false;
        PlayerPrefs.SetString(stringSave + level, JsonUtility.ToJson(this).ToString());
    }

    public virtual void SaveData() {
        if (!IsChangeData) return;
        IsChangeData = false;
        PlayerPrefs.SetString(stringSave, JsonUtility.ToJson(this).ToString());
    }

    public string GetJsonData(int level)
    {
        return PlayerPrefs.GetString(stringSave + level);
    }

    public string GetJsonData() {
        return PlayerPrefs.GetString(stringSave);
    }
}
