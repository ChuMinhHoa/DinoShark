using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnimCurveType
{
    Default01,
    Linear010,
    Curve01n10,
    Wave
}
[System.Serializable]
public class AnimCurveData
{
    public AnimCurveType animType;
    public AnimationCurve animCurve;
}
[CreateAssetMenu(fileName = "AnimCurveData", menuName = "ScriptAbleObjects/NewAnimCurveData")]
public class AnimationCurveDataConfig : ScriptableObject
{
    public List<AnimCurveData> animCurveDatas;
    public AnimationCurve GetAnimCurve(AnimCurveType type)
    {
        for (int i = 0; i < animCurveDatas.Count; i++)
        {
            if (type == animCurveDatas[i].animType)
                return animCurveDatas[i].animCurve;
        }
        return null;
    }
    public AnimCurveData GetRandomCurve(AnimCurveType currentType)
    {
        int indexR = Random.Range(1, animCurveDatas.Count);
        if (animCurveDatas[indexR].animType != currentType)
            return animCurveDatas[indexR];
        else return GetRandomCurve(currentType);
    }
}
