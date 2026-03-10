using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    public class PiecePickup : UdonSharpBehaviour
    {
        public Piece piece;
        public RaycastButton.Cell cellCurrent;
        public override void OnPickup()
        {
            base.OnPickup();
            piece.gameField.GrabPiece(piece, true);
            //piece.StartGrab(true);
        }
        public override void OnDrop()
        {
            base.OnDrop();
            piece.gameField.DropPiece();
            /*
            if (cellCurrent != null) {
                piece.StopGrab(cellCurrent.cell);
            } else {
                piece.StopGrab(null);
            }
            */
            
        }
        public void AddCell(RaycastButton.Cell cell) {
            RemoveCell();
            cellCurrent = cell;
            cell.OnRaycastEnter();
        }
        public void RemoveCell() {
            if (cellCurrent != null) {
                cellCurrent.OnRaycastExit();
            }
            cellCurrent = null;
        }
    }
}
