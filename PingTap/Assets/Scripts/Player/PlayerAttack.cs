﻿using Fralle.Core;
using Fralle.PingTap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;

namespace Fralle
{
  [RequireComponent(typeof(Combatant))]
  public class PlayerAttack : MonoBehaviour, IWeaponActions
  {
    public Transform weaponCamera;

    [SerializeField] Weapon[] weapons = new Weapon[0];

    Combatant combatant;
    Vector3 defaultWeaponCameraPosition;
    Quaternion defaultWeaponCameraRotation;
    int firstPersonObjectsLayer;
    bool primaryFireHold;
    bool secondaryFireHold;

    void Awake()
    {
      firstPersonObjectsLayer = LayerMask.NameToLayer("FPO");

      combatant = GetComponent<Combatant>();
      combatant.OnWeaponSwitch += OnWeaponSwitch;

      defaultWeaponCameraPosition = weaponCamera.transform.localPosition;
      defaultWeaponCameraRotation = weaponCamera.transform.localRotation;
    }

    void Start()
    {
      Player.controls.Weapon.SetCallbacks(this);
      Player.controls.Weapon.Enable();

      if (combatant.equippedWeapon == null)
        combatant.EquipWeapon(weapons[0]);
    }

    void Update()
    {
      if (primaryFireHold)
      {
        combatant.PrimaryAction();
      }
      else if (secondaryFireHold)
        combatant.SecondaryAction();
    }

    [Button("Equip Weapon")]
    public void EquipFirstWeaponInList()
    {
      combatant.EquipWeapon(weapons[0], false);
      combatant.equippedWeapon.gameObject.SetLayerRecursively(firstPersonObjectsLayer);
      Debug.Log($"Equipped: {combatant.equippedWeapon}");
    }

    [Button("Remove Weapon")]
    public void RemoveWeapon()
    {
      Debug.Log($"Removed: {combatant.equippedWeapon}");
      combatant.ClearWeapons();
    }

    void OnWeaponSwitch(Weapon weapon, Weapon oldWeapon)
    {
      if (combatant.equippedWeapon == null)
      {
        weaponCamera.localPosition = defaultWeaponCameraPosition;
        weaponCamera.localRotation = defaultWeaponCameraRotation;
        return;
      }

      combatant.equippedWeapon.gameObject.SetLayerRecursively(firstPersonObjectsLayer);
      weaponCamera.localPosition = combatant.equippedWeapon.weaponCameraTransform.localPosition;
      weaponCamera.localRotation = combatant.equippedWeapon.weaponCameraTransform.localRotation;
    }

    void IWeaponActions.OnItemSelect(InputAction.CallbackContext context)
    {
      if (!context.performed)
        return;
      int number = (int)context.ReadValue<float>();
      combatant.EquipWeapon(weapons.Length >= number + 1 ? weapons[number] : null);
    }

    void IWeaponActions.OnPrimaryFire(InputAction.CallbackContext context)
    {
      if (context.performed)
      {
        primaryFireHold = true;
        if (context.duration <= 0)
          combatant.PrimaryAction(true);
      }
      else if (context.canceled)
        primaryFireHold = false;
    }

    void IWeaponActions.OnSecondaryFire(InputAction.CallbackContext context)
    {
      if (context.performed)
      {
        secondaryFireHold = true;
        if (context.duration <= 0)
          combatant.SecondaryAction(true);
      }
      else if (context.canceled)
        secondaryFireHold = false;
    }
  }
}
