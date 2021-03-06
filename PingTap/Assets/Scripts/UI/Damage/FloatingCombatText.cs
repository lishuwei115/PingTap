using Fralle.Core.Pooling;
using Fralle.PingTap;
using TMPro;
using UnityEngine;

namespace Fralle.UI
{
  public class FloatingCombatText : MonoBehaviour
  {
    [SerializeField] GameObject combatTextPrefab;
    [SerializeField] Vector2 randomPosition = new Vector2(0.5f, 0.5f);

    void Awake()
    {
      DamageController damageController = GetComponentInParent<DamageController>();
      damageController.OnDamageTaken += HandleDamageTaken;
    }

    void HandleDamageTaken(DamageController dc, DamageData damageData)
    {
      if (damageData.Attacker.hasActiveCamera)
        AddFloatingCombatText(damageData);
    }

    void AddFloatingCombatText(DamageData damageData)
    {
      Transform cameraTransform = Camera.main?.transform;
      if (!cameraTransform)
        return;

      GameObject instance = ObjectPool.Spawn(combatTextPrefab, transform.position, Quaternion.identity);

      SetPosition(instance);

      TextMeshPro text = instance.GetComponentInChildren<TextMeshPro>();
      SetText(text, Mathf.RoundToInt(damageData.DamageAmount));

      instance.transform.LookAt(instance.transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }

    void SetPosition(GameObject go)
    {
      Vector3 position = new Vector3(Random.Range(-randomPosition.x, randomPosition.x), Random.Range(0, randomPosition.y), 0);
      go.transform.position = transform.position + position;
    }

    static void SetText(TMP_Text text, int number)
    {
      text.text = number.ToString();
    }
  }
}
