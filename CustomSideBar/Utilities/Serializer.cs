﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomSideBar;


namespace CustomSideBar.Serializer
{
  public interface ISerializableObject
  {
    JObject Serialize();
    void Deserialize(JToken token);
  }

  class CSB_SaveLoad
  {
    public static void CSBSaveLoad_UnitTest()
    {
      LoadDocItems();
    }
    public static void SaveDocItems()
    {
      string AppDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string CSBConfigPath = @"\CustomSideBar\";
      string SavingPath = AppDataPath + CSBConfigPath + "CSB_DocItems.sav";
      Console.WriteLine("[CSB_SaveLoad]:Saving to {0}", SavingPath);

      string JsonString = UserControls.UC_Collections.SerializeDocItems().ToString();
      // WriteAllText creates a file, writes the specified string to the file,
      // and then closes the file.    You do NOT need to call Flush() or Close().
      System.IO.FileInfo file = new System.IO.FileInfo(SavingPath);
      file.Directory.Create(); // If the directory already exists, this method does nothing.
      System.IO.File.WriteAllText(SavingPath, JsonString);
    }

    public static void LoadDocItems()
    {
      string AppDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string CSBConfigPath = @"\CustomSideBar\";
      string SavingPath = AppDataPath + CSBConfigPath + "CSB_DocItems.sav";
      Console.WriteLine("[CSB_SaveLoad]:Loading from {0}", SavingPath);

      if (System.IO.File.Exists(SavingPath))
      {
        string JsonString = System.IO.File.ReadAllText(SavingPath);
        JObject JsonObject = JObject.Parse(JsonString);

        UserControls.UC_Collections.DeserializeDocItems(JsonObject);
      }

    }
  }

  namespace Serializer
  {
    class CSB_Serializer
    {
    }

    class CSB_Deserializer
    {
    }
  }

}
