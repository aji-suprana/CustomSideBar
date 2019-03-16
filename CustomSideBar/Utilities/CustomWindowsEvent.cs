using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace CustomSideBar
{
  class CSB_WindowsEvent
  {
    //Event handler when windows is activated, binded in MainWindows.xaml.cs
    public static void WindowsActivated(object obj, EventArgs args)
    {
      Console.WriteLine("[CSB_WindowsEvent] WindowsActinvated()");
      Window window = (Window)obj;
      window.Topmost = true;
      WpfAppBar.AppBarFunctions.SetAppBar((Window)obj, WpfAppBar.ABEdge.Left);

    }

    //Event handler when windows is deactivated, binded in MainWindows.xaml.cs
    public static void WindowsDeactivated(object obj, EventArgs args)
    {
      Console.WriteLine("[CSB_WindowsEvent] WindowsDeactivated()");
    }

    //Event handler when windows state is changed, binded in MainWindows.xaml.cs
    public static void WindowsStateChanged(object obj, EventArgs args)
    {
      Console.WriteLine("[CSB_WindowsEvent] WindowsStateChanged()");
    }

    //Drag Event handler when windows state is changed, binded in MainWindows.xaml.cs
    public static void DragEnter(object obj, DragEventArgs e)
    {
      Console.WriteLine("[CSB_WindowsEvent] DragEnter()");
      e.Effects = DragDropEffects.Move;
    }

    //Drag Event handler when windows state is changed, binded in MainWindows.xaml.cs
    public static void Drop(object obj, DragEventArgs e)
    {
      Console.WriteLine("[CSB_WindowsEvent] Drop()");
    }

    public static void WindowClosing(object obj, EventArgs e)
    {
      Console.WriteLine("[CSB_WindowsEvent] WindowClosing()");

      WpfAppBar.AppBarFunctions.SetAppBar((Window)obj, WpfAppBar.ABEdge.None);
    }

  }
}
