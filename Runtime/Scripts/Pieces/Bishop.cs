using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    public class Bishop : Piece
    {
        public override void CalcAttack(Piece piece, bool isRemove = false)
        {
            base.CalcAttack(piece);
            piece.AddSlidingCellAttack(new Vector2Int(1,1), isRemove);
            piece.AddSlidingCellAttack(new Vector2Int(-1,1), isRemove);
            piece.AddSlidingCellAttack(new Vector2Int(1,-1), isRemove);
            piece.AddSlidingCellAttack(new Vector2Int(-1,-1), isRemove);
        }
    }
}
