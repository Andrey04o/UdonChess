using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RaycastButton : UdonSharpBehaviour
    {
        virtual public void OnRaycastEnter() {

        }
        virtual public void OnRaycastExit() {

        }
        virtual public void OnRaycastClick(Vector2 position) {

        }
        virtual public void OnRaycastMouseUp(TileRaycastHandler handler) {

        }
        virtual public void OnRaycastDrag(TileRaycastHandler handler) {

        }
    }
}
