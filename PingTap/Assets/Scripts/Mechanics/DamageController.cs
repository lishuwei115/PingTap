﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Fralle
{
  public class DamageController : MonoBehaviour, IDamageable
  {
    public static event Action<DamageController> OnHealthBarAdded = delegate { };
    public static event Action<DamageController> OnHealthBarRemoved = delegate { };

    public event Action<DamageController, DamageData> OnDeath = delegate { };
    public event Action<float, float> OnHealthChange = delegate { };
    public event Action<DamageData, float, bool> OnDamage = delegate { };

    [Header("HealthBar")]
    public float yLowestOffset = 2f;
    public float yHighestOffset = 3.5f;

    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] int armor;
    [SerializeField] bool immortal;
    public bool isDead;


    bool isTouched;

    public float damageMultiplier => 1 - 0.06f * armor / (1 + 0.06f * armor);

    void Start()
    {
      if (currentHealth == 0) currentHealth = maxHealth;
    }

    public void TakeDamage(DamageData damageData)
    {
      if (!isTouched)
      {
        OnHealthBarAdded(this);
        isTouched = true;
      }
      if (isDead) return;

      float damage = damageData.damage * damageMultiplier;
      currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
      OnHealthChange(currentHealth, maxHealth);
      OnDamage(damageData, damage, false);
      if (currentHealth <= 0) Death(damageData);
    }

    public void Death(DamageData damageData)
    {
      if (isDead) return;
      OnHealthBarRemoved(this);
      if (immortal)
      {
        currentHealth = maxHealth;
        OnHealthChange(currentHealth, maxHealth);
      }
      else
      {
        isDead = true;
        OnDeath(this, damageData);
      }
    }

  }
}