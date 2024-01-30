using UnityEngine;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        float remapped = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        if (to1 > to2)
        {
            return Mathf.Clamp(remapped, to2, to1);
        }
        else
        {
            return Mathf.Clamp(remapped, to1, to2);
        }
    }
}
