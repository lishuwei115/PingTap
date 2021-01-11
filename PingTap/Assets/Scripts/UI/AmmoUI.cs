using CombatSystem;
using CombatSystem.Addons;
using CombatSystem.Combat;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI currentAmmoText;
	[SerializeField] TextMeshProUGUI maxAmmoText;

	void Awake()
	{
		var combatant = GetComponentInParent<Combatant>();
		combatant.OnWeaponSwitch += HandleWeaponSwitch;

		if (combatant.weapon)
			HandleWeaponSwitch(combatant.weapon);
	}


	void HandleWeaponSwitch(Weapon weapon)
	{
		var ammoAddon = weapon.GetComponent<AmmoAddon>();
		ammoAddon.OnAmmoChanged += HandleAmmoChanged;

		currentAmmoText.text = ammoAddon.currentAmmo.ToString();
		maxAmmoText.text = ammoAddon.maxAmmo.ToString();
	}


	void HandleAmmoChanged(int ammoCount)
	{
		currentAmmoText.text = ammoCount.ToString();
	}
}
