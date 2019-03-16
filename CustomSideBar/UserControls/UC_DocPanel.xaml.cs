using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomSideBar
{

  public partial class UC_DocPanel : UserControl
  {
    //delegates
    public delegate void DocPanelClicked(int DocPanelID);

    //events
    public static DocPanelClicked docPanelClicked = null;

    //selection states
    public static List< UC_DocPanel> docPanelObjects = new List< UC_DocPanel>();
    private static int selected = -1;
    public static int hoveredID = -1;

    //Property
    public string DocName { get; set; }
    public string DocPath { get; set; }
    public ImageSource Icon { get; set; }
    public int Id { get; set; }
    public string FullName { get; set; }

    //Privates
    private bool isHovered = false;

    public UC_DocPanel()
    {
      InitializeComponent();
      DataContext = this;
      Id = docPanelObjects.Count();
      docPanelObjects.Add(this);

      Testing();
    }

    ~UC_DocPanel()
    {

    }

    private void DocPanelLoaded(object sender, RoutedEventArgs e)
    {
      var window = (MainWindow)Window.GetWindow(this);
      window.Root.MouseLeftButtonUp += Select;
      window.Root.MouseMove += CheckIsHovered;

      window.Root.KeyDown += RemoveCurrentDocPanel;

    }

    void Testing()
    {
      //DocPath = "C:\\Users\\Aji Suprana\\Desktop\\Might & Magic Heroes VI.url";
      //DocName = "Might & Magic Heroes VI";
      //Icon = CSB_FileDropDetection.getExtensionIcon(DocPath);
    }

    void Select(object obj, EventArgs e)
    {
      isHovered = IsMouseOver;
      hoveredID = GetHoveredID();

      if (hoveredID == -1)
      {
        foreach (var it in docPanelObjects)
        {
          it.Root.Background = Brushes.Gray;
        }
        return;
      }


      if (hoveredID == Id)
      {
        Console.WriteLine("[UC_DocPanel]:Id {1} Name {0} is Clicked", DocName, Id);
        docPanelObjects[Id].Root.Background = Brushes.DarkGray;
        selected = Id;
      }
      else
      {
        docPanelObjects[Id].Root.Background = Brushes.Gray;
      }

      if(docPanelClicked!=null)
        docPanelClicked(Id);
    }

    void CheckIsHovered(object obj, MouseEventArgs e)
    {
      isHovered = IsMouseOver;
      hoveredID = GetHoveredID();
    }

    int GetHoveredID()
    {
      foreach(var it in docPanelObjects)
      {
        if (it.isHovered)
          return it.Id;
      }
      return -1;
    }

    private void UpdateIDs()
    {
      int i = 0;
      foreach(var it in docPanelObjects)
      {
        it.Id = i;
        i++;
      }
    }

    private void RemoveCurrentDocPanel(object sender, KeyEventArgs e)
    {
      if (selected != Id)
        return;

      if (e.Key == Key.Delete)
      {
        var window = (MainWindow)Window.GetWindow(this);
        window.Root.MouseLeftButtonUp -= this.Select;
        Console.WriteLine("[UC_DocPanel]:{0} going to be deleted", Id);
        docPanelObjects.RemoveAt(Id);
        ((StackPanel)this.Parent).Children.Remove(this);
        UpdateIDs();
        e.Handled = true;
      }
    }
  }
}
