using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMath
{
    public static int RandomRangeI(RangeI range)
    {
        return Random.Range(range.minVal, range.maxVal);
    }
    public static float RandomRangeF(RangeF range)
    {
        return Random.Range(range.minVal, range.maxVal);
    }

    public static int PercentRangeValI(RangeI range, float t)
    {
        return range.minVal + (int)((range.maxVal - range.minVal) * t);
    }
    public static float PercentRangeValF(RangeF range, float t)
    {
        return range.minVal + (range.maxVal - range.minVal) * t;
    }

    public static float PercentOfRangeF(RangeF range, float val)
    {
        return (val - range.minVal) / (range.maxVal - range.minVal);
    }
}

[System.Serializable]
public struct RangeI
{
    public int minVal;
    public int maxVal;
}

[System.Serializable]
public struct RangeF
{
    public float minVal;
    public float maxVal;
}