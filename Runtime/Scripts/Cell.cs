using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using System;
using TMPro;
namespace Andrey04o.Chess {
    public class Cell : UdonSharpBehaviour
    {
        public byte position;
        public Line line;
        public GameObject positionPiece;
        public Piece pieceCurrent;
        public Piece pieceEnPassant;
        public Cell cellLeft;
        public GameField gameField;
        public byte index;
        //public byte[] attackBy;
        public byte attackByCount;
        public byte attackByCountBlack;
        public byte attackVector;
        public Material materialNormal;
        public Material materialAttack;
        public Material materialCurrent;
        public Material materialOrange;
        public MeshRenderer meshRenderer;
        [HideInInspector] public bool isCanMoveHere = false;
        [HideInInspector] public byte castling = 0;
        [HideInInspector] public bool isCheck = false;
        [HideInInspector] public bool isCheck2 = false;
        public TextMeshPro text1;
        public TextMeshPro text2;
        public TextMeshPro text3;
        public TextMeshPro text4;
        Piece[] piecesVector = new Piece[8];
        public byte piecesVectorCount;
        public byte isCalculatedAttacks = 0;
        public void PlacePiece(Piece piece) {
            //gameField.cells[piece.position].pieceCurrent = null;
            pieceCurrent = piece;
            piece.transform.parent = transform;
            piece.transform.position = positionPiece.transform.position;
            piece.objectSync.transform.localPosition = piece.offset;
            piece.objectSync.TeleportTo(piece.objectSync.transform);
        }

        public Cell GetNeighbour(Vector2Int dir) {
            if (gameField == null) gameField = line.gameField;
            if (dir == Vector2Int.zero) {
                return this;
            }
            if (dir == Vector2Int.down) {
                return GetDown();
            }
            if (dir == Vector2Int.left) {
                return GetLeft();
            }
            if (dir == Vector2Int.right) {
                return GetRight();
            }
            if (dir == Vector2Int.up) {
                return GetUp();
            }
            return null;
        }
        public Cell GetNeighbourByOffset(Vector2Int offset) {
            Vector2Int myMovement = Vector2Int.zero;
            Cell currentNeighbour = this;
            int dir = 0;
            while (myMovement.x != offset.x) {
                dir = Mathf.Clamp(myMovement.x + offset.x, -1, 1);
                myMovement.x += dir;
                currentNeighbour = currentNeighbour.GetNeighbour(new Vector2Int(dir, 0));
                if (currentNeighbour == null) return null;
            }
            while (myMovement.y != offset.y) {
                dir = Mathf.Clamp(myMovement.y + offset.y, -1, 1);
                myMovement.y += dir;
                currentNeighbour = currentNeighbour.GetNeighbour(new Vector2Int(0, dir));
                if (currentNeighbour == null) return null;
            }
            return currentNeighbour;
        }
        public Cell GetLeft() {
            if (index < line.cells.Length - 1) {
                return line.cells[index + 1];
            }
            return null;
        }

