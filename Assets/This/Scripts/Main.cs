using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace penguin {
  public class Main : MonoBehaviour {
    public static Main o;
    [SerializeField] private Canvas mainCanvas = default;
    [SerializeField] private Canvas uiCanvas = default;
    private bool isPlay;
    private int playCount;
    private float playTime;
    private Text timeText;
    private string[] texturePaths;
    private List<string> textureList;
    private List<string> textureHistory;
    private CanvasScaler canvasScaler;
    private RawImage image;
    private List<Config.Data> configs;
    private readonly string configFile = "config.txt";

    private void Awake() {
      UnityEngine.Application.targetFrameRate = 60;
      o = this;
      Sound.bgmVolume = 0.5f;
      Sound.seVolume = 0.5f;
      canvasScaler = mainCanvas.GetComponent<CanvasScaler>();
      image = GameObject.FindObjectOfType<RawImage>();
      timeText = GameObject.FindObjectOfType<Text>();
      loadConfig(0);
      Menu.root = Menu.Create(uiCanvas.gameObject);
      TimePicker.o = TimePicker.Create(uiCanvas.gameObject);
    }

    private void OnDestroy() {
      if (Window.IsPopUp()) {
        Window.Overlap();
      }
      saveConfig(0);
    }

    private void Update() {
      if (Input.GetMouseButtonDown(1)) {
        MenuItem.Config[] historyItemConfigs = null;
        if (textureHistory.Count > 0) {
          historyItemConfigs = new MenuItem.Config[textureHistory.Count];
          for(int i = 0; i < historyItemConfigs.Length; i++) {
            var path = textureHistory[historyItemConfigs.Length - i - 1];
            var label = File.GetFileName(path);
            historyItemConfigs[i] = new MenuItem.Config(label, (MenuItem.Config config) => { onHistoryItem(path); });
          }
        }
        MenuItem.Config[] saveDataConfigs = null;
        MenuItem.Config[] loadDataConfigs = null;
        MenuItem.Config[] deleteDataConfigs = null;
        saveDataConfigs = new MenuItem.Config[Mathf.Min(configs.Count, 10)];
        if (configs.Count > 1) {
          loadDataConfigs = new MenuItem.Config[configs.Count - 1];
          deleteDataConfigs = new MenuItem.Config[configs.Count - 1];
        }
        for(int i = 1; i < configs.Count; i++) {
          var n = i;
          var data = configs[i];
          var directory = File.GetFileName(data.folder);
          var time = data.time;
          var num = data.num;
          var label = $"{directory}_{time}x{num}";
          saveDataConfigs[i - 1] = new MenuItem.Config(label, (MenuItem.Config config) => { saveConfig(n); });
          if (configs.Count > 1) {
            loadDataConfigs[i - 1] = new MenuItem.Config(label, (MenuItem.Config config) => { loadConfig(n); });
            deleteDataConfigs[i - 1] = new MenuItem.Config(label, (MenuItem.Config config) => { deleteConfig(n); });
          }
        }
        if (configs.Count <= 10) {
          saveDataConfigs[configs.Count - 1] = new MenuItem.Config("new...", (MenuItem.Config config) => { saveConfig(configs.Count); });
        }
        var settingItemConfigs = new MenuItem.Config[] {
          new MenuItem.Config("読み込み", null, loadDataConfigs),
          new MenuItem.Config("書き出し", null, saveDataConfigs),
          new MenuItem.Config("削除", null, deleteDataConfigs),
          new MenuItem.Config("フォルダ", (MenuItem.Config config) => { onFolder(); }),
          new MenuItem.Config("時間/回数", (MenuItem.Config config) => { onTimePicker(); }),
          new MenuItem.Config("ロック/解除", (MenuItem.Config config) => { onLock(); })
        };
        var menuItemConfigs = new MenuItem.Config[] {
          new MenuItem.Config("開始", (MenuItem.Config config) => { onPlay(); }),
          new MenuItem.Config("停止/再開", (MenuItem.Config config) => { onPauseResume(); }),
          new MenuItem.Config("履歴", null, historyItemConfigs),
          new MenuItem.Config("設定", null, settingItemConfigs),
          new MenuItem.Config("終了", (MenuItem.Config config) => { onQuit(); })
        };
        Menu.root.Open(menuItemConfigs, Input.mousePosition);
      }
      if (Input.GetKey(KeyCode.LeftControl)) {
        if (Input.GetKeyDown(KeyCode.L)) {
          onLock();
        } else if (Input.GetKeyDown(KeyCode.O)) {
          onFolder();
        } else if (Input.GetKeyDown(KeyCode.P)) {
          onPlay();
        } else if (Input.GetKeyDown(KeyCode.Q)) {
          onQuit();
        }
      }
      if (Input.GetKeyDown(KeyCode.Space)) {
        onPauseResume();
      }
    }

    private void FixedUpdate() {
      if (isPlay) {
        playTime -= Time.deltaTime;
        if (playTime <= 0.0f) {
          if (isFinish()) {
            isPlay = false;
            playCount = 0;
            playTime = 0.0f;
            Sound.PlaySe("Se/finish");
            clearImage();
          } else {
            playCount++;
            playTime = configs[0].time;
            Sound.PlaySe("Se/next");
            setNextImage();
          }
        }
        updateTimeUI();
      }
    }

    private void onLock() {
      if (Window.IsPopUp()) {
        Window.Overlap();
      } else {
        Window.PopUp();
      }
    }

    private void onPlay() {
      if (texturePaths.Length <= 0) {
        return;
      }
      isPlay = true;
      playCount = 0;
      playTime = 0.0f;
      textureList = texturePaths.ToList();
      textureHistory = new List<string>();
    }

    private void onPauseResume() {
      isPlay = !isPlay;
    }

    private void onHistoryItem(string path) {
      isPlay = false;
      playCount = 0;
      playTime = 0.0f;
      updateTimeUI();
      changeImage(path);
    }

    private void updateTimeUI() {
      timeText.text = $"{playTime:F1}";
    }

    private void onFolder() {
      var browser = new FolderBrowserDialog();
      browser.RootFolder = Environment.SpecialFolder.UserProfile;
      var isPopUp = Window.IsPopUp();
      if (isPopUp) {
        Window.Overlap();
      }
      if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        var folderPath = File.UnifyDelimiter(browser.SelectedPath);
        configs[0].folder = folderPath;
        texturePaths = getTexturePaths(folderPath);
        textureList = texturePaths.ToList();
        saveConfig(0);
      }
      if (isPopUp) {
        Window.PopUp();
      }
    }

    private void onTimePicker() {
      TimePicker.o.Open(configs[0].time, configs[0].num);
    }

    private string[] getTexturePaths(string folderPath) {
      var pngs = File.GetFilePaths(folderPath, "*.png", true);
      var jpgs = File.GetFilePaths(folderPath, "*.jpg", true);
      return pngs.Concat(jpgs).Select(f=>File.UnifyDelimiter(f)).ToArray();
    }

    private void onQuit() {
      UnityEngine.Application.Quit();
    }

    private bool isFinish() {
      return textureList.Count <= 0 || playCount >= configs[0].num;
    }

    private void clearImage() {
      image.texture = null;
      image.color = Color.clear;
    }

    private void setNextImage() {
      var n = UnityEngine.Random.Range(0, textureList.Count);
      changeImage(textureList[n]);
      textureList.RemoveAt(n);
    }

    private void changeImage(string path) {
      var texture = new Texture2D(2, 2);
      var bin = File.ReadBinary(path);
      texture.LoadImage(bin);
      image.texture = texture;
      image.color = Color.white;
      image.SetNativeSize();
      var w = canvasScaler.referenceResolution.x / texture.width;
      var h = canvasScaler.referenceResolution.y / texture.height;
      var scale = Mathf.Min(w, h);
      image.transform.localScale = new Vector3(scale, scale, 1);
      if (textureHistory.Contains(path)) {
        textureHistory.Remove(path);
      }
      if (textureHistory.Count >= 10) {
        textureHistory.RemoveAt(0);
      }
      textureHistory.Add(path);
      configs[0].history = textureHistory.ToArray();
    }

    private void saveConfig(int n) {
      if (n >= configs.Count) {
        configs.Add(new Config.Data(configs[0]));
      } else {
        configs[n] = new Config.Data(configs[0]);
      }
      var path = File.DataPath;
      path = File.CombinePath(path, configFile);
      var version = UnityEngine.Application.version;
      var text = Serializer.SerializeTextArray(version, configs.ToArray());
      File.WriteText(path, text);
    }

    private void deleteConfig(int n) {
      configs.RemoveAt(n);
    }

    private void loadConfig(int n) {
      string version;
      Config.Data[] datas;
      if (configs == null) {
        texturePaths = new string[] {};
        textureList = new List<string>();
        textureHistory = new List<string>();
        var path = File.DataPath;
        path = File.CombinePath(path, configFile);
        if (File.IsExistsFile(path)) {
          var text = File.ReadText(path);
          (version, datas) = Serializer.DeserializeTextArray<Config.Data>(text);
          if (version == default /* for version 1.0.3 or earlier */) {
            datas = new Config.Data[] { Serializer.DeserializeText<Config.Data>(text) };
          }
          configs = datas.ToList();
        } else {
          var assets = Resources.Load<TextAsset>("Levels/ConfigTable");
          (version, datas) = Serializer.DeserializeTextArray<Config.Data>(assets.text);
          configs = datas.ToList();
        }
      }
      var config = configs[n];
      if (File.IsExistsDirectory(config.folder)) {
        texturePaths = getTexturePaths(config.folder);
        textureList = texturePaths.ToList();
      }
      foreach(var historyPath in config.history) {
        if (File.IsExistsFile(historyPath)) {
          textureHistory.Add(historyPath);
        }
      }
      configs[0] = new Config.Data(config);
    }

    public static void ApplyTimeConfig(int time, int num) {
      o.configs[0].time = time;
      o.configs[0].num = num;
      o.saveConfig(0);
    }
  }
}
