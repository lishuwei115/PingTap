using Fralle.Core;
using Fralle.Core.AI;
using Fralle.PingTap.AI;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Fralle.PingTap
{
  [CreateAssetMenu(menuName = "AI/Basic enemy")]
  public class BasicEnemy : AIPersonality
  {
    [SerializeField] float maxSearchStateTime;

    StateMachine<AIState> stateMachine;
    AIBrain aiBrain;
    NavMeshAgent navMeshAgent;
    DamageController damageController;
    AITargetingSystem aiTargetingSystem;

    [SerializeField] WanderState wanderState;
    [SerializeField] SearchState searchState;
    [SerializeField] StartledState startledState;
    [SerializeField] ChaseState chaseState;
    [SerializeField] BattleState battleState;

    public override AIPersonality CreateInstance() => Instantiate(this);
    public override void Load(AIBrain aiBrain, StateMachine<AIState> stateMachine)
    {
      this.stateMachine = stateMachine;
      ResolveDependencies(aiBrain);
      SetupStates();
      SetupTransitions();

      this.stateMachine.SetState(wanderState);
    }

    public override void Alert(Vector3 position, AIState statePriority)
    {
      if (stateMachine.CurrentState.Identifier < statePriority)
        aiBrain.StartCoroutine(SetStartledState(position));
      else
        aiBrain.ResetAlertTimer();
    }

    IEnumerator SetStartledState(Vector3 position)
    {
      yield return new WaitForSeconds(aiBrain.reactionTimeRange.GetValueBetween());
      if (stateMachine.CurrentState.Identifier != AIState.Startled)
        stateMachine.SetState(startledState);

      startledState.NewOrigin(position);
    }

    void ResolveDependencies(AIBrain aiBrain)
    {
      this.aiBrain = aiBrain;
      aiBrain.GetComponent<AIAttack>();
      aiTargetingSystem = aiBrain.GetComponent<AITargetingSystem>();
      navMeshAgent = aiBrain.GetComponent<NavMeshAgent>();
      damageController = aiBrain.GetComponent<DamageController>();
      damageController.OnReceiveAttack += OnReceiveAttack;
    }

    void SetupStates()
    {
      wanderState = wanderState.CreateInstance();
      wanderState.Setup(aiBrain);

      searchState = searchState.CreateInstance();
      searchState.Setup(aiBrain);

      startledState = startledState.CreateInstance();
      startledState.Setup(aiBrain);

      chaseState = chaseState.CreateInstance();
      chaseState.Setup(aiBrain);

      battleState = battleState.CreateInstance();
      battleState.Setup(aiBrain);
    }

    void SetupTransitions()
    {
      stateMachine.AddTransition(startledState, searchState, () => navMeshAgent.remainingDistance < 0.5f);
      stateMachine.AddTransition(searchState, wanderState, () => stateMachine.CurrentStateTime > maxSearchStateTime);

      stateMachine.AddTransition(wanderState, chaseState, () => aiTargetingSystem.TargetInSight);
      stateMachine.AddTransition(searchState, chaseState, () => aiTargetingSystem.TargetInSight);
      stateMachine.AddTransition(startledState, chaseState, () => aiTargetingSystem.TargetInSight);

      stateMachine.AddTransition(chaseState, wanderState, () => !aiTargetingSystem.HasTarget); // if target died
      stateMachine.AddTransition(chaseState, searchState, () => navMeshAgent.remainingDistance < 0.5f && !aiTargetingSystem.TargetInSight); // if target was lost

      stateMachine.AddTransition(chaseState, battleState, () => navMeshAgent.remainingDistance < aiBrain.attackRange && aiTargetingSystem.TargetInSight); // target in range
      stateMachine.AddTransition(battleState, wanderState, () => !aiTargetingSystem.HasTarget); // target died
      stateMachine.AddTransition(battleState, chaseState, () => !aiTargetingSystem.TargetInSight); // target out of sight
      stateMachine.AddTransition(battleState, chaseState, () => navMeshAgent.remainingDistance > aiBrain.attackRange); // target out of range
    }

    void OnReceiveAttack(DamageController damageController, DamageData damageData)
    {
      if (!damageController.IsDead)
        Alert(damageData.Attacker.aimTransform.position, AIState.Chasing);
    }

    void OnDestroy()
    {
      if (damageController)
        damageController.OnReceiveAttack -= OnReceiveAttack;
    }

  }
}
