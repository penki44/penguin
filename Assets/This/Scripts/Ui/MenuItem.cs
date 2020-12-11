using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace penguin {
  public class MenuItem :
    MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler {
    public class Config {
      public string label;
      public System.Action<Config> action;
      public Config[] sub;

      public Config(string label,
                    System.Action<Config> action,
                    Config[] sub = null) {
        this.label = label;
        this.action = action;
        this.sub = sub;
      }
    }

    [SerializeField] private RectTransform rect = default;
    [SerializeField] private Text label = default;
    public float width { get { return rect.sizeDelta.x; } }
    public float height { get { return rect.sizeDelta.y; } }
    private Config config;
    private Menu menu;

    public void Initialize(Config config, Menu menu) {
      this.config = config;
      this.menu = menu;
      label.text = config.label;
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
      if (pointerEventData.pointerId == -1) {
        if (config.action != null) {
          config.action(config);
          Menu.root.Close();
        }
      }
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
      if (config.sub != null) {
        if (menu.child != null && menu.child.open) {
          menu.child.Close();
        }
        menu.child = Menu.Create(gameObject);
        menu.child.Open(config.sub, transform.position);
      }
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
      if (menu.child != null && menu.child.open) {
        menu.child.Close();
      }
    }
  }
}
