using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeFill : MonoBehaviour {
    public Image imgFill;
    public float deltaTime;
    public float fillTime;
    bool IsUpdate = true;
    public void Init() {
        IsUpdate = false;
        this.gameObject.SetActive(false);
        //transform.localScale = new Vector3(0.012f, 0.012f, 1);
    }
    public void InitTime(float time) {
        IsUpdate = true;
        fillTime = time;
        deltaTime = 0;
        if (imgFill) imgFill.fillAmount = 0;
        this.gameObject.SetActive(true);
    }
    public bool IsFinish() {
        return deltaTime >= fillTime;
    }
    public void Hide() {
        IsUpdate = false;
        this.gameObject.SetActive(false);
    }
    private void Update() {
        if (!IsUpdate) return;
        deltaTime += Time.deltaTime;
        if (imgFill) imgFill.fillAmount = deltaTime / fillTime;
        if(imgFill.fillAmount >= 1)
        {
            Hide();
        }
    }
}
