using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MagnitudeRangeType
{
    lowerMinRange,
    minRange,
    upperMinRange,
    lowerMediumRange,
    mediumRange,
    upperMediumRange,
    lowerMaxRange,
    maxRange,
    upperMaxRange
}
[System.Serializable]
public class MagnitudeRange
{
    public MagnitudeRangeType rangeType;
    public float minMagnitude;
    public float maxMagnitude;
}
[CreateAssetMenu(fileName = "NewMagnitudeRangeData", menuName = "ScriptAbleObjects/NewMagnitudeRange")]
public class MagnitudeRangeDataConfig : ScriptableObject
{
    [SerializeField] MagnitudeRange lowerMinRange;
    [SerializeField] MagnitudeRange minRange;
    [SerializeField] MagnitudeRange upperMinRange;
    [SerializeField] MagnitudeRange lowerMediumRange;
    [SerializeField] MagnitudeRange mediumRange;
    [SerializeField] MagnitudeRange upperMediumRange;
    [SerializeField] MagnitudeRange lowerMaxRange;
    [SerializeField] MagnitudeRange maxRange;
    [SerializeField] MagnitudeRange upperMaxRange;

    private void OnEnable()
    {
        lowerMinRange = new MagnitudeRange();
        lowerMinRange.rangeType = MagnitudeRangeType.lowerMinRange;
        lowerMinRange.minMagnitude = -.1f;
        lowerMinRange.maxMagnitude = .1f;

        minRange = new MagnitudeRange();
        minRange.rangeType = MagnitudeRangeType.minRange;
        minRange.minMagnitude = -.25f;
        minRange.maxMagnitude = .25f;

        upperMinRange = new MagnitudeRange();
        upperMinRange.rangeType = MagnitudeRangeType.upperMinRange;
        upperMinRange.minMagnitude = -.5f;
        upperMinRange.maxMagnitude = .5f;

        lowerMediumRange = new MagnitudeRange();
        lowerMediumRange.rangeType = MagnitudeRangeType.lowerMediumRange;
        lowerMediumRange.minMagnitude = -1f;
        lowerMediumRange.maxMagnitude = 1f;

        mediumRange = new MagnitudeRange();
        mediumRange.rangeType = MagnitudeRangeType.mediumRange;
        mediumRange.minMagnitude = -1.25f;
        mediumRange.maxMagnitude = 1.25f;

        upperMediumRange = new MagnitudeRange();
        upperMediumRange.rangeType = MagnitudeRangeType.upperMediumRange;
        upperMediumRange.minMagnitude = -1.5f;
        upperMediumRange.maxMagnitude = 1.5f;

        lowerMaxRange = new MagnitudeRange();
        lowerMaxRange.rangeType = MagnitudeRangeType.lowerMaxRange;
        lowerMaxRange.minMagnitude = -1.75f;
        lowerMaxRange.maxMagnitude = 1.75f;

        maxRange = new MagnitudeRange();
        maxRange.rangeType = MagnitudeRangeType.maxRange;
        maxRange.minMagnitude = -2f;
        maxRange.maxMagnitude = 2f;

        upperMaxRange = new MagnitudeRange();
        upperMaxRange.rangeType = MagnitudeRangeType.upperMaxRange;
        upperMaxRange.minMagnitude = -2.25f;
        upperMaxRange.maxMagnitude = 2.25f;
    }
    MagnitudeRange GetMagnitudeRange(MagnitudeRangeType type)
    {
        switch (type)
        {
            case MagnitudeRangeType.lowerMinRange:
                return lowerMinRange;
            case MagnitudeRangeType.minRange:
                return minRange;
            case MagnitudeRangeType.upperMinRange:
                return upperMinRange;
            case MagnitudeRangeType.lowerMediumRange:
                return lowerMediumRange;
            case MagnitudeRangeType.mediumRange:
                return mediumRange;
            case MagnitudeRangeType.upperMediumRange:
                return upperMediumRange;
            case MagnitudeRangeType.lowerMaxRange:
                return lowerMaxRange;
            case MagnitudeRangeType.maxRange:
                return maxRange;
            case MagnitudeRangeType.upperMaxRange:
                return upperMaxRange;
            default:
                return null;
        }
    }
    public float GetRange(MagnitudeRangeType type)
    {
        MagnitudeRange magnitudeRange = GetMagnitudeRange(type);
        if (type == 0)
            return Random.Range(magnitudeRange.minMagnitude, magnitudeRange.maxMagnitude);
        MagnitudeRange magnitudeRangeLower = GetMagnitudeRange(type - 1);
        float rangeResultNegative = Random.Range(magnitudeRangeLower.minMagnitude, magnitudeRange.minMagnitude);
        float rangeResultPositive = Random.Range(magnitudeRangeLower.maxMagnitude, magnitudeRange.maxMagnitude);
        if (Random.Range(-1, 2) > 0) return rangeResultPositive;
        else return rangeResultNegative;
    }
}
