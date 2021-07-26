﻿using System;
using System.Linq;
using UnityEngine;

namespace Fralle.Pingtap
{
	[Serializable]
	public class DamageGraphicsHandler
	{
		static readonly int RendererColor = Shader.PropertyToID("_EmissionColor");

		[SerializeField] GameObject gibModel;

		DamageController damageController;
		Rigidbody[] rigidbodies;
		SkinnedMeshRenderer[] renderers;
		MaterialPropertyBlock propBlock;
		Color currentColor;
		float colorLerpTime;

		public void Setup(DamageController damageController)
		{
			this.damageController = damageController;
			this.damageController.OnDamageTaken += HandleDamageTaken;
			this.damageController.OnDeath += HandleDeath;

			SetupRagdoll();

			propBlock = new MaterialPropertyBlock();
			renderers = this.damageController.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (var r in renderers)
				r.GetPropertyBlock(propBlock);
		}

		public void Clean()
		{
			damageController.OnDamageTaken -= HandleDamageTaken;
			damageController.OnDeath -= HandleDeath;
		}

		public void LerpColors()
		{
			if (colorLerpTime <= 0)
				return;

			currentColor = Color.Lerp(currentColor, Color.black, 1 - colorLerpTime);
			propBlock.SetColor(RendererColor, currentColor);
			foreach (Renderer renderer in renderers)
				renderer.SetPropertyBlock(propBlock);

			colorLerpTime -= Time.deltaTime;
		}

		void SetupRagdoll()
		{
			rigidbodies = damageController.GetComponentsInChildren<Rigidbody>().Where(x => x.isKinematic).ToArray();
		}

		void ToggleRagdoll(bool enable)
		{
			foreach (var rigidBody in rigidbodies)
			{
				rigidBody.isKinematic = !enable;
				rigidBody.velocity = Vector3.zero;
			}
		}

		void HandleDamageTaken(DamageController damageController, DamageData damageData)
		{
			if (!damageData.DamageFromHit)
				return;

			currentColor = Color.white;

			propBlock.SetColor(RendererColor, currentColor);
			foreach (var r in renderers)
				r.SetPropertyBlock(propBlock);

			colorLerpTime = 1f;
		}

		void HandleDeath(DamageController damageController, DamageData damageData)
		{
			propBlock.SetColor(RendererColor, Color.black);
			foreach (Renderer renderer in renderers)
				renderer.SetPropertyBlock(propBlock);

			if (damageData.Gib)
			{
				Debug.Log("Spawn gib stuff");
			}
			else
			{
				ToggleRagdoll(true);
				foreach (var rigidBody in rigidbodies)
				{
					rigidBody.AddForceAtPosition(damageData.Force, damageData.Position);
				}
			}
		}

	}
}
