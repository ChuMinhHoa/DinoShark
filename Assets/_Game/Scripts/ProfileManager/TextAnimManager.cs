using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
[System.Serializable]
public class TextRegisted {
    public Text textRegisted;
    Vector3 vectorScale;
    Vector3 vectorDefault;
    bool onChange;
    public TextRegisted(Text textRegisted, Vector3 vectorScale, Vector3 vectorDefault) {
        
        this.textRegisted = textRegisted;
        this.vectorScale = vectorScale;
        this.vectorDefault = vectorDefault;
    }
    public void ChangeString(string newString, float timeAnim) {
        if (!onChange)
        {
            onChange = true;
            textRegisted.transform.DOScale(vectorScale, timeAnim).OnComplete(() => {
                textRegisted.transform.DOScale(new Vector3(1.1f, 0.9f, 1f), timeAnim).OnComplete(() => {
                    textRegisted.text = newString;
                    textRegisted.transform.DOScale(vectorDefault, timeAnim).OnComplete(() =>
                    {
                        onChange = false;
                    });
                });
            });
        }
    }
}
public class TextAnimManager : Singleton<TextAnimManager>
{
    public Dictionary<int, TextRegisted> listText = new Dictionary<int, TextRegisted>();
    [SerializeField] Vector3 vectorScale;
    [SerializeField] Vector3 vectorDefault;
    int countText = 0;
    public void RegisterText(Text textRegis) {
        TextRegisted textRegisted = new TextRegisted(textRegis, vectorScale, vectorDefault);
        listText.Add(countText, textRegisted);
        countText++;
    }
    public TextRegisted GetText(Text textCheck)
    {
        foreach (KeyValuePair<int, TextRegisted> item in listText)
        {
            if (item.Value.textRegisted == textCheck)
            {
                return item.Value;
            }
        }
        return null;
    }
    public void ChangeText(Text textCheck, string newString, float timeAnim) {
        if (GetText(textCheck) != null)
            GetText(textCheck).ChangeString(newString, timeAnim);
        else { 
        
        }
    }
}
