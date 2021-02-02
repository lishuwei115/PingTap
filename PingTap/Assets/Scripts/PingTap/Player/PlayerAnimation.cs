﻿using Fralle.FpsController;
using UnityEngine;

namespace Fralle
{
	public class PlayerAnimation : MonoBehaviour
	{
		Animator animator;
		PlayerController playerController;

		void Awake()
		{
			animator = GetComponent<Animator>();
			playerController = GetComponentInParent<PlayerController>();
		}

		void Update()
		{
			animator.SetBool("IsMoving", playerController.IsMoving);

		}
	}
}