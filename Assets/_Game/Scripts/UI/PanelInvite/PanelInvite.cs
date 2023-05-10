using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelInvite : UIPanel
{
    public static PanelInvite Instance;
    [SerializeField] Button btnExit;
    public List<UIInviteSlot> uIInviteSlots = new List<UIInviteSlot>();
    bool dataInited = false;

    public override void Awake()
    {
        Instance = this;
        panelType = UIPanelType.PanelInvite;
        base.Awake();
        btnExit.onClick.AddListener(ClosePanel);
    }

    private void OnEnable()
    {
        if(!dataInited)
        {
            InitData();
            dataInited = true;
        }
    }

    void InitData() {
        for (int i = 0; i < uIInviteSlots.Count; i++)
            uIInviteSlots[i].InitData();
    }

    public void UnInit()
    {
        dataInited = false;
    }

    void ClosePanel() {
        UIManager.instance.ClosePanelInvite();
    }

}
