﻿using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace penguin {
  public class Window : MonoBehaviour {
    #if !UNITY_EDITOR && UNITY_STANDALONE
    struct POINT {
      public uint x;
      public uint y;
    }

    [DllImport("User32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("User32.dll")] private static extern bool GetCursorPos(out POINT lpPoint);
    [DllImport("User32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("User32.dll")] private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const int GWL_STYLE = -16;
    private const uint WS_POPUP = 0x80000000;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;

    const int HWND_TOPMOST = -1;
    const int HWND_NOTOPMOST = -2;

    private static Window o;
    private IntPtr windowHandle;
    #endif

    public void Awake() {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      o = this;
      windowHandle = GetActiveWindow();
      SetWindowLong(windowHandle, GWL_STYLE, WS_POPUP);
      Show(true);
      #endif
    }

    public static void Show(bool isTopMost) {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      var flags = SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVE | SWP_SHOWWINDOW;
      setWindowPos(0, 0, 0, 0, isTopMost ? HWND_TOPMOST : HWND_NOTOPMOST, flags);
      #endif
    }

    public static void Move(int x, int y) {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      POINT point;
      if (GetCursorPos(out point)) {
        var flags = SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVE;
        setWindowPos((int)(point.x) - x, (int)(point.y) - y, Screen.width, Screen.height, HWND_TOPMOST, flags);
      }
      #endif
    }

    private static void setWindowPos(int x, int y, int w, int h, int z, uint flags) {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      SetWindowPos(o.windowHandle, new IntPtr(z), x, y, w, h, flags);
      #endif
    }
  }
}