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
      public T[] contents;

      public Wrapper() {

      }

      public Wrapper(T[] contents) {
        this.contents = contents;
      }

      public Wrapper(SerializationInfo info, StreamingContext context) {
        this.contents = (T[])info.GetValue("contents", typeof(T[]));
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("contents", this.contents);
      }
    }

    public static byte[] SerializeBinary<T>(T obj) {
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

    public static byte[] SerializeBinaryArray<T>(T[] objs) {
      if (objs?.Length > 0) {
        return SerializeBinary(new Wrapper<T>(objs));
      }
      return null;
    }

    public static T DeserializeBinary<T>(byte[] data) {
      T obj = default;
      if (data?.Length > 0) {
        using (var stream = new MemoryStream(data)) {
          var formatter = new BinaryFormatter();
          obj = (T)formatter.Deserialize(stream);
        }
      }
      return obj;
    }

    public static T[] DeserializeBinaryArray<T>(byte[] data) {
      if (data?.Length > 0) {
        return DeserializeBinary<Wrapper<T>>(data).contents;
      }
      return null;
    }

    public static string SerializeText<T>(T obj) {
      if (obj != null) {
        return JsonUtility.ToJson(obj);
      }
      return string.Empty;
    }

    public static string SerializeTextArray<T>(T[] objs) {
      if (objs?.Length > 0) {
        return JsonUtility.ToJson(new Wrapper<T>(objs));
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

    public static T[] DeserializeTextArray<T>(string data) {
      T[] objs = default;
      if (data?.Length > 0) {
        objs = JsonUtility.FromJson<Wrapper<T>>(data).contents;
      }
      return objs;
    }
  }
}
