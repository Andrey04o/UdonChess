using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Cell : RaycastButton
    {
        public Chess.Cell cell;
        public MeshRenderer meshRenderer;
        public Material materialHighlight;
        public override void OnRaycastEnter()
        {
            base.OnRaycastEnter();
            meshRenderer.material.Lerp(cell.materialCurrent, materialHighlight, 0.5f);
            

        }
        public override void OnRaycastExit()
        {
            base.OnRaycastExit();
            meshRenderer.material = cell.materialCurrent;
        }
        public override void OnRaycastDrag(TileRaycastHandler handler)
        {
            base.OnRaycastDrag(handler);
            if (cell.pieceCurrent != null) {
                cell.pieceCurrent.StartGrab(handler);
            }
        }
        public override void OnRaycastMouseUp(TileRaycastHandler handler)
        {
            base.OnRaycastMouseUp(handler);
            if (handler.currentPiece != null) {
                handler.currentPiece.StopGrab(cell);
            }
        }
    }
}

