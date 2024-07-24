using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IHealth
{
    public GameObject player;

    [SerializeField] private float maxHealth;

    public float CurrentHealth { get; set; }

    public float MaxHealth => maxHealth;

    public event IHealth.HealthChangedHandler OnHealthChanged;

    public event IHealth.DeathHandler OnDeath;


    protected Animator animator;
    protected NavMeshAgent navAgent;

    protected void InitializeEnemy()
    {
        CurrentHealth = MaxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }


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

    protected void Died(object source)
    {
        animator.Play("Death");
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
