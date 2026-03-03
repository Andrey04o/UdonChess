using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using System;
namespace Andrey04o.Chess {
    public class Pawn : Piece
    {
        public override void CalcAttack(Piece piece, bool isRemove = false, bool isVisualMoving = false)
        {
            base.CalcAttack(piece);
            CheckAttack(piece, false, isRemove, isVisualMoving);
        }
        void CheckAttack(Piece piece, bool checkEnPassant, bool isRemove = false, bool isVisualMoving = false) {
            if (piece.isBlack) {
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(1,1), isRemove, isVisualMoving);
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(-1,1), isRemove, isVisualMoving);
            } else {
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(1,-1), isRemove, isVisualMoving);
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(-1,-1), isRemove, isVisualMoving);
            }
        }
        void AddCellAttackPawn(Piece piece, bool checkEnPassant, Vector2Int dir, bool isRemove, bool isVisualMoving) {
            if (isVisualMoving) {
                piece.gameField.ShowEnPassant();
                Cell cell = piece.GetCurrentCell().GetNeighbourByOffset(dir);
                if (cell == null) return;
                if (cell.pieceEnPassant != null) {
                    piece.gameField.AddMove(piece.gameField.enPassantCell);
                }
                if (cell.pieceCurrent != null) {
                    piece.gameField.AddMove(cell, piece);
                }
            } else {
                piece.AddCellAttack(dir, isRemove, isVisualMoving);
            }
        }
        public override void ShowMove(Piece piece)
        {
            base.ShowMove(piece);
            if (piece.isBlack) {
                if (PawnMove(piece, new Vector2Int(0,1)) != null)
                if (piece.isNotMoved == 0) {
                    PawnMove(piece, new Vector2Int(0,2));
                }
            } else {
                if (PawnMove(piece, new Vector2Int(0,-1)) != null)
                if (piece.isNotMoved == 0) {
                    PawnMove(piece, new Vector2Int(0,-2));
                }
            }
        }
        
        public Cell PawnMove(Piece piece, Vector2Int dir) {
            Cell cell = piece.GetCurrentCell().GetNeighbourByOffset(dir);
            if (cell != null) {
                if (cell.pieceCurrent == null) {
                    piece.gameField.AddMove(cell, piece);
                    return cell;
                }
            }
            return null;
        }
        public override void PerformMove(Cell cell, Piece piece)
        {
            if (cell.pieceEnPassant != null) {
                piece.gameField.AddRemovePiece(cell.pieceEnPassant);
            }
            base.PerformMove(cell, piece);
            Cell cell1;
            cell1 = piece.GetCurrentCellPrevious();
            if (piece.isBlack) {
                cell1 = cell1.GetUp();
                if (cell1 == null) return;
                cell1 = cell1.GetUp();
                if (cell1 == null) return;
                if (piece.GetCurrentCell().position == cell1.position) {
                    piece.gameField.SetEnPassant(piece);
                }
            } else {
                cell1 = cell1.GetDown();
                if (cell1 == null) return;
                cell1 = cell1.GetDown();
                if (cell1 == null) return;
                if (piece.GetCurrentCell().position == cell1.position) {
                    piece.gameField.SetEnPassant(piece);
                }
            }
        }
    }
}
