﻿using Fralle.Core.Enums;
using UnityEngine;

namespace Fralle.Pingtap
{
	[RequireComponent(typeof(Weapon))]
	[RequireComponent(typeof(AmmoAddon))]
	[RequireComponent(typeof(RecoilAddon))]
	public abstract class AttackAction : MonoBehaviour
	{
		public MouseButton Button { get; set; }

		[Header("Shooting")]
		[SerializeField] internal float MinDamage = 1;
		[SerializeField] internal float MaxDamage = 10;
		[SerializeField] internal int AmmoPerShot = 1;
		[SerializeField] internal int ShotsPerSecond = 20;
		[SerializeField] internal bool Tapable = false;
		[SerializeField] internal Element Element;
		[SerializeField] internal DamageEffect[] DamageEffects = new DamageEffect[0];

		internal int HitboxLayer;
		internal Weapon Weapon;
		internal Combatant Combatant;
		int nextMuzzle;

		float fireRate;

		internal float Damage => Random.Range(MinDamage, MaxDamage);
		bool HasAmmo => Weapon.AmmoAddonController && Weapon.AmmoAddonController.HasAmmo();

		internal virtual void Awake()
		{
			HitboxLayer = LayerMask.NameToLayer("Hitbox");
			fireRate = 1f / ShotsPerSecond;
		}

		internal virtual void Start()
		{
			Weapon = GetComponent<Weapon>();
			Combatant = Weapon.GetComponentInParent<Combatant>();
		}

#if UNITY_EDITOR
		internal virtual void OnValidate()
		{
			fireRate = 1f / ShotsPerSecond;
		}
#endif

		public void Perform()
		{
			if (!Weapon || Weapon.ActiveWeaponAction != Status.Ready)
				return;

			int shotsToFire = Mathf.RoundToInt(-Weapon.NextAvailableShot / fireRate);
			for (int i = 0; i <= shotsToFire; i++)
			{
				Fire();
				Weapon.NextAvailableShot += fireRate;
				Weapon.AmmoAddonController?.ChangeAmmo(-AmmoPerShot);

				if (Weapon.RecoilAddon)
					Weapon.RecoilAddon.AddRecoil();

				if (!HasAmmo)
					break;
			}

			if (HasAmmo)
			{
				Weapon.ChangeWeaponAction(Status.Firing);
			}
		}

		public abstract void Fire();
		public abstract float GetRange();

		internal Transform GetMuzzle()
		{
			var muzzle = Weapon.Muzzles[nextMuzzle];
			if (Weapon.Muzzles.Length <= 1)
				return muzzle;

			nextMuzzle++;
			if (nextMuzzle > Weapon.Muzzles.Length - 1)
				nextMuzzle = 0;

			return muzzle;
		}
	}
}