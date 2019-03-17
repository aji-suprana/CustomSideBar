using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSideBar.Serializer;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;

namespace CustomSideBar.UserControls
{
  class UC_Collections
  {
    public static void UnitTest()
    {
      //UnitTest_DocItems();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // UCDocItems
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static void UnitTest_DocItems()
    {
      UC_Collections.DocItems.Clear();
      string _name = "CustomSideBar";
      string _fName = "CustomSideBar.winDirectory";
      string _path = @"C:\Users\Aji Suprana\Documents\Projects\Applications\20180314_WPFCustomSideBar\CustomSideBar";
      UC_DocItem newDocPanel = UC_DocItem.Add(_name,_path,_fName,FileFormats.folder);

      _name = "Test123";
      _fName = "Test123.winDirectory";
      _path = @"C:\Users\Aji Suprana\Documents\Projects\Applications\20180314_WPFCustomSideBar\Test123";
      UC_DocItem newDocPanel1 = UC_DocItem.Add(_name, _path, _fName, FileFormats.folder);

      //newDocPanel.Serialize();

      JObject testResult = UC_Collections.SerializeDocItems();
      Console.WriteLine("[UC_Collections][UnitTest_DocItems]");
      Console.WriteLine(testResult.ToString());

    }
    public static JObject SerializeDocItems()
    {
      JObject Root = new JObject();

      JArray docItemsArray = new JArray();
      foreach(var docItem in DocItems)
      {
        JObject docItemData = docItem.Serialize();
        docItemsArray.Add(docItemData);
      }

      Root.Add(new JProperty("docItems", docItemsArray));
      return Root;
    }

    public static void DeserializeDocItems(JObject obj)
    {
      Console.WriteLine("[UC_Collection]DeserializeDocItems");
      DocItems.Clear();
      if(obj["docItems"] == null)
        return;
      IList<JToken> docItemsObject =  obj["docItems"].Children().ToList();
      JArray test = new JArray(obj["docitems"]);
      foreach(var docItemObj in docItemsObject)
      {
        string content = docItemObj.ToString();
        Console.WriteLine(content);
        UC_DocItem newItem = UC_DocItem.Add(docItemObj);
        //JObject temp = docItemObj;
        //Console.WriteLine(docItemObj["DocName"].ToString());
      }
      UpdateIDs();
    }

    public static void DeleteDocItem(UC_DocItem removeThis)
    {
      ((StackPanel)removeThis.Parent).Children.Remove(removeThis);
      DocItems.RemoveAt(removeThis.Id);
      UpdateIDs();
    }

    public static void UpdateIDs()
    {
      Console.WriteLine("[UC_Collection]UpdateIDs");

      int i = 0;
      foreach (var it in UC_Collections.DocItems)
      {
        it.Id = i;
        i++;
      }
    }


    public static List<UC_DocItem> DocItems = new List<UC_DocItem>();

  }
}
