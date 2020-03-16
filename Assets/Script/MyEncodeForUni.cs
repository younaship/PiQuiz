using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MyEncodeForUni
{
    const float DELTATIME_60FPS = 0.0166f;

    public static float GetDelta(float dt)
    {
        return dt - DELTATIME_60FPS;
    }
}
