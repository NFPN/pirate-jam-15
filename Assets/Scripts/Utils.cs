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

    public enum Direction
    {
        Left,
        Right, 
        Up,
        Down,
        None,
    };

    public enum HeartState
    {
        None,
        Half,
        Full,
    }

    public static Vector3 ToVector3(this Vector2 vector2)
    {
        return new(vector2.x, vector2.y, 0.0f);
    }
}
