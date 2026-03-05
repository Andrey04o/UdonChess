using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using UnityEngine.UI;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TileTouch : UdonSharpBehaviour
    {
        public TileRaycastHandler raycastHandler;
        public Cell cell;
        public ScrollRect scrollRect;
        public RectTransform cursor;
        public void StartDrag() {
            cursor.transform.position = transform.position;
            raycastHandler.gameObject.SetActive(true);
            cell.OnRaycastClick();
            cell.OnRaycastDrag(raycastHandler);
            Debug.Log("stargDrag");
        }
        public void Drag() {
            //Debug.Log("Drag" + scrollRect.velocity + " " + scrollRect.content.position);
        }
        public void StopDrag() {
            cell.OnRaycastDrag(raycastHandler);
            raycastHandler.MouseUp();
            raycastHandler.gameObject.SetActive(false);
            Debug.Log("stopDrag");
        }
    }
}
