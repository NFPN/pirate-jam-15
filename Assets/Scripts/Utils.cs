using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public enum ActionType
    {
        Move,
        Fire,
        Jump,
        Look,
    };

    public static Vector3 GetVec3(Vector2 vector2)
    {
        return new(vector2.x, vector2.y, 0.0f);
    }
}

