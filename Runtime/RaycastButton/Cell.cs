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
        public override void OnRaycastEnter(bool isUnityMouse = false)
        {
            base.OnRaycastEnter(isUnityMouse);
            if (isUnityMouse && cell.gameField.is2DMode) return;
            cell.gameField.SetHoveredCell(this);
            meshRenderer.material.Lerp(cell.materialCurrent, materialHighlight, 0.5f);
            if (isUnityMouse) MoveHitPosition();
        }
        public override void OnRaycastExit(bool isUnityMouse = false)
        {
            base.OnRaycastExit(isUnityMouse);
            if (isUnityMouse && cell.gameField.is2DMode) return;
            cell.gameField.SetHoveredCell(this, true);
        }
        public void VisuallyRemove() {
            meshRenderer.material = cell.materialCurrent;
        }
        public override void OnRaycastDrag(bool isUnityMouse = false)
        {
            base.OnRaycastDrag(isUnityMouse);
            if (isUnityMouse && cell.gameField.is2DMode) return;
            if (cell.pieceCurrent != null) {
                if (cell.gameField.IsCanUse(cell.pieceCurrent) == false) return;
                //pieceGrab.StartGrab(gameField.hitPosition);
                cell.gameField.GrabPiece(cell.pieceCurrent);
                //cell.pieceCurrent.StartGrab(false);
            }
        }
        public override void OnRaycastMouseUp(bool isUnityMouse = false)
        {
            base.OnRaycastMouseUp();
            if (isUnityMouse && cell.gameField.is2DMode) return;
            if (cell.gameField.pieceHolding != null) {
                cell.gameField.pieceHolding.StopGrab(cell);
                cell.gameField.pieceHolding = null;
            }
        }
        void MoveHitPosition() {
            cell.gameField.hitPosition.position = transform.position;
        }
    }
}

