using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action OnDeath;
    public Action<float> OnHealthChange;
    public Action OnRevive;

    [SerializeField] float maxHealth;
    public float MaxHealth { get => maxHealth; }

    [SerializeField] float _currentHealth;
    public float currentHealth
    {
        private set
        {
            if (_currentHealth != value)
            {
                _currentHealth = value;
                OnHealthChange?.Invoke(currentHealth);
            }

        }
        get => _currentHealth;
    }

    bool isDead;
    public bool IsDead
    {
        get => isDead;
    }



    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        if (currentHealth == 0)
        {
            Die();
        }
    }

    internal void SetHealth(float healthPoints)
    {
        currentHealth = healthPoints;
        if (currentHealth > 0) isDead = false;
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
    }

    internal void Revive()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnRevive();
    }
}