using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UdonSharp;
namespace Andrey04o.RaycastButton {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ChangeValueSlider : UdonSharpBehaviour
    {
        public Slider slider;
        public Scrollbar scrollbar;
        public float changeValue = 5f;
        public void Press() {
            if (slider != null) {
                slider.value += changeValue;
            }
            if (scrollbar != null) {
                scrollbar.value += changeValue;
            }
        }
    }
}
