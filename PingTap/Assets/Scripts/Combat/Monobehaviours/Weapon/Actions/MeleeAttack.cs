//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Fralle.PingTap
//{
//  public class MeleeAttack : AttackAction
//  {
//    public float meleeRadius = 2f;

//    [SerializeField] float swingTime = 0.25f;
//    [SerializeField] float recoveryTime = 1f;

//    bool swinging;
//    float activeSwingTime;
//    float activeRecoveryTime;

//    readonly Vector3 swingPosition = new Vector3(0.4f, 0, 0.75f);
//    readonly Quaternion swingRotation = Quaternion.Euler(0, -160f, 0);

//    public override void Fire()
//    {
//      PerformMelee();
//    }

//    public override float GetRange() => meleeRadius;

//    void LateUpdate()
//    {
//      Animate();
//    }

//    void Animate()
//    {
//      if (activeSwingTime > 0)
//      {
//        activeSwingTime -= Time.deltaTime;
//        if (swinging)
//        {
//          float delta = -(Mathf.Cos(Mathf.PI * (activeSwingTime / swingTime)) - 1f) / 2f;
//          transform.localPosition = Vector3.Lerp(swingPosition, Vector3.zero, delta);
//          transform.localRotation = Quaternion.Lerp(swingRotation, Quaternion.identity, delta);
//        }
//        else
//        {
//          float delta = -(Mathf.Cos(Mathf.PI * (activeSwingTime / swingTime)) - 1f) / 2f;
//          transform.localPosition = Vector3.Lerp(Vector3.zero, swingPosition, delta);
//          transform.localRotation = Quaternion.Lerp(Quaternion.identity, swingRotation, delta);
//          if (activeSwingTime <= 0)
//            Weapon.ChangeWeaponAction(Status.Ready);
//        }
//      }
//      else if (activeRecoveryTime > 0)
//      {
//        activeRecoveryTime -= Time.deltaTime;
//        swinging = false;
//        if (activeRecoveryTime <= 0)
//          activeSwingTime = swingTime;
//      }
//    }

//    void PerformMelee()
//    {
//      activeSwingTime = swingTime;
//      activeRecoveryTime = recoveryTime;
//      swinging = true;
//      Weapon.ChangeWeaponAction(Status.Melee);

//      IEnumerable<DamageController> targets = GetTargets();
//      foreach (DamageController target in targets)
//      {
//        if (!TargetInArc(target))
//          continue;

//        DamageData damageData = new DamageData
//        {
//          DamageAmount = Damage,
//          Attacker = Combatant,
//          Element = element,
//          Effects = damageEffects.Select(x => x.Setup(Combatant, Damage)).ToArray(),
//          HitAngle = Vector3.Angle((transform.position - target.transform.position).normalized,
//            target.transform.forward),
//          Position = target.transform.position
//        };
//        target.ReceiveAttack(damageData);
//      }
//    }

//    bool TargetInArc(Component target)
//    {
//      Vector3 vectorToCollider = (target.transform.position - transform.position).normalized;
//      return Vector3.Dot(vectorToCollider, transform.forward) > 0;
//    }

//    IEnumerable<DamageController> GetTargets()
//    {
//      Collider[] colliders = Physics.OverlapSphere(transform.position, meleeRadius);
//      return colliders.Select(x => x.GetComponentInParent<DamageController>())
//        .Where(x => x != null).Distinct();
//    }
//  }
//}
