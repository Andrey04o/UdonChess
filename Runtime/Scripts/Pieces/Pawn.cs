using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using System;
using VRC.SDKBase;
namespace Andrey04o.Chess {
    public class Pawn : Piece
    {
        public override void CalcAttack(Piece piece, bool isRemove = false, bool isVisualMoving = false)
        {
            base.CalcAttack(piece);
            CheckAttack(piece, false, isRemove, isVisualMoving);
        }
        void CheckAttack(Piece piece, bool checkEnPassant, bool isRemove = false, bool isVisualMoving = false) {
            AddCellAttackPawn(piece, checkEnPassant, piece.forward+piece.left, isRemove, isVisualMoving);
            AddCellAttackPawn(piece, checkEnPassant, piece.forward+piece.left*-1, isRemove, isVisualMoving);
        }
        void AddCellAttackPawn(Piece piece, bool checkEnPassant, Vector2Int dir, bool isRemove, bool isVisualMoving) {
            if (isVisualMoving) {
                piece.gameField.ShowEnPassant();
                Cell cell = piece.GetCurrentCell().GetNeighbourByOffset(dir);
                if (cell == null) return;
                if (piece.gameField.enPassantCell != null) {
                    if (cell.pieceEnPassant != null) piece.gameField.AddMove(piece.gameField.enPassantCell);
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
            if (PawnMove(piece, piece.forward) != null)
            if (piece.isNotMoved == 0) {
                PawnMove(piece, piece.forward * 2);
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
            if (cell.GetNeighbour(piece.forward) == null) {
                piece.ShowPromotion(true, cell.position);
                return;
            }
            if (Networking.IsOwner(Networking.LocalPlayer, piece.gameField.gameObject)) {
                if (cell == gameField.enPassantCell) {
                    piece.gameField.AddRemovePiece(gameField.enPassantPiece);
                }
            }
            base.PerformMove(cell, piece);
            if (Networking.IsOwner(Networking.LocalPlayer, piece.gameField.gameObject) == false) return;
            Cell cell1;
            cell1 = piece.GetCurrentCellPrevious();
            cell1 = cell1.GetNeighbour(piece.forward);
            if (cell1 == null) return;
            cell1 = cell1.GetNeighbour(piece.forward);
            if (cell1 == null) return;
            if (piece.GetCurrentCell().position == cell1.position) {
                piece.gameField.SetEnPassant(piece);
            }
        }
    }
}
