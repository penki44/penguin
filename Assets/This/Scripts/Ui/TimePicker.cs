using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace penguin {
  public class TimePicker : MonoBehaviour {
    public enum Section {
      Hours,
      Minutes,
      Seconds,
      NumberOfTimes
    };
    public static TimePicker o;
    [SerializeField] private Dropdown[] dropdowns = default;

    public static TimePicker Create(GameObject parent) {
      if (o != null) {
        return o;
      }
      var picker = Instantiate(Resources.Load<TimePicker>("Prefabs/TimePicker"));
      picker.transform.SetParent(parent.transform);
      var dropdown = picker.dropdowns[(int)Section.Hours];
      for(int i = 0; i < 24; i++) {
        dropdown.options.Add(new Dropdown.OptionData($"{i}"));
      }
      dropdown = picker.dropdowns[(int)Section.Minutes];
      for(int i = 0; i < 60; i++) {
        dropdown.options.Add(new Dropdown.OptionData($"{i}"));
      }
      dropdown = picker.dropdowns[(int)Section.Seconds];
      for(int i = 0; i < 60; i++) {
        dropdown.options.Add(new Dropdown.OptionData($"{i}"));
      }
      dropdown = picker.dropdowns[(int)Section.NumberOfTimes];
      for(int i = 1; i <= 99; i++) {
        dropdown.options.Add(new Dropdown.OptionData($"{i}"));
      }
      return picker;
    }

    public void Open(int time, int num) {
      var dropdown = dropdowns[(int)Section.Hours];
      dropdown.value = time / 3600;
      time %= 3600;
      dropdown = dropdowns[(int)Section.Minutes];
      dropdown.value = time / 60;
      time %= 60;
      dropdown = dropdowns[(int)Section.Seconds];
      dropdown.value = time;
      dropdown = dropdowns[(int)Section.NumberOfTimes];
      dropdown.value = num - 1;
      dropdown.RefreshShownValue();
      gameObject.SetActive(true);
    }

    public void Close() {
      gameObject.SetActive(false);
    }

    public void OnApply() {
      var time = 0;
      var dropdown = dropdowns[(int)Section.Hours];
      time += dropdown.value * 3600;
      dropdown = dropdowns[(int)Section.Minutes];
      time += dropdown.value * 60;
      dropdown = dropdowns[(int)Section.Seconds];
      time += dropdown.value;
      dropdown = dropdowns[(int)Section.NumberOfTimes];
      var num = dropdown.value + 1;
      Main.ApplyTimeConfig(time, num);
      Close();
    }

    public void OnCancel() {
      Close();
    }
  }
}
