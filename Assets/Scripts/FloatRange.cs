using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FloatRange
{
    [SerializeField]
    private float min, max;
    public float Min => min;
    public float Max => max;
    public float RangeValueRange
    {
        get
        {
            return Random.Range(min, max);
        }
    }

    public FloatRange(float range)
    {
        min = max = range;
    }

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max < min ? min : max;
    }
}


public class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }
    public FloatRangeSliderAttribute(float min, float max)
    {
        this.Min = min;
        this.Max = max < min ? min : max;
    }
}
