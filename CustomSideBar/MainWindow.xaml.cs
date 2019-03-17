using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json;
using CustomSideBar.UserControls;
namespace CustomSideBar
{

  public partial class MainWindow : Window
  {
    public static MainWindow instance = null;
    public CSB_FileDropDetection filedetection;

    public MainWindow()
    {
      Console.WriteLine("Windows Initialized");
      InitializeComponent();

      //Event Listener
      Activated += WindowsActivated;
      Deactivated += WindowsDeactivated;
      StateChanged += WindowsStateChanged;
      DragEnter += DragEnterHandler;
      Closing += WindowClosing;
      Drop += DropHandler;
      Closed += WindowClosed;
      KeyDown += wnd_KeyDown;

      //MouseMove += CheckIsHovered;

      //Setup Statuses
      AllowDrop = true;
      instance = this;
      ShowInTaskbar = false;

      filedetection = new CSB_FileDropDetection();
      UnitTesting();
    }
    ~MainWindow()
    {
      Activated -= WindowsActivated;
      Deactivated -= WindowsDeactivated;
      StateChanged -= WindowsStateChanged;
      DragEnter -= DragEnterHandler;
      Closing -= WindowClosing;
      Drop -= DropHandler;
      Closed -= WindowClosed;

    }
    void UnitTesting()
    {
      //Utilities.CSB_SaveLoad.SaveDocPanel();
      UC_Collections.UnitTest();
      Serializer.CSB_SaveLoad.CSBSaveLoad_UnitTest();
    }


    //void CheckIsHovered(object obj, MouseEventArgs e)
    //{
    //  //isHovered = IsMouseOver;
    //  Console.WriteLine("test");
    //}


    protected override void OnClosing(CancelEventArgs e)
    {
      WpfAppBar.AppBarFunctions.SetAppBar(this, WpfAppBar.ABEdge.None);
      base.OnClosing(e);
      //Closing(this, e);
      //Do whatever you want here..
    }

    private void CloseClicked(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("Closing");
      //WpfAppBar.AppBarFunctions.SetAppBar(this, WpfAppBar.ABEdge.None);
      //e.Handled = false;
      Close();
    }

    //Event handler when windows is activated, binded in MainWindows.xaml.cs
    public static void WindowsActivated(object obj, EventArgs args)
    {
      Console.WriteLine("[MainWindow] WindowsActinvated()");
      Window window = (Window)obj;
      window.Topmost = true;
      WpfAppBar.AppBarFunctions.SetAppBar((Window)obj, WpfAppBar.ABEdge.Left);

    }

    //Event handler when windows is deactivated, binded in MainWindows.xaml.cs
    public static void WindowsDeactivated(object obj, EventArgs args)
    {
      Console.WriteLine("[MainWindow] WindowsDeactivated()");
    }

    //Event handler when windows state is changed, binded in MainWindows.xaml.cs
    private void WindowsStateChanged(object obj, EventArgs args)
    {
      Console.WriteLine("[MainWindow] WindowsStateChanged()");
      this.Hide();
    }

    //Drag Event handler when windows state is changed, binded in MainWindows.xaml.cs
    public static void DragEnterHandler(object obj, DragEventArgs e)
    {
      Console.WriteLine("[MainWindow] DragEnter()");
      e.Effects = DragDropEffects.Move;
    }

    //Drag Event handler when windows state is changed, binded in MainWindows.xaml.cs
    public static void DropHandler(object obj, DragEventArgs e)
    {
      Console.WriteLine("[MainWindow] Drop()");
    }

    public static void WindowClosing(object obj, EventArgs e)
    {
      Console.WriteLine("[MainWindow] WindowClosing()");

      WpfAppBar.AppBarFunctions.SetAppBar((Window)obj, WpfAppBar.ABEdge.None);
    }
    public static void WindowClosed(object obj, EventArgs e)
    {
      Console.WriteLine("[MainWindow] WindowClosed()");

    }


    void wnd_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.System && e.SystemKey == Key.F4)
      {
        e.Handled = true;
      }
    }

    private void UC_HamburgerPanel_Loaded(object sender, RoutedEventArgs e)
    {

    }
  }
}
