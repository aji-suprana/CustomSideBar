using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSideBar.Serializer;
using Newtonsoft.Json.Linq;
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
      UC_DocItem newDocPanel = new UC_DocItem();
      newDocPanel.DocName = "CustomSideBar";
      newDocPanel.FullName = "CustomSideBar.winDirectory";
      newDocPanel.DocPath = @"C:\Users\Aji Suprana\Documents\Projects\Applications\20180314_WPFCustomSideBar\CustomSideBar";

      UC_DocItem newDocPanel1 = new UC_DocItem(); ;
      newDocPanel1.DocName = "Test123";
      newDocPanel1.FullName = "Test123.winDirectory";
      newDocPanel1.DocPath = @"C:\Users\Aji Suprana\Documents\Projects\Applications\20180314_WPFCustomSideBar\Test123";
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
    public static List<UC_DocItem> DocItems = new List<UC_DocItem>();

  }
}
