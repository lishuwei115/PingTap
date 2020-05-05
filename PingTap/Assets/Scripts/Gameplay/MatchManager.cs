﻿using Fralle.AI;
using Fralle.Core.Attributes;
using Fralle.UI.HUD;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fralle.Gameplay
{
  public class MatchManager : MonoBehaviour
  {
    public static event Action<MatchManager> OnDefeat = delegate { };
    public static event Action<MatchManager> OnVictory = delegate { };
    public static event Action<MatchManager> OnNewRound = delegate { };
    public static event Action<GameState> OnNewState = delegate { };

    public GameState gameState;
    public float prepareTime = 30f;

    [Header("UI")] [SerializeField] GameObject prepareUi;
    [FormerlySerializedAs("nexusUI")] [SerializeField] GameObject nexusUi;
    [FormerlySerializedAs("roundsUI")] [SerializeField] GameObject roundsUi;
    [SerializeField] GameObject waveInfoUi;

    [Space(10)] [Readonly] public int enemiesAlive;
    [Readonly] public int totalEnemies;
    [Readonly] public float prepareTimer;
    [Readonly] public float totalTimer;
    [Readonly] public float waveTimer;

    [HideInInspector] public bool isVictory;

    StateController stateController;
    Nexus nexus;

    void Awake()
    {
      stateController = GetComponent<StateController>();

      nexus = FindObjectOfType<Nexus>();
      nexus.OnDeath += Defeat;

      Enemy.OnAnyEnemyDeath += HandleEnemyDeath;
      WaveManager.OnWavesComplete += HandleWavesComplete;

      SetupUi();
    }

    void Update()
    {
      if (gameState == GameState.End) return;
      totalTimer += Time.deltaTime;
    }

    public void NewState(GameState newState)
    {
      gameState = newState;
      OnNewState(newState);
    }

    public void NewWave(int enemyCount)
    {
      enemiesAlive = enemyCount;
      totalEnemies = enemyCount;
      OnNewRound(this);
    }

    public void HandleEnemyDeath(Enemy enemy)
    {
      enemiesAlive--;
    }

    void SetupUi()
    {
      var ui = new GameObject("UI");
      ui.transform.parent = transform;
      Instantiate(prepareUi, ui.transform);
      Instantiate(roundsUi, ui.transform);
      Instantiate(waveInfoUi, ui.transform);

      var nexusUiInstance = Instantiate(nexusUi, ui.transform);
      nexusUiInstance.GetComponent<NexusHealthUi>().SetNexus(nexus);
    }

    void HandleWavesComplete(WaveManager waveManager)
    {
      Victory();
    }

    void Victory()
    {
      stateController.enabled = false;

      OnVictory(this);
    }

    void Defeat(Nexus nexus)
    {
      OnDefeat(this);
    }

    void OnDestroy()
    {
      nexus.OnDeath -= Defeat;

      Enemy.OnAnyEnemyDeath -= HandleEnemyDeath;
      WaveManager.OnWavesComplete -= HandleWavesComplete;
    }
  }
}