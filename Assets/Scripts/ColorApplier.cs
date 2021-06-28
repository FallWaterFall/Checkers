using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorApplier
{
    public static Color32 ColorCombine(Color32 c1, Color32 c2)
    {
        return new Color32((byte)((c1.r + c2.r)/2), (byte)((c1.g + c2.g)/2), (byte)((c1.b + c2.b)/2), 255);
    }
    public static void ApplyNewColor(GameObject obj, Color32 c2)
    {
        if (obj.tag == "Particle") return;
        Renderer objRenderer = obj.GetComponent<Renderer>();
        for (int j = 0; j < objRenderer.materials.Length; j++)
        {
            objRenderer.materials[j].color = ColorCombine(objRenderer.materials[j].color, c2);
        }
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            ApplyNewColor(obj.transform.GetChild(i).gameObject, c2);
        }
    }
    public static void ApplyNewColor(GameObject obj, Color32 c1, Color32 c2)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        for (int j = 0; j < objRenderer.materials.Length; j++)
        {
            objRenderer.materials[j].color = ColorCombine(c1, c2);
        }
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            ApplyNewColor(obj.transform.GetChild(i).gameObject, c1, c2);
        }
    }
}
