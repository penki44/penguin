using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace penguin {
  public class Sound : MonoBehaviour {
    private static Sound o;
    [SerializeField] private AudioSource bgmSource = default;
    [SerializeField] private AudioSource seSource = default;

    public static float bgmVolume {
      get { return o.bgmSource.volume; }
      set { o.bgmSource.volume = value; }
    }

    public static bool bgmMute {
      get { return o.bgmSource.mute; }
      set { o.bgmSource.mute = value; }
    }

    public static float seVolume {
      get { return o.seSource.volume; }
      set { o.seSource.volume = value; }
    }

    public static bool seMute {
      get { return o.seSource.mute; }
      set { o.seSource.mute = value; }
    }

    public void Awake() {
      o = this;
    }

    public static void PlayBgm(string path, bool loop) {
      if (path?.Length > 0) {
        o.bgmSource.clip = Resources.Load<AudioClip>(path);
        o.bgmSource.loop = loop;
        o.bgmSource.Play();
      }
    }

    public static void StopBgm() {
      o.bgmSource.Stop();
    }

    public static void PauseBgm() {
      o.bgmSource.Pause();
    }

    public static void ResumeBgm() {
      o.bgmSource.UnPause();
    }

    public static void PlaySe(string path) {
      if (path?.Length > 0) {
        o.seSource.clip = Resources.Load<AudioClip>(path);
        o.seSource.PlayOneShot(o.seSource.clip, o.seSource.volume);
      }
    }
  }
}
