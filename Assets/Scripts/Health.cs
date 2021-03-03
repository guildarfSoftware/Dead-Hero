using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action OnDeath;
    public Action<float> OnHealthChange;
    public Action OnRevive;

    [SerializeField] float maxHealth;
    public float MaxHealth { get => maxHealth; }
    public float currentHealth { private set; get; }
    bool isDead;
    public bool IsDead
    {
        get => isDead;
    }



    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChange?.Invoke(-amount);
        if (currentHealth == 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        OnDeath?.Invoke();
    }

    internal void Heal(float amount)
    {
        if (currentHealth == maxHealth) return;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChange?.Invoke(amount);
    }

    internal void Revive()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChange(maxHealth);
        OnRevive();
    }
}