using Fralle.Core;
using Fralle.Core.AI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Fralle.PingTap.AI
{
  public class AIBrain : MonoBehaviour, IDisableOnDeath
  {
    [HideInInspector] public TeamController teamController;
    [HideInInspector] public float lastAlert;

    [ReadOnly] public AIState currentState;

    [Header("Configuration")]
    public AIPersonality personality;
    public AIDifficulty difficulty;

    [Header("Range - Distance")]
    public float attackRange = 15f;
    public float attackStoppingDistance = 5f;
    public float alertDistance = 4f;

    [Header("Timers")]
    public Vector2 reactionTimeRange = new Vector2(0.25f, 0.5f);
    public float alertFrequency = 1f;
    public int idleScanFrequency = 1;
    public int searchScanFrequency = 4;

    [Header("Debug")]
    [SerializeField] bool debugTransitions;

    StateMachine<AIState> stateMachine;

    AIAttack aiAttack;

    public void ResetAlertTimer() => lastAlert = Time.time + alertFrequency;

    public void AlertOthers(Vector3 position, AIState priorityState)
    {
      ResetAlertTimer();
      Collider[] colliders = Physics.OverlapSphere(transform.position, alertDistance, 1 << teamController.allyTeam);
      foreach (Collider item in colliders)
      {
        AIBrain allyBrain = item.GetComponent<AIBrain>();
        if (allyBrain)
          allyBrain.personality.Alert(position, priorityState);
      }
    }

    void Awake()
    {
      aiAttack = GetComponent<AIAttack>();
      teamController = GetComponent<TeamController>();

      personality = personality.CreateInstance();
    }

    void Start()
    {
      stateMachine = new StateMachine<AIState>();
      stateMachine.OnTransition += OnTransition;
      personality.Load(this, stateMachine);
      AdjustDifficulty();
    }

    void AdjustDifficulty()
    {
      aiAttack.AdjustOffset(difficulty.GetAimOffset());
      aiAttack.AdjustAccuracy(difficulty.GetAccuracy());
    }

    void Update()
    {
      stateMachine.OnLogic();
    }

    void OnTransition(IState<AIState> newState)
    {
      if (debugTransitions)
        Debug.Log($"Transitioned from {currentState} to {newState.Identifier}");

      currentState = newState.Identifier;
    }

    void OnDrawGizmos()
    {
      Gizmos.color = currentState.Color();
      Gizmos.DrawSphere(transform.position + Vector3.up * 2.5f, 0.25f);
    }
  }
}
