using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    public class King : Piece
    {
        public override void CalcAttack(Piece piece, bool isRemove = false)
        {
            base.CalcAttack(piece);
            piece.AddCellAttack(new Vector2Int(1,1), isRemove);
            piece.AddCellAttack(new Vector2Int(-1,1), isRemove);
            piece.AddCellAttack(new Vector2Int(1,-1), isRemove);
            piece.AddCellAttack(new Vector2Int(-1,-1), isRemove);
            piece.AddCellAttack(new Vector2Int(1,0), isRemove);
            piece.AddCellAttack(new Vector2Int(-1,0), isRemove);
            piece.AddCellAttack(new Vector2Int(0,1), isRemove);
            piece.AddCellAttack(new Vector2Int(0,-1), isRemove);
        }
        public override void ShowMove(Piece piece)
        {
            Cell cell;
            for (int i = 0; i < piece.dir1Count; i++) {
                cell = piece.gameField.cells[piece.dir1[i]];
                if (piece.isBlack) {
                    if (cell.attackByCount == 0) {
                        piece.gameField.AddMove(cell, piece);
                    }
                } else {
                    if (cell.attackByCountBlack == 0) {
                        piece.gameField.AddMove(cell, piece);
                    }
                }
            }

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
            if (isNotMoved == 0) {
                if (cell.castling == 0) return;
                castling = true;
            }
            if (GetCastle(cell, out Piece castle, out Cell cellCastle)) {
                Cell cellCastleOld = castle.GetCurrentCell();
                Cell cellOld = piece.GetCurrentCell();
                cellCastleOld.VectorGetPieces();
                cellCastle.VectorGetPieces();
                cellOld.VectorGetPieces();
                cell.VectorGetPieces();
                
                piece.GetPiece().CalcAttack(piece, true);
                cellCastleOld.VectorCalcAttack(true);
                cellCastle.VectorCalcAttack(true);

                piece.GetPiece().CalcAttack(piece, true);
                cellOld.VectorCalcAttack(true);
                cell.VectorCalcAttack(true);

                piece.isNotMoved = 1;
                castle.isNotMoved = 1;
                
                piece.positionPrevious = piece.position;
                castle.positionPrevious = castle.position;
                piece.gameField.cells[piece.position].pieceCurrent = null;
                castle.gameField.cells[castle.position].pieceCurrent = null;
                piece.position = cell.position;
                castle.position = cellCastle.position;
                
                piece.gameField.HideMove();
                piece.gameField.ChangeSide();

                cell.PlacePiece(piece);
                cellCastle.PlacePiece(castle);
                piece.GetPiece().CalcAttack(piece, false);
                cellOld.VectorCalcAttack(false);
                cell.VectorCalcAttack(false);
                castle.GetPiece().CalcAttack(piece, false);
                cellCastleOld.VectorCalcAttack(false);
                cellCastle.VectorCalcAttack(false);
            }
            else {
                base.PerformMove(cell, piece);
            }

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
