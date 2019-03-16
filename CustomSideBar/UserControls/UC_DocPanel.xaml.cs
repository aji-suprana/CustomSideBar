using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using CustomSideBar.Serializer;

namespace CustomSideBar.UserControls
{

  public partial class UC_DocItem : UserControl, ISerializableObject
  {
    //delegates
    public delegate void DocPanelClicked(int DocPanelID);

    //events
    public static DocPanelClicked docPanelClicked = null;

    //selection states
   // public static List< UC_DocItem> docPanelObjects = new List< UC_DocItem>();
    private static int selected = -1;
    public static int hoveredID = -1;

    //Property
    public string DocName { get; set; }
    public string DocPath { get; set; }
    public ImageSource Icon { get; set; }
    public int Id { get; set; }
    public string FullName { get; set; }



    public SolidColorBrush SelectedColor
    {
      get { return (SolidColorBrush)GetValue(SelectedColorProperty); }
      set { SetValue(SelectedColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(
          "SelectedColor",
          typeof(SolidColorBrush),
          typeof(UC_DocItem),
          new PropertyMetadata((SolidColorBrush)(new BrushConverter().ConvertFrom("#333333"))));



    public SolidColorBrush NormalColor
    {
      get { return (SolidColorBrush)GetValue(NormalColorProperty); }
      set { SetValue(NormalColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NormalColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NormalColorProperty =
        DependencyProperty.Register(
          "NormalColor", 
          typeof(SolidColorBrush),
          typeof(UC_DocItem), 
          new PropertyMetadata(new BrushConverter().ConvertFrom("#1e1e1e")));



    //Privates
    private bool isHovered = false;

    public UC_DocItem()
    {
      InitializeComponent();
      DataContext = this;
      Id = UC_Collections.DocItems.Count();
      UC_Collections.DocItems.Add(this);
    }

    ~UC_DocItem()
    {
      Console.WriteLine("adsfadfsdfasdfasdfasdfasdfasdf");
    }

    public JObject Serialize()
    {
      JObject DocData = new JObject();
      DocData.Add(new JProperty("DocName", DocName));
      DocData.Add(new JProperty("DocPath", DocPath));

     // Console.WriteLine(DocData.ToString());

      return DocData;
    }

    public UC_DocItem Deserialize(string JsonString)
    {
      //JProperty DocName = j.Property("DocName");
      //JProperty DocPath = j.Property("DocPath");


      return null;
    }

    private void DocPanelLoaded(object sender, RoutedEventArgs e)
    {
      var window = (MainWindow)Window.GetWindow(this);
      window.Root.MouseLeftButtonUp += Select;
      window.Root.MouseMove += CheckIsHovered;

      window.Root.KeyDown += RemoveCurrentDocItem;

    }

    void Select(object obj, EventArgs e)
    {
      isHovered = IsMouseOver;
      hoveredID = GetHoveredID();

      if (hoveredID == -1)
      {
        foreach (var it in UC_Collections.DocItems)
        {
          it.Root.Background =NormalColor;
        }
        return;
      }


      if (hoveredID == Id)
      {
        Console.WriteLine("[UC_DocItem]:Id {1} Name {0} is Clicked", DocName, Id);

        UC_Collections.DocItems[Id].Root.Background = SelectedColor;
        selected = Id;
      }
      else
      {
        UC_Collections.DocItems[Id].Root.Background = NormalColor;
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
      foreach(var it in UC_Collections.DocItems)
      {
        if (it.isHovered)
          return it.Id;
      }
      return -1;
    }

    private void UpdateIDs()
    {
      int i = 0;
      foreach(var it in UC_Collections.DocItems)
      {
        it.Id = i;
        i++;
      }
    }

    private void RemoveCurrentDocItem(object sender, KeyEventArgs e)
    {
      if (selected != Id)
        return;

      if (e.Key == Key.Delete)
      {
        Console.WriteLine("[UC_DocItem]:{0} going to be deleted", Id);
        RemoveUC_DocItem();
        e.Handled = true;
      }
    }

    private void RemoveUC_DocItem()
    {
      var window = (MainWindow)Window.GetWindow(this);

      window.Root.MouseLeftButtonUp -= this.Select;
      window.Root.MouseMove -= CheckIsHovered;
      window.Root.KeyDown -= RemoveCurrentDocItem;

      ((StackPanel)this.Parent).Children.Remove(this);
      UC_Collections.DocItems.RemoveAt(Id);
      UpdateIDs();

    }
  }
}
