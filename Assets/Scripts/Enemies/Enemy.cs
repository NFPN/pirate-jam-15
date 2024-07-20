using UnityEngine;

public class Enemy : MonoBehaviour, IHealth
{
    public GameObject player;

    [SerializeField] private float maxHealth;

    public float CurrentHealth { get; set; }

    public float MaxHealth => maxHealth;

    public event IHealth.HealthChangedHandler OnHealthChanged;

    public event IHealth.DeathHandler OnDeath;

    public void DealDamage(object source, float damage)
    {
        var oldCurrentHealth = CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        InvokeCurrentHealthEvents(source, oldCurrentHealth, CurrentHealth);
    }

    public void SetHealth(float amount)
    {
        var oldCurrentHealth = CurrentHealth;
        CurrentHealth = amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        InvokeCurrentHealthEvents(this, oldCurrentHealth, CurrentHealth);
    }

    private void InvokeCurrentHealthEvents(object sender, float oldCurrentHealth, float newCurrentHealth)
    {
        OnHealthChanged?.Invoke(sender, oldCurrentHealth, newCurrentHealth);
        if (newCurrentHealth == 0)
            OnDeath?.Invoke(sender);
    }

    private void Start()
    {
        OnDeath += Died;
        CurrentHealth = MaxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
    }

    public void Died(object source)
    {
        Destroy(gameObject);
    }
}
