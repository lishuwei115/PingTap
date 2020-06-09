﻿using Fralle.Attack.Defense;
using Fralle.Attack.Effect;
using Fralle.Core.Extensions;
using Fralle.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fralle.Attack.Offense
{
  public class Health : MonoBehaviour, IDamageable
  {
    static readonly int RendererColor = Shader.PropertyToID("_EmissionColor");

    public static event Action<Damage> OnAnyDamage = delegate { };
    public static event Action<Health> OnHealthBarAdded = delegate { };
    public static event Action<Health> OnHealthBarRemoved = delegate { };

    public event Action<Health, Damage> OnDeath = delegate { };
    public event Action<Health, Damage> OnDamageTaken = delegate { };
    public event Action<float, float> OnHealthChange = delegate { };

    [HideInInspector] public List<DamageEffect> damageEffects = new List<DamageEffect>();
    [HideInInspector] public bool isDead;

    [Header("Stats")]
    public float currentHealth;
    public float maxHealth = 100f;
    public bool immortal;
    public Armor armor;
    [SerializeField] float dropChance = 0.25f;
    [SerializeField] Loot dropOnDeath;

    [Header("Graphics")]
    [SerializeField] Renderer renderer;
    [SerializeField] GameObject deathModel;

    MaterialPropertyBlock propBlock;
    Color defaultColor;
    Color currentColor;
    int colorID;
    bool isTouched;
    float colorLerpTime;

    void Start()
    {
      propBlock = new MaterialPropertyBlock();
      renderer.GetPropertyBlock(propBlock);
      defaultColor = propBlock.GetColor(RendererColor);

      if (currentHealth == 0) currentHealth = maxHealth;
    }

    void Update()
    {
      if (isDead) return;
      for (var i = 0; i < damageEffects.Count; i++)
      {
        damageEffects[i].Tick(this);
        if (!(damageEffects[i].timer > damageEffects[i].time)) continue;
        damageEffects[i].Exit(this);
        damageEffects.RemoveAt(i);
      }

      if (colorLerpTime > 0)
      {
        currentColor = Color.Lerp(currentColor, defaultColor, 1 - colorLerpTime);
        propBlock.SetColor(RendererColor, currentColor);
        renderer.SetPropertyBlock(propBlock);
        colorLerpTime -= Time.deltaTime * 0.25f;
      }
    }

    public void ReceiveAttack(Damage damage)
    {
      if (isDead) return;

      damage = armor.Protect(damage, this);
      TakeDamage(damage);
      ApplyEffects(damage);

      if (damage.hitAngle != -1)
      {
        currentColor = Color.white;
        propBlock.SetColor(RendererColor, currentColor);
        renderer.SetPropertyBlock(propBlock);
        colorLerpTime = 1f;
      }
    }

    public void TakeDamage(Damage damage)
    {
      if (isDead) return;

      if (damage.player) damage.player.stats.ReceiveDamageStats(damage);

      if (damage.damageAmount <= 0)
      {
        OnAnyDamage(damage);
        return;
      }

      if (!isTouched)
      {
        OnHealthBarAdded(this);
        isTouched = true;
      }

      currentHealth = Mathf.Clamp(currentHealth - damage.damageAmount, 0, maxHealth);
      OnHealthChange(currentHealth, maxHealth);
      OnAnyDamage(damage);
      OnDamageTaken(this, damage);
      if (currentHealth <= 0) Death(damage);
    }

    void ApplyEffects(Damage damage)
    {
      foreach (var t in damage.effects)
      {
        var effect = t;
        var oldEffect = damageEffects.FirstOrDefault(x => x.name == effect.name);
        effect = effect.Append(oldEffect);
        effect.Enter(this);
        damageEffects.Upsert(oldEffect, effect);
      }
    }

    void GraphicDeathEffect(Damage damage)
    {
      if (!renderer || !deathModel) return;
      var deathModelInstance = Instantiate(deathModel, transform.position, transform.rotation);
      Destroy(deathModelInstance, 3f);

      foreach (var rigidBody in deathModelInstance.GetComponentsInChildren<Rigidbody>())
      {
        rigidBody.AddForceAtPosition(damage.force, damage.position);
      }

      Destroy(gameObject);
    }

    void Death(Damage damage)
    {
      if (isDead) return;

      if (damage.player) damage.player.stats.ReceiveKillingBlow(1);
      OnHealthBarRemoved(this);
      if (immortal)
      {
        currentHealth = maxHealth;
        OnHealthChange(currentHealth, maxHealth);
      }
      else
      {
        isDead = true;
        OnDeath(this, damage);
        GraphicDeathEffect(damage);

        if (!dropOnDeath) return;
        if (Random.value > 1 - dropChance) Instantiate(dropOnDeath, transform.position, Quaternion.identity);
      }
    }
  }
}
