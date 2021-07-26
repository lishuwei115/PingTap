﻿using UnityEngine;

namespace Fralle.Pingtap
{
	public class DamageData
	{
		public Combatant Attacker;
		public DamageController Victim;
		public GameObject ImpactEffect;

		public HitArea HitArea;
		public Element Element;
		public DamageEffect[] Effects;

		public Vector3 Position;
		public Vector3 Normal;
		public Vector3 Force;

		public float HitAngle;
		public float DamageAmount;

		public bool KillingBlow;
		public bool Gib;
		public bool DamageFromHit = true;

		public DamageData()
		{
			Effects = new DamageEffect[0];
		}
	}
}
