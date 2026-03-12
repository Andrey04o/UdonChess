#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UdonSharpEditor;
namespace Andrey04o.Chess {
    [CustomEditor(typeof(Chess04o), true)]
    public class Chess04oEditor : Editor
    {
        public VisualTreeAsset visualTree;
        private Chess04o myTarget;
        private Button buttonCameraChangeSize;
        private Label labelCameraProperties;

        private void OnEnable() {
            myTarget = (Chess04o)target;
        }
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            visualTree.CloneTree(root);

            buttonCameraChangeSize = root.Q<Button>("ButtonCameraChangeSize");
            
            buttonCameraChangeSize.RegisterCallback<ClickEvent>(SetCameraSize);

            labelCameraProperties = root.Q<Label>("LabelCameraProperties");

            SetLabelText();

            return root;
        }

        public void ChangeCameraSize(Transform myTarget, Camera camera) {
            float scale = 1 / myTarget.lossyScale.x;
            camera.transform.localScale = new Vector3(scale, scale, scale);
            camera.orthographicSize = myTarget.lossyScale.x;
            EditorUtility.SetDirty(camera);
        }

        void SetCameraSize(ClickEvent evt) {
            if (myTarget.cameraDesktop == null) return;
            ChangeCameraSize(myTarget.transform, myTarget.cameraDesktop);
            SetLabelText();
        }
        void SetLabelText() {
            if (myTarget.cameraDesktop != null) {
                labelCameraProperties.text = "Scale: " + myTarget.cameraDesktop.transform.localScale + "; Orthographic Size: " + myTarget.cameraDesktop.orthographicSize;
            } else {
                labelCameraProperties.text = "";
            }
        }
        /*
        public override void OnInspectorGUI()
        {
            //if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();
            
            Chess04o myTarget = (Chess04o)target;
            if (myTarget.transform.lossyScale.x != myTarget.cameraDesktop.orthographicSize) {
                ChangeCameraSize(myTarget.transform, myTarget.cameraDesktop);
            }
            
            //EditorGUILayout.Space();
            /*
            if (GUILayout.Button("Set Offset"))
            {
                //myTarget.offset = myTarget.meshRenderer.gameObject.transform.position;
                EditorUtility.SetDirty(myTarget);
            }
            
        }
        */
    }
}
#endif