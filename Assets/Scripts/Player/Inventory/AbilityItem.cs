using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityItem
{
    public Utils.Abilities ability;

    public event Action<bool> OnAbilityLockStateChanged;

    // For now not get set (debuging purposes)
    public bool IsLocked = true;

    public bool hasLevels = true;
    public int level = 1;


    public void UnlockAbility()
    {
        IsLocked = false;
        OnAbilityLockStateChanged?.Invoke(IsLocked);
    }
    public void LockAbility()
    {
        IsLocked = true;
        OnAbilityLockStateChanged?.Invoke(IsLocked);
    }
    public void AddLevels(int amount)
    {
        if (hasLevels)
            level = level + amount > 1 ? level : 1;
    }
}
