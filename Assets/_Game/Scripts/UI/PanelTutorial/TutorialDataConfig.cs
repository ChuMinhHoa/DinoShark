using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialDataConfig", menuName = "ScriptAbleObjects/New TutorialConfig")]
public class TutorialDataConfig : ScriptableObject
{
    public List<TutorialData> tutorialDatas = new List<TutorialData>();
}

[System.Serializable]
public class TutorialData
{
    public string tutorialName;
    public TutorialType tutorialType;
    public TutorialConditionType tutorialConditionType;
    public float conditionAmount;
    public int countSteps;
    public int stepMarkTutorialDone;
    public int level;
    public List<string> stepDescriptions;
}
