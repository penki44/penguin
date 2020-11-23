using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace penguin {
  public class Back : MonoBehaviour, IPointerClickHandler, IDragHandler {
    public void OnPointerClick(PointerEventData e) {
      if (e.pointerId == -1) {
        Menu.root.Close();
      }
    }

    public void OnDrag(PointerEventData e) {
      if (e.pointerId == -3) {
        if (Input.GetKey(KeyCode.LeftControl)) {
          int x = (int)e.pressPosition.x;
          int y = Screen.height - (int)e.pressPosition.y;
          Window.Move(x, y);
        }
      }
    }
  }
}
