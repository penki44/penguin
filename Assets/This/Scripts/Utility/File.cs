using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace penguin {
  public static class File {
    public static string DataPath {
      get {
        #if UNITY_EDITOR
        return Path.Combine(Path.GetDirectoryName(Application.dataPath), "Datas");
        #else
        return Application.persistentDataPath;
        #endif
      }
    }

    public static bool IsExistsFile(string path) {
      if (path?.Length > 0) {
        return System.IO.File.Exists(path);
      }
      return false;
    }

    public static bool IsExistsDirectory(string path) {
      if (path?.Length > 0) {
        return Directory.Exists(path);
      }
      return false;
    }

    public static string GetFileName(string path) {
      if (path?.Length > 0) {
        return Path.GetFileName(path);
      }
      return string.Empty;
    }

    public static string[] GetFilePaths(string path, string extension, bool recursively = false) {
      if (path?.Length > 0 && extension?.Length >0) {
        var option = recursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return Directory.GetFiles(path, extension, option);
      }
      return null;
    }

    public static void CreateDirectory(string path) {
      if (!IsExistsDirectory(path)) {
        Directory.CreateDirectory(path);
      }
    }

    public static string GetDirectoryName(string path) {
      if (path?.Length > 0) {
        return Path.GetDirectoryName(path);
      }
      return string.Empty;
    }

    public static string ChangeExtension(string path, string extension) {
      if (path?.Length > 0 && extension?.Length > 0) {
        return Path.ChangeExtension(path, extension);
      }
      return string.Empty;
    }

    public static string UnifyDelimiter(string path) {
      if (path?.Length > 0) {
        return path.Replace("\\\\", "\\").Replace("\\", "/");
      }
      return string.Empty;
    }

    public static string CombinePath(string front, string back) {
      if (front?.Length > 0 && back?.Length > 0) {
        return UnifyDelimiter(Path.Combine(front, back));
      }
      return string.Empty;
    }

    public static byte[] ReadBinary(string path) {
      byte[] data = null;
      if (IsExistsFile(path)) {
        using (var stream = new FileStream(path, FileMode.Open)) {
          using (var reader = new BinaryReader(stream)) {
            data = reader.ReadBytes((int)stream.Length);
          }
        }
      }
      return data;
    }

    public static void WriteBinary(string path, byte[] data) {
      if (path?.Length > 0 && data?.Length > 0) {
        var directory = Path.GetDirectoryName(path);
        if (directory != null && directory != string.Empty) {
          CreateDirectory(directory);
        }
        using (var stream = new FileStream(path, FileMode.Create)) {
          using (var writer = new BinaryWriter(stream)) {
            writer.Write(data);
          }
        }
      }
    }

    public static string ReadText(string path) {
      if (IsExistsFile(path)) {
        using (var stream = new FileStream(path, FileMode.Open)) {
          using (var reader = new StreamReader(stream)) {
            return reader.ReadToEnd();
          }
        }
      }
      return string.Empty;
    }

    public static void WriteText(string path, string data) {
      if (path?.Length > 0 && data?.Length > 0) {
        var directory = Path.GetDirectoryName(path);
        if (directory != null && directory != string.Empty) {
          CreateDirectory(directory);
        }
        using (var stream = new FileStream(path, FileMode.Create)) {
          using (var writer = new StreamWriter(stream)) {
            writer.NewLine = "\n";
            writer.Write(data);
          }
        }
      }
    }
  }
}
