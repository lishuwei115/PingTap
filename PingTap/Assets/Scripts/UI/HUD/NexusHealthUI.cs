﻿using Fralle.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Fralle.UI.HUD
{
  public class NexusHealthUi : MonoBehaviour
  {
    [SerializeField] Image foregroundImage = null;

    PlayerHome playerHome;

    void Awake()
    {
      playerHome = FindObjectOfType<PlayerHome>();
    }

    void Update()
    {
      if (!playerHome) return;
      playerHome.damageController.OnHealthChange += UpdateHealthbar;
    }

    public void UpdateHealthbar(float currentHealth, float maxHealth)
    {
      var percentage = currentHealth / maxHealth;
      foregroundImage.fillAmount = percentage;
    }
  }
}