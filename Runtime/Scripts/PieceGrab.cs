using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using Andrey04o.RaycastButton;
using VRC.SDK3.Components;
using VRC.SDKBase;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PieceGrab : UdonSharpBehaviour
    {
        public Piece piece;
        bool isGrab = false;
        //public CursorController cursor;
        public Transform cursor;
        public Transform transform_move;
        public Vector3 offsetGrab;
        void Update() {
            if (isGrab == false) return;
            transform_move.position = cursor.position;
            transform_move.position += (offsetGrab + piece.offset) * transform.lossyScale.y;
            piece.objectSync.transform.position = transform_move.position;
            if (Input.GetMouseButtonUp(0)) {
                piece.gameField.DropPiece();
            }
            //piece.objectSync.TeleportTo(transform_move);
        }
        public void StartGrab(Transform cursor) {
            gameObject.SetActive(true);
            if (Networking.IsOwner(Networking.LocalPlayer, piece.objectSync.gameObject) == false) {
                Networking.SetOwner(Networking.LocalPlayer, piece.objectSync.gameObject);
            }
            Debug.Log("grabb");
            //piece.gameField.GrabPiece(piece);
            this.cursor = cursor;
            isGrab = true;
        }
        public void StopGrab() {
            gameObject.SetActive(false);
            cursor = null;
            isGrab = false;
        }
    }
}

