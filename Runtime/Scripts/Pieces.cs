using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Pieces : UdonSharpBehaviour
    {
        public Bishop bishop;
        public King king;
        public Knight knight;
        public Pawn pawn;
        public Queen queen;
        public Rook rook;
        public Material materialWhite;
        public Material materialBlack;
        public Piece[] InTableAll;
        public Piece GetPiece(byte index) {
            switch (index)
            {
                case 1:
                    return bishop;
                case 2:
                    return king;
                case 3:
                    return knight;
                case 4:
                    return pawn;
                case 5:
                    return queen;
                case 6:
                    return rook;
                default:
                    return null;
            }
        }
        public Piece[] allKings;
    }
}
