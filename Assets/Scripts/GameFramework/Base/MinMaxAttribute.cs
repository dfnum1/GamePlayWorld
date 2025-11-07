using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxAttributeInt : PropertyAttribute
{
    public int min;
    public int max;
    
    public MinMaxAttributeInt(int a, int b)
    {
        min = a;
        max = b;
    }
    
}

public class MinMaxAttributeFloat : PropertyAttribute
{
    public float min;
    public float max;

    public MinMaxAttributeFloat(float a, float b)
    {
        min = a;
        max = b;
    }

}