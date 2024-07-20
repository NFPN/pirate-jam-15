using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    delegate void HealthChangedHandler(object source, float oldHealth,  float newHealth);
    delegate void DeathHandler(object source);

    event HealthChangedHandler OnHealthChanged;
    event DeathHandler OnDeath;

    float CurrentHealth { get; }
    float MaxHealth { get; }

    void DealDamage(object source, float damage);
    void SetHealth(float amount);
}