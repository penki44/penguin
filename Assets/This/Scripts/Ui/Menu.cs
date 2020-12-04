using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace penguin {
  public class Menu : MonoBehaviour {
    [SerializeField] private MenuItem itemTemplate = default;
    public static Menu root { get; set; }
    public Menu child { get; set; }
    public bool open { get; private set; }
    private List<MenuItem> items;

    public void Update() {
      if (Input.GetMouseButtonDown(0) &&
          !EventSystem.current.IsPointerOverGameObject()) {
        Menu.root.Close();
      }
    }

    public static Menu Create(GameObject parent) {
      var menu = Instantiate(Resources.Load<Menu>("Prefabs/Menu"));
      menu.open = false;
      menu.items = new List<MenuItem>();
      menu.transform.SetParent(parent.transform, false);
      return menu;
    }

    private void Destroy() {
      Destroy(this.gameObject);
    }

    public void Open(MenuItem.Config[] configs, Vector2 position) {
      if (open) {
        Close();
      }
      SetItems(configs);
      if (this == root) {
        position.x += items.First().width * 0.5f;
      } else {
        position.x += items.First().width;
      }
      position.y -= items.First().height * (configs.Length - 1) * 0.5f;
      transform.position = position;
      gameObject.SetActive(true);
      open = true;
    }

    private void SetItems(MenuItem.Config[] configs) {
      foreach(var config in configs) {
        var menuItem = Instantiate(itemTemplate);
        menuItem.transform.SetParent(this.transform, false);
        menuItem.gameObject.SetActive(true);
        menuItem.Initialize(config, this);
        items.Add(menuItem);
      }
    }

    public void Close() {
      if (child != null) {
        child.Close();
        child = null;
      }

      foreach(var item in items) {
        GameObject.Destroy(item.gameObject);
      }
      items.Clear();

      if (this == root) {
        gameObject.SetActive(false);
      } else {
        Destroy(gameObject);
      }
      open = false;
    }
  }
}
