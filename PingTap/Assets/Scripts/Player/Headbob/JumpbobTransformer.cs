using Fralle.Core;
using UnityEngine;

namespace Fralle.PingTap
{
  public class JumpbobTransformer : LocalTransformer, IPositioner, IRotator
  {
    [Header("Clamp")]
    [SerializeField] float clampMomentum = 0.2f;

    [Header("Speeds")]
    [SerializeField] float smoothSpeed = 20f;
    [SerializeField] float resetSpeed = 10f;

    [Header("Magnitudes")]
    [SerializeField] float rotationMagnitude = 20f;
    [SerializeField] float jumpMagnitude = 0.001f;
    [SerializeField] float fallMagnitude = 0.002f;
    [SerializeField] float landMagnitude = 0.025f;

    PlayerController playerController;
    Combatant combatant;

    Vector3 currentPosition = Vector3.zero;
    Quaternion currentRotation = Quaternion.identity;

    bool pause;
    float momentum;

    void Awake()
    {
      playerController = GetComponentInParent<PlayerController>();

      combatant = GetComponentInParent<Combatant>();
      combatant.OnWeaponSwitch += OnWeaponSwitch;
      if (combatant.equippedWeapon != null)
        OnWeaponSwitch(combatant.equippedWeapon, null);
    }


    public Vector3 GetPosition() => currentPosition;
    public Quaternion GetRotation() => currentRotation;
    public override void Calculate()
    {
      if (pause)
        Reset();
      else
      {
        if (playerController.isGrounded)
          ResetMomentum();

        PerformBob();
      }
    }

    void ResetMomentum()
    {
      momentum = Mathf.Lerp(momentum, 0f, Time.deltaTime * resetSpeed);
    }

    void PerformBob()
    {
      if (playerController.rigidBody.velocity.y > 0) // Jumping
      {
        momentum -= playerController.rigidBody.velocity.y * jumpMagnitude;
      }
      else // Falling
      {
        momentum -= playerController.rigidBody.velocity.y * fallMagnitude;
      }

      momentum = Mathf.Clamp(momentum, -clampMomentum, clampMomentum);
      currentPosition = Vector3.Lerp(currentPosition, currentPosition.With(y: momentum), Time.deltaTime * smoothSpeed);
      currentRotation = Quaternion.AngleAxis(-momentum * rotationMagnitude, new Vector3(1, 0, 0));
    }

    void Reset()
    {
      momentum = Mathf.Lerp(momentum, 0f, Time.deltaTime * resetSpeed);
      currentPosition = Vector3.Lerp(currentPosition, Vector3.zero, Time.deltaTime * resetSpeed);
      currentRotation = Quaternion.Lerp(currentRotation, Quaternion.identity, Time.deltaTime * resetSpeed);
    }

    //void OnLanding(float impact)
    //{
    //  if (!pause)
    //    momentum += impact * landMagnitude;
    //}
    void OnWeaponSwitch(Weapon newWeapon, Weapon oldWeapon)
    {
      if (oldWeapon != null)
        oldWeapon.OnActiveWeaponActionChanged -= OnWeaponActionChanged;

      if (newWeapon != null)
        newWeapon.OnActiveWeaponActionChanged += OnWeaponActionChanged;
    }
    void OnWeaponActionChanged(Status status)
    {
      pause = status == Status.Firing;
    }
  }
}