        public Cell GetRight() {
            if (index > 0) {
                return line.cells[index - 1];
            }
            return null;
        }
        public Cell GetUp() {
            if (gameField == null) gameField = line.gameField;
            
            if (line.index >= gameField.lines.Length - 1) return null;
            
            Line lineAbove = gameField.lines[line.index + 1];
            if (lineAbove == null) return null;
            return lineAbove.cells[index];
        }
        public Cell GetDown() {
            if (gameField == null) gameField = line.gameField;
            
            if (line.index <= 0) return null;
            
            Line lineBelow = gameField.lines[line.index - 1];
            if (lineBelow == null) return null;
            
            return lineBelow.cells[index];
        }
        /*
        void PerformArray() {
            if (attackBy.Length != attackByCount) {
                byte[] newArray = new byte[attackByCount];
                for (int i = 0; i < attackByCount && i < attackBy.Length; i++) {
                    newArray[i] = attackBy[i];
                }
                attackBy = newArray;
                Debug.Log("new array, " + attackBy.Length);
            }
        }
        

        public void SetAttack(Piece piece, bool isAttack) {
            Debug.Log(name);
            if (isAttack) {
                attackByCount++;
                PerformArray();
                attackBy[attackByCount - 1] = piece.id;
            } else {
                int index = Array.IndexOf<byte>(attackBy, piece.id, 0, attackByCount);
                Debug.Log(name + " " + index);
                Debug.Log("Piece id " + piece.id + ", Cell attacked by ");
                //if (index == -1) return;
                foreach(Byte b in attackBy) {
                    Debug.Log(b);
                }
                attackBy[index] = byte.MaxValue;
                Array.Sort((Array)attackBy);
                attackByCount--;
                PerformArray();
            }
        }
        */
        public void SetAttack(Piece piece, bool isAttack, bool isVisualMoving, bool ignoreKingCheck) {
            if (isVisualMoving) {
                if (isAttack)
                    piece.gameField.AddMove(this, piece, ignoreKingCheck);
                else
                    SetMove(false, piece.gameField.IsKingCheck(ignoreKingCheck), false);
                return;
            }
            if (isAttack) {
                piece.gameField.PerformCheckIsKing(this, piece);
                if (piece.isBlack) attackByCountBlack++;
                else attackByCount++;
            } else {
                if (piece.isBlack) attackByCountBlack--;
                else attackByCount--;
            }
            text1.text = attackByCount + "";
            text2.text = attackByCountBlack + "";
        }
        public void RemoveAttack() {
            attackByCount = 0;
            attackByCountBlack = 0;
        }
        public bool CheckMove(Piece piece, bool isKingCheck) {
            bool isCan = false;
            if (pieceCurrent == null) {
                isCan = true;
            } else if (pieceCurrent.isBlack != piece.isBlack){
                isCan = true;
            }
            if (isCan && isKingCheck) {
                isCan = isCheck;
            }

            return isCan;
        }
        public bool CheckMove(bool isCan, bool isKingCheck) {
            if (isCan && isKingCheck) {
                isCan = isCheck;
            }

            return isCan;
        }
        public bool SetMove(bool isCan, bool isKingCheck, bool isKingCheck2) {
            isCanMoveHere = isCan;
            if (isCanMoveHere && isKingCheck) {
                isCanMoveHere = isCheck;
            }
            if (isCanMoveHere && isKingCheck2) {
                isCanMoveHere = isCheck2;
            }
            if (isCanMoveHere) {
                SetMaterial(1);
            } else {
                SetMaterial(0);
            }
            return isCanMoveHere;
        }
        public bool SetMove(Piece piece, bool isKingCheck, bool isKingCheck2) {
            isCanMoveHere = false;
            if (pieceCurrent == null) {
                isCanMoveHere = true;
            } else if (pieceCurrent.isBlack != piece.isBlack){
                isCanMoveHere = true;
            }
            if (isCanMoveHere && isKingCheck) {
                isCanMoveHere = isCheck;
            }
            if (isCanMoveHere && isKingCheck2) {
                isCanMoveHere = isCheck2;
            }
            
            if (isCanMoveHere) {
                SetMaterial(1);
            } else {
                SetMaterial(0);
            }
            return isCanMoveHere;
        }
        /*
        bool CheckIsSafe(Piece piece) {
            if (gameField.pieces.king.id == piece.id) {
                for(int i = 0; i < attackByCount; i++) {
                    if (gameField.pieces.InTableAll[attackBy[i]].isBlack != piece.isBlack) {
                        return false;
                    }
                }
            }
            return true;
        }
        */
        public void RemoveMove() {
            isCanMoveHere = false;
            SetMaterial(0);
        }
        public void SetMaterial(byte index) {
            materialCurrent = materialNormal;
            if (index == 0) {
                materialCurrent = materialNormal;
            } else {
                materialCurrent = materialAttack;
            }
            meshRenderer.material = materialCurrent;
        }

