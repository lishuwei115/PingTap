﻿#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fralle.AI.Spawning
{
  [CustomPropertyDrawer(typeof(WaveDefinition))]
  public class WaveDefinitionInspector : PropertyDrawer
  {
    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
      var sp = prop.FindPropertyRelative("enemy");
      var sp2 = prop.FindPropertyRelative("count");

      DrawEnemy(rect, sp);
      DrawCount(rect, sp2);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return 48;
    }

    static void DrawEnemy(Rect rect, SerializedProperty prop)
    {
      Object[] enemies = Resources.FindObjectsOfTypeAll<Enemy>();
      enemies = enemies.OrderBy(x => x.name).ToArray();

      var options = enemies.Select(x => x.name).ToArray();
      var values = enemies.Select((x, i) => i).ToArray();

      var currentIndex = prop.objectReferenceValue ? Array.IndexOf(enemies, prop.objectReferenceValue) : 0;
      var selectValue = EditorGUI.IntPopup(new Rect(rect.x, rect.y, rect.width, 20), "Enemy", currentIndex, options, values);

      prop.objectReferenceValue = enemies[selectValue];
    }

    static void DrawCount(Rect rect, SerializedProperty prop)
    {
      prop.intValue = EditorGUI.IntField(new Rect(rect.x, rect.y + 20, rect.width, 20), "Count", prop.intValue);
    }
  }
}
#endif