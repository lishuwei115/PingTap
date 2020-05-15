﻿using Fralle.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Fralle.UI.HUD
{
  public class NexusHealthUi : MonoBehaviour
  {
    [SerializeField] Image foregroundImage;

    Nexus nexus;

    void Awake()
    {
      nexus = FindObjectOfType<Nexus>();
    }

    void Update()
    {
      if (!nexus) return;
      nexus.health.OnHealthChange += UpdateHealthbar;
    }

    public void UpdateHealthbar(float currentHealth, float maxHealth)
    {
      float percentage = currentHealth / maxHealth;
      foregroundImage.fillAmount = percentage;
    }
  }
}