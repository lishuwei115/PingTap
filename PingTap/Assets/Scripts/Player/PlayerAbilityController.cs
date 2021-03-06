using Fralle.AbilitySystem;
using UnityEngine.InputSystem;
using static PlayerControls;

namespace Fralle.PingTap
{
  public class PlayerAbilityController : AbilityController, IAbilityActions
  {
    protected override void Start()
    {
      base.Start();
      Player.controls.Ability.SetCallbacks(this);
      Player.controls.Ability.Enable();
    }

    public void OnAttackAbility(InputAction.CallbackContext context)
    {
      if (context.performed && AttackAbility && AttackAbility.IsReady)
        AttackAbility.Perform();
    }

    public void OnMovementAbility(InputAction.CallbackContext context)
    {
      if (context.performed && MovementAbility && MovementAbility.IsReady)
        MovementAbility.Perform();
    }

    public void OnUltimateAbility(InputAction.CallbackContext context)
    {
      if (context.performed && UltimateAbility && UltimateAbility.IsReady)
        UltimateAbility.Perform();
    }
  }
}
