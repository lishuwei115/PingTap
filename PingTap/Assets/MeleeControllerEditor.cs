﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeleeController))]
public class MeleeControllerEditor : Editor
{
  void OnSceneGUI()
  {
    MeleeController t = target as MeleeController;

    Handles.color = new Color(1, 1, 1, 0.2f);
    Handles.DrawSolidArc(t.transform.position, t.transform.up, -t.transform.right, 180, t.meleeRadius);

    Handles.color = Color.white;
    t.meleeRadius = Handles.ScaleValueHandle(t.meleeRadius, t.transform.position + t.transform.forward * t.meleeRadius, t.transform.rotation, 1, Handles.ConeCap, 1);
  }
}