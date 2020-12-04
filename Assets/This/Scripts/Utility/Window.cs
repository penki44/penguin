using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace penguin {
  public class Window : MonoBehaviour {
    #if !UNITY_EDITOR && UNITY_STANDALONE
    [DllImport("User32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("User32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("User32.dll")] private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const int GWL_STYLE = -16;
    private const uint WS_OVERLAPPED_WINDOW = 0x00CF0000;
    private const uint WS_POPUP_WINDOW = 0x80880000;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;

    const int HWND_TOPMOST = -1;
    const int HWND_NOTOPMOST = -2;

    private static Window o;
    private IntPtr windowHandle;
    private bool isPopUp;
    #endif

    public void Awake() {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      o = this;
      windowHandle = GetActiveWindow();
      Overlap();
      #endif
    }

    public static void PopUp() {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      SetWindowLong(o.windowHandle, GWL_STYLE, WS_POPUP_WINDOW);
      var flags = SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVE | SWP_SHOWWINDOW;
      setWindowPos(0, 0, 0, 0, HWND_TOPMOST, flags);
      o.isPopUp = true;
      #endif
    }

    public static void Overlap() {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      SetWindowLong(o.windowHandle, GWL_STYLE, WS_OVERLAPPED_WINDOW);
      var flags = SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVE | SWP_SHOWWINDOW;
      setWindowPos(0, 0, 0, 0, HWND_NOTOPMOST, flags);
      o.isPopUp = false;
      #endif
    }

    public static bool IsPopUp() {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      return o.isPopUp;
      #else
      return false;
      #endif
    }

    private static void setWindowPos(int x, int y, int w, int h, int z, uint flags) {
      #if !UNITY_EDITOR && UNITY_STANDALONE
      SetWindowPos(o.windowHandle, new IntPtr(z), x, y, w, h, flags);
      #endif
    }
  }
}
