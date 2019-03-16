using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSideBar.Utilities
{
  class CSB_SaveLoad
  {
    

    public static void SaveDocPanel()
    {
      string AppDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string CSBConfigPath = @"\CustomSideBar\";
      Console.WriteLine("[CSB_SaveLoad]:{0}", AppDataPath+CSBConfigPath);
    }

    public static void LoadDocPanel()
    {

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
