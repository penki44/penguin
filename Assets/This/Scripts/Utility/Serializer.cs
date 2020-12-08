using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace penguin {
  public static class Serializer {
    [System.Serializable]
    private class Wrapper<T> : ISerializable {
      public string version;
      public T[] contents;

      public Wrapper() {

      }

      public Wrapper(string version, T[] contents) {
        this.version = version;
        this.contents = contents;
      }

      public Wrapper(SerializationInfo info, StreamingContext context) {
        this.version = (string)info.GetValue("version", typeof(string));
        this.contents = (T[])info.GetValue("contents", typeof(T[]));
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("version", this.version);
        info.AddValue("contents", this.contents);
      }
    }

    private static byte[] serializeBinary<T>(T obj) {
      byte[] data = null;
      if (obj != null) {
        using (var stream = new MemoryStream()) {
          var formatter = new BinaryFormatter();
          formatter.Serialize(stream, obj);
          data = stream.ToArray();
        }
      }
      return data;
    }

    public static byte[] SerializeBinary<T>(string version, T obj) {
      return SerializeBinaryArray(version, new T[] { obj });
    }

    public static byte[] SerializeBinaryArray<T>(string version, T[] objs) {
      if (objs?.Length > 0) {
        return serializeBinary(new Wrapper<T>(version, objs));
      }
      return null;
    }

    public static T deserializeBinary<T>(byte[] data) {
      T obj = default;
      if (data?.Length > 0) {
        using (var stream = new MemoryStream(data)) {
          var formatter = new BinaryFormatter();
          obj = (T)formatter.Deserialize(stream);
        }
      }
      return obj;
    }

    public static (string version, T obj) DeserializeBinary<T>(byte[] data) {
      var (version, objs) = DeserializeBinaryArray<T>(data);
      return (version, objs[0]);
    }

    public static (string version, T[] objs) DeserializeBinaryArray<T>(byte[] data) {
      if (data?.Length > 0) {
        var wrapper = deserializeBinary<Wrapper<T>>(data);
        return (wrapper.version, wrapper.contents);
      }
      return default;
    }

    public static string SerializeText<T>(T obj) {
      if (obj != null) {
        return JsonUtility.ToJson(obj);
      }
      return string.Empty;
    }

    public static string SerializeTextSingle<T>(string version, T obj) {
      return SerializeTextArray(version, new T[] { obj });
    }

    public static string SerializeTextArray<T>(string version, T[] objs) {
      if (objs?.Length > 0) {
        return SerializeText(new Wrapper<T>(version, objs));
      }
      return string.Empty;
    }

    public static T DeserializeText<T>(string data) {
      T obj = default;
      if (data?.Length > 0) {
        obj = JsonUtility.FromJson<T>(data);
      }
      return obj;
    }

    public static (string version, T obj) DeserializeTextSingle<T>(string data) {
      var (version, objs) = DeserializeTextArray<T>(data);
      return (version, objs[0]);
    }

    public static (string version, T[] objs) DeserializeTextArray<T>(string data) {
      if (data?.Length > 0) {
        var wrapper = DeserializeText<Wrapper<T>>(data);
        return (wrapper.version, wrapper.contents);
      }
      return default;
    }
  }
}
