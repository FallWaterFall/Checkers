using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataBetweenScenes
{
    private static int ModelNum = 0;
    private static int ColorNum = 0;
    private static int ImageNum = 0;
    public static int Model
    {
        get
        {
            return ModelNum;
        }
        set
        {
            ModelNum = value;
        }
    }
    public static int Color
    {
        get
        {
            return ColorNum;
        }
        set
        {
            ColorNum = value;
        }
    }
    public static int Image
    {
        get
        {
            return ImageNum;
        }
        set
        {
            ImageNum = value;
        }
    }
}
