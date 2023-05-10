using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundID {
    MAIN_ORDER,
    MAIN_COOK,
    MAIN_MONEY,
    UI_BUTTON,
    UI_POPUP
}
public class SoundManager : MonoBehaviour {
    public static SoundManager instance;
    public AudioSource _SourceBG;
    public AudioSource _SourceEffect;
    public AudioClip main_order, main_cook, main_money, ui_button, ui_popup;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        PlayAudio();
    }
    public void PlayAudio()
    {
        if (ProfileManager.Instance.IsMusicOn()) PlayMusic();
        else PauseMusic();
        if (ProfileManager.Instance.IsSoundOn()) PlaySound();
        else PauseSound();
    }
    public void ChangeMusicState(bool play)
    {
        if(play)
        {
            PlayMusic();
        }
        else
        {
            PauseMusic();
        }
    }
    public void PauseMusic() {
        _SourceBG.volume = 0;
    }
    public void PlayMusic() {
        if (ProfileManager.Instance.IsMusicOn()) {
            if (!_SourceBG.isPlaying) _SourceBG.Play();
             _SourceBG.volume = 1;
        }
       
    }

    public void ChangeSoundState(bool play)
    {
        if (play)
        {
            PlaySound();
        }
        else
        {
            PauseSound();
        }
    }
    public void PlaySound() {
        _SourceEffect.volume = 0.35f;
    }
    public void PauseSound() {
        _SourceEffect.volume = 0;
    }
    
    public void PlaySoundEffect(SoundID id) {
        switch (id)
        {
            case SoundID.MAIN_ORDER:
                _SourceEffect.PlayOneShot(main_order);
                break;
            case SoundID.MAIN_COOK:
                _SourceEffect.PlayOneShot(main_cook);
                break;
            case SoundID.MAIN_MONEY:
                _SourceEffect.PlayOneShot(main_money);
                break;
            case SoundID.UI_BUTTON:
                _SourceEffect.PlayOneShot(ui_button);
                break;
            case SoundID.UI_POPUP:
                _SourceEffect.PlayOneShot(ui_popup);
                break;
            default:
                break;
        }
    }
}
