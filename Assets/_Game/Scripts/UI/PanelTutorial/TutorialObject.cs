using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum TutorialConditionType
{
    None,
    Money,
    Gem,
    Skin,
    CanExpand,
    WaitOrder,
    TotalFoodType,
    TotalUpgrade,
    UpgradeID
}
public class TutorialObject : MonoBehaviour, IPointerClickHandler
{
    public TutorialType tutorialType;
    public int levelTutorial;
    public int stepIndex;
    bool isRegisted = false;
    public bool hide;
    public bool autoShow;
    public Vector2 vectorOffset;
    bool isDoneStep;
    public bool stepDontNeedTouch;

    void Update() {
        if (TutorialManager.Instance.canRegistutorial && !isRegisted)
        {
            TutorialManager.Instance.RegisterStep(this);
            isRegisted = true;
        }
    }

    public void OnStepDone() {
        if (stepDontNeedTouch) return;
        if (isDoneStep)
            return;
        Tutorial currentTutorial = TutorialManager.Instance.GetCurrentTutorial();
        if (currentTutorial == null) return;
        int currentStep = TutorialManager.Instance.GetCurrentIndexStep();
        if (currentTutorial.tutorialType != tutorialType || !UIManager.instance.IsHaveTutorial() || stepIndex > currentStep)
            return;
        isDoneStep = TutorialManager.Instance.IsDoneStepTutorial(tutorialType, stepIndex);
        if (ProfileManager.Instance.playerData.currentLevel == levelTutorial && !isDoneStep)
        {
            isDoneStep = true;
            TutorialManager.Instance.MarkStepDone(this);
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (TutorialManager.Instance == null) return;
        if (tutorialType == TutorialType.UpgradeMachineLevel1 || tutorialType == TutorialType.UpgradeMachineLevel2)
        {
            if (TutorialManager.Instance.levelHaveTutOrder && !TutorialManager.Instance.isDoneOrder)
                return;
        }
        OnStepDone();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnStepDone();
    }

}
