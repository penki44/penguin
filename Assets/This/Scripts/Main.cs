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
    public const int Resolution = 512;
    public static Main o;
    private bool isPlay;
    private int playCount;
    private float playTime;
    private Text timeText;
    private string[] texturePaths;
    private List<string> textureList;
    private List<string> textureHistory;
    private RawImage image;
    private Config.Data config;
    private readonly string configFile = "config.txt";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeFirstSceneLoad() {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      UnityEngine.Application.targetFrameRate = 60;
      UnityEngine.Screen.SetResolution(Resolution,
                                       Resolution,
                                       UnityEngine.Screen.fullScreen,
                                       UnityEngine.Application.targetFrameRate);
      #endif
    }

    void Awake() {
      o = this;
      Sound.bgmVolume = 0.5f;
      Sound.seVolume = 0.5f;
      var canvas = GameObject.FindObjectOfType<Canvas>();
      image = GameObject.FindObjectOfType<RawImage>();
      timeText = GameObject.FindObjectOfType<Text>();
      loadConfig();
      Menu.root = Menu.Create(canvas.gameObject);
      TimePicker.o = TimePicker.Create(canvas.gameObject);
    }

    private void OnDestroy() {
      saveConfig();
    }

    void FixedUpdate() {
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

        var settingItemConfigs = new MenuItem.Config[] {
          new MenuItem.Config("フォルダ", (MenuItem.Config config) => { onFolder(); }),
          new MenuItem.Config("時間/回数", (MenuItem.Config config) => { onTimePicker(); })
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

      if (isPlay) {
        playTime -= 1.0f / 60.0f;
        if (playTime <= 0.0f) {
          if (isFinish()) {
            isPlay = false;
            playCount = 0;
            playTime = 0.0f;
            Sound.PlaySe("Se/finish");
            clearImage();
          } else {
            playCount++;
            playTime = config.time;
            Sound.PlaySe("Se/next");
            setNextImage();
          }
        }
        updateTimeUI();
      }
      if (Input.GetKey(KeyCode.LeftControl)) {
        if (Input.GetKeyDown(KeyCode.O)) {
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
      Window.Show(false);
      if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        var folderPath = File.UnifyDelimiter(browser.SelectedPath);
        config.folder = folderPath;
        texturePaths = getTexturePaths(folderPath);
        textureList = texturePaths.ToList();
        saveConfig();
      }
      Window.Show(true);
    }

    private void onTimePicker() {
      TimePicker.o.Open(config.time, config.num);
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
      return textureList.Count <= 0 || playCount >= config.num;
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
      var w = (float)UnityEngine.Screen.width / texture.width;
      var h = (float)UnityEngine.Screen.height / texture.height;
      var scale = Mathf.Min(w, h);
      image.transform.localScale = new Vector3(scale, scale, 1);
      if (textureHistory.Contains(path)) {
        textureHistory.Remove(path);
      }
      if (textureHistory.Count >= 10) {
        textureHistory.RemoveAt(0);
      }
      textureHistory.Add(path);
      config.history = textureHistory.ToArray();
    }

    private void saveConfig() {
      var path = File.DataPath;
      path = File.CombinePath(path, configFile);
      var text = Serializer.SerializeText(config);
      File.WriteText(path, text);
    }

    private void loadConfig() {
      texturePaths = new string[] {};
      textureList = new List<string>();
      textureHistory = new List<string>();
      var path = File.DataPath;
      path = File.CombinePath(path, configFile);
      if (File.IsExistsFile(path)) {
        var text = File.ReadText(path);
        config = Serializer.DeserializeText<Config.Data>(text);
        if (File.IsExistsDirectory(config.folder)) {
          texturePaths = getTexturePaths(config.folder);
          textureList = texturePaths.ToList();
        }
        foreach(var historyPath in config.history) {
          if (File.IsExistsFile(historyPath)) {
            textureHistory.Add(historyPath);
          }
        }
      } else {
        var assets = Resources.Load<TextAsset>("Levels/ConfigTable");
        config = Serializer.DeserializeTextArray<Config.Data>(assets.text)[0];
      }
    }

    public static void ApplyTimeConfig(int time, int num) {
      o.config.time = time;
      o.config.num = num;
      o.saveConfig();
    }
  }
}
