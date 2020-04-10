﻿using UnityEngine;

namespace Fralle
{
  [CreateAssetMenu(menuName = "State/Live")]
  public class LiveState : State
  {
    public State prepareState;
    public State endState;

    public override void Enter(StateController controller)
    {
      base.Enter(controller);
      if (!controller.waveManager.WavesRemaining) return;
      int enemyCount = controller.waveManager.NextWave();
      controller.matchManager.enemiesAlive = enemyCount;
      controller.matchManager.totalEnemies = enemyCount;
    }

    public override void Exit(StateController controller)
    {
      controller.matchManager.roundTimer = 0;
    }

    internal override void Tick(StateController controller)
    {
      controller.matchManager.roundTimer += Time.deltaTime;
    }

    internal override void Transition(StateController controller)
    {
      if (controller.matchManager.enemiesAlive == 0)
      {
        controller.SetState(controller.waveManager.WavesRemaining ? prepareState : endState);
      }
    }
  }
}