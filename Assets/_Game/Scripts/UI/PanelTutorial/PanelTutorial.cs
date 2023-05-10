using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelTutorial : MonoBehaviour// UIPanel
{
    public static PanelTutorial Instacne;
    [SerializeField] GameObject wrapBorder;
    [SerializeField] GameObject panelBlock;
    [SerializeField] RectTransform rectPanel;

    [SerializeField] RectTransform b_Center;
    [SerializeField] RectTransform b_Left;
    [SerializeField] RectTransform b_Right;
    [SerializeField] RectTransform b_Bottom;
    [SerializeField] RectTransform b_Top;

    [SerializeField] RectTransform desWrapTR;
    [SerializeField] RectTransform desWrapTL;
    [SerializeField] RectTransform desWrapBR;
    [SerializeField] RectTransform desWrapBL;

    [SerializeField] Text tutorialDescriptionTR;
    [SerializeField] Text tutorialDescriptionTL;
    [SerializeField] Text tutorialDescriptionBR;
    [SerializeField] Text tutorialDescriptionBL;
    [SerializeField] Button btnNextStep;
    RectTransform currentRect;
    float currentMultiplyX;
    float currentMultiplyY;
    string currentDescription = "";
    Transform trsTarget;
    Vector3 offSet;
    Vector2 centerSizeDelta;
    bool isOnUI;

    float b_HeighLR;
    float b_WidthL;
    float b_WidthR;
    float b_WidthTB;
    float b_HeighT;
    float b_HeighB;

    float screenHeight;
    float screenWidth;

    Vector2 b_SizeDelta;
    Vector2 b_Position;

    private void Awake()
    {
        Instacne = this;
    }
    private void Start()
    {
        screenHeight = rectPanel.rect.height;
        screenWidth = rectPanel.rect.width;
        btnNextStep.onClick.AddListener(OnNextStep);
    }
    private void Update()
    {
        if (trsTarget != null)
        {
            if (trsTarget.position != Vector3.zero)
            {
                if (isOnUI)
                {
                    b_Center.transform.position = trsTarget.position;
                }
                else
                {
                    b_Center.transform.position = Camera.main.WorldToScreenPoint(trsTarget.position + offSet);
                }
                SetUpCurrentDesWrap();
                SetUpDesWrap(currentRect);
            }
        }
    }

    void SetUpCurrentDesWrap() {
        desWrapTR.gameObject.SetActive(false);
        desWrapTL.gameObject.SetActive(false);
        desWrapBR.gameObject.SetActive(false);
        desWrapBL.gameObject.SetActive(false);
        if (b_Center.anchoredPosition.x > 0 && b_Center.anchoredPosition.y > 0)
        {
            currentRect = desWrapTR;
            desWrapTR.gameObject.SetActive(currentDescription != "" && !string.IsNullOrEmpty(currentDescription));
            tutorialDescriptionTR.text = currentDescription;
            currentMultiplyX = 1;
            currentMultiplyY = -1;
        }
        else if (b_Center.anchoredPosition.x < 0 && b_Center.anchoredPosition.y < 0)
        {
            currentRect = desWrapBL;
            desWrapBL.gameObject.SetActive(currentDescription != "" && !string.IsNullOrEmpty(currentDescription));
            tutorialDescriptionBL.text = currentDescription;
            currentMultiplyX = -1;
            currentMultiplyY = 1;
        }
        else if (b_Center.anchoredPosition.x > 0 && b_Center.anchoredPosition.y < 0)
        {
            currentRect = desWrapBR;
            desWrapBR.gameObject.SetActive(currentDescription != "" && !string.IsNullOrEmpty(currentDescription));
            tutorialDescriptionBR.text = currentDescription;
            currentMultiplyX = 1;
            currentMultiplyY = 1;
        }
        else if (b_Center.anchoredPosition.x < 0 && b_Center.anchoredPosition.y > 0)
        {
            currentRect = desWrapTL;
            desWrapTL.gameObject.SetActive(currentDescription != "" && !string.IsNullOrEmpty(currentDescription));
            tutorialDescriptionTL.text = currentDescription;
            currentMultiplyX = -1;
            currentMultiplyY = -1;
        }
    }

    void SetupBorder() {
        b_HeighLR = screenHeight;
        b_WidthTB = b_Center.rect.width;
        b_WidthL = (screenWidth / 2 + (b_Center.anchoredPosition.x - b_Center.rect.width / 2));
        b_WidthR = (screenWidth / 2 - (b_Center.anchoredPosition.x + b_Center.rect.width / 2));
        b_HeighB = (screenHeight / 2 + (b_Center.anchoredPosition.y - b_Center.rect.height / 2));
        b_HeighT = (screenHeight / 2 - (b_Center.anchoredPosition.y + b_Center.rect.height / 2));

        b_SizeDelta.x = b_WidthL;
        b_SizeDelta.y = b_HeighLR;
        b_Left.sizeDelta = b_SizeDelta;
        b_Position.x = -(b_WidthL / 2 - (b_Center.anchoredPosition.x - b_Center.rect.width / 2));
        b_Position.y = 0;
        b_Left.anchoredPosition = b_Position;

        b_SizeDelta.x = b_WidthR;
        b_SizeDelta.y = b_HeighLR;
        b_Right.sizeDelta = b_SizeDelta;
        b_Position.x = b_WidthR / 2 + (b_Center.anchoredPosition.x + b_Center.rect.width / 2);
        b_Position.y = 0;
        b_Right.anchoredPosition = b_Position;

        b_SizeDelta.x = b_WidthTB;
        b_SizeDelta.y = b_HeighB;
        b_Bottom.sizeDelta = b_SizeDelta;
        b_Position.y = -(b_HeighB / 2 - (b_Center.anchoredPosition.y - b_Center.rect.height / 2));
        b_Position.x = b_Center.anchoredPosition.x;
        b_Bottom.anchoredPosition = b_Position;

        b_SizeDelta.x = b_WidthTB;
        b_SizeDelta.y = b_HeighT;
        b_Top.sizeDelta = b_SizeDelta;
        b_Position.y = b_HeighT / 2 + (b_Center.anchoredPosition.y + b_Center.rect.height / 2);
        b_Position.x = b_Center.anchoredPosition.x;
        b_Top.anchoredPosition = b_Position;

    }

    public void SettingBlackPanel(Transform trsTarget, Vector3 offSet, string description = "", float heigh = 0, float width = 0, bool waitPanel = false) {
        transform.SetAsLastSibling();
        currentDescription = description;
        if (heigh != 0 || width != 0)
        {
            centerSizeDelta.x = width + 200f;
            centerSizeDelta.y = heigh + 200f;
            isOnUI = true;
        }
        else {
            centerSizeDelta.x = 300f;
            centerSizeDelta.y = 300f;
            isOnUI = false;
        }

        b_Center.sizeDelta = centerSizeDelta;
        if (centerSizeDelta.x != 300f)
        {
            centerSizeDelta.x -= 150f;
            centerSizeDelta.y -= 150f;
        }
        else {
            centerSizeDelta.x -= 100f;
            centerSizeDelta.y -= 100f;
        }
        this.trsTarget = trsTarget;
        this.offSet = offSet;

        SetupBorder();

        if (waitPanel)
            StartCoroutine(IE_WaitToNextStep());
        else
        {
            wrapBorder.SetActive(true);
            SetupBorder();
            b_Center.DOSizeDelta(centerSizeDelta, 1f).OnUpdate(() =>
            {
                SetupBorder();
            }).OnComplete(() =>
            {
                SetupBorder();
                TurnOffBlock();
            });
        }
    }
    void SetUpDesWrap(RectTransform desWrap) {
        desWrap.transform.position = b_Center.transform.position;
        Vector2 pointDes = desWrap.anchoredPosition;
        pointDes.y -= ((b_Center.rect.height / 2f + desWrap.rect.height / 2f) * currentMultiplyY);
        pointDes.x -= (((2 * (desWrap.rect.width / 2f)) / 3) * currentMultiplyX);
        desWrap.anchoredPosition = pointDes;
    }
    //cho panel hien len
    IEnumerator IE_WaitToNextStep() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.5f);
        wrapBorder.SetActive(true);
        SetupBorder();
        b_Center.DOSizeDelta(centerSizeDelta, 1f).OnUpdate(() => {
            SetupBorder();
        }).OnComplete(() => {
            SetupBorder();
            TurnOffBlock();
        });
    }

    RectTransform rectTransform;
    public void ShowStep(TutorialStep step) {
        if (step.dontNeedTouch) TurnOnBtnNextStep();
        TurnOnBlock();
        rectTransform = step.trsTarget.GetComponent<RectTransform>();
        if (rectTransform != null)
            SettingBlackPanel(step.trsTarget, step.offSet, step.description, rectTransform.rect.height, rectTransform.rect.width, true);
        else SettingBlackPanel(step.trsTarget, step.offSet, step.description);
    }

    public void TurnOnBlock()
    {
        panelBlock.SetActive(true);
        wrapBorder.SetActive(false);
    }

    public void TurnOnBtnNextStep() {
        btnNextStep.gameObject.SetActive(true);
    }

    public void TurnOffBtnNextStep()
    {
        btnNextStep.gameObject.SetActive(false);
    }

    public void TurnOffBlock()
    {
        panelBlock.SetActive(false);
    }

    void OnNextStep() {
        TutorialManager.Instance.OnNextStep();
        TurnOffBtnNextStep();
    }
}
