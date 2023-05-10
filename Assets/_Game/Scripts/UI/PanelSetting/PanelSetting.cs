using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSetting : UIPanel
{
    [SerializeField] Button btnExit;
    [SerializeField] Button bgExit;
    [SerializeField] UISwitch soundSwitch;
    [SerializeField] UISwitch musicSwitch;
    [SerializeField] Button btnCheat;
    public override void Awake()
    {
        panelType = UIPanelType.PanelSetting;
        base.Awake();
        btnExit.onClick.AddListener(ClosePanel);
        bgExit.onClick.AddListener(ClosePanel);
        soundSwitch.AddListener(ChangeSoundState);
        musicSwitch.AddListener(ChangeMusicState);
        if(btnCheat)
            btnCheat.onClick.AddListener(OpenPanelCheat);
    }
    private void OnEnable()
    {
        soundSwitch.Setup(ProfileManager.Instance.playerData.globalResourceSave.soundOn);
        musicSwitch.Setup(ProfileManager.Instance.playerData.globalResourceSave.musicOn);
    }
    void ClosePanel()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (openAndCloseAnim != null)
            openAndCloseAnim.OnClose(UIManager.instance.ClosePanelSetting);
        else UIManager.instance.ClosePanelSetting();
    }

    public void ChangeSoundState()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.globalResourceSave.ChangeSoundState();
        ProfileManager.Instance.playerData.SaveData();
    }
    public void ChangeMusicState()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.globalResourceSave.ChangeMusicState();
        ProfileManager.Instance.playerData.SaveData();
    }

    void OpenPanelCheat()
    {
        UIManager.instance.ShowPanelCheat();
    }
}
