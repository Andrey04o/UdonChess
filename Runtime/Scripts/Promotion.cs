using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDK3.Dynamics.Constraint.Components;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Promotion : UdonSharpBehaviour
    {
        public Piece piece;
        public Quaternion rotation;
        public VRCRotationConstraint rotationConstraint;
        public void SetPromotion(byte id) {
            piece.ConfirmPromotion(id);
        }
        public void CancelPromotion() {
            piece.CancelPromotion();
        }
        public void ChangeRotation(Quaternion rotation) {
            rotationConstraint.ActivateConstraint();
        }
        public void ResetRotation() {
            //rotationConstraint.ZeroConstraint();
            rotationConstraint.IsActive = false;
            transform.localRotation = rotation;
        }
        
    }
}