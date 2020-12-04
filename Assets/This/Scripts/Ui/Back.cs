using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace penguin {
  public class Back : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData e) {
      if (e.pointerId == -1) {
        Menu.root.Close();
      }
    }
  }
}
