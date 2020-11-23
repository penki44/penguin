using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class Config {
  [System.Serializable]
  public class Data : ISerializable {
    public string folder;
    public int time;
    public int num;
    public string[] history;

    public Data() {

    }

    public Data(Data data) {
      this.folder = data.folder;
      this.time = data.time;
      this.num = data.num;
      this.history = data.history;
    }
 
    public Data(string folder, int time, int num, string[] history) {
      this.folder = folder;
      this.time = time;
      this.num = num;
      this.history = history;
    }
 
    public Data(SerializationInfo info, StreamingContext context) {
      this.folder = (string)info.GetValue("folder", typeof(string));
      this.time = (int)info.GetValue("time", typeof(int));
      this.num = (int)info.GetValue("num", typeof(int));
      this.history = (string[])info.GetValue("history", typeof(string[]));
    }
 
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("folder", this.folder);
      info.AddValue("time", this.time);
      info.AddValue("num", this.num);
      info.AddValue("history", this.history);
    }
  }
}
