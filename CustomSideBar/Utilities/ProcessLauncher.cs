using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace CustomSideBar.Utilities
{
  public class ProcessManager
  {
    Dictionary<string, Process> processes;

    public static void AddProcess(string key, string path,FileFormats format)
    {
      Process newProcess = new Process();
      switch(format)
      {
        case FileFormats.folder:
          newProcess.StartInfo.FileName = Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe";
          newProcess.StartInfo.Arguments = path;
          break;
        case FileFormats.file:
          newProcess.StartInfo.FileName = path;
          break;
      }
      newProcess.Start();
    }
  }

  public class ProcessLauncher
  {

    
  }
}
