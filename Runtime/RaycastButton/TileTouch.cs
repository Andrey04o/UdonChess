using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using UnityEngine.UI;
namespace Andrey04o.Chess.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TileTouch : UdonSharpBehaviour
    {
        public TileRaycastHandler raycastHandler;
        public Cell cell;
        public ScrollRect scrollRect;
        public RectTransform cursor;
        bool isDrag = false;
        public void StartDrag() {
            if (isDrag) return; //this method for some reason invokes 3 times
            isDrag = true;
            cursor.transform.position = transform.position;
            raycastHandler.gameObject.SetActive(true);
            cell.OnRaycastClick();
            cell.OnRaycastDrag(raycastHandler);
        }
        public void Drag() {
            //Debug.Log("Drag" + scrollRect.velocity + " " + scrollRect.content.position);
        }
        public void StopDrag() {
            if (!isDrag) return;
            isDrag = false;
            //cell.OnRaycastDrag(raycastHandler);
            raycastHandler.MouseUp();
            raycastHandler.gameObject.SetActive(false);
        }
    }
}
