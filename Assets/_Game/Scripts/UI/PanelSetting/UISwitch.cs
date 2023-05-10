using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISwitch : MonoBehaviour
{
    [SerializeField] Color onColor;
    [SerializeField] Color offColor;
    [SerializeField] Transform onPos;
    [SerializeField] Transform offPos;
    [SerializeField] Text des;
    [SerializeField] Transform handle;
    [SerializeField] Image bg;
    [SerializeField] Button switchBtn;
    bool currentActive;
    UnityAction onClickAction;

    private void Start()
    {
        switchBtn.onClick.AddListener(SwitchState);
    }

    public void AddListener(UnityAction actionIn)
    {
        onClickAction = actionIn;
    }

    public void Setup(bool active)
    {
        currentActive = active;
        if (active)
        {
            handle.position = onPos.position;
            des.text = "On";
            des.transform.position = offPos.position;
            bg.color = onColor;
        }
        else
        {
            handle.position = offPos.position;
            des.text = "Off";
            des.transform.position = onPos.position;
            bg.color = offColor;
        }
    }

    public void SwitchState()
    {
        switchBtn.interactable = false;
        if (currentActive)
        {
            handle.DOMove(offPos.position, 0.25f).OnComplete( () => { switchBtn.interactable = true; } );
            des.text = "Off";
            des.transform.position = onPos.position;
            bg.color = offColor;
        }
        else
        {
            handle.DOMove(onPos.position, 0.25f).OnComplete(() => { switchBtn.interactable = true; });
            des.text = "On";
            des.transform.position = offPos.position;
            bg.color = onColor;
        }
        currentActive = !currentActive;
        onClickAction();
    }
}
