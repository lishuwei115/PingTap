﻿using Fralle.Resource;
using Fralle.UI;
using UnityEngine;

public class Treasure : MonoBehaviour
{
  [Header("Drops")]
  [SerializeField] LootTable lootTable;

  public float destroyAfterDrop = 5f;

  void Awake()
  {
    Drop();

    var uiTweener = GetComponentInChildren<UiTweener>();
    uiTweener.delay = destroyAfterDrop - uiTweener.duration;
  }

  void Drop()
  {
    if (!lootTable) return;
    lootTable.DropLoot(transform.position);
    Destroy(gameObject, destroyAfterDrop);
  }
}
