using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace Andrey04o.Chess {
    public class PiecePickup : UdonSharpBehaviour
    {
        public Piece piece;
        public RaycastButton.Cell cellCurrent;
        public bool isGrab;
        public override void OnPickup()
        {
            base.OnPickup();
            piece.gameField.GrabPiece(piece, true);
            isGrab = true;
            if (Networking.LocalPlayer.IsUserInVR() == false)
                piece.gameField.ShowPieceColliders(false);
            //piece.StartGrab(true);
        }
        public override void OnDrop()
        {
            base.OnDrop();
            piece.gameField.DropPiece();
            piece.gameField.ShowPieceColliders(true);
            isGrab = false;
            /*
            if (cellCurrent != null) {
                piece.StopGrab(cellCurrent.cell);
            } else {
                piece.StopGrab(null);
            }
            */
            
        }
        public void AddCell(RaycastButton.Cell cell) {
            if (!isGrab) return;
            RemoveCell();
            cellCurrent = cell;
            cell.OnRaycastEnter();
        }
        public void RemoveCell() {
            if (!isGrab) return;
            if (cellCurrent != null) {
                cellCurrent.OnRaycastExit();
            }
            cellCurrent = null;
        }
    }
}