        public Cell[] GetSlidingCells(Vector2Int direction)
        {
            Cell[] result = new Cell[7]; // Max 7 cells in any direction on 8x8 board
            int count = 0;

            Cell currentCell = this;

            for (int distance = 1; distance <= 7; distance++)
            {
                currentCell = currentCell.GetNeighbourByOffset(direction);

                if (currentCell == null) break; // Hit board edge

                result[count] = currentCell;
                count++;

                if (currentCell.pieceCurrent != null) break; // Hit a piece, stop
            }

            // Resize array to actual count
            if (count < result.Length)
            {
                Cell[] trimmed = new Cell[count];
                for (int i = 0; i < count; i++)
                {
                    trimmed[i] = result[i];
                }
                return trimmed;
            }

            return result;
        }
        public bool IsAttacking(Piece piece) {
            if (piece.isBlack) {
                if (attackByCount != 0) return true;
            } else {
                if (attackByCountBlack != 0) return true;
            }
            return false;
        }
        public void SetAttackVector(Vector2Int dir, bool isAttack) {
            
            int bitPosition = GetBitPositionFromDirection(dir);
            text3.text = "";
            if (bitPosition >= 0) {
                if (isAttack)
                attackVector |= (byte)(1 << bitPosition);
                else {
                    attackVector &= (byte)(~(1 << bitPosition) & 0xFF);
                }
                
            }
            for (int bitPosition1 = 0; bitPosition1 < 8; bitPosition1++) {
                // Check if this direction has an attack vector
                if ((attackVector & (1 << bitPosition1)) == 0) continue;
                text3.text += GetTextFromDirection(bitPosition1) + "/";
            }
            
        }
        public void VectorGetPieces(Piece piece, bool isKingCheck = false, bool checkKingSafe = false) {
            // Check if this cell has an attack vector
            text4.text = "0";
            piecesVectorCount = 0;
            if (attackVector == 0) return;
            // Iterate through all 8 directions (bits 0-7)
            for (int bitPosition = 0; bitPosition < 8; bitPosition++) {
                // Check if this direction has an attack vector
                if ((attackVector & (1 << bitPosition)) == 0) continue;
                
                Vector2Int negativeMovement = GetDirectionFromBitPosition(bitPosition) * -1;
                text4.text += GetTextFromDirection(bitPosition) + "/";
                // Find the piece by moving in the negative direction
                Cell neighbourCell = this;
                for(;;) {
                    neighbourCell = neighbourCell.GetNeighbourByOffset(negativeMovement);
                    if (neighbourCell == null) return; // impossible
                    if (isKingCheck) gameField.AddCellCheck(neighbourCell);
                    if (neighbourCell.pieceCurrent != null) break;
                }
                Piece neighbourPiece = neighbourCell.pieceCurrent;
                if (isKingCheck) {
                    if (piece.isBlack == neighbourPiece.isBlack) {
                        gameField.ResetCellsCheck();
                        continue;
                    }
                    return;
                }
                if (neighbourPiece == piece) continue;
                gameField.AddChangedCell(neighbourCell);
                piecesVector[piecesVectorCount] = neighbourPiece;
                piecesVectorCount++;

                neighbourCell.meshRenderer.material = materialOrange;
            }
        }
        Piece GetKing(Vector2Int movement) {
                // Find the king
                Cell neighbourCell = this;
                for(;;) {
                    neighbourCell = neighbourCell.GetNeighbourByOffset(movement);
                    if (neighbourCell == null) return null; // impossible
                    if (neighbourCell.pieceCurrent != null) break;
                }
                Piece king = neighbourCell.pieceCurrent;
                if (gameField.IsHisTurn(king) == false) return null;
                if (king.GetPiece() == gameField.pieces.king) {
                    return king;
                }
                return null;
            }
        public void VectorCheckKing() {
            
            // Check if this cell has an attack vector
            if (attackVector == 0) return;
            // Iterate through all 8 directions (bits 0-7)
            for (int bitPosition = 0; bitPosition < 8; bitPosition++) {
                // Check if this direction has an attack vector
                if ((attackVector & (1 << bitPosition)) == 0) continue;
                
                Vector2Int movement = GetDirectionFromBitPosition(bitPosition);
                // Find the king
                
                Piece king = GetKing(movement);
                if (king == null) continue;

                movement = movement * -1;
                Cell neighbourCell = this;
                for(;;) {
                    neighbourCell = neighbourCell.GetNeighbourByOffset(movement);
                    if (neighbourCell == null) return; // impossible
                    gameField.AddCellCheck2(neighbourCell);
                    if (neighbourCell.pieceCurrent != null) break;
                }
                Piece neighbourPiece = neighbourCell.pieceCurrent;
                
                if (neighbourPiece.isBlack == king.isBlack) {
                    Debug.Log("reset, king in " + king.GetCurrentCell().name + " movement " + movement + " found piece " + neighbourCell.name);
                    gameField.ResetCellsCheck2();
                    return;
                }
                Debug.Log("resetnot, king in " + king.GetCurrentCell().name + " movement " + movement + " found piece " + neighbourCell.name);
            }
        }
        /*
        public void VectorCalcAttack(bool isRemove = false) {
            for (int i = 0; i < piecesVectorCount; i++) {
                if (piecesVector[i].isCaptured == 1) continue;
                piecesVector[i].GetPiece().CalcAttack(piecesVector[i], isRemove);
            }
        }
        */

        int GetBitPositionFromDirection(Vector2Int dir) {
            // Кодируем направление в индекс через смещение
            // x: -1→0, 0→1, 1→2 | y: -1→0, 0→1, 1→2
            int ix = dir.x + 1; // 0,1,2
            int iy = dir.y + 1; // 0,1,2
            int index = iy * 3 + ix; // 0..8
            
            // Таблица соответствия 3x3 → биты (9 = недопустимое)
            int[] map = { 0, 2, 1, 6, 9, 7, 5, 3, 4 }; // [y= -1,0,1][x=-1,0,1]
            
            int result = map[index];
            return result == 9 ? -1 : result;
        }
        
        Vector2Int GetDirectionFromBitPosition(int bitPosition) {
            switch (bitPosition) {
                case 7: return new Vector2Int(1, 0);
                case 6: return new Vector2Int(-1, 0);
                case 3: return new Vector2Int(0, 1);
                case 2: return new Vector2Int(0, -1);
                case 4: return new Vector2Int(1, 1);
                case 5: return new Vector2Int(-1, 1);
                case 1: return new Vector2Int(1, -1);
                case 0: return new Vector2Int(-1, -1);
                default: return Vector2Int.zero;
            }
        }
        string GetTextFromDirection(int bitPosition) {
            switch (bitPosition) {
                case 7: return "Right";
                case 6: return "Left";
                case 3: return "Up";
                case 2: return "Down";
                case 4: return "Up-Right";
                case 5: return "Up-Left";
                case 1: return "Down-Right";
                case 0: return "Down-Left";
                default: return "-1";
            }
        }


    }
}
