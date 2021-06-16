using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataBetweenScenes
{
    private static int SkinNum = 0;
    public static int Skin
    {
        get
        {
            return SkinNum;
        }
        set
        {
            SkinNum = value;
        }
    }
}
