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
        HalfShadow,
        FullShadow,
    }

    public enum TextBubbleEffects
    {
        None,
        SinUpDown,
    };


    public enum Iteraction
    {
        Interact,
        Attack,
        SecondaryAttack,
        Dash,
        SwitchWorld,
    };

    public enum ObjectType
    {
        Sprite,
        Text,
    }

    public enum Abilities
    {
        Fireball,
        AOEMagic,
        Transcend,
        Dash,
    }

    public enum Items
    {
        AncientRune,
        Shard,
        Heart,
    }

    public enum AltarType
    {
        Health,
        Upgrade,
    }

    public enum SoundType
    {
        WorldChange,
        Fireball,
        Explosion,
        AOEMagic,
        Dash,
        SlimeDeath,
        SlimeHit,
        PlayerHit,
        UIClickBig,
        UIClickSmall,
        UIHover,
        BossAttack,
        BossHit,
        BossDeath,
        SkellyHit,
        SkellyDeath,
    }

    public enum AudioParameters
    {
        Time,   //0 Present, 1 Past
        IsWalking, // 0 - 1
        AreEnemiesAround, // 0 - 1
        VolMaster, // 0 - 100
        VolSFX, // 0 - 100
        VolMX, // 0 - 100
        GamePaused, // 0 - 100
    }


    public static Vector3 ToVector3(this Vector2 vector2)
    {
        return new(vector2.x, vector2.y, 0.0f);
    }

    public static Vector3 GetRandomRotationZ()
    {
        return new Vector3(0, 0, Random.Range(-360.0f, 360.0f));
    }

}
