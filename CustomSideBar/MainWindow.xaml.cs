using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

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
      Activated+=CSB_WindowsEvent.WindowsActivated;
      Deactivated += CSB_WindowsEvent.WindowsDeactivated;
      StateChanged += CSB_WindowsEvent.WindowsStateChanged;
      DragEnter += CSB_WindowsEvent.DragEnter;
      Closing += CSB_WindowsEvent.WindowClosing;
      Drop += CSB_WindowsEvent.Drop;
      //MouseMove += CheckIsHovered;

      //Setup Statuses
      AllowDrop = true;
      instance = this;

      filedetection = new CSB_FileDropDetection();

      Testing();
    }

    void Testing()
    {
      Utilities.CSB_SaveLoad.SaveDocPanel();
    }


    //void CheckIsHovered(object obj, MouseEventArgs e)
    //{
    //  //isHovered = IsMouseOver;
    //  Console.WriteLine("test");
    //}


    protected override void OnClosing(CancelEventArgs e)
    {
      e.Cancel = false;
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
  }
}
