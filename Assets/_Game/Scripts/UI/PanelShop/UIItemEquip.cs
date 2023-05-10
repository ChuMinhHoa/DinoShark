using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemEquip : MonoBehaviour
{
    [SerializeField] Image frame;
    [SerializeField] Image icon;
    [SerializeField] Text des;
    [SerializeField] Image type;
    [SerializeField] Image typeFrame;
    public void Init(ItemEquip item)
    {
        gameObject.SetActive(true);
        if(item.icon)
            icon.sprite = item.icon;
        des.text = "Level 1";
    }
    
    public void ShowItemInfo()
    {

    }
}
