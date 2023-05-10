using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    public Transform trsTarget;
    public Vector3 offSet;
    public bool isRegisted;
    public bool isDoneStep;
    public string description;
    public bool dontNeedTouch;
}
[System.Serializable]
public class Tutorial {

    public TutorialType tutorialType;

    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    public TutorialConditionType conditionType;
    public float conditionAmount;
    public int stepIndexMarkDone;
    public int levelTutorial;
    public bool isDone;

    public void InitStep(TutorialData tutorialData) {
        
        for (int i = 0; i < tutorialData.countSteps; i++)
        {
            TutorialStep newStep = new TutorialStep();
            if (tutorialData.stepDescriptions.Count>0)
            {
                if (!string.IsNullOrEmpty(tutorialData.stepDescriptions[i]))
                    newStep.description = tutorialData.stepDescriptions[i];
            }
            
            tutorialSteps.Add(newStep);
        }
    }

    public void RegisterStep(TutorialObject t_Object, bool able) {
        if (tutorialSteps[t_Object.stepIndex].isRegisted)
            return;
        tutorialSteps[t_Object.stepIndex].trsTarget = t_Object.transform;
        tutorialSteps[t_Object.stepIndex].isRegisted = true;
        tutorialSteps[t_Object.stepIndex].offSet = t_Object.vectorOffset;
        tutorialSteps[t_Object.stepIndex].dontNeedTouch = t_Object.stepDontNeedTouch;
        t_Object.gameObject.SetActive(able);
    }

    public bool IsDoneTutorial() {
        return ProfileManager.Instance.playerData.GetTutorialSave().IsDoneTutorial(tutorialType);
    }

    public bool CheckCondition() {
        switch (conditionType)
        {
            case TutorialConditionType.None:
                return true;
            case TutorialConditionType.Money:
                return ProfileManager.Instance.playerData.GetResource().IsHaveEnoughMoney(conditionAmount);
            case TutorialConditionType.Gem:
                //if (!ProfileManager.Instance.playerData.globalResourceSave.IsEnoughGem((int)conditionAmount))
                    //ProfileManager.Instance.playerData.globalResourceSave.AddGem((int)conditionAmount);
                return true;
            case TutorialConditionType.Skin:
                return true;
            case TutorialConditionType.CanExpand:
                return ProfileManager.Instance.playerData.GetMenuSave().CanExpandNewMap();
            case TutorialConditionType.WaitOrder:
                if (!TutorialManager.Instance.isDoneOrder)
                {
                    UIManager.instance.ShowPanelTutorial();
                    PanelTutorial.Instacne.TurnOnBlock();
                }
                return TutorialManager.Instance.isDoneOrder;
            case TutorialConditionType.TotalFoodType:
                return ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.GetToralFoodAble() >= conditionAmount;
            case TutorialConditionType.UpgradeID:
                return ProfileManager.Instance.playerData.GetUpgradeSave().IsUpgraded((int)conditionAmount); // conditionAmount is upgradeID
            default:
               return false;
        }
    }
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public bool canUpdateTutorial = false;
    public bool onTutorial = false;

    public bool canRegistutorial;

    public bool isDoneOrder;
    public bool levelHaveTutOrder;
    public bool tableMoveDone;
    public bool onUnlockMachine;
    public bool onRest;

    int currentLevel;
    private void Awake()
    {
        Instance = this;
    }

    public List<Tutorial> tutorials = new List<Tutorial>();

    public void InitData() {
        TutorialDataConfig tutorialDataConfig = ProfileManager.Instance.dataConfig.tutorialDataConfig;
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
        for (int i = 0; i < tutorialDataConfig.tutorialDatas.Count; i++)
        {
            if (tutorialDataConfig.tutorialDatas[i].level < currentLevel)
                continue;

            if (ProfileManager.Instance.playerData.tutorialSave.IsDoneTutorial(tutorialDataConfig.tutorialDatas[i].tutorialType))
                continue;

            Tutorial newTutorial = new Tutorial();
            newTutorial.tutorialType = tutorialDataConfig.tutorialDatas[i].tutorialType;
            newTutorial.conditionType = tutorialDataConfig.tutorialDatas[i].tutorialConditionType;
            newTutorial.conditionAmount = tutorialDataConfig.tutorialDatas[i].conditionAmount;
            newTutorial.stepIndexMarkDone = tutorialDataConfig.tutorialDatas[i].stepMarkTutorialDone;
            newTutorial.levelTutorial = tutorialDataConfig.tutorialDatas[i].level;

            if (newTutorial.levelTutorial == currentLevel && (newTutorial.tutorialType == TutorialType.UpgradeMachineLevel1 || newTutorial.tutorialType == TutorialType.UpgradeMachineLevel2))
            {
                ChangeDoneOrder(false);
                ChangeLevelHaveTutOrder(true);
            }

            newTutorial.InitStep(tutorialDataConfig.tutorialDatas[i]);

            tutorials.Add(newTutorial);
        }
        canRegistutorial = true;
    }

    public void ResetTutorial() {
        canUpdateTutorial = true;
        onTutorial = false;
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
        stepIndex = 0;
        for (int i = 0; i < tutorials.Count; i++)
        {
            if (tutorials[i].levelTutorial == currentLevel && (tutorials[i].tutorialType == TutorialType.UpgradeMachineLevel1 || tutorials[i].tutorialType == TutorialType.UpgradeMachineLevel2))
            {
                levelHaveTutOrder = true;
                isDoneOrder = false;
                break;
            }
        }
    }

