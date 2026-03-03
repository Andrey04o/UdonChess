using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    public class King : Piece
    {
        public override void CalcAttack(Piece piece, bool isRemove = false, bool isVisualMoving = false)
        {
            base.CalcAttack(piece);
            KingMove(piece, new Vector2Int(1,1), isRemove, isVisualMoving);
            KingMove(piece, new Vector2Int(-1,1), isRemove, isVisualMoving);
            KingMove(piece, new Vector2Int(1,-1), isRemove, isVisualMoving);
            KingMove(piece, new Vector2Int(-1,-1), isRemove, isVisualMoving);
            KingMove(piece, new Vector2Int(1,0), isRemove, isVisualMoving);
            KingMove(piece, new Vector2Int(-1,0), isRemove, isVisualMoving);
            KingMove(piece, new Vector2Int(0,1), isRemove, isVisualMoving);
            KingMove(piece, new Vector2Int(0,-1), isRemove, isVisualMoving);
        }
        void KingMove(Piece piece, Vector2Int dir, bool isRemove, bool isVisualMoving) {
            if (isVisualMoving == false) {
                piece.AddCellAttack(dir, isRemove, isVisualMoving, true);
                return;
            }
            Cell cell = piece.GetCurrentCell().GetNeighbourByOffset(dir);
            if (cell != null) {
                if (cell.IsAttacking(piece) == false) {
                    piece.AddCellAttack(dir, isRemove, isVisualMoving, true);
                    piece.countPossibleMoves++;
                }
            }
        }
        public override void ShowMove(Piece piece)
        {
            base.ShowMove(piece);
            // castling
            if (piece.isNotMoved != 0) return;
            if (piece.GetCurrentCell().IsAttacking(piece)) return;
            //piece.gameField.
            CheckCastlingQueenside(piece);
            CheckCastlingKingside(piece);
        }
        bool GetLeftEmpty(ref Cell cell) {
            cell = cell.GetLeft();
            if (cell.pieceCurrent == null) return false;
            return true;
        }
        bool GetRightEmpty(ref Cell cell) {
            cell = cell.GetRight();
            //Debug(cell.name)
            if (cell.pieceCurrent == null) return false;
            return true;
        }
        public void CheckCastlingKingside(Piece piece) {
            Cell cell;
            //Debug.Log("queenside");
            cell = piece.GetCurrentCell();
            if (GetLeftEmpty(ref cell)) return;
            if (cell.IsAttacking(piece)) return;
            if (GetLeftEmpty(ref cell)) return;
            if (cell.IsAttacking(piece)) return;
            cell.castling = 1;
            cell = cell.GetLeft();
            if (cell.pieceCurrent != null && cell.pieceCurrent.isNotMoved == 0) {
                piece.gameField.AddMove(piece.GetCurrentCell().GetLeft().GetLeft()); 
            }
        }
        public void CheckCastlingQueenside(Piece piece) {
            Cell cell;
            cell = piece.GetCurrentCell();
            if (GetRightEmpty(ref cell)) return;
            if (cell.IsAttacking(piece)) return;
            if (GetRightEmpty(ref cell)) return;
            if (cell.IsAttacking(piece)) return;
            cell.castling = 2;
            if (GetRightEmpty(ref cell)) return;
            cell = cell.GetRight();
            if (cell.pieceCurrent != null && cell.pieceCurrent.isNotMoved == 0) {
                piece.gameField.AddMove(piece.GetCurrentCell().GetRight().GetRight()); 
            }
        }
        bool castling = false;
        public override void PerformMove(Cell cell, Piece piece)
        {
            if (piece.isNotMoved == 0) {
                if (cell.castling != 0)
                    castling = true;
            }
            if (GetCastle(cell, out Piece castle, out Cell cellCastle)) {
                piece.gameField.AddChangePosition(castle, cellCastle);
            }
            base.PerformMove(cell, piece);
        }
        bool GetCastle(Cell cell, out Piece castle, out Cell castleCell) {
            castle = null;
            castleCell = null;
            if (castling == false) return false;
            castling = false;
            if (cell.castling == 1) {
                castle = cell.GetLeft().pieceCurrent;
                castleCell = cell.GetRight();
                return true;
                //PlacePiece(cell.GetRight(), piece1);
            }
            if (cell.castling == 2) {
                castle = cell.GetRight().GetRight().pieceCurrent;
                castleCell = cell.GetLeft();
                return true;
                //PlacePiece(cell.GetLeft(), piece1);
            }
            return false;
        }
    }
}
