using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace Andrey04o.Chess {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Line : UdonSharpBehaviour
    {
        public GameField gameField;
        public Cell[] cells;
        public byte index;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