    int tutorialIndex;
    int stepIndex;
    TutorialStep currentStep;
    private void Update()
    {
        if (!onTutorial && canUpdateTutorial && !onRest && !UIManager.instance.isHasPriorityPanel) 
        {
            if (tutorialIndex == tutorials.Count) return;

            if (tutorials[tutorialIndex].levelTutorial > currentLevel)
            {
                canUpdateTutorial = false;
                return;
            }

            if (!tutorials[tutorialIndex].CheckCondition() && stepIndex == 0)
            {
                return;
            }
            if (tutorials[tutorialIndex].tutorialType == TutorialType.UnlockNewTable && !tableMoveDone) return;
            currentStep = tutorials[tutorialIndex].tutorialSteps[stepIndex];
            if (!currentStep.isRegisted) return;
            currentStep.trsTarget.gameObject.SetActive(true);
            if (stepIndex == 0) UIManager.instance.ShowPanelTutorial();
            UIManager.instance.panelTutorial.ShowStep(tutorials[tutorialIndex].tutorialSteps[stepIndex]);
            onTutorial = true;
        }
    }

    Tutorial tutorialOnMarkDoneStep;
    public void MarkStepDone(TutorialObject t_Object) {
        tutorialOnMarkDoneStep = GetTutorial(t_Object.tutorialType);

        if (tutorialOnMarkDoneStep == null)
            return;

        if (GetTutorial(t_Object.tutorialType).stepIndexMarkDone == t_Object.stepIndex)
        {
            ProfileManager.Instance.playerData.GetTutorialSave().AddTutorialDone(t_Object.tutorialType);
            ProfileManager.Instance.playerData.SaveData();
        }
        OnNextStep();
    }

    public void TutorialRest() {
        onRest = true;
        StartCoroutine(IE_TutorialRest());
    }

    IEnumerator IE_TutorialRest()
    {
        yield return new WaitForSeconds(3f);
        onRest = false;
        tutorialIndex++;
        if (tutorialIndex < tutorials.Count)
            if (tutorials[tutorialIndex].tutorialType == TutorialType.UnlockNewTable) tableMoveDone = false;
        stepIndex = 0;
    }

    public void ChangeDoneOrder(bool isDone) { isDoneOrder = isDone; }

    public void ChangeLevelHaveTutOrder(bool have) { levelHaveTutOrder = have; }

    Tutorial tempTutorial;
    public void RegisterStep(TutorialObject t_Object)
    {
        bool able = true;
        if ((t_Object.levelTutorial > currentLevel && t_Object.hide)
        || (t_Object.levelTutorial == currentLevel && t_Object.stepIndex > stepIndex && GetTutorialIndex(t_Object.tutorialType) == tutorialIndex)
        || (t_Object.levelTutorial == currentLevel && GetTutorialIndex(t_Object.tutorialType) > tutorialIndex))
            able = false;
        if (t_Object.autoShow) able = true;
        tempTutorial = GetTutorial(t_Object.tutorialType);
        if (tempTutorial == null)
            return;
        tempTutorial.RegisterStep(t_Object, able);
    }

    Tutorial GetTutorial(TutorialType tutorialType) {
        for (int i = 0; i < tutorials.Count; i++)
        {
            if (tutorials[i].tutorialType == tutorialType)
                return tutorials[i];
        }
        return null;
    }

    int GetTutorialIndex(TutorialType tutorialType)
    {
        for (int i = 0; i < tutorials.Count; i++)
        {
            if (tutorials[i].tutorialType == tutorialType)
                return i;
        }
        return -1;
    }

    public bool IsDoneStepTutorial(TutorialType tutorialType, int tutorialIndex) {
        tempTutorial = GetTutorial(tutorialType);
        if (tempTutorial == null) return true;
        return tempTutorial.tutorialSteps[tutorialIndex].isDoneStep;
    }

    public Tutorial GetCurrentTutorial() {
        if (tutorialIndex >= tutorials.Count)
            return null;
        return tutorials[tutorialIndex];
    }

    public int GetCurrentIndexStep() { return stepIndex; }
    public void TableUnlockMoveDone() { tableMoveDone = true; }

    public void OnNextStep() {
        tutorials[tutorialIndex].tutorialSteps[stepIndex].isDoneStep = true;
        if (stepIndex == tutorials[tutorialIndex].tutorialSteps.Count - 1)
        {
            if (tutorials[tutorialIndex].tutorialType == TutorialType.UpgradeMachineLevel1 || tutorials[tutorialIndex].tutorialType == TutorialType.UpgradeMachineLevel2)
                onUnlockMachine = false;
            tutorials[tutorialIndex].isDone = true;
            UIManager.instance.ClosePanelTutorial();
            onTutorial = false;
            TutorialRest();
        }
        else
        {
            stepIndex++;
            if ((tutorials[tutorialIndex].tutorialType == TutorialType.UpgradeMachineLevel1 || tutorials[tutorialIndex].tutorialType == TutorialType.UpgradeMachineLevel2))
                onUnlockMachine = (stepIndex == 3);
            onTutorial = false;
        }

        if (tutorialIndex == tutorials.Count)
            UIManager.instance.ClosePanelTutorial();
    }
}
