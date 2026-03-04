using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PromotionButton : UdonSharpBehaviour
    {
        public Promotion promotion;
        public byte id;
        public void SetPromotion() {
            promotion.SetPromotion(id);
        }
    }
}
