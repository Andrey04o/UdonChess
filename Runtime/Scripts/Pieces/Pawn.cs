using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using System;
namespace Andrey04o.Chess {
    public class Pawn : Piece
    {
        public override void CalcAttack(Piece piece, bool isRemove = false)
        {
            base.CalcAttack(piece);
            CheckAttack(piece, false, isRemove);
        }
        void CheckAttack(Piece piece, bool checkEnPassant, bool isRemove = false) {
            if (piece.isBlack) {
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(1,1), isRemove);
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(-1,1), isRemove);
            } else {
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(1,-1), isRemove);
                AddCellAttackPawn(piece, checkEnPassant, new Vector2Int(-1,-1), isRemove);
            }
        }
        void AddCellAttackPawn(Piece piece, bool checkEnPassant, Vector2Int dir, bool isRemove) {
            if (checkEnPassant == false) {
                piece.AddCellAttack(dir, isRemove);
            } else {
                Cell cell = piece.GetCurrentCell().GetNeighbourByOffset(dir);
                if (cell == null) return;
                if (cell.pieceEnPassant != null) {
                    piece.gameField.AddMove(piece.gameField.enPassantCell);
                }
            }
            
        }
        public override void ShowMove(Piece piece)
        {
            Cell cell;
            for (int i = 0; i < piece.dir1Count; i++) {
                cell = piece.gameField.cells[piece.dir1[i]];
                if (cell.pieceCurrent != null || cell.pieceEnPassant != null) {
                    if (cell.pieceCurrent.isBlack != piece.isBlack) {
                        piece.gameField.AddMove(cell, piece); 
                    }
                }
            }
            if (piece.isBlack) {
                PawnMove(piece, new Vector2Int(0,1));
                if (piece.isNotMoved == 0) {
                    PawnMove(piece, new Vector2Int(0,2));
                }
            } else {
                PawnMove(piece, new Vector2Int(0,-1));
                if (piece.isNotMoved == 0) {
                    PawnMove(piece, new Vector2Int(0,-2));
                }
            }
            if (piece.gameField.ShowEnPassant()) {
                CheckAttack(piece, true);
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
                cell.pieceEnPassant.isCaptured = 1;
                cell.pieceEnPassant.PerformCapture();
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
