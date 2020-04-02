﻿using UnityEngine;

namespace Fralle
{
  public class DamageController : MonoBehaviour, IDamageable
  {
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] int armor;

    [SerializeField] bool immortal;

    public float damageMultiplier { get { return 1 - 0.06f * armor / (1 + 0.06f * armor); } }

    void Start()
    {
      if (currentHealth == 0) currentHealth = maxHealth;
    }

    public void TakeDamage(float rawDamage)
    {
      float damage = rawDamage * damageMultiplier;
      currentHealth -= damage;
      if (currentHealth <= 0) Death();
    }

    public void Death()
    {
      if (immortal) currentHealth = maxHealth;
    }

  }
}