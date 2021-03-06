using UnityEngine;

namespace Fralle.PingTap
{
  [RequireComponent(typeof(Weapon))]
  public abstract class AttackAction : MonoBehaviour
  {
    [Header("Shooting")]
    [SerializeField] internal float minDamage = 1;
    [SerializeField] internal float maxDamage = 10;
    [SerializeField] internal int ammoPerShot = 1;
    [SerializeField] internal int shotsPerSecond = 20;
    [SerializeField] internal bool isTapable = false;
    [SerializeField] internal Element element;
    [SerializeField] internal DamageEffect[] damageEffects = new DamageEffect[0];

    internal Weapon Weapon;
    internal Combatant Combatant;
    int nextMuzzle;

    public float fireRate;

    internal float Damage => Random.Range(minDamage, maxDamage);

    internal virtual void Awake()
    {
      OnValidate();
    }

    internal virtual void Start()
    {
      Weapon = GetComponent<Weapon>();
      Combatant = Weapon.Combatant;
    }

    internal virtual void OnValidate()
    {
      fireRate = 1f / shotsPerSecond;
    }

    public void Perform(bool buttonTap = false)
    {
      if (Weapon.Status != Status.Ready || isTapable && !buttonTap || !Weapon.Ammo.HasAmmo())
        return;

      Weapon.ChangeWeaponAction(Status.Firing);
      Weapon.Animator.SetTrigger("Shoot");
      int shotsToFire = Mathf.RoundToInt(-Weapon.NextAvailableShot / fireRate);
      for (int i = 0; i <= shotsToFire; i++)
      {
        Fire();
        Weapon.Attack(this);

        if (!Weapon.HasAmmo)
          break;
      }
    }

    public abstract void Fire();
    public abstract float GetRange();

    internal Transform GetMuzzle()
    {
      Transform muzzle = Weapon.Muzzles[nextMuzzle];
      if (Weapon.Muzzles.Length <= 1)
        return muzzle;

      nextMuzzle++;
      if (nextMuzzle > Weapon.Muzzles.Length - 1)
        nextMuzzle = 0;

      return muzzle;
    }
  }
}
