using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RaycastButton : UdonSharpBehaviour
    {
        virtual public void OnRaycastEnter(bool isUnityMouse = false) {

        }
        virtual public void OnRaycastExit(bool isUnityMouse = false) {

        }
        virtual public void OnRaycastClick() {

        }
        virtual public void OnRaycastMouseUp(bool isUnityMouse = false) {

        }
        virtual public void OnRaycastDrag(bool isUnityMouse = false) {

        }
        void OnMouseEnter() {
            OnRaycastEnter(true);
        }
        void OnMouseExit() {
            OnRaycastExit(true);
        }
        void OnMouseUpAsButton() {
            //Debug.Log("mouse up unity");
            //OnRaycastMouseUp(true);
        }
        void OnMouseDown() {
            Debug.Log("mouse down unity");
            OnRaycastDrag(true);
        }
    }
}
