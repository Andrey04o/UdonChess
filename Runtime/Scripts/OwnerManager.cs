using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
namespace Andrey04o.Chess {

    public class OwnerManager : UdonSharpBehaviour
    {
        public GameField gameField;
        public void BecameOwner() {
            VRCPlayerApi player = Networking.LocalPlayer;
            Networking.SetOwner(player, gameField.gameObject);
            foreach (Piece piece in gameField.pieces.InTableAll) {
                Networking.SetOwner(player, piece.gameObject);
            }
            foreach (Cell cell in gameField.cells) {
                Networking.SetOwner(player, cell.gameObject);
            }
        }
        public override void Interact()
        {
            base.Interact();
            BecameOwner();
        }
    }

}