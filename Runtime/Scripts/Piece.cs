using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.Components;
using Andrey04o.RaycastButton;
using System;
using System.Linq;
using VRC.SDK3.Dynamics.Constraint.Components;
using VRC.SDKBase;
using VRC.SDK3.UdonNetworkCalling;
using VRC.Udon.Common.Interfaces;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Piece : UdonSharpBehaviour
    {
        public byte id;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        public byte indexType; //s
        public Vector3 offset;
        public Vector2Int forward;
        public Vector2Int left;
        public GameField gameField;
        public PieceGrab pieceGrab;
        public VRCPickup pickup;
        public VRCObjectSync objectSync;
        public byte position; //s
        public byte positionPrevious;
        public bool isBlack = false;
        public byte isNotMoved = 0; //s
        public byte isCaptured = 0; //s
        bool isMoved = false;
        public byte isCalculatedAttacks = 0;
        public byte countPossibleMoves = 0;
        public Promotion promotion;
        public byte isShowPromotion;
        public SpriteRenderer spriteRenderer;
        public Sprite spriteWhite;
        public Sprite spriteBlack;
        public VRCRotationConstraint rotationConstraint;
        public bool is2DMode = false;
        public byte originalPosition = 0;
        public byte originalIndexType = 0;
        public byte indexTypePrevious = 0;
        public Cell GetCurrentCell() {
            return gameField.cells[position];
        }
        public Cell GetCurrentCellPrevious() {
            return gameField.cells[positionPrevious];
        }
        public void ChangeType(byte indexType) {
            if (isCaptured == 1) return;
            if (indexTypePrevious == indexType) return;
            Piece piece = gameField.pieces.GetPiece(indexType);
            if (piece != null) {
                meshFilter.sharedMesh = piece.meshFilter.sharedMesh;
                meshCollider.sharedMesh = piece.meshFilter.sharedMesh;
                offset = piece.offset;
                meshRenderer.gameObject.transform.localPosition = offset;
                this.indexType = indexType;
                indexTypePrevious = indexType;
                if (isBlack) spriteRenderer.sprite  = piece.spriteBlack;
                else spriteRenderer.sprite = piece.spriteWhite;
            }
        }
        public void StartGrab(TileRaycastHandler handler) {
            //if (Networking.IsOwner(Networking.LocalPlayer, gameObject) == false)
            //    Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            if (gameField.IsHisTurn(this) == false) return;
            if (handler != null) handler.currentPiece = this;
            gameField.CancelPromotion();
            if (handler != null) pieceGrab.StartGrab(handler.hitPosition);
            gameField.CheckKing();
            gameField.CheckKingSafe(this); // 2
            gameField.SetCellsToCheck2();
            GetPiece().ShowMove(this);
        }
        public void StopGrab(Cell cell) {
            if (cell == null) {
                cell = gameField.cells[position];
            }
            if (cell.isCanMoveHere == false) {
                cell = gameField.cells[position];
            }
            pieceGrab.StopGrab();
            gameField.HideMove();
            gameField.ResetCellsCheck2();
            gameField.ResetCellsCheck();
            if (cell != GetCurrentCell()) {
                isMoved = true;
                GetPiece().PerformMove(cell,this);
                GetPiece().AfterMove(cell,this);
            } else {
                cell.PlacePiece(this);
            }
        }
        public void PromotionCheck() {
            if (GetPiece() == gameField.pieces.pawn) {

            }
        }
        public void ShowPromotion(bool value, byte destination) {
            if (value == true) isShowPromotion = 1;
            else isShowPromotion = 0;
            gameField.SetPromotion(this, destination);
            promotion.gameObject.SetActive(value);
        }
        public void CancelPromotion() {
            ShowPromotion(false, byte.MaxValue);
            GetCurrentCell().PlacePiece(this);
            gameField.promotionPiece = byte.MaxValue;
        }
        public void ConfirmPromotion(byte id) {
            //Cell currentCell = GetCurrentCell();
            //gameField.AddPromotion()
            if (Networking.IsOwner(Networking.LocalPlayer, gameField.gameObject)) {
                gameField.ConfirmPromotion(id);
            } else {
                NetworkCalling.SendCustomNetworkEvent((IUdonEventReceiver)gameField, NetworkEventTarget.Owner, nameof(GameField.ConfirmPromotionNetwork), id, gameField.promotionPiece, gameField.promotionDestination);
                gameField.UpdatePromotion();
            }
        }
        virtual public void PerformMove(Cell cell, Piece piece) {
            
            if (Networking.IsOwner(Networking.LocalPlayer, piece.gameField.gameObject) == false) {
                NetworkCalling.SendCustomNetworkEvent((IUdonEventReceiver)piece.gameField, NetworkEventTarget.Owner, nameof(GameField.PerformMoveNetwork), cell.position, piece.id);
                cell.PlacePiece(piece);
                return;
            }
            
            if (cell.pieceCurrent != null) {
                piece.gameField.AddRemovePiece(cell.pieceCurrent);
            }
            piece.gameField.AddChangePosition(piece, cell);

            piece.gameField.MakeMove();
        }
        virtual public void AfterMove(Cell cell, Piece piece) {
            if (piece.isMoved == true) {
                piece.isMoved = false;
            }
        }
        public void PlacePiece(Cell cell) {
            positionPrevious = position;
            gameField.cells[position].pieceCurrent = null;
            position = cell.position;
            isNotMoved = 1;
            //isMoved = true;
            cell.PlacePiece(this);
            
        }

        public void PerformCapture(bool isHost = true) {
            if (isCaptured == 1) {
                //GetPiece().CalcAttack(this, true);
                meshRenderer.enabled = false;
                meshCollider.enabled = false;
                spriteRenderer.gameObject.SetActive(false);
                if (isHost) GetCurrentCell().pieceCurrent = null;
                //RemoveAttack();
            } else {
                ShowPiece(Quaternion.identity);
            }
        }
        public virtual void CalcAttack(Piece piece, bool isRemove = false, bool isVisualMoving = false) {

        }
        public void AddAttack(Cell cell, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            cell.SetAttack(this, !isRemove, isVisualMoving, ignoreKingCheck);
        }
        public void AddAttack(Cell cell, Vector2Int dir, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            cell.SetAttack(this, !isRemove, isVisualMoving, ignoreKingCheck);
            if (isVisualMoving == false) cell.SetAttackVector(dir, !isRemove);
        }
        virtual public void RemoveAttack(Piece piece) {
            piece.GetPiece().CalcAttack(piece, false);
        }
        public void AddCellAttack(Vector2Int dir, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            Cell cell = GetCurrentCell().GetNeighbourByOffset(dir);
            if (cell != null) {
                AddAttack(cell, isRemove, isVisualMoving, ignoreKingCheck);
            }
        }
        public void AddSlidingCellAttack(Vector2Int dir, bool isRemove = false, bool isVisualMoving = false, bool ignoreKingCheck = false) {
            Cell[] cells = GetCurrentCell().GetSlidingCells(dir);
            foreach (Cell cell in cells) {
                //dir2[dir1Count] = dir;
                AddAttack(cell, dir, isRemove, isVisualMoving, ignoreKingCheck);
                //cell.SetAttackVector(dir, true);
            }
        }
        virtual public void ShowMove(Piece piece) {
            piece.GetPiece().CalcAttack(piece, false, true);
            //for (int i = 0; i < piece.dir1Count; i++) {
            //    piece.gameField.AddMove(piece.gameField.cells[piece.dir1[i]], piece);
            //}
        }
        public Piece GetPiece() {
            return gameField.pieces.GetPiece(indexType);
        }
        public void Set2DMode(bool value, Quaternion rotation) {
            is2DMode = value;
            if (isCaptured == 0) {
                spriteRenderer.gameObject.SetActive(value);
                //spriteRenderer.transform.rotation = rotation;
                if (value == true) {
                    rotationConstraint.ActivateConstraint();
                    promotion.ChangeRotation(rotation);
                }
                else {
                    rotationConstraint.ZeroConstraint();
                    promotion.ResetRotation();
                }
                meshRenderer.enabled = !value;
                meshCollider.enabled = !value;
            }
        }
        public void Set2DMode(bool value) {
            is2DMode = value;
            if (isCaptured == 0) {
                spriteRenderer.gameObject.SetActive(value);
                promotion.ResetRotation();
                meshRenderer.enabled = !value;
            }
        }
        public void ShowPiece(Quaternion rotation) {
            if (isCaptured == 1) return;
            meshRenderer.enabled = true;
            meshCollider.enabled = true;
            if (gameField.is2DMode == false) {
                spriteRenderer.gameObject.SetActive(false);
                rotationConstraint.ZeroConstraint();
            }
            promotion.ResetRotation();
            if (gameField.is2DMode) {
                spriteRenderer.gameObject.SetActive(true);
                if (rotation != Quaternion.identity) {
                    rotationConstraint.ActivateConstraint();
                    promotion.ChangeRotation(rotation);
                }
                meshRenderer.enabled = false;
                meshCollider.enabled = false;
            }
            if (gameField.isTouchMode) {
                meshCollider.enabled = false;
            }
        }
    }
}
