#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
namespace Andrey04o.Chess.RaycastButton {
    [CustomEditor(typeof(UIButtonController))]
    public class UIButtonControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();

            UIButtonController myTarget = (UIButtonController)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("Set collider"))
            {
                myTarget.CreateCollider();
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
}
#endif