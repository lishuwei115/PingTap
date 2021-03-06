using Fralle.Core;
using System;
using System.Linq;

namespace Fralle.PingTap
{
  [Serializable]
  public class CombatIKHandler
  {
    public bool enabled;
    public bool useLeftHand = true;

    Combatant combatant;
    HandIK[] leftHandIKs;
    HandIK[] rightHandIKs;
    HandIKTarget[] leftHandIKTargets;
    HandIKTarget[] rightHandIKTargets;

    public void Setup(Combatant combatant)
    {
      this.combatant = combatant;
      this.combatant.OnWeaponSwitch += HandleWeaponSwitch;

      HandIK[] handIKs = this.combatant.GetComponentsInChildren<HandIK>();
      HandIKTarget[] handIKTargets = this.combatant.GetComponentsInChildren<HandIKTarget>();

      leftHandIKs = handIKs.Where(x => x.hand == Hand.Left).ToArray();
      rightHandIKs = handIKs.Where(x => x.hand == Hand.Right).ToArray();
      leftHandIKTargets = handIKTargets.Where(x => x.hand == Hand.Left).ToArray();
      rightHandIKTargets = handIKTargets.Where(x => x.hand == Hand.Right).ToArray();

      if (combatant.equippedWeapon)
        SetupIK();
      else
      {
        foreach (HandIK toggleIK in handIKs)
          toggleIK.Toggle(false);
      }
    }

    public void Clean()
    {
      combatant.OnWeaponSwitch -= HandleWeaponSwitch;
    }

    void SetupIK()
    {
      if (useLeftHand)
      {
        foreach (HandIK ik in leftHandIKs)
          ik.Toggle(combatant.equippedWeapon?.leftHandGrip);

        foreach (HandIKTarget target in leftHandIKTargets)
          target.Target(combatant.equippedWeapon?.leftHandGrip);
      }

      foreach (HandIK ik in rightHandIKs)
        ik.Toggle(combatant.equippedWeapon?.rightHandGrip);

      foreach (HandIKTarget target in rightHandIKTargets)
        target.Target(combatant.equippedWeapon?.rightHandGrip);
    }

    void HandleWeaponSwitch(Weapon newWeapon, Weapon oldWeapon)
    {
      SetupIK();
    }
  }
}
